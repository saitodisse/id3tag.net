using System.Collections.ObjectModel;
using System.Windows;
using Id3Tag.HighLevel;
using Microsoft.Win32;

namespace Id3TagUtility
{
    /// <summary>
    /// Interaction logic for ID3V1Dialog.xaml
    /// </summary>
    public partial class ID3V1Dialog : Window
    {
        public ID3V1Dialog()
        {
            InitializeComponent();

            TagData = new Id3V1Tag();
        }

        public Id3V1Tag TagData { get; set; }
        public string SourceFile { get; set; }
        public string TargetFile { get; set; }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            ReadOnlyCollection<string> genres = Genre.Instance.AllGenres;
            comboBoxGenre.ItemsSource = genres;
            comboBoxGenre.SelectedIndex = 0;
        }

        private void buttonApply_Click(object sender, RoutedEventArgs e)
        {
            TagData.IsId3V1Dot1Compliant = true;
            TagData.Title = textBoxTitle.Text;
            TagData.Album = textBoxAlbum.Text;
            TagData.Artist = textBoxArtist.Text;
            TagData.Comment = textBoxComment.Text;
            TagData.Year = textBoxYear.Text;
            TagData.TrackNumber = textBoxTrackNr.Text;
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
            bool? result = dialog.ShowDialog(this);

            if (result == true)
            {
                string filename = dialog.FileName;
                textBoxSourceFile.Text = filename;
            }
        }

        private void buttonTargetFileSelect_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            bool? result = dialog.ShowDialog(this);

            if (result == true)
            {
                string filename = dialog.FileName;
                textBoxTargetFile.Text = filename;
            }
        }
    }
}