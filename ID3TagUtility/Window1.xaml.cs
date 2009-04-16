using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ID3Tag;
using ID3Tag.Factory;
using ID3Tag.HighLevel;
using ID3Tag.LowLevel;

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
                stringBuilder.AppendFormat("{0:X2} ",b);
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
    }
}
