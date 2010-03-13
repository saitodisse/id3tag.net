using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Id3Tag.HighLevel
{
	/// <summary>
	/// Reads data from the Raw Frame Payload.
	/// </summary>
	public sealed class FrameDataReader : IDisposable
	{
		private const int _SIZE = 64;
		private readonly MemoryStream _stream;
		private byte[] _bytesBuffer;
		private char[] _charsBuffer;
		private bool _isDisposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="FrameDataReader"/> class.
		/// </summary>
		/// <param name="payload">The payload.</param>
		public FrameDataReader(ICollection<byte> payload)
		{
			if (payload == null)
			{
				throw new ArgumentNullException("payload");
			}

			var payload2 = new byte[payload.Count];
			payload.CopyTo(payload2, 0);

			_stream = new MemoryStream(payload2, 0, payload2.Length, false, true);
		}

		/// <summary>
		/// Gets or sets the position in the stream.
		/// </summary>
		/// <value>The position.</value>
		public long Position
		{
			get { return _stream.Position; }
			set { _stream.Position = value; }
		}

		#region Private Methods

		/// <summary>
		/// Reads UTF-16 encoding from the stream.
		/// </summary>
		/// <returns>Encoding</returns>
		private Encoding CreateUnicodeFromBom()
		{
			// The standard requires BOM for Unicode (and only for Unicode)

			if (_stream.Length > _stream.Position + 1)
			{
				//  Read Byte Order Mask (BOM) for details (http://www.unicode.org/faq/utf_bom.html#BOM)
				var bom1 = (byte)_stream.ReadByte();
				var bom2 = (byte)_stream.ReadByte();

				if (bom1 == 0xFE && bom2 == 0xFF)
				{
					return new UnicodeEncoding(true, true);
				}

				if (bom1 == 0xFF && bom2 == 0xFE)
				{
					return new UnicodeEncoding(false, true);
				}
			}

			throw new InvalidId3StructureException("Unicode encoded text does not start with the byte order mark (BOM).");
		}

		/// <summary>
		/// Laizy init buffer of bytes
		/// </summary>
		/// <returns></returns>
		private byte[] GetBytesBuffer()
		{
			if (_bytesBuffer == null)
			{
				_bytesBuffer = new byte[_SIZE];
			}
			return _bytesBuffer;
		}

		/// <summary>
		/// Laizy init buffer of chars
		/// </summary>
		private char[] GetCharsBuffer()
		{
			if (_charsBuffer == null)
			{
				_charsBuffer = new char[_SIZE];
			}
			return _charsBuffer;
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="FrameDataReader"/> is reclaimed by garbage collection.
		/// </summary>
		~FrameDataReader()
		{
			Dispose(false);
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		private void Dispose(bool disposing)
		{
			if (!_isDisposed)
			{
				if (disposing && _stream != null)
				{
					_stream.Dispose();
				}

				_bytesBuffer = null;
				_charsBuffer = null;
				_isDisposed = true;
			}
		}

		/// <summary>
		/// Discovers and reads encoding type from the stream using current stream index as starting position of the encoded text.
		/// The encoding byte is always located at position 0. However, additional information from the text is needed to determine
		/// byte order mark (BOM) for unicode encodings (UTF). After this method is executed, the position will be set to the first
		/// byte of the actual text.
		/// </summary>
		/// <param name="encodingTypeValue">The encoding byte.</param>
		/// <param name="codePage">The default code page for ASCI encodings.</param>
		/// <returns></returns>
		public Encoding ReadEncoding(int encodingTypeValue, int codePage)
		{
			if (!Enum.IsDefined(typeof(TextEncodingType), encodingTypeValue))
			{
				throw new Id3TagException("Text encoding type value of {0:x2} is not supported.");
			}

			switch ((TextEncodingType)encodingTypeValue)
			{
				case TextEncodingType.Ansi:
					return codePage == 0 ? Encoding.Default : Encoding.GetEncoding(codePage);

				case TextEncodingType.Unicode:
					return CreateUnicodeFromBom();

				case TextEncodingType.BigEndianUnicode:
					return new UnicodeEncoding(true, false);

				case TextEncodingType.Utf8:
					return new UTF8Encoding();

				default:
					throw new NotImplementedException(); // We should never get here.
			}
		}

		/// <summary>
		/// Reads fixed string from the stream starting with current position.
		/// </summary>
		/// <param name="encoding">The encoding.</param>
		/// <param name="length">Number of bytes to read.</param>
		/// <returns>Decoded string</returns>
		public string ReadFixedString(Encoding encoding, int length)
		{
			var position = (int)_stream.Position;
			_stream.Seek(length, SeekOrigin.Current);
			return encoding.GetString(_stream.GetBuffer(), position, length);
		}

		/// <summary>
		/// Reads the variable-length string that is terminated either by zero-char or end of stream.
		/// After reading, the position will be set after zero-char terminator.
		/// </summary>
		/// <param name="encoding">The encoding.</param>
		/// <returns>String</returns>
		public string ReadVariableString(Encoding encoding)
		{
			// This method reads a block of bytes to "bytes" from stream, then it decodes it into "chars" by using
			// the decoder of specified "encoding". This ensures correct decoding of single- or multi-byte strings.
			// Then "chars" are scanned for zero-char (terminator) or end of stream. Non-zero chars are added to the
			// output "text" list and then returned as string.
			// If zero-char is found then the stream postion is returned back to first byte after zero-char.
			// This seems to be better than using some complex zero-char locator method and then calling
			// Encoding.GetString() method.

			var text = new List<char>(_SIZE);
			byte[] bytes = GetBytesBuffer();
			char[] chars = GetCharsBuffer();
			Decoder decoder = encoding.GetDecoder();
			Encoder encoder = encoding.GetEncoder();
			bool isTerminated = false;

			decoder.Reset();
			while (!isTerminated)
			{
				int byteCount = _stream.Read(bytes, 0, _SIZE);
				int charCount = encoding.GetChars(bytes, 0, byteCount, chars, 0);

				for (int i = 0; i < charCount; i++)
				{
					char c = chars[i];
					if (c == Char.MinValue)
					{
						int offset = encoder.GetByteCount(chars, 0, i + 1, true) - byteCount;
						_stream.Seek(offset, SeekOrigin.Current);
						isTerminated = true;
						break;
					}

					text.Add(c);
				}

				if (!isTerminated && byteCount < _SIZE)
				{
					isTerminated = true;
				}
			}

			return new String(text.ToArray());
		}

		/// <summary>
		/// Reads byte from the stream starting with current position.
		/// </summary>
		/// <returns>Byte</returns>
		public byte ReadByte()
		{
			int value = _stream.ReadByte();

			if (value < 0)
			{
				throw new Id3TagException(
					"An error has occured during data reading from the Raw Frame data. End of stream has been reached.");
			}

			return (byte)value;
		}

		/// <summary>
		/// Reads specified number of bytes from the stream starting with current position.
		/// </summary>
		/// <param name="count">The count.</param>
		/// <returns>Data</returns>
		public byte[] ReadBytes(int count)
		{
			var bytes = new byte[count];
			int len = _stream.Read(bytes, 0, count);
			if (len != count)
			{
				throw new Id3TagException(
					String.Format(
						CultureInfo.InvariantCulture,
						"An error has occured during data reading from the Raw Frame data. Expected number of bytes: {0}. Actual number of bytes: {1}.",
						count,
						len));
			}
			return bytes;
		}

		/// <summary>
		/// Reads all bytes from the stream starting with current position.
		/// </summary>
		/// <returns>Data</returns>
		public byte[] ReadBytes()
		{
			return ReadBytes((int)(_stream.Length - _stream.Position));
		}

		/// <summary>
		/// Reads Unsigned Integer (2 bytes) from the stream starting with current position.
		/// Assumes that integer bytes are recorded in the reverse order.
		/// </summary>
		/// <returns>Value</returns>
		public UInt16 ReadUInt16()
		{
			var bytes = new byte[2];
			_stream.Read(bytes, 0, 2);
			Array.Reverse(bytes);
			return BitConverter.ToUInt16(bytes, 0);
		}

		/// <summary>
		/// Reads unsigned integer (4 bytes) from the stream starting with current position.
		/// </summary>
		/// <returns>Value</returns>
		public UInt32 ReadUInt32()
		{
			var bytes = new byte[4];
			_stream.Read(bytes, 0, 4);
			Array.Reverse(bytes);
			return BitConverter.ToUInt32(bytes, 0);
		}

		/// <summary>
		/// Reads unsigned big integer (min 4 - max 8 bytes) from the stream starting with current position.
		///  </summary>
		/// <returns>Value</returns>
		/// <remarks>
		/// NOTE: This method assumes that there will be no more data to read from the stram after this.
		/// </remarks>
		public UInt64 ReadUInt64()
		{
			// The value is stored using less significan byte (LSB) format (bit 0 is most right) and variable length with at least 4 bytes used.
			// For example: 1 = 00 00 00 01.
			// If value exceeds max UInt32 then another byte is added to the front. For example: 4,294,967,296 = 01 00 00 00 00.

			var bytes = new byte[8];
			int len = _stream.Read(bytes, 0, 8);
			Array.Reverse(bytes, 0, len);
			return BitConverter.ToUInt64(bytes, 0);
		}
	}
}