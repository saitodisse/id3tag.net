using System;
using System.IO;
using System.Text;
using System.Windows;
using ID3Tag;
using ID3Tag.Factory;
using ID3Tag.HighLevel;
using ID3Tag.HighLevel.ID3Frame;
using Microsoft.Win32;

namespace ID3TagUtility
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void OnImportFile(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            var ok = dialog.ShowDialog(this);
            if (ok == true)
            {
                var filename = dialog.FileName;

                //
                //  Read the tag here!
                //
                var tagContainer = ReadTag(dialog.FileName);
                var tagDescriptor = tagContainer.Tag;

                // OK. Update the UI.
                UpdateView(filename, tagDescriptor);
                ShowTagFrames(tagContainer);
            }
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private static string ConvertToString(byte[] data)
        {
            var stringBuilder = new StringBuilder();
            foreach (var b in data)
            {
                stringBuilder.AppendFormat("{0:X2} ", b);
            }

            return stringBuilder.ToString();
        }

        private void UpdateView(string filename, TagDescriptor tagDescriptor)
        {
            //
            //  Decode the header of the tag.
            //
            labelFilename.Content = filename;
            labelTagVersion.Content = String.Format("ID3v2.{0}.{1}", tagDescriptor.MajorVersion, tagDescriptor.Revision);

            checkBoxExperimentalIndicator.IsChecked = tagDescriptor.ExperimentalIndicator;
            checkExtendedHeader.IsChecked = tagDescriptor.ExtendedHeader;
            checkBoxUnsync.IsChecked = tagDescriptor.Unsynchronisation;

            if (tagDescriptor.ExtendedHeader)
            {
                labelPaddingDescriptor.IsEnabled = true;
                labelPaddingSize.IsEnabled = true;
                labelPaddingSize.Content = tagDescriptor.PaddingSize;

                labelCRCBytes.IsEnabled = tagDescriptor.CrcDataPresent;
                labelCRCBytesDescriptor.IsEnabled = tagDescriptor.CrcDataPresent;
                if (tagDescriptor.CrcDataPresent)
                {
                    var crc = ConvertToString(tagDescriptor.Crc);
                    labelCRCBytes.Content = crc;
                }
                else
                {
                    labelCRCBytes.Content = String.Empty;
                }
            }
            else
            {
                labelCRCBytesDescriptor.IsEnabled = false;
                labelCRCBytes.Content = String.Empty;
                labelCRCBytes.IsEnabled = false;
                labelPaddingDescriptor.IsEnabled = false;
                labelPaddingSize.IsEnabled = false;
                labelPaddingSize.Content = String.Empty;
            }
        }

        private void ShowTagFrames(TagContainer tagContainer)
        {
            //
            //  Iterate over the frame collection and show the ToString representation.
            //
            listView1Tags.Items.Clear();
            foreach (var frame in tagContainer)
            {
                listView1Tags.Items.Add(frame.ToString());
            }
        }

        private static TagContainer ReadTag(string filename)
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

        private void OnWrite(object sender, RoutedEventArgs e)
        {
            var dialog = new TagDataDialog();
            var result = dialog.ShowDialog();

            if (result == true)
            {
                // Create and configure a new tag.
                var data = dialog.Data;
                var tagController = BuildTag(data);

                if (String.IsNullOrEmpty(data.SourceFile) || String.IsNullOrEmpty(data.TargetFile))
                {
                    MessageBox.Show("Please validate your Source and Target file.");
                    return;
                }

                FileStream inputStream = null;
                FileStream outputStream = null;
                try
                {
                    var ioController = Id3TagFactory.CreateIoController();

                    // Write the tag.
                    inputStream = File.Open(data.SourceFile, FileMode.Open);
                    outputStream = File.OpenWrite(data.TargetFile);
                    ioController.Write(tagController,inputStream,outputStream);
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

        }

        private TagContainer BuildTag(TagData data)
        {
            var tagController = new TagContainer();
            tagController.Tag.SetVersion(3,0);
            tagController.Tag.SetExtendedHeader(0,false,new byte[0]);
            tagController.Tag.SetHeaderFlags(false,false,false);

            // OK. Build the frames.
            var albumFrame = new TextFrame("TALB", data.Album, data.EncodingType);
            var yearFrame = new TextFrame("TYER", data.Year, data.EncodingType);
            var titleFrame = new TextFrame("TIT2", data.Title, data.EncodingType);
            var textComment = new UserDefinedTextFrame("Your comment", data.Comment, data.EncodingType);
            var comment = new CommentFrame("ENG", "Your Comment", data.Comment, data.EncodingType);
            var encoder = new TextFrame("TENC", data.Encoder, data.EncodingType);

            tagController.Add(albumFrame);
            tagController.Add(yearFrame);
            tagController.Add(titleFrame);
            tagController.Add(textComment);
            tagController.Add(comment);
            tagController.Add(encoder);
            return tagController;
        }
    }
}