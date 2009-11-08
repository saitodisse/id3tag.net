using System.Text;
using ID3Tag.HighLevel;

namespace ID3TagUtility
{
    public class ID3V2TagData
    {
        public TagVersion Version { get; set; }

        public Encoding TextEncoding { get; set; }
        public string Album { get; set; }
        public string Artist { get; set; }
        public string Year { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }

        public bool Unsynchronisation { get; set; }
        public bool ExperimentalIndicator { get; set; }
        public bool ExtendedHeader { get; set; }

        public bool CrCPresent { get; set; }
        public int PaddingSize { get; set; }
        public byte[] Crc { get; set; }

        public string SourceFile { get; set; }
        public string TargetFile { get; set; }

        public bool PictureFrameEnabled { get; set; }
        public string PictureFile { get; set; }

        // Some advertisement ;-)
        public string Encoder
        {
            get { return "ID3TagLib.Net"; }
        }

        public bool WriteLyricsFlag { get; set; }
        public string LyricsDescriptor { get; set; }
        public string Lyrics { get; set; }
    }
}