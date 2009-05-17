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

            // Analyse the content
            var tagInfo = new Id3TagInfo();
            using (var reader = new BinaryReader(inputStream))
            {
                var headerBytes = new byte[10];
                reader.Read(headerBytes, 0, 10);

                // OK. Start with reading the header
                long curPos = 0;
                var tagLength = AnalyseHeader(headerBytes, tagInfo);

                if (tagInfo.ExtendedHeaderAvailable)
                {
                    // Read the extended header
                    var extendedHeaderByteCount = AnalyseExtendedHeader(reader, tagInfo);
                    curPos += extendedHeaderByteCount;
                }

                //
                //  Read all bytes until EOF.
                //
                while (curPos + 10 < tagLength)
                {
                    bool abort;
                    var frameByteCount = AnalyseFrames(reader, tagInfo, out abort);
                    curPos += frameByteCount;

                    if (abort)
                    {
                        //
                        //  Analyse Frames detects a header with a "null byte" ID. 
                        //
                        break;
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

            byte[] extendedHeaderBytes;
            var tagHeader = GetTagHeader(tagContainer);
            var extendedHeaderLength = GetExtendedHeaderLength(tagContainer, out extendedHeaderBytes);
            var frameBytes = GetFrameBytes(tagContainer);

            //
            //  encode the length
            //
            var length = frameBytes.LongLength;
            if (tagContainer.Tag.ExtendedHeader)
            {
                // Header + Size Coding.
                length += extendedHeaderLength + 4;
            }

            var bits = GetBitCoding(length);
            var lengthBytes = new byte[4];

            EncodeLength(bits, lengthBytes);
            Array.Copy(lengthBytes, 0, tagHeader, 6, 4);

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

            var tagBytes = BuildTag(tagHeader, extendedHeaderBytes, frameBytes, tagContainer.Tag.PaddingSize);
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

                var buffer = new byte[64000];
                while (input.Position < input.Length)
                {
                    var diff = input.Length - input.Position;
                    if (diff < buffer.Length)
                    {
                        // Read the rest.
                        var count = Convert.ToInt32(diff);
                        input.Read(buffer, 0, count);
                        output.Write(buffer, 0, count);
                    }
                    else
                    {
                        // Read the next 64K Byte buffer.
                        input.Read(buffer, 0, buffer.Length);
                        output.Write(buffer, 0, buffer.Length);
                    }
                }
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
                var flagsBytes = rawFrame.GetFlags();

                Array.Copy(idBytes, 0, headerBytes, 0, 4);
                Array.Copy(lengthBytes, 0, headerBytes, 4, 4);
                Array.Copy(flagsBytes, 0, headerBytes, 8, 2);

                listBytes.AddRange(headerBytes);
                listBytes.AddRange(rawFrame.Payload);
            }

            return listBytes.ToArray();
        }

        private static long AnalyseExtendedHeader(BinaryReader reader, Id3TagInfo tagInfo)
        {
            int size;

            try
            {
                // Read the extended header size
                var extendedHeaderSize = new byte[4];
                reader.Read(extendedHeaderSize, 0, 4);

                size = Convert.ToInt32(Utils.CalculateExtendedHeaderSize(extendedHeaderSize));
                var content = new byte[size];
                reader.Read(content, 0, size);

                var extendedHeader = ExtendedTagHeader.Create(content);
                tagInfo.ExtendHeader = extendedHeader;
            }
            catch (Exception ex)
            {
                throw new ID3TagException("Could not analyse Extended Header", ex);
            }

            return 4 + size;
        }

        private static long AnalyseFrames(BinaryReader reader, Id3TagInfo tagInfo, out bool nullBytesDetected)
        {
            long size = 0;
            var abortAnalysing = false;

            try
            {
                var frameHeader = new byte[10];
                reader.Read(frameHeader, 0, 10);

                var frameIDBytes = new byte[4];
                var sizeBytes = new byte[4];
                var flagsBytes = new byte[2];

                Array.Copy(frameHeader, 0, frameIDBytes, 0, 4);
                Array.Copy(frameHeader, 4, sizeBytes, 0, 4);
                Array.Copy(frameHeader, 8, flagsBytes, 0, 2);

                var frameID = Utils.GetFrameID(frameIDBytes);

                if (frameIDBytes[0] == 0x00 && frameIDBytes[1] == 0x00 && frameIDBytes[2] == 0x00)
                {
                    // Invalid frame. Discard it.
                    abortAnalysing = true;
                }
                else
                {
                    size = Utils.CalculateFrameHeaderSize(sizeBytes);
                    var payloadBytes = new byte[size];
                    reader.Read(payloadBytes, 0, (int) size);

                    var frame = RawFrame.CreateFrame(frameID, flagsBytes, payloadBytes);
                    tagInfo.Frames.Add(frame);
                }
            }
            catch (Exception ex)
            {
                throw new ID3TagException("Could not analyse frame", ex);
            }

            nullBytesDetected = abortAnalysing;
            return 10 + size;
        }

        private static long AnalyseHeader(byte[] headerBytes, Id3TagInfo tagInfo)
        {
            // Check ID3 pattern
            var id3PatternFound = (headerBytes[0] == 0x49) && (headerBytes[1] == 0x44) && (headerBytes[2] == 0x33);

            if (id3PatternFound)
            {
                long size;

                try
                {
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

                    Array.Copy(headerBytes, 6, sizeBytes, 0, 4);
                    size = Utils.CalculateTagHeaderSize(sizeBytes);
                }
                catch (Exception ex)
                {
                    throw new ID3TagException("Could not analyse header", ex);
                }

                return size;
            }
            else
            {
                // Check for ID3v1
                var tagPatternFound = (headerBytes[0] == 0x54) && (headerBytes[1] == 0x41) && (headerBytes[2] == 0x47);
                if (tagPatternFound)
                {
                    throw new InvalidTagFormatException("ID3v1 tag is not supported");
                }

                throw new ID3HeaderNotFoundException();
            }
        }

        #endregion
    }
}