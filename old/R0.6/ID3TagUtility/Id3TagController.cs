﻿using System;
using System.IO;
using System.Text;
using System.Windows;
using ID3Tag;
using ID3Tag.HighLevel;
using ID3Tag.HighLevel.ID3Frame;

namespace ID3TagUtility
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
				tag = Id3TagManager.ReadV2Tag(filename);
            }
            catch (ID3IOException ioException)
            {
                MessageBox.Show("IO Exception caught : " + ioException.Message);
            }
            catch (ID3HeaderNotFoundException headerNotFoundException)
            {
                MessageBox.Show("ID3 header not found : " + headerNotFoundException.Message);
            }
            catch (ID3TagException tagException)
            {
                MessageBox.Show("ID3TagException caught : " + tagException.Message);
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
				Id3TagManager.WriteV2Tag(sourceFile, targetFile, tagContainer);
            }
            catch (ID3IOException ioException)
            {
                MessageBox.Show("IO Exception caught : " + ioException.Message);
            }
            catch (ID3TagException tagException)
            {
                MessageBox.Show("ID3TagException caught : " + tagException.Message);
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
            	state = Id3TagManager.GetTagsStatus(filename);
            }
            catch (ID3IOException ioException)
            {
                MessageBox.Show("IO Exception caught : " + ioException.Message);
            }
            catch (ID3TagException tagException)
            {
                MessageBox.Show("ID3TagException caught : " + tagException.Message);
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
                extendedHeaderV23.SetHeaderFlags(data.Unsynchronisation, data.ExtendedHeader, data.ExperimentalIndicator);

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
                extendedHeaderV24.SetHeaderFlags(false, false, false, true);
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
            	container = Id3TagManager.ReadV1Tag(filename, codePage);
            }
            catch (ID3IOException ioException)
            {
                MessageBox.Show("IO Exception caught : " + ioException.Message);
            }
            catch (ID3TagException tagException)
            {
                MessageBox.Show("ID3TagException caught : " + tagException.Message);
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
				Id3TagManager.WriteV1Tag(sourceFile, targetFile, tag);
            }
            catch (ID3IOException ioException)
            {
                MessageBox.Show("IO Exception caught : " + ioException.Message);
            }
            catch (ID3TagException tagException)
            {
                MessageBox.Show("ID3TagException caught : " + tagException.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unknown exception caught : " + ex.Message);
            }
        }

        private static void WritePictureFrame(ID3V2TagData data, TagContainer container)
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

        private static void WriteUnsychronisedLyrics(string descriptor, string lyrics, TagContainer container)
        {
            var uslt = new UnsynchronisedLyricFrame("ENG",descriptor,lyrics,Encoding.ASCII);

            container.Add(uslt);
        }
    }
}