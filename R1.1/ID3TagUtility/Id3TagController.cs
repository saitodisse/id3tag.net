using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using Id3Tag;
using Id3Tag.HighLevel;
using Id3Tag.HighLevel.Id3Frame;

namespace Id3TagUtility
{
    public class Id3TagController
    {
        /// <summary>
        /// Reads a tag from a file.
        /// </summary>
        /// <param name="filename">the filename.</param>
        /// <returns>the tag.</returns>
        public TagContainer ReadTag(string filename)
        {
            TagContainer tag = null;

			try
            {
				tag = Id3TagFactory.CreateId3TagManager().ReadV2Tag(filename);
            }
            catch (Id3IOException ioException)
            {
                MessageBox.Show("IO Exception caught : " + ioException.Message);
            }
            catch (Id3HeaderNotFoundException headerNotFoundException)
            {
                MessageBox.Show("ID3 header not found : " + headerNotFoundException.Message);
            }
            catch (Id3TagException tagException)
            {
                MessageBox.Show("Id3TagException caught : " + tagException.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unknown exception caught : " + ex.Message);
            }

            return tag;
        }

        /// <summary>
        /// Writes a tag to file.
        /// </summary>
        /// <param name="tagContainer"></param>
        /// <param name="sourceFile">the source file.</param>
        /// <param name="targetFile">the target file.</param>
        /// <remarks>the old tags will be removed.</remarks>
        public void WriteTag(TagContainer tagContainer, string sourceFile, string targetFile)
        {
            try
            {
				Id3TagFactory.CreateId3TagManager().WriteV2Tag(sourceFile, targetFile, tagContainer);
            }
            catch (Id3IOException ioException)
            {
                MessageBox.Show("IO Exception caught : " + ioException.Message);
            }
            catch (Id3TagException tagException)
            {
                MessageBox.Show("Id3TagException caught : " + tagException.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unknown exception caught : " + ex.Message);
            }
        }

        public FileState ReadTagStatus(string filename)
        {
            FileState state = null;

			try
            {
				state = Id3TagFactory.CreateId3TagManager().GetTagsStatus(filename);
            }
            catch (Id3IOException ioException)
            {
                MessageBox.Show("IO Exception caught : " + ioException.Message);
            }
            catch (Id3TagException tagException)
            {
                MessageBox.Show("Id3TagException caught : " + tagException.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unknown exception caught : " + ex.Message);
            }

            return state;
        }

        public TagContainer BuildTag(ID3V2TagData data)
        {
            var container = Id3TagFactory.CreateId3Tag(data.Version);
            if (data.Version == TagVersion.Id3V23)
            {
                //
                //  Configure the ID3v2.3 header
                //
                var extendedHeaderV23 = container.GetId3V23Descriptor();
                // Configure the tag header.
                extendedHeaderV23.SetHeaderOptions(data.Unsynchronisation, data.ExtendedHeader, data.ExperimentalIndicator);

                if (data.ExtendedHeader)
                {
                    extendedHeaderV23.SetExtendedHeader(data.PaddingSize, data.CrCPresent);
                    extendedHeaderV23.SetCrc32(data.Crc);
                }
            }
            else
            {
                //
                //  Configure the ID3v2.4 header
                //
                var extendedHeaderV24 = container.GetId3V24Descriptor();
                extendedHeaderV24.SetHeaderOptions(false, false, false, true);
            }

            // OK. Build the frames.
            var albumFrame = new TextFrame("TALB", data.Album, data.TextEncoding);
            var artistFrame = new TextFrame("TPE2", data.Artist, data.TextEncoding);
            var yearFrame = new TextFrame("TYER", data.Year, data.TextEncoding);
            var titleFrame = new TextFrame("TIT2", data.Title, data.TextEncoding);
            var textComment = new UserDefinedTextFrame("Your comment", data.Comment, data.TextEncoding);
            var comment = new CommentFrame("ENG", "Your Comment", data.Comment, data.TextEncoding);
            var encoder = new TextFrame("TENC", data.Encoder, data.TextEncoding);

            container.Add(albumFrame);
            container.Add(artistFrame);
            container.Add(yearFrame);
            container.Add(titleFrame);
            container.Add(textComment);
            container.Add(comment);
            container.Add(encoder);

            if (data.PictureFrameEnabled)
            {
                if (File.Exists(data.PictureFile))
                {
                    WritePictureFrame(data, container);
                }
                else
                {
                    MessageBox.Show("Picture file not found.");
                }
            }

            if (data.WriteLyricsFlag)
            {
                WriteUnsychronisedLyrics(data.LyricsDescriptor, data.Lyrics,container);
            }

            return container;
        }



        public Id3V1Tag ReadId3V1Tag(string filename, int codePage)
        {
            Id3V1Tag container = null;

            try
            {
				container = Id3TagFactory.CreateId3TagManager().ReadV1Tag(filename, codePage);
            }
            catch (Id3IOException ioException)
            {
                MessageBox.Show("IO Exception caught : " + ioException.Message);
            }
            catch (Id3TagException tagException)
            {
                MessageBox.Show("Id3TagException caught : " + tagException.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unknown exception caught : " + ex.Message);
            }

            return container;
        }

        public void WriteId3V1Tag(Id3V1Tag tag, string sourceFile, string targetFile)
        {
            try
            {
				Id3TagFactory.CreateId3TagManager().WriteV1Tag(sourceFile, targetFile, tag);
            }
            catch (Id3IOException ioException)
            {
                MessageBox.Show("IO Exception caught : " + ioException.Message);
            }
            catch (Id3TagException tagException)
            {
                MessageBox.Show("Id3TagException caught : " + tagException.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unknown exception caught : " + ex.Message);
            }
        }

        private static void WritePictureFrame(ID3V2TagData data, ICollection<IFrame> container)
        {
            using (FileStream stream = File.Open(data.PictureFile, FileMode.Open))
            {
                //
                //  Read the picture.
                //
                int byteCount = Convert.ToInt32(stream.Length);
                var pictureData = new byte[byteCount];
                stream.Read(pictureData, 0, byteCount);

                //
                //  Add the picture frame.
                //  
                var pictureFrame = new PictureFrame(
                    Encoding.Default,
                    "image/jpg",
                    "Other",
                    PictureType.Other,
                    pictureData);

                container.Add(pictureFrame);
            }
        }

        private static void WriteUnsychronisedLyrics(string descriptor, string lyrics, ICollection<IFrame> container)
        {
            var uslt = new UnsynchronisedLyricFrame("ENG",descriptor,lyrics,Encoding.ASCII);

            container.Add(uslt);
        }
    }
}