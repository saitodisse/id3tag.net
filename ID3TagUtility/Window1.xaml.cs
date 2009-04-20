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
        private readonly Id3TagController m_Controller;

        public Window1()
        {
            InitializeComponent();

            m_Controller = new Id3TagController();
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
                var tagContainer = m_Controller.ReadTag(dialog.FileName);
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

        private void OnWrite(object sender, RoutedEventArgs e)
        {
            var dialog = new TagDataDialog();
            var result = dialog.ShowDialog();

            if (result == true)
            {
                // Create and configure a new tag.
                var data = dialog.Data;
                var tagController = m_Controller.BuildTag(data);

                if (String.IsNullOrEmpty(data.SourceFile) || String.IsNullOrEmpty(data.TargetFile))
                {
                    MessageBox.Show("Please validate your Source and Target file.");
                    return;
                }

                m_Controller.WriteTag(tagController, data.SourceFile, data.TargetFile);
            }

        }


    }
}