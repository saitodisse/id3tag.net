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
    /// Interaction logic for ID3V1Dialog.xaml
    /// </summary>
    public partial class ID3V1Dialog : Window
    {
        public Id3V1Tag TagData { get; set; }
        public string SourceFile { get; set; }
        public string TargetFile { get; set; }

        public ID3V1Dialog()
        {
            InitializeComponent();

            TagData = new Id3V1Tag();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            var genres = Genre.Instance.GetGenres();
            comboBoxGenre.ItemsSource = genres;
            comboBoxGenre.SelectedIndex = 0;
        }

        private void buttonApply_Click(object sender, RoutedEventArgs e)
        {
            TagData.IsID3V1_1Compliant = true;
            TagData.Title = textBoxTitle.Text;
            TagData.Album = textBoxAlbum.Text;
            TagData.Artist = textBoxArtist.Text;
            TagData.Comment = textBoxComment.Text;
            TagData.Year = textBoxYear.Text;
            TagData.TrackNr = textBoxTrackNr.Text;
            TagData.GenreIdentifier = comboBoxGenre.SelectedIndex;

            SourceFile = textBoxSourceFile.Text;
            TargetFile = textBoxTargetFile.Text;

            DialogResult = true;
            Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
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
