using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ID3Tag.HighLevel;
using Microsoft.Win32;

namespace ID3TagUtility
{
    /// <summary>
    /// Interaction logic for TagDataDialog.xaml
    /// </summary>
    public partial class TagDataDialog : Window
    {
        public TagData Data { get; private set; }

        public TagDataDialog()
        {
            InitializeComponent();

            Data = new TagData
                       {
                           EncodingType = TextEncodingType.ISO_8859_1,
                           Album = "My Album",
                           Title = "My Title",
                           Year = "2009",
                           Comment = ""
                       };
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

            Close();
        }

        private void buttonApply_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;

            if (comboBoxEncoding.SelectedIndex == 0)
            {
                Data.EncodingType = TextEncodingType.ISO_8859_1;
            }

            if (comboBoxEncoding.SelectedIndex == 1)
            {
                Data.EncodingType = TextEncodingType.UTF16;
            }

            // The other codings are only valid for 2.4 !

            Data.Album = textBoxAlbum.Text;
            Data.Title = textBoxTitle.Text;
            Data.Year = textBoxYear.Text;
            Data.Comment = textBoxComments.Text;
            Data.SourceFile = textBoxSourceFile.Text;
            Data.TargetFile = textBoxTargetFile.Text;

            Close();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            textBoxAlbum.Text = Data.Album;
            textBoxTitle.Text = Data.Title;
            textBoxYear.Text = Data.Year;
            textBoxComments.Text = Data.Comment;
        }

        private void buttonSourceFileSelect_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            var result = dialog.ShowDialog(this);

            if (result == true)
            {
                var filename = dialog.FileName;
                textBoxSourceFile.Text = filename;
            }
        }

        private void buttonTargetFileSelect_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            var result = dialog.ShowDialog(this);

            if (result == true)
            {
                var filename = dialog.FileName;
                textBoxTargetFile.Text = filename;
            }
        }
    }
}
