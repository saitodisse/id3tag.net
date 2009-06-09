using System;
using System.IO;
using System.Windows;
using ID3Tag;
using ID3Tag.Factory;
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
            var file = new FileInfo(filename);

            //
            //  Create the controller from the factory.
            //
            var ioController = Id3TagFactory.CreateIoController();
            var tagController = Id3TagFactory.CreateTagController();

            TagContainer tag = null;
            try
            {
                //
                // Read the raw tag ...
                //
                var tagInfo = ioController.Read(file);
                //
                //  ... and decode the frames.
                //
                tag = tagController.Decode(tagInfo);
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
        /// <param name="tagController"></param>
        /// <param name="sourceFile">the source file.</param>
        /// <param name="targetFile">the target file.</param>
        /// <remarks>the old tags will be removed.</remarks>
        public void WriteTag(TagContainer tagController, string sourceFile, string targetFile)
        {
            FileStream inputStream = null;
            FileStream outputStream = null;
            try
            {
                var ioController = Id3TagFactory.CreateIoController();

                // Write the tag.
                inputStream = File.Open(sourceFile, FileMode.Open);
                outputStream = File.OpenWrite(targetFile);
                ioController.Write(tagController, inputStream, outputStream);
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
            finally
            {
                if (inputStream != null)
                {
                    inputStream.Close();
                    inputStream.Dispose();
                }

                if (outputStream != null)
                {
                    outputStream.Close();
                    outputStream.Dispose();
                }
            }
        }

        public FileState ReadTagStatus(string filename)
        {
            //
            //  Create the controller from the factory.
            //

            var file = new FileInfo(filename);
            var ioController = Id3TagFactory.CreateIoController();
            FileState state = null;
            try
            {
                state= ioController.DetermineTagStatus(file);
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
            var tagController = new TagContainer();

            // Configure the tag header.
            tagController.Tag.SetVersion(3, 0);
            tagController.Tag.SetHeaderFlags(data.Unsynchronisation, data.ExtendedHeader, data.ExperimentalIndicator);

            if (data.ExtendedHeader)
            {
                tagController.Tag.SetExtendedHeader(data.PaddingSize, data.CrCPresent, data.Crc);
            }

            // OK. Build the frames.
            var albumFrame = new TextFrame("TALB", data.Album, data.EncodingType);
            var artistFrame = new TextFrame("TPE2", data.Artist, data.EncodingType);
            var yearFrame = new TextFrame("TYER", data.Year, data.EncodingType);
            var titleFrame = new TextFrame("TIT2", data.Title, data.EncodingType);
            var textComment = new UserDefinedTextFrame("Your comment", data.Comment, data.EncodingType);
            var comment = new CommentFrame("ENG", "Your Comment", data.Comment, data.EncodingType);
            var encoder = new TextFrame("TENC", data.Encoder, data.EncodingType);

            tagController.Add(albumFrame);
            tagController.Add(artistFrame);
            tagController.Add(yearFrame);
            tagController.Add(titleFrame);
            tagController.Add(textComment);
            tagController.Add(comment);
            tagController.Add(encoder);

            if (data.PictureFrameEnabled)
            {
                if (File.Exists(data.PictureFile))
                {
                    WritePictureFrame(data, tagController);
                }
                else
                {
                    MessageBox.Show("Picture file not found.");
                }
            }

            return tagController;
        }

        public Id3V1Tag ReadId3V1Tag(string filename)
        {
            Id3V1Tag container = null;

            try
            {
                var file = new FileInfo(filename);
                var id3Converter = Id3TagFactory.CreateId3V1Controller();
                
                container = id3Converter.Read(file);
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

        public void WriteId3V1Tag (Id3V1Tag tag, string sourceFile, string targetFile)
        {
            FileStream inputStream = null;
            FileStream outputStream = null;
            try
            {
                var id3V1Controller = Id3TagFactory.CreateId3V1Controller();

                // Write the tag.
                inputStream = File.Open(sourceFile, FileMode.Open);
                outputStream = File.OpenWrite(targetFile);

                id3V1Controller.Write(tag, inputStream, outputStream);
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
            finally
            {
                if (inputStream != null)
                {
                    inputStream.Close();
                    inputStream.Dispose();
                }

                if (outputStream != null)
                {
                    outputStream.Close();
                    outputStream.Dispose();
                }
            }
        }

        private static void WritePictureFrame(ID3V2TagData data, TagContainer container)
        {
            using (var stream = File.Open(data.PictureFile, FileMode.Open))
            {
                //
                //  Read the picture.
                //
                var byteCount = Convert.ToInt32(stream.Length);
                var pictureData = new byte[byteCount];
                stream.Read(pictureData, 0, byteCount);

                //
                //  Add the picture frame.
                //  
                var pictureFrame = new PictureFrame(TextEncodingType.ISO_8859_1, "image/jpg", "Other", PictureType.Other,
                                                    pictureData);
                container.Add(pictureFrame);
            }
        }
    }
}