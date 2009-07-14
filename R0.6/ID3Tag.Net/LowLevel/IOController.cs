using System;
using System.Collections.Generic;
using System.IO;
using ID3Tag.HighLevel;

namespace ID3Tag.LowLevel
{
    internal class IoController : IIoController
    {
        #region IIoController Members

        public Id3TagInfo Read(FileInfo file)
        {
            var fileExists = file.Exists;
            if (!fileExists)
            {
                throw new FileNotFoundException("File " + file.FullName + " not found!.");
            }

            FileStream fs = null;
            Id3TagInfo info;
            try
            {
                fs = File.Open(file.FullName, FileMode.Open);

                info = Read(fs);
            }
            catch (ID3TagException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ID3TagException("Unknown Exception during reading.", ex);
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
                throw new ArgumentNullException("inputStream");
            }

            if (!inputStream.CanRead)
            {
                throw new ID3IOException("Cannot read data stream.");
            }

            //
            //  Read the bytes from the I/O stream.
            //
            var tagInfo = new Id3TagInfo();
            byte[] rawTagContent;

            using (var reader = new BinaryReader(inputStream))
            {
                var headerBytes = new byte[10];
                reader.Read(headerBytes, 0, 10);

                var rawTagLength = AnalyseHeader(headerBytes, tagInfo);
                rawTagContent = new byte[rawTagLength];

                reader.Read(rawTagContent, 0, rawTagLength);
            }

            //
            //  Check for Unsynchronisation Bytes
            //
            byte[] tagContent;
            if (tagInfo.UnsynchronisationFlag)
            {
                // Scan for unsynchronisation bytes!
                tagContent = RemoveUnsyncBytes(rawTagContent);
            }
            else
            {
                tagContent = rawTagContent;
            }

            Stream tagStream = new MemoryStream(tagContent);
            var length = tagContent.Length;
            using (var reader = new BinaryReader(tagStream))
            {
                //
                //  Check for Extended Header
                //
                if (tagInfo.ExtendedHeaderAvailable)
                {
                    AnalyseExtendedHeader(reader, tagInfo);
                }

                //
                //  Read the content
                //
                var frameBytes = new List<byte>();
                var pos = reader.BaseStream.Position;
                while ((pos + 10) < length)
                {
                    var continueReading = ReadContent(reader, tagInfo, frameBytes);
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
                    var tagData = frameBytes.ToArray();
                    var crc32Value = tagInfo.ExtendedHeader.Crc32;

                    var crc32 = new Crc32(Crc32.DefaultPolynom);
                    var crcOk = crc32.Validate(tagData, crc32Value);

                    if (!crcOk)
                    {
                        throw new ID3TagException("The CRC32 validation failed!");
                    }
                }
            }

            return tagInfo;
        }

        public void Write(TagContainer tagContainer, Stream input, Stream output)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            if (output == null)
            {
                throw new ArgumentNullException("output");
            }
            if (tagContainer == null)
            {
                throw new ArgumentNullException("tagContainer");
            }

            //
            //  Validate whether the tag container is in ID3V2.3 formaz
            //
            string message;
            var isTagValid = ValidateTag(tagContainer, out message);
            if (!isTagValid)
            {
                throw new InvalidID3StructureException(message);
            }

            //
            //  OK. ID3Tag is valid. Let's write the tag.
            //
            //  Calculate the CRC32 value of the frameBytes ( before unsync!)
            //
            byte[] extendedHeaderBytes;
            var tagHeader = GetTagHeader(tagContainer);
            var frameBytes = GetFrameBytes(tagContainer);

            if (tagContainer.Tag.CrcDataPresent)
            {
                var crc32 = new Crc32(Crc32.DefaultPolynom);
                var crcValue = crc32.Calculate(frameBytes);

                tagContainer.Tag.SetCrc32(crcValue);
            }

            //
            //  OK. Build the complete tag
            //
            byte[] tagBytes;
            var extendedHeaderLength = GetExtendedHeaderLength(tagContainer, out extendedHeaderBytes);
            var rawTagBytes = BuildTag(tagHeader, extendedHeaderBytes, frameBytes, tagContainer.Tag.PaddingSize);
            if (tagContainer.Tag.Unsynchronisation)
            {
                tagBytes = AddUnsyncBytes(rawTagBytes);
            }
            else
            {
                tagBytes = rawTagBytes;
            }

            //
            //  encode the length
            //
            var length = tagBytes.LongLength - 10;
            //if (tagContainer.Tag.ExtendedHeader)
            //{
            //    // Header + Size Coding.
            //    length += extendedHeaderLength + 4;
            //}

            var bits = GetBitCoding(length);
            var lengthBytes = new byte[4];

            EncodeLength(bits, lengthBytes);
            Array.Copy(lengthBytes, 0, tagBytes, 6, 4);

            //
            //  Build the tag bytes and start writing.
            //
            if (!input.CanRead)
            {
                throw new ID3IOException("Cannot read input stream");
            }
            if (!output.CanWrite)
            {
                throw new ID3IOException("Cannot write to output stream");
            }

            WriteToStream(input, output, tagBytes);
        }

        public FileState DetermineTagStatus(Stream audioStream)
        {
            if (audioStream == null)
            {
                throw new ArgumentNullException("audioStream");
            }

            if (!audioStream.CanRead)
            {
                throw new ID3IOException("Cannot read data stream.");
            }

            if (!audioStream.CanSeek)
            {
                throw new ID3TagException("Cannot read ID3v1 tag because the stream does not support seek.");
            }

            if (audioStream.Length < 128)
            {
                throw new ID3IOException("Cannot read ID3v1 tag because the stream is too short");
            }

            var id3V1Found = false;
            var id3V2Found = false;
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
            var fileExists = file.Exists;
            if (!fileExists)
            {
                throw new FileNotFoundException("File " + file.FullName + " not found!.");
            }

            FileStream fs = null;
            FileState state;
            try
            {
                fs = File.Open(file.FullName, FileMode.Open);
                state = DetermineTagStatus(fs);
            }
            catch (ID3TagException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ID3TagException("Unknown Exception during reading.", ex);
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
                var bytes = SuppressTags(input);
                if (bytes != null)
                {
                    // No Tag available. Write the already read bytes.
                    output.Write(bytes, 0, bytes.Length);
                }
                var length = input.Length;
                Utils.WriteAudioStream(output, input, length);
            }
            catch (Exception ex)
            {
                throw new ID3IOException("Cannot write Tag.", ex);
            }
        }

        private static byte[] SuppressTags(Stream input)
        {
            var headerBytes = new byte[10];
            input.Read(headerBytes, 0, 10);

            var id3PatternFound = (headerBytes[0] == 0x49) && (headerBytes[1] == 0x44) && (headerBytes[2] == 0x33);
            if (id3PatternFound)
            {
                // Ignore the tag
                var sizeBytes = new byte[4];
                Array.Copy(headerBytes, 6, sizeBytes, 0, 4);
                var size = Utils.CalculateTagHeaderSize(sizeBytes);
                input.Position = input.Position + size;
                return null;
            }
            else
            {
                return headerBytes;
            }
        }

        private static int GetExtendedHeaderLength(TagContainer tagContainer, out byte[] extendedHeaderBytes)
        {
            extendedHeaderBytes = null;
            var extendedHeaderLength = 0;
            if (tagContainer.Tag.ExtendedHeader)
            {
                if (tagContainer.Tag.CrcDataPresent)
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

                var paddingBytes = BitConverter.GetBytes(tagContainer.Tag.PaddingSize);
                Array.Reverse(paddingBytes);
                Array.Copy(paddingBytes, 0, extendedHeaderBytes, 6, 4);
                if (tagContainer.Tag.CrcDataPresent)
                {
                    extendedHeaderBytes[4] |= 0x80;
                    Array.Copy(tagContainer.Tag.Crc, 0, extendedHeaderBytes, 10, 4);
                }
            }
            return extendedHeaderLength;
        }

        private static byte[] GetTagHeader(TagContainer tagContainer)
        {
            var tagHeader = new byte[10];
            tagHeader[0] = 0x49;
            tagHeader[1] = 0x44;
            tagHeader[2] = 0x33;
            tagHeader[3] = Convert.ToByte(tagContainer.Tag.MajorVersion);
            tagHeader[4] = Convert.ToByte(tagContainer.Tag.Revision);

            if (tagContainer.Tag.Unsynchronisation)
            {
                tagHeader[5] |= 0x80;
            }

            if (tagContainer.Tag.ExtendedHeader)
            {
                tagHeader[5] |= 0x40;
            }

            if (tagContainer.Tag.ExperimentalIndicator)
            {
                tagHeader[5] |= 0x20;
            }
            return tagHeader;
        }

        private static byte[] BuildTag(byte[] tagHeader, byte[] extendedHeaderBytes, byte[] frameBytes, int padding)
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
                for (var i = 0; i < padding; i++)
                {
                    arrayBuilder.Add(0x00);
                }
            }

            var tagBytes = arrayBuilder.ToArray();
            return tagBytes;
        }

        private static void EncodeLength(List<int> bits, byte[] lengthBytes)
        {
            var curBytePos = 0;
            var curBitPos = 0;
            foreach (var bitValue in bits)
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
            var bytes = BitConverter.GetBytes(size);
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
            foreach (var curByte in bytes)
            {
                foreach (var curPattern in patterns)
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
            foreach (var frame in tagContainer)
            {
                var rawFrame = frame.Convert();

                var headerBytes = new byte[10];
                var idBytes = rawFrame.GetIDBytes();
                var lengthBytes = BitConverter.GetBytes(rawFrame.Payload.Length);
                // Convert from LSB to MSB. Better way here??
                Array.Reverse(lengthBytes);
                var flagsBytes = rawFrame.EncodeFlags();

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
            var id3PatternFound = (headerBytes[0] == 0x49) && (headerBytes[1] == 0x44) && (headerBytes[2] == 0x33);

            if (!id3PatternFound)
            {
                throw new ID3HeaderNotFoundException();
            }

            var majorVersion = Convert.ToInt32(headerBytes[3]);
            var revision = Convert.ToInt32(headerBytes[4]);
            var flagByte = headerBytes[5];
            var sizeBytes = new byte[4];

            // Analyse the header...
            tagInfo.MajorVersion = majorVersion;
            tagInfo.Revision = revision;

            var unsynchronisationFlag = (flagByte & 0x80) == 0x80;
            var extendedHeaderFlag = (flagByte & 0x40) == 0x40;
            var experimentalFlag = (flagByte & 0x20) == 0x20;

            tagInfo.UnsynchronisationFlag = unsynchronisationFlag;
            tagInfo.ExtendedHeaderAvailable = extendedHeaderFlag;
            tagInfo.Experimental = experimentalFlag;

            if (majorVersion == 4)
            {
                //
                //  ID3V2.4 tag found! check for footer.
                //

                var footerFlag = (flagByte & 0x10) == 0x10;
                tagInfo.FooterFlag = footerFlag;
            }

            Array.Copy(headerBytes, 6, sizeBytes, 0, 4);
            var size = Utils.CalculateTagHeaderSize(sizeBytes);

            return size;
        }

        private static bool ValidateTag(TagContainer tagContainer, out string message)
        {
            Validator validator = new Id3V2Validator();
            var isValid = validator.Validate(tagContainer);

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

        private static byte[] RemoveUnsyncBytes(byte[] tagContent)
        {
            /*
             *  wenn FF 00 gefunden wird, dann die 00 entfernen
             *  wenn FF am Ende gefunden wird, dann nix machen
             */

            var counter = 0;
            var filteredBytes = new List<byte>();
            byte previousByte = 0x00;
            foreach (var curByte in tagContent)
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
                counter++;
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
            foreach (var curByte in rawTagBytes)
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

            var size = Utils.CalculateExtendedHeaderSize(extendedHeaderSize);
            byte[] content;

            switch (tagInfo.MajorVersion)
            {
                case 3:
                    //
                    // Read the ID3v2.3 extended header
                    //
                    content = new byte[size];
                    reader.Read(content, 0, size);

                    var extendedHeader = ExtendedTagHeaderV3.Create(content);
                    tagInfo.ExtendedHeader = extendedHeader;
                    break;
                case 4:
                    //
                    //  Read the ID3v2.4 extended header
                    //
                    // We already read the length of the header...
                    size = size - 4;

                    content = new byte[size];
                    reader.Read(content, 0, size);

                    var extendedHeaderv4 = ExtendedTagHeaderV4.Create(content);
                    tagInfo.ExtendedHeader = extendedHeaderv4;
                    break;
                default:
                    throw new ID3TagException("Unknown extended header found! ");
            }
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

            var isFooter = frameHeader[0] == 0x33 && frameHeader[1] == 0x44 && frameHeader[2] == 0x49;
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
                var frameID = Utils.GetFrameID(frameIdBytes);
                var size = Utils.CalculateFrameHeaderSize(sizeBytes);
                var payloadBytes = new byte[size];
                reader.Read(payloadBytes, 0, (int)size);

                var frame = RawFrame.CreateFrame(frameID, flagsBytes, payloadBytes);
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