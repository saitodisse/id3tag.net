using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ID3Tag.HighLevel
{
    internal class Id3V1Controller : IId3V1Controller
    {
        #region IId3V1Controller Members

        public Id3V1Tag Read(Stream inputStream)
        {
            if (!inputStream.CanSeek)
            {
                throw new ID3TagException("Cannot read ID3v1 tag because the stream does not support seek.");
            }

            if (inputStream.Length < 128)
            {
                throw new ID3IOException("Cannot read ID3v1 tag because the stream is too short");
            }

            //
            //  Read the last 128 Bytes from the stream (ID3v1 Position)
            //
            var tagBytes = new byte[128];
            inputStream.Seek(-128, SeekOrigin.End);
            inputStream.Read(tagBytes, 0, 128);

            var isValidTag = CheckID(tagBytes);
            if (!isValidTag)
            {
                throw new ID3HeaderNotFoundException("TAG header not found");
            }

            var v1Tag = ExtractTag(tagBytes);
            return v1Tag;
        }

        public Id3V1Tag Read(FileInfo file)
        {
            var fileExists = file.Exists;
            if (!fileExists)
            {
                throw new FileNotFoundException("File " + file.FullName + " not found!.");
            }

            FileStream fs = null;
            Id3V1Tag info;
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

        public void Write(Id3V1Tag tag, Stream input, Stream output)
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
                throw new ID3TagException("Cannot write ID3V1 tag because the source does not support seek.");
            }

            if (!output.CanWrite)
            {
                throw new ID3TagException("Cannot write ID3V1 tag because the output does not support writing.");
            }

            try
            {
                //
                //  Read the last 128 Bytes from the stream (ID3v1 Position)
                //
                var audioBytesCount = GetAudioBytesCount(input);

                //
                //  Write the audio data and tag
                //
                input.Seek(0, SeekOrigin.Begin);
                Utils.WriteAudioStream(output, input, audioBytesCount);

                var tagBytes = ConvertToByte(tag);
                output.Write(tagBytes, 0, tagBytes.Length);
            }
            catch (Exception ex)
            {
                throw new ID3IOException("Cannot write ID3v1 tag", ex);
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

                var id3TagFound = CheckID(tagBytes);
                if (id3TagFound)
                {
                    // Ignore the ID3Tag from the source
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

        private byte[] ConvertToByte(Id3V1Tag tag)
        {
            var tagBytes = new byte[128];
            // Write the tag ID ( TAG)
            tagBytes[0] = 0x54;
            tagBytes[1] = 0x41;
            tagBytes[2] = 0x47;

            // Write the fields...
            var titleBytes = GetField(tag.Title, 30);
            var artistBytes = GetField(tag.Artist, 30);
            var albumBytes = GetField(tag.Album, 30);
            var year = GetField(tag.Year, 4);

            Array.Copy(titleBytes, 0, tagBytes, 3, 30);
            Array.Copy(artistBytes, 0, tagBytes, 33, 30);
            Array.Copy(albumBytes, 0, tagBytes, 63, 30);
            Array.Copy(year, 0, tagBytes, 93, 4);

            byte[] commentBytes;
            if (tag.IsID3V1_1Compliant)
            {
                commentBytes = GetField(tag.Comment, 28);
                Array.Copy(commentBytes, 0, tagBytes, 97, 28);

                var trackNr = tag.TrackNr;
                tagBytes[125] = 0x00;
                tagBytes[126] = Convert.ToByte(trackNr);
            }
            else
            {
                commentBytes = GetField(tag.Comment, 30);
                Array.Copy(commentBytes, 0, tagBytes, 97, 30);
            }

            // Add genre
            tagBytes[127] = Convert.ToByte(tag.GenreIdentifier);

            return tagBytes;
        }

        private byte[] GetField(string value, int size)
        {
            var valueBytes = Encoding.ASCII.GetBytes(value);
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
                    var fieldCount = fieldBytes.Length;
                    Array.Copy(valueBytes, fieldBytes, valueBytes.Length);

                    for (var i = valueBytes.Length; i < fieldCount; i++)
                    {
                        // Add Space code
                        fieldBytes[i] = 0x20;
                    }
                }
            }

            return fieldBytes;
        }

        private Id3V1Tag ExtractTag(byte[] tagBytes)
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
            var genreByte = tagBytes[127];

            var title = GetString(titleBytes);
            var artits = GetString(artistBytes);
            var album = GetString(albumBytes);
            var year = GetString(yearBytes);

            var id3v1_1Support = ((commentBytes[28] == 0) && (commentBytes[29] != 0));
            var trackNr = String.Empty;
            var comment = String.Empty;

            if (id3v1_1Support)
            {
                var trackNrValue = commentBytes[29];
                trackNr = Convert.ToString(trackNrValue);

                var newComments = new byte[28];
                Array.Copy(commentBytes, 0, newComments, 0, newComments.Length);

                comment = GetString(newComments);
            }
            else
            {
                comment = GetString(commentBytes);
            }

            var id3v1 = new Id3V1Tag
                            {
                                Title = title,
                                Artist = artits,
                                Album = album,
                                Year = year,
                                Comment = comment,
                                GenreIdentifier = genreByte,
                                IsID3V1_1Compliant = id3v1_1Support,
                                TrackNr = trackNr
                            };

            return id3v1;
        }

        private static string GetString(IEnumerable<byte> array)
        {
            var sb = new StringBuilder();

            foreach (var byteValue in array)
            {
                if (byteValue != 0)
                {
                    var vharValue = Convert.ToChar(byteValue);
                    sb.Append(vharValue);
                }
            }

            var stringValue = sb.ToString();
            return stringValue.Trim();
        }

        private static bool CheckID(byte[] tag)
        {
            // TAG
            return (tag[0] == 0x54) && (tag[1] == 0x41) && (tag[2] == 0x47);
        }

        #endregion
    }
}