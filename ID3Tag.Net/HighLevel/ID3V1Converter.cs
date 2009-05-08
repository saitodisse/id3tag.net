using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ID3Tag.HighLevel.ID3Frame;

namespace ID3Tag.HighLevel
{
    internal class Id3V1Converter : IId3V1Converter
    {
        private readonly Dictionary<int, string> m_GenreDict;

        internal Id3V1Converter()
        {
            m_GenreDict = GetDictionary();
        }

        #region IId3V1Converter Members

        public TagContainer Convert(Stream inputStream)
        {
            if (!inputStream.CanSeek)
            {
                throw new ID3TagException("Cannot read ID3v1 tag because the stream does not support seek.");
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
            var tagContainer = new TagContainer();

            var titleFrame = CreateTextFrame(v1Tag.Title, "TIT2");
            var artistFrame = CreateTextFrame(v1Tag.Artist, "TPE2");
            var albumFrame = CreateTextFrame(v1Tag.Album, "TALB");
            var yearFrame = CreateTextFrame(v1Tag.Year, "TYER");
            var genreFrame = CreateTextFrame(v1Tag.Genre, "TCON");

            tagContainer.Add(titleFrame);
            tagContainer.Add(artistFrame);
            tagContainer.Add(albumFrame);
            tagContainer.Add(yearFrame);
            tagContainer.Add(genreFrame);

            if (v1Tag.Comment.Length > 0)
            {
                var descrValue = "Comment";
                var commentValue = v1Tag.Comment;
                var commentFrame = new UserDefinedTextFrame
                                       {
                                           TextEncoding = TextEncodingType.ISO_8859_1,
                                           Description = descrValue,
                                           Value = commentValue,
                                           Descriptor = {ID = "TXXX"}
                                       };
                tagContainer.Add(commentFrame);
            }

            // Add the track if the tag supports version 1.1
            var id3v1_1compliant = v1Tag.IsID3V1_1Compliant;
            if (id3v1_1compliant)
            {
                var trackNr = CreateTextFrame(v1Tag.TrackNr, "TRCK");
                tagContainer.Add(trackNr);
            }

            return tagContainer;
        }

        public TagContainer Convert(FileInfo file)
        {
            var fileExists = file.Exists;
            if (!fileExists)
            {
                throw new FileNotFoundException("File " + file.FullName + " not found!.");
            }

            FileStream fs = null;
            TagContainer info;
            try
            {
                fs = File.Open(file.FullName, FileMode.Open);
                info = Convert(fs);
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

        #endregion

        #region Private helper

        private static TextFrame CreateTextFrame(string content, string frameID)
        {
            var frame = new TextFrame
                            {
                                TextEncoding = TextEncodingType.ISO_8859_1,
                                Content = content,
                                Descriptor = {ID = frameID}
                            };

            return frame;
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
            var comment = GetString(commentBytes);
            string genre;

            if (m_GenreDict.ContainsKey(genreByte))
            {
                genre = m_GenreDict[genreByte];
            }
            else
            {
                // Keine Ahnung.. nimm einfach den ersten...
                genre = m_GenreDict[0];
            }

            var id3v1_1Support = ((commentBytes[28] == 0) && (commentBytes[29] != 0));
            var trackNr = String.Empty;
            if (id3v1_1Support)
            {
                trackNr = commentBytes[29].ToString();
            }

            var id3v1 = new Id3V1Tag
                            {
                                Title = title,
                                Artist = artits,
                                Album = album,
                                Year = year,
                                Comment = comment,
                                Genre = String.Format("({0}){1}", genreByte, genre),
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
                    var vharValue = System.Convert.ToChar(byteValue);
                    sb.Append(vharValue);
                }
            }

            return sb.ToString();
        }

        private static bool CheckID(byte[] tag)
        {
            // TAG
            return (tag[0] == 0x54) && (tag[1] == 0x41) && (tag[2] == 0x47);
        }

        private static Dictionary<int, string> GetDictionary()
        {
            var genres = new Dictionary<int, string>
                             {
                                 {0, "Blues"},
                                 {1, "Classic Rock"},
                                 {2, "Country"},
                                 {3, "Dance"},
                                 {4, "Disco"},
                                 {5, "Funk"},
                                 {6, "Grunge"},
                                 {7, "Hip-Hop"},
                                 {8, "Jazz"},
                                 {9, "Metal"},
                                 {10, "New Age"},
                                 {11, "Oldies"},
                                 {12, "Other"},
                                 {13, "Pop"},
                                 {14, "R&B"},
                                 {15, "RAP"},
                                 {16, "Reggae"},
                                 {17, "Rock"},
                                 {18, "Techo"},
                                 {19, "Industrial"},
                                 {20, "Alternative"},
                                 {21, "Ska"},
                                 {22, "Death Metal"},
                                 {23, "Pranks"},
                                 {24, "Soundtrack"},
                                 {25, "Euro-Techno"},
                                 {26, "Ambient"},
                                 {27, "Trip-Hop"},
                                 {28, "Vocal"},
                                 {29, "Jazz&Funk"},
                                 {30, "Fusion"},
                                 {31, "Trance"},
                                 {32, "Classical"},
                                 {33, "Instrumental"},
                                 {34, "Acid"},
                                 {35, "House"},
                                 {36, "Game"},
                                 {37, "Sound Clip"},
                                 {38, "Gospel"},
                                 {39, "Noise"},
                                 {40, "Alternative Rock"},
                                 {41, "Bass"},
                                 {42, "Soul"},
                                 {43, "Punk"},
                                 {44, "Space"},
                                 {45, "Meditative"},
                                 {46, "Instrumental Pop"},
                                 {47, "Instrumental Rock"},
                                 {48, "Ethnic"},
                                 {49, "Gothic"},
                                 {50, "Darkwave"},
                                 {51, "Techo-Industrial"},
                                 {52, "Electronic"},
                                 {53, "Pop-Folk"},
                                 {54, "Eurodance"},
                                 {55, "Dream"},
                                 {56, "Southern Rock"},
                                 {57, "Comedy"},
                                 {58, "Cult"},
                                 {59, "Gangsta"},
                                 {60, "Top 40"},
                                 {61, "Christian Rap"},
                                 {62, "Pop/Funk"},
                                 {63, "Jungle"},
                                 {64, "Native US"},
                                 {65, "Cabaret"},
                                 {66, "New Wave"},
                                 {67, "Psychodelic"},
                                 {68, "Rave"},
                                 {69, "Showtunes"},
                                 {70, "Trailer"},
                                 {71, "Lo-Fi"},
                                 {72, "Tribal"},
                                 {73, "Acid Punk"},
                                 {74, "Acid Jazz"},
                                 {75, "Polka"},
                                 {76, "Retro"},
                                 {77, "Musical"},
                                 {78, "Rock & Roll"},
                                 {79, "Hard Rock"},
                                 {80, "Folk"}
                             };

            return genres;
        }

        #endregion
    }
}