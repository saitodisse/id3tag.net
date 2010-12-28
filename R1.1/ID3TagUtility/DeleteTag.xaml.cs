using System.Windows;
using Microsoft.Win32;

namespace Id3TagUtility
{
    /// <summary>
    /// Interaction logic for DeleteTag.xaml
    /// </summary>
    public partial class DeleteTag : Window
    {
        public DeleteTag()
        {
            InitializeComponent();
        }

        public string SourceFile { get; set; }
        public string TargetFile { get; set; }

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
                textBoxSourceFile.Text = filename;
            }
        }

        private void buttonApply_Click(object sender, RoutedEventArgs e)
        {
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
    }
}