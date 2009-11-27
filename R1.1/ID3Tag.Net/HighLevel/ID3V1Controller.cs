using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Id3Tag.HighLevel
{
	internal class Id3V1Controller : IId3V1Controller
	{
		#region IId3V1Controller Members

		/// <summary>
		/// Reads the ID3v1 tag using specified code page for text encoding.
		/// </summary>
		/// <param name="inputStream">the input stream (i.e. the file.)</param>
		/// <param name="codePage">The code page for text encoding.</param>
		/// <returns>An ID3v1 container.</returns>
		public Id3V1Tag Read(Stream inputStream, int codePage)
		{
			if (!inputStream.CanSeek)
			{
				throw new Id3TagException("Cannot read ID3v1 tag because the stream does not support seek.");
			}

			if (inputStream.Length < 128)
			{
				throw new Id3IOException("Cannot read ID3v1 tag because the stream is too short");
			}

			//
			//  Read the last 128 Bytes from the stream (ID3v1 Position)
			//
			var tagBytes = new byte[128];
			inputStream.Seek(-128, SeekOrigin.End);
			inputStream.Read(tagBytes, 0, 128);

			bool isValidTag = CheckID(tagBytes);
			if (!isValidTag)
			{
				throw new Id3HeaderNotFoundException("TAG header not found");
			}

			Id3V1Tag v1Tag = ExtractTag(tagBytes, codePage);
			return v1Tag;
		}

		/// <summary>
		/// Reads the ID3v1 tag using default code page for text encoding.
		/// </summary>
		/// <param name="file">the file.</param>
		/// <returns>An ID3v1 container.</returns>
		public Id3V1Tag Read(FileInfo file)
		{
			return Read(file, 0);
		}

		/// <summary>
		/// Reads the ID3v1 tag using default code page for text encoding.
		/// </summary>
		/// <param name="inputStream">the input stream (i.e. the file.)</param>
		/// <returns>An ID3v1 container.</returns>
		public Id3V1Tag Read(Stream inputStream)
		{
			return Read(inputStream, 0);
		}

		/// <summary>
		/// Reads the ID3v1 tag using specified code page for text encoding.
		/// </summary>
		/// <param name="file">the file.</param>
		/// <param name="codePage">The code page.</param>
		/// <returns>An ID3v1 container.</returns>
		public Id3V1Tag Read(FileInfo file, int codePage)
		{
			bool fileExists = file.Exists;
			if (!fileExists)
			{
				throw new FileNotFoundException("File " + file.FullName + " not found!.");
			}

			FileStream fs = null;
			Id3V1Tag info;
			try
			{
				fs = File.Open(file.FullName, FileMode.Open, FileAccess.Read);
				info = Read(fs, codePage);
			}
			catch (Id3TagException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new Id3TagException("Unknown Exception during reading.", ex);
			}
			finally
			{
				if (fs != null)
				{
					fs.Close();
				}
			}

			return info;
		}

		/// <summary>
		/// Writes a new ID3v1 tag using default code page for text encoding.
		/// </summary>
		/// <param name="tag">the tag.</param>
		/// <param name="input">the audio input stream.</param>
		/// <param name="output">the target stream.</param>
		public void Write(Id3V1Tag tag, Stream input, Stream output)
		{
			Write(tag, input, output);
		}

		/// <summary>
		/// Writes a new ID3v1 tag using specified code page for text encoding.
		/// </summary>
		/// <param name="tag">the tag.</param>
		/// <param name="input">the audio input stream.</param>
		/// <param name="output">the target stream.</param>
		/// <param name="codePage">The code page for text encoding.</param>
		public void Write(Id3V1Tag tag, Stream input, Stream output, int codePage)
		{
			//
			//  Validate the parameter.
			//
			if (tag == null)
			{
				throw new ArgumentNullException("tag");
			}

			if (input == null)
			{
				throw new ArgumentNullException("input");
			}

			if (output == null)
			{
				throw new ArgumentNullException("output");
			}

			if (!input.CanSeek)
			{
				throw new Id3TagException("Cannot write ID3V1 tag because the source does not support seek.");
			}

			if (!output.CanWrite)
			{
				throw new Id3TagException("Cannot write ID3V1 tag because the output does not support writing.");
			}

			try
			{
				//
				//  Read the last 128 Bytes from the stream (ID3v1 Position)
				//
				long audioBytesCount = GetAudioBytesCount(input);

				//
				//  Write the audio data and tag
				//
				input.Seek(0, SeekOrigin.Begin);
				Utils.WriteAudioStream(output, input, audioBytesCount);

				byte[] tagBytes = ConvertToByte(tag, codePage);
				output.Write(tagBytes, 0, tagBytes.Length);
			}
			catch (Exception ex)
			{
				throw new Id3IOException("Cannot write ID3v1 tag", ex);
			}
		}

		#endregion

		#region Private helper

		private static long GetAudioBytesCount(Stream input)
		{
			var tagBytes = new byte[128];
			long audioBytesCount;

			if (input.Length > 127)
			{
				input.Seek(-128, SeekOrigin.End);
				input.Read(tagBytes, 0, 128);

				bool Id3TagFound = CheckID(tagBytes);
				if (Id3TagFound)
				{
					// Ignore the Id3Tag from the source
					audioBytesCount = input.Length - 128;
				}
				else
				{
					audioBytesCount = input.Length;
				}
			}
			else
			{
				audioBytesCount = input.Length;
			}

			return audioBytesCount;
		}

		private static byte[] ConvertToByte(Id3V1Tag tag, int codePage)
		{
			var tagBytes = new byte[128];
			// Write the tag ID ( TAG)
			tagBytes[0] = 0x54;
			tagBytes[1] = 0x41;
			tagBytes[2] = 0x47;

			// Write the fields...
			byte[] titleBytes = GetField(tag.Title, 30, codePage);
			byte[] artistBytes = GetField(tag.Artist, 30, codePage);
			byte[] albumBytes = GetField(tag.Album, 30, codePage);
			byte[] year = GetField(tag.Year, 4, codePage);

			Array.Copy(titleBytes, 0, tagBytes, 3, 30);
			Array.Copy(artistBytes, 0, tagBytes, 33, 30);
			Array.Copy(albumBytes, 0, tagBytes, 63, 30);
			Array.Copy(year, 0, tagBytes, 93, 4);

			byte[] commentBytes;
			if (tag.IsId3V1Dot1Compliant)
			{
				commentBytes = GetField(tag.Comment, 28, codePage);
				Array.Copy(commentBytes, 0, tagBytes, 97, 28);

				string trackNr = tag.TrackNumber;
				tagBytes[125] = 0x00;
				tagBytes[126] = Convert.ToByte(trackNr, CultureInfo.InvariantCulture);
			}
			else
			{
				commentBytes = GetField(tag.Comment, 30, codePage);
				Array.Copy(commentBytes, 0, tagBytes, 97, 30);
			}

			// Add genre
			tagBytes[127] = Convert.ToByte(tag.GenreIdentifier);

			return tagBytes;
		}

		private static byte[] GetField(string value, int size, int codePage)
		{
			Encoding encoding = codePage == 0 ? Encoding.Default : Encoding.GetEncoding(codePage);

			if (!encoding.IsSingleByte)
			{
				throw new InvalidOperationException("Text encoding {0} is not a single-byte and it cannot be used in ID3v1 tags.");
			}

			byte[] valueBytes = encoding.GetBytes(value);
			var fieldBytes = new byte[size];

			if (valueBytes.Length == size)
			{
				fieldBytes = valueBytes;
			}
			else
			{
				// OK. Fit to size
				if (valueBytes.Length > size)
				{
					Array.Copy(valueBytes, fieldBytes, size);
				}
				else
				{
					int fieldCount = fieldBytes.Length;
					Array.Copy(valueBytes, fieldBytes, valueBytes.Length);

					for (int i = valueBytes.Length; i < fieldCount; i++)
					{
						// Add Space code
						fieldBytes[i] = 0x20;
					}
				}
			}

			return fieldBytes;
		}

		private static Id3V1Tag ExtractTag(byte[] tagBytes, int codePage)
		{
			// Read the tag

			var titleBytes = new byte[30];
			var artistBytes = new byte[30];
			var albumBytes = new byte[30];
			var yearBytes = new byte[4];
			var commentBytes = new byte[30];

			Array.Copy(tagBytes, 3, titleBytes, 0, 30);
			Array.Copy(tagBytes, 33, artistBytes, 0, 30);
			Array.Copy(tagBytes, 63, albumBytes, 0, 30);
			Array.Copy(tagBytes, 93, yearBytes, 0, 4);
			Array.Copy(tagBytes, 97, commentBytes, 0, 30);
			byte genreByte = tagBytes[127];

			string title = GetString(titleBytes, codePage);
			string artits = GetString(artistBytes, codePage);
			string album = GetString(albumBytes, codePage);
			string year = GetString(yearBytes, codePage);

			bool id3v1_1Support = ((commentBytes[28] == 0) && (commentBytes[29] != 0));
			string trackNr = String.Empty;
			string comment = String.Empty;

			if (id3v1_1Support)
			{
				byte trackNrValue = commentBytes[29];
				trackNr = Convert.ToString(trackNrValue, CultureInfo.InvariantCulture);

				var newComments = new byte[28];
				Array.Copy(commentBytes, 0, newComments, 0, newComments.Length);

				comment = GetString(newComments, codePage);
			}
			else
			{
				comment = GetString(commentBytes, codePage);
			}

			var id3v1 = new Id3V1Tag
			            	{
			            		Title = title,
			            		Artist = artits,
			            		Album = album,
			            		Year = year,
			            		Comment = comment,
			            		GenreIdentifier = genreByte,
			            		IsId3V1Dot1Compliant = id3v1_1Support,
			            		TrackNumber = trackNr
			            	};

			return id3v1;
		}

		private static string GetString(byte[] array, int codePage)
		{
			int count = array.Length;

			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == 0x00)
				{
					count = i;
					break;
				}
			}

			Encoding encoding = codePage == 0 ? Encoding.Default : Encoding.GetEncoding(codePage);

			if (!encoding.IsSingleByte)
			{
				throw new InvalidOperationException("Text encoding {0} is not a single-byte and it cannot be used in ID3v1 tags.");
			}

			return encoding.GetString(array, 0, count).TrimEnd();
		}

		private static bool CheckID(byte[] tag)
		{
			// TAG
			return (tag[0] == 0x54) && (tag[1] == 0x41) && (tag[2] == 0x47);
		}

		#endregion
	}
}