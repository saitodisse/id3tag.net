using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using Id3Tag.HighLevel;

namespace Id3Tag.LowLevel
{
	internal class IoController : IIOController
	{
		#region IIOController Members

		public Id3TagInfo Read(FileInfo file)
		{
			bool fileExists = file.Exists;
			if (!fileExists)
			{
				var ex =  new FileNotFoundException("File " + file.FullName + " not found!.");
                Logger.LogError(ex);

			    throw ex;
			}

			FileStream fs = null;
			Id3TagInfo info;
			try
			{
			    var filename = file.FullName;

			    Logger.LogInfo(String.Format("Open File {0}", filename));
                fs = File.Open(filename, FileMode.Open, FileAccess.Read);

				info = Read(fs);
			}
			catch (Id3TagException idEx)
			{
                Logger.LogError(idEx);
				throw;
			}
			catch (Exception ex)
			{
                Logger.LogError(ex);
				throw new Id3TagException("Unknown Exception during reading.", ex);
			}
			finally
			{
				if (fs != null)
				{
					fs.Close();
					fs.Dispose();
				}
			}

			return info;
		}

		public Id3TagInfo Read(Stream inputStream)
		{
			if (inputStream == null)
			{
				var ex =  new ArgumentNullException("inputStream");
                Logger.LogError(ex);

			    throw ex;
			}

			if (!inputStream.CanRead)
			{
				var ex =  new Id3IOException("Cannot read data stream.");
                Logger.LogError(ex);

			    throw ex;
			}

			//
			//  Read the bytes from the I/O stream.
			//
			var tagInfo = new Id3TagInfo();
			byte[] rawTagContent;

            Logger.LogInfo("Reading ID3v2 tag from Stream.");
			using (var reader = new BinaryReader(inputStream))
			{
                Logger.LogInfo("Reading ID3v2 Header");
				var headerBytes = new byte[10];
				reader.Read(headerBytes, 0, 10);

				int rawTagLength = AnalyseHeader(headerBytes, tagInfo);
				long bytesLeft = reader.BaseStream.Length - reader.BaseStream.Position;
				if (rawTagLength > bytesLeft)
				{
					var ex =  new Id3TagException(
						String.Format(
							CultureInfo.InvariantCulture, "Specified tag size {0} exceeds actual content size {1}.", rawTagLength, bytesLeft));

                    Logger.LogError(ex);
				    throw ex;
				}

			    Logger.LogInfo("Reading ID3v2 Content");
				rawTagContent = new byte[rawTagLength];
				reader.Read(rawTagContent, 0, rawTagLength);
			}

			//
			//  Check for Unsynchronisation Bytes
			//
			byte[] tagContent;
			if (tagInfo.Unsynchronised)
			{
				// Scan for unsynchronisation bytes!
			    Logger.LogInfo("Remove Unsynchronisatzion bytes.");
				tagContent = RemoveUnsyncBytes(rawTagContent);
			}
			else
			{
				tagContent = rawTagContent;
			}

			Stream tagStream = new MemoryStream(tagContent);
			int length = tagContent.Length;
			using (var reader = new BinaryReader(tagStream))
			{
				//
				//  Check for Extended Header
				//
				if (tagInfo.ExtendedHeaderAvailable)
				{
				    Logger.LogInfo("Analyse Extended Header");
					AnalyseExtendedHeader(reader, tagInfo);
				}

                Logger.LogInfo(String.Format("Start reading ID3v2.{0} frame.",tagInfo.MajorVersion));

				//
				//  Read the content
				//
				var frameBytes = new List<byte>();
				long pos = reader.BaseStream.Position;
				while ((pos + 10) < length)
				{
                    Logger.LogInfo("Getting frame...");
					bool continueReading = ReadContent(reader, tagInfo, frameBytes);
					if (!continueReading)
					{
						break;
					}

					pos = reader.BaseStream.Position;
				}

				//
				//  Check CRC if available
				//
				if (tagInfo.ExtendedHeader != null && tagInfo.ExtendedHeader.CrcDataPresent)
				{
					if (tagInfo.MajorVersion == 3)
					{
						byte[] tagData = frameBytes.ToArray();
						ReadOnlyCollection<byte> crc32Value = tagInfo.ExtendedHeader.Crc32;

						var crc32 = new Crc32(Crc32.DefaultPolynom);
						bool crcOk = crc32.Validate(tagData, crc32Value);

						if (!crcOk)
						{
                            var ex =  new Id3TagException("The CRC32 validation failed!");
                            Logger.LogError(ex);

						    throw ex;
						}
					}
					else
					{
						/*
                         *    c - CRC data present

                             If this flag is set, a CRC-32 [ISO-3309] data is included in the
                             extended header. The CRC is calculated on all the data between the
                             header and footer as indicated by the header's tag length field,
                             minus the extended header. Note that this includes the padding (if
                             there is any), but excludes the footer. The CRC-32 is stored as an
                             35 bit synchsafe integer, leaving the upper four bits always
                             zeroed.

                                Flag data length       $05
                                Total frame CRC    5 * %0xxxxxxx

                         */

						// TODO: Implement the CRC32 check for ID3v2.4
                        var ex = new NotSupportedException("CRC32 check is not support for > ID3 V2.3");
                        Logger.LogError(ex);

					    throw ex;
					}
				}
			}

			return tagInfo;
		}

		public void Write(TagContainer tagContainer, Stream input, Stream output)
		{
			if (input == null)
			{
				var ex =  new ArgumentNullException("input");
                Logger.LogError(ex);

			    throw ex;
			}
			if (output == null)
			{
				var ex =  new ArgumentNullException("output");
                Logger.LogError(ex);

			    throw ex;
			}
			if (tagContainer == null)
			{
				var ex =  new ArgumentNullException("tagContainer");
                Logger.LogError(ex);

			    throw ex;
			}

			//
			//  Validate whether the tag container is in ID3V2.3 formaz
			//
			string message;
			bool isTagValid = ValidateTag(tagContainer, out message);
			if (!isTagValid)
			{
				var ex = new InvalidId3StructureException(message);
                Logger.LogError(ex);

			    throw ex;
			}

			//
			//  OK. Id3Tag is valid. Let's write the tag.
			//
			byte[] tagBytes;
			switch (tagContainer.TagVersion)
			{
				case TagVersion.Id3V23:
					tagBytes = BuildId3V3Tag(tagContainer);
					break;
				case TagVersion.Id3V24:
					tagBytes = BuildId3V4Tag(tagContainer);
					break;
				default:
					var ex =  new Id3TagException("This TagVersion is not supported!");
                    Logger.LogError(ex);

			        throw ex;
			}

			//
			//  encode the length
			//
			long length;
			if (tagContainer.TagVersion == TagVersion.Id3V24)
			{
				TagDescriptorV4 descriptor = tagContainer.GetId3V24Descriptor();
				if (descriptor.Footer)
				{
					length = tagBytes.LongLength - 20;
				}
				else
				{
					length = tagBytes.LongLength - 10;
				}
			}
			else
			{
				length = tagBytes.LongLength - 10;
			}

			List<int> bits = GetBitCoding(length);
			var lengthBytes = new byte[4];

			EncodeLength(bits, lengthBytes);
			Array.Copy(lengthBytes, 0, tagBytes, 6, 4);

			//
			//  Build the tag bytes and start writing.
			//
			if (!input.CanRead)
			{
				var ex = new Id3IOException("Cannot read input stream");
                Logger.LogError(ex);

			    throw ex;
			}
			if (!output.CanWrite)
			{
				var ex = new Id3IOException("Cannot write to output stream");
                Logger.LogError(ex);

			    throw ex;
			}

			WriteToStream(input, output, tagBytes);
		}

		public FileState DetermineTagStatus(Stream audioStream)
		{
			if (audioStream == null)
			{
				var ex = new ArgumentNullException("audioStream");
                Logger.LogError(ex);

			    throw ex;
			}

			if (!audioStream.CanRead)
			{
				var ex = new Id3IOException("Cannot read data stream.");
                Logger.LogError(ex);

			    throw ex;
			}

			if (!audioStream.CanSeek)
			{
				var ex = new Id3TagException("Cannot read ID3v1 tag because the stream does not support seek.");
                Logger.LogError(ex);

			    throw ex;
			}

			if (audioStream.Length < 128)
			{
				var ex =  new Id3IOException("Cannot read ID3v1 tag because the stream is too short");
                Logger.LogError(ex);

			    throw ex;
			}

			bool id3V1Found;
			bool id3V2Found;
			//
			// Search for ID3v2 tags
			//
			using (var reader = new BinaryReader(audioStream))
			{
				//
				// Search for ID3v2 tags
				//
				var headerBytes = new byte[3];
				reader.Read(headerBytes, 0, headerBytes.Length);

				id3V2Found = (headerBytes[0] == 0x49) && (headerBytes[1] == 0x44) && (headerBytes[2] == 0x33);

				//
				// Search for ID3v1 tags
				//
				var tagBytes = new byte[3];
				audioStream.Seek(-128, SeekOrigin.End);
				audioStream.Read(tagBytes, 0, tagBytes.Length);

				id3V1Found = (tagBytes[0] == 0x54) && (tagBytes[1] == 0x41) && (tagBytes[2] == 0x47);
			}

			return new FileState(id3V1Found, id3V2Found);
		}

		public FileState DetermineTagStatus(FileInfo file)
		{
			bool fileExists = file.Exists;
			if (!fileExists)
			{
				var ex = new FileNotFoundException("File " + file.FullName + " not found!.");
                Logger.LogError(ex);

			    throw ex;
			}

			FileStream fs = null;
			FileState state;
			try
			{
				fs = File.Open(file.FullName, FileMode.Open, FileAccess.Read);
				state = DetermineTagStatus(fs);
			}
			catch (Id3TagException idEx)
			{
                Logger.LogError(idEx);
				throw;
			}
			catch (Exception ex)
			{
                Logger.LogError(ex);
				throw new Id3TagException("Unknown Exception during reading.", ex);
			}
			finally
			{
				if (fs != null)
				{
					fs.Close();
					fs.Dispose();
				}
			}

			return state;
		}

		#endregion

		#region Private Helper

		private static void WriteToStream(Stream input, Stream output, byte[] tagBytes)
		{
			try
			{
				//
				// Write the tag
				//
				output.Write(tagBytes, 0, tagBytes.Length);

				//
				// Write the audio content
				//
				byte[] bytes = SuppressTags(input);
				if (bytes != null)
				{
					// No Tag available. Write the already read bytes.
					output.Write(bytes, 0, bytes.Length);
				}
				long length = input.Length;
				Utils.WriteAudioStream(output, input, length);
			}
			catch (Exception ex)
			{
				var ioException =  new Id3IOException("Cannot write Tag.", ex);
                Logger.LogError(ioException);
			    throw ioException;
			}
		}

		private static byte[] BuildId3V4Tag(TagContainer tagContainer)
		{
			TagDescriptorV4 tag = tagContainer.GetId3V24Descriptor();

			byte[] frameBytes = GetFrameBytes(tagContainer);
			//TODO: Implement CRC32 code here...

			byte[] extendedHeaderBytes = GetExtendedHeaderV4(tag);
			byte[] tagHeader = GetTagHeader(tagContainer);
			//TODO: Implement Unsync Code...

			byte[] tagBytes = BuildFinalTag(tagHeader, extendedHeaderBytes, frameBytes, 0, tag.Footer);
			return tagBytes;
		}

		private static byte[] GetExtendedHeaderV4(TagDescriptorV4 descriptor)
		{
			if (!descriptor.ExtendedHeader)
			{
				return new byte[0];
			}

			//
			//  Create a list with dummy bytes for length and flags...
			//

			var bytes = new List<byte> { 0x00, 0x00, 0x00, 0x00, 0x01, 0x00 };

			byte flagByte = 0x00;
			if (descriptor.UpdateTag)
			{
				flagByte |= 0x40;
				bytes.Add(0x00);
			}

			if (descriptor.CrcDataPresent)
			{
				flagByte |= 0x20;
				bytes.Add(0x05);

				//TODO: Check byte array here...
				bytes.AddRange(descriptor.Crc);
			}

			if (descriptor.RestrictionPresent)
			{
				flagByte |= 0x10;
				bytes.Add(0x01);
				bytes.Add(descriptor.Restriction);
			}

			bytes[5] = flagByte;

			byte[] byteArray = bytes.ToArray();
			List<int> bits = GetBitCoding(byteArray.Length);
			var lengthBytes = new byte[4];

			EncodeLength(bits, lengthBytes);
			Array.Copy(lengthBytes, 0, byteArray, 0, 4);

			return byteArray;
		}

		private static byte[] SuppressTags(Stream input)
		{
			var headerBytes = new byte[10];
			input.Read(headerBytes, 0, 10);

			bool id3PatternFound = (headerBytes[0] == 0x49) && (headerBytes[1] == 0x44) && (headerBytes[2] == 0x33);
			if (!id3PatternFound)
			{
				return headerBytes;
			}

			// Ignore the tag
			var sizeBytes = new byte[4];
			Array.Copy(headerBytes, 6, sizeBytes, 0, 4);
			int size = Utils.Convert7BitEncodedToInt32(sizeBytes);
			input.Position = input.Position + size;

			return null;
		}

		private static byte[] BuildId3V3Tag(TagContainer tagContainer)
		{
			byte[] tagBytes;
			TagDescriptorV3 tag = tagContainer.GetId3V23Descriptor();
			byte[] frameBytes = GetFrameBytes(tagContainer);

			//
			//  Calculate the CRC32 value of the frameBytes ( before unsync!)
			//
			if (tag.CrcDataPresent)
			{
				var crc32 = new Crc32(Crc32.DefaultPolynom);
				byte[] crcValue = crc32.Calculate(frameBytes);

				tag.SetCrc32(crcValue);
			}

			//
			//  OK. Build the complete tag
			//
			byte[] extendedHeaderBytes = GetExtendedHeaderV3(tagContainer);
			byte[] tagHeader = GetTagHeader(tagContainer);
			byte[] rawTagBytes = BuildFinalTag(tagHeader, extendedHeaderBytes, frameBytes, tag.PaddingSize, false);
			if (tag.Unsynchronisation)
			{
				tagBytes = AddUnsyncBytes(rawTagBytes);
			}
			else
			{
				tagBytes = rawTagBytes;
			}

			return tagBytes;
		}

		private static byte[] GetExtendedHeaderV3(TagContainer tagContainer)
		{
			var extendedHeaderBytes = new byte[0];
			TagDescriptorV3 tag = tagContainer.GetId3V23Descriptor();

			if (tag.ExtendedHeader)
			{
				int extendedHeaderLength = 0;

				if (tag.CrcDataPresent)
				{
					extendedHeaderLength = 10;
				}
				else
				{
					extendedHeaderLength = 6;
				}

				// Create and set the length
				extendedHeaderBytes = new byte[extendedHeaderLength + 4];
				extendedHeaderBytes[3] = Convert.ToByte(extendedHeaderLength);

				byte[] paddingBytes = BitConverter.GetBytes(tag.PaddingSize);
				Array.Reverse(paddingBytes);
				Array.Copy(paddingBytes, 0, extendedHeaderBytes, 6, 4);
				if (tag.CrcDataPresent)
				{
					extendedHeaderBytes[4] |= 0x80;

					tag.Crc.CopyTo(extendedHeaderBytes, 10);
				}
			}

			return extendedHeaderBytes;
		}

		private static byte[] GetTagHeader(TagContainer tagContainer)
		{
			var tagHeader = new byte[10];
			tagHeader[0] = 0x49;
			tagHeader[1] = 0x44;
			tagHeader[2] = 0x33;

			switch (tagContainer.TagVersion)
			{
				case TagVersion.Id3V23:
					TagDescriptorV3 descriptorV3 = tagContainer.GetId3V23Descriptor();
					tagHeader[3] = 0x03;
					tagHeader[4] = 0x00;

					if (descriptorV3.Unsynchronisation)
					{
						tagHeader[5] |= 0x80;
					}

					if (descriptorV3.ExtendedHeader)
					{
						tagHeader[5] |= 0x40;
					}

					if (descriptorV3.ExperimentalIndicator)
					{
						tagHeader[5] |= 0x20;
					}
					break;

				case TagVersion.Id3V24:
					TagDescriptorV4 descriptorV4 = tagContainer.GetId3V24Descriptor();
					tagHeader[3] = 0x04;
					tagHeader[4] = 0x00;

					if (descriptorV4.Unsynchronisation)
					{
						tagHeader[5] |= 0x80;
					}

					if (descriptorV4.ExtendedHeader)
					{
						tagHeader[5] |= 0x40;
					}

					if (descriptorV4.ExperimentalIndicator)
					{
						tagHeader[5] |= 0x20;
					}

					if (descriptorV4.Footer)
					{
						tagHeader[5] |= 0x10;
					}

					break;
				default:
					var ex = new Id3TagException("Unknown version!");
                    Logger.LogError(ex);

			        throw ex;
			}

			return tagHeader;
		}

		private static byte[] BuildFinalTag(
			byte[] tagHeader, byte[] extendedHeaderBytes, byte[] frameBytes, int padding, bool writeFooter)
		{
			var arrayBuilder = new List<byte>();
			arrayBuilder.AddRange(tagHeader);
			if (extendedHeaderBytes != null)
			{
				arrayBuilder.AddRange(extendedHeaderBytes);
			}
			arrayBuilder.AddRange(frameBytes);

			if (padding != 0)
			{
				for (int i = 0; i < padding; i++)
				{
					arrayBuilder.Add(0x00);
				}
			}

			if (writeFooter)
			{
				//
				//  Copy the tag header and replace the header ( ID3 -> 3DI )
				//
				var footer = new byte[10];
				Array.Copy(tagHeader, footer, tagHeader.Length);

				footer[0] = 0x33;
				footer[1] = 0x44;
				footer[2] = 0x49;

				arrayBuilder.AddRange(footer);
			}

			byte[] tagBytes = arrayBuilder.ToArray();
			return tagBytes;
		}

		private static void EncodeLength(List<int> bits, byte[] lengthBytes)
		{
			int curBytePos = 0;
			int curBitPos = 0;
			foreach (int bitValue in bits)
			{
				if (bitValue == 1)
				{
					byte bitMask = 0;
					switch (curBitPos)
					{
						case 0:
							bitMask = 0x01;
							break;
						case 1:
							bitMask = 0x02;
							break;
						case 2:
							bitMask = 0x04;
							break;
						case 3:
							bitMask = 0x08;
							break;
						case 4:
							bitMask = 0x10;
							break;
						case 5:
							bitMask = 0x20;
							break;
						case 6:
							bitMask = 0x40;
							break;
							//
							//  Bit 7 is alwys zero. 
							//
					}

					lengthBytes[curBytePos] |= bitMask;
				}

				if (curBitPos == 6)
				{
					curBitPos = 0;
					curBytePos++;
				}
				else
				{
					curBitPos++;
				}
			}

			// Switch from LSB to MSB.
			Array.Reverse(lengthBytes);
		}

		private static List<int> GetBitCoding(long size)
		{
			byte[] bytes = BitConverter.GetBytes(size);
			var bits = new List<int>();

			var patterns = new byte[]
			               	{
			               		0x01,
			               		0x02,
			               		0x04,
			               		0x08,
			               		0x10,
			               		0x20,
			               		0x40,
			               		0x80
			               	};

			//
			//  Decode to bits here..
			//
			foreach (byte curByte in bytes)
			{
				foreach (byte curPattern in patterns)
				{
					if ((curByte & curPattern) == curPattern)
					{
						bits.Add(1);
					}
					else
					{
						bits.Add(0);
					}
				}
			}

			return bits;
		}

		private static byte[] GetFrameBytes(TagContainer tagContainer)
		{
			var listBytes = new List<byte>();
			foreach (IFrame frame in tagContainer)
			{
				RawFrame rawFrame = frame.Convert(tagContainer.TagVersion);

				var headerBytes = new byte[10];
				byte[] idBytes = rawFrame.GetIdBytes();
				byte[] lengthBytes = BitConverter.GetBytes(rawFrame.Payload.Count);
				// Convert from LSB to MSB. Better way here??
				Array.Reverse(lengthBytes);
				byte[] flagsBytes = rawFrame.EncodeFlags();

				Array.Copy(idBytes, 0, headerBytes, 0, 4);
				Array.Copy(lengthBytes, 0, headerBytes, 4, 4);
				Array.Copy(flagsBytes, 0, headerBytes, 8, 2);

				listBytes.AddRange(headerBytes);
				listBytes.AddRange(rawFrame.Payload);
			}

			return listBytes.ToArray();
		}

		private static int AnalyseHeader(byte[] headerBytes, Id3TagInfo tagInfo)
		{
			// Check ID3 pattern
			bool id3PatternFound = (headerBytes[0] == 0x49) && (headerBytes[1] == 0x44) && (headerBytes[2] == 0x33);

			if (!id3PatternFound)
			{
				var ex = new Id3HeaderNotFoundException();
                Logger.LogError(ex);

			    throw ex;
			}

			int majorVersion = Convert.ToInt32(headerBytes[3]);
			int revision = Convert.ToInt32(headerBytes[4]);
			byte flagByte = headerBytes[5];
			var sizeBytes = new byte[4];

			// Analyse the header...
			tagInfo.MajorVersion = majorVersion;
			tagInfo.Revision = revision;

			bool unsynchronisationFlag = (flagByte & 0x80) == 0x80;
			bool extendedHeaderFlag = (flagByte & 0x40) == 0x40;
			bool experimentalFlag = (flagByte & 0x20) == 0x20;

			tagInfo.Unsynchronised = unsynchronisationFlag;
			tagInfo.ExtendedHeaderAvailable = extendedHeaderFlag;
			tagInfo.Experimental = experimentalFlag;

			if (majorVersion == 4)
			{
				//
				//  ID3V2.4 tag found! check for footer.
				//

				bool footerFlag = (flagByte & 0x10) == 0x10;
				tagInfo.HasFooter = footerFlag;
			}

			Array.Copy(headerBytes, 6, sizeBytes, 0, 4);
			int size = Utils.Convert7BitEncodedToInt32(sizeBytes);

			return size;
		}

		private static bool ValidateTag(TagContainer tagContainer, out string message)
		{
			Validator validator;
			switch (tagContainer.TagVersion)
			{
				case TagVersion.Id3V23:
					validator = new Id3V2Validator();
					break;
				case TagVersion.Id3V24:
					validator = new Id3V24Validator();
					break;
				default:
					var ex = new Id3TagException("Unknown version!");
                    Logger.LogError(ex);
			        throw ex;
			}

			bool isValid = validator.Validate(tagContainer);

			if (isValid)
			{
				message = String.Empty;
			}
			else
			{
				message = validator.FailureDescription;
			}

			return isValid;
		}

		#endregion

		private static byte[] RemoveUnsyncBytes(IEnumerable<byte> tagContent)
		{
			/*
             *  wenn FF 00 gefunden wird, dann die 00 entfernen
             *  wenn FF am Ende gefunden wird, dann nix machen
             */

			//var counter = 0;
			var filteredBytes = new List<byte>();
			byte previousByte = 0x00;
			foreach (byte curByte in tagContent)
			{
				//
				//  FF E0 darf nicht vorkommen!
				//

				if (previousByte != 0xFF)
				{
					filteredBytes.Add(curByte);
				}
				else
				{
					if (curByte != 0x00)
					{
						filteredBytes.Add(curByte);
					}
					//else
					//{
					//    // For debug purposes!
					//    Debug.WriteLine(String.Format("Unsync Byte found! Counter = {0}", counter));
					//}
				}

				previousByte = curByte;
				//counter++;
			}

			return filteredBytes.ToArray();
		}

		private static byte[] AddUnsyncBytes(byte[] rawTagBytes)
		{
			/*
             *      What to do ?
             * 
             *      1. FF >Ex -> FF 00 >Ex
             *      2. FF 00 -> FF 00 00
             *      3. xx xx ... FF -> No changes!
             */

			var syncedTagBytes = new List<byte>();
			byte previousByte = 0x00;
			foreach (byte curByte in rawTagBytes)
			{
				if (previousByte != 0xFF)
				{
					syncedTagBytes.Add(curByte);
				}
				else
				{
					//
					//  previous byte was 0xFF
					//
					if ((curByte == 0x00) || (curByte > 0xE0))
					{
						if (curByte == 0x00)
						{
							syncedTagBytes.Add(0x00);
							syncedTagBytes.Add(0x00);
						}
						else
						{
							syncedTagBytes.Add(0x00);
							syncedTagBytes.Add(curByte);
						}
					}
					else
					{
						syncedTagBytes.Add(curByte);
					}
				}

				previousByte = curByte;
			}

			return syncedTagBytes.ToArray();
		}

		private static void AnalyseExtendedHeader(BinaryReader reader, Id3TagInfo tagInfo)
		{
			var extendedHeaderSize = new byte[4];
			reader.Read(extendedHeaderSize, 0, 4);

			int size = Utils.CalculateSize(extendedHeaderSize);
			byte[] content;

			ExtendedHeader extendedHeader;
			switch (tagInfo.MajorVersion)
			{
				case 3:
					//
					// Read the ID3v2.3 extended header
					//
					content = new byte[size];
					reader.Read(content, 0, size);
					extendedHeader = ExtendedTagHeaderV3.Create(content);
					break;
				case 4:
					//
					//  Read the ID3v2.4 extended header
					//
					// We already read the length of the header...
					size = size - 4;

					content = new byte[size];
					reader.Read(content, 0, size);
					extendedHeader = ExtendedTagHeaderV4.Create(content);
					break;
				default:
					var ex = new Id3TagException("Unknown extended header found! ");
                    Logger.LogError(ex);
			        throw ex;
			}

			tagInfo.ExtendedHeader = extendedHeader;
		}

		private static bool ReadContent(BinaryReader reader, Id3TagInfo tagInfo, List<byte> frameBytes)
		{
			/*
             * 3.4.   ID3v2 footer

               To speed up the process of locating an ID3v2 tag when searching from
               the end of a file, a footer can be added to the tag. It is REQUIRED
               to add a footer to an appended tag, i.e. a tag located after all
               audio data. The footer is a copy of the header, but with a different
               identifier.

                 ID3v2 identifier           "3DI"
                 ID3v2 version              $04 00
                 ID3v2 flags                %abcd0000
                 ID3v2 size             4 * %0xxxxxxx

             */

			//
			//  Read the header ( footer or frame )
			//
			var frameHeader = new byte[10];
			reader.Read(frameHeader, 0, 10);

			bool isFooter = frameHeader[0] == 0x33 && frameHeader[1] == 0x44 && frameHeader[2] == 0x49;
			if (!isFooter)
			{
				//
				//  Frame found!
				//
				var frameIdBytes = new byte[4];
				var sizeBytes = new byte[4];
				var flagsBytes = new byte[2];

				Array.Copy(frameHeader, 0, frameIdBytes, 0, 4);
				Array.Copy(frameHeader, 4, sizeBytes, 0, 4);
				Array.Copy(frameHeader, 8, flagsBytes, 0, 2);

				if (frameIdBytes[0] == 0 ||
				    frameIdBytes[1] == 0 ||
				    frameIdBytes[2] == 0 ||
				    frameIdBytes[3] == 0)
				{
					// No valid frame. Padding bytes?
					return false;
				}

				//
				//  Read the frame bytes
				//
				string frameId = Encoding.ASCII.GetString(frameIdBytes);
				int size = Utils.CalculateSize(sizeBytes);
                Logger.LogInfo(String.Format("Frame found : ID = {0}, Size = {1}",frameId,size));

				long bytesLeft = reader.BaseStream.Length - reader.BaseStream.Position;
				if (size > bytesLeft)
				{
					var ex = new Id3TagException(
						String.Format(
							CultureInfo.InvariantCulture, "Specified frame size {0} exceeds actual frame size {1}", size, bytesLeft));

                    Logger.LogError(ex);
				    throw ex;
				}

				var payloadBytes = new byte[size];
				reader.Read(payloadBytes, 0, size);

				RawFrame frame;
				switch (tagInfo.MajorVersion)
				{
					case 3:
						frame = RawFrame.CreateV3Frame(frameId, flagsBytes, payloadBytes);
						break;
					case 4:
						frame = RawFrame.CreateV4Frame(frameId, flagsBytes, payloadBytes);
						break;
					default:
						var ex = new Id3TagException("Not supported major revision found!");
                        Logger.LogError(ex);

				        throw ex;
				}

				tagInfo.Frames.Add(frame);

				// Add the frames to the buffer ( for CRC computing )
				frameBytes.AddRange(frameHeader);
				frameBytes.AddRange(payloadBytes);

				return true;
			}

			return false;
		}
	}
}