using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using ID3Tag;
using ID3Tag.Factory;
using ID3Tag.HighLevel;
using ID3Tag.HighLevel.ID3Frame;

namespace ID3TagUtility
{
    public class Id3TagController
    {
        public Id3TagController()
        {
            
        }

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
        public void WriteTag(TagContainer tagController,string sourceFile, string targetFile)
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

        public TagContainer BuildTag(TagData data)
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
            return tagController;
        }
    }
}
