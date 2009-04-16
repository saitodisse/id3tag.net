using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ID3Tag.HighLevel;

namespace ID3TagUtility
{
    public class TagData
    {
        public TextEncodingType EncodingType { get; set; }
        public string Album { get; set; }
        public string Year { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }

        public string SourceFile { get; set; }
        public string TargetFile { get; set; }

        // Some advertisement ;-)
        public string Encoder
        {
            get
            {
                return "ID3TagLib.Net";
            }
        }
    }
}
