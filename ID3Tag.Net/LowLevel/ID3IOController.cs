using System;
using System.Collections.Generic;
using System.IO;
using ID3Tag.HighLevel;

namespace ID3Tag.LowLevel
{
    internal class ID3IOController : IID3IOController
    {
        #region IID3IOController Members

        public ID3TagInfo Read(FileInfo file)
        {
            var fileExists = file.Exists;
            if (!fileExists)
            {
                throw new FileNotFoundException("File " + file.FullName + " not found!.");
            }

            FileStream fs = null;
            ID3TagInfo info;
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
                }
            }

            return info;
        }

        public ID3TagInfo Read(Stream inputStream)
        {
            var tagInfo = new ID3TagInfo();

            // Analyse the content
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

        public void Write(TagContainer tagContainer, FileInfo file)
        {
            var frameBytes = GetFrameBytes(tagContainer);

            //
            //  Build the header
            //
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

            //
            //  Determine the length
            //
            //TODO: Hier...

            //
            //  ExtendedHeader
            //
            //TODO: Hier ...


        }

        private byte[] GetFrameBytes(TagContainer tagContainer)
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

                Array.Copy(idBytes,0,headerBytes,0,4);
                Array.Copy(lengthBytes, 0, headerBytes, 4, 4);
                Array.Copy(flagsBytes,0,headerBytes,8,2);

                listBytes.AddRange(headerBytes);
                listBytes.AddRange(rawFrame.Payload);
            }

            return listBytes.ToArray();
        }

        #endregion

        private static long AnalyseExtendedHeader(BinaryReader reader, ID3TagInfo tagInfo)
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

        private static long AnalyseFrames(BinaryReader reader, ID3TagInfo tagInfo, out bool nullBytesDetected)
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

        private static long AnalyseHeader(byte[] headerBytes, ID3TagInfo tagInfo)
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
    }
}