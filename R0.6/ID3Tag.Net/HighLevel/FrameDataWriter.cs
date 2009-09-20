using System;
using System.IO;
using System.Text;
//using ID3Tag.LowLevel;

namespace ID3Tag.HighLevel
{
	/// <summary>
	/// Writes data for the Raw Frame Payload
	/// </summary>
	public sealed class FrameDataWriter : IDisposable
	{
		private readonly MemoryStream _stream;

		/// <summary>
		/// Initializes a new instance of the <see cref="FrameDataWriter"/> class using specified capacity.
		/// </summary>
		/// <param name="capacity">The initial buffer capacity.</param>
		public FrameDataWriter(int capacity)
		{
			_stream = new MemoryStream(capacity);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FrameDataWriter"/> class using default capacity.
		/// </summary>
		public FrameDataWriter()
		{
			_stream = new MemoryStream(128);
		}

		#region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			_stream.Dispose();
		}

		#endregion

		/// <summary>
		/// Writes the encoded text preamble (byte order mark [BOM]) to the stream.
		/// Preamble is required for unicode strings only. BE Unicode and UTF8 should not have it.
		/// Preamble should be written only once per frame at the beginning of the encoded text.
		/// </summary>
		/// <param name="encoding">The encoding.</param>
		public void WritePreamble(Encoding encoding)
		{
			if (encoding.CodePage == 1200) // Unicode
			{
				byte[] bom = encoding.GetPreamble();

				if (bom.Length == 0)
				{
					throw new ID3TagException("Unicode encoding must provide byte order mark (BOM).");
				}

				_stream.Write(bom, 0, bom.Length);
			}
		}

		/// <summary>
		/// Writes fixed string to the stream.
		/// </summary>
		/// <param name="text">The string.</param>
		/// <param name="encoding">The encoding.</param>
		/// <param name="count">The count of bytes to write.</param>
		public void WriteString(String text, Encoding encoding, int count)
		{
			byte[] bytes = encoding.GetBytes(text);
			_stream.Write(bytes, 0, count);
		}

		/// <summary>
		/// Writes variable string to the stream without termination bytes.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="encoding">The encoding.</param>
		public void WriteString(String text, Encoding encoding)
		{
			WriteString(text, encoding, false);
		}

		/// <summary>
		/// Writes variable string to the stream and adds termination bytes if specified.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="encoding">The encoding.</param>
		/// <param name="addTerminationBytes">if set to <c>true</c> [add termination bytes].</param>
		public void WriteString(String text, Encoding encoding, bool addTerminationBytes)
		{
			if (!string.IsNullOrEmpty(text))
			{
				// Write text
				byte[] bytes = encoding.GetBytes(text);
				_stream.Write(bytes, 0, bytes.Length);
			}

			if (addTerminationBytes)
			{
				// Write termination bytes
				byte[] term = encoding.GetBytes(new[] { Char.MinValue });
				_stream.Write(term, 0, term.Length);
			}
		}

		/// <summary>
		/// Writes the unsigned short integer to the stream using less significant byte (LSB) first order.
		/// Uses 2 bytes for storage.
		/// </summary>
		/// <param name="value">The value.</param>
		public void WriteUInt16(UInt16 value)
		{
			// BitConverter uses most significant byte (MSB) first order, so it must be reversed
			byte[] bytes = BitConverter.GetBytes(value);
			Array.Reverse(bytes);
			_stream.Write(bytes, 0, 2);
		}

		/// <summary>
		/// Writes the unsigned integer to the stream using using less significant byte (LSB) first order.
		/// Uses 4 bytes for storage.
		/// </summary>
		/// <param name="value">The value.</param>
		public void WriteUInt32(UInt32 value)
		{
			// BitConverter uses most significant byte (MSB) first order, so it must be reversed
			byte[] bytes = BitConverter.GetBytes(value);
			Array.Reverse(bytes);
			_stream.Write(bytes, 0, 4);
		}

		/// <summary>
		/// Writes the unsigned big integer to the stream using using variable less significant byte (LSB) first order.
		/// Uses at least 4 and at max 8 bytes for storage.
		/// </summary>
		/// <param name="value">The value.</param>
		public void WriteUInt64(UInt64 value)
		{
			// The value should be stored using less significan byte (LSB) format (bit 0 is most right) and variable length with at least 4 bytes used.
			// For example: 1 = 00 00 00 01.
			// If value exceeds max UInt32 then another byte is added to the front. For example: 4,294,967,296 = 01 00 00 00 00.

			byte[] bytes = BitConverter.GetBytes(value);

			// BitConverter uses most significant byte (MSB) first order, so it must be truncated and then reversed
			// For example: 1 = 01 00 00 00 00 00 00 00 (MSB) will be stored as 00 00 00 01 (LSB)
			// For example: 4,294,967,296 = 00 00 00 00 01 00 00 00 (MSB) will be stored as 01 00 00 00 00 (variable length LSB)

			// Count array length without trailing zeros
			int len = bytes.Length;
			for (int i = bytes.Length - 1; i > 3; i--)
			{
				if (bytes[i] == Byte.MinValue)
				{
					len--;
				}
				else
				{
					break;
				}
			}

			Array.Reverse(bytes, 0, len);
			_stream.Write(bytes, 0, len);
		}

		/// <summary>
		/// Writes the value to the stream.
		/// </summary>
		/// <param name="value">The data to write.</param>
		public void WriteBytes(byte[] value)
		{
			_stream.Write(value, 0, value.Length);
		}

		/// <summary>
		/// Returns the stream contents as byte array, regardless of the current position.
		/// </summary>
		public byte[] ToArray()
		{
			return _stream.ToArray();
		}

		/// <summary>
		/// Writes the text encoding type byte.
		/// </summary>
		/// <param name="encoding">The encoding that defines the encoding byte.</param>
		public void WriteEncodingByte(Encoding encoding)
		{
			TextEncodingType type;

			if (encoding == null)
			{
				type = TextEncodingType.Ansi;
			}
			else
			{
				switch (encoding.CodePage)
				{
					case 1200: // Unicode
						type = TextEncodingType.Unicode;
						break;
					case 1201: // Unicode BE
						type = TextEncodingType.BigEndianUnicode;
						break;
					case 65001: // UTF8
						type = TextEncodingType.Utf8;
						break;
					default:
						if (encoding.IsSingleByte)
						{
							type = TextEncodingType.Ansi;
						}
						else
						{
							throw new NotSupportedException(String.Format("{0} encoding is not supported.", encoding.EncodingName));
						}
						break;
				}
			}

			_stream.WriteByte((byte)type);
		}

		/// <summary>
		/// Writes a byte to the stream.
		/// </summary>
		/// <param name="value">The value to write.</param>
		public void WriteByte(byte value)
		{
			_stream.WriteByte(value);
		}
	}
}