namespace ID3Tag.HighLevel
{
    internal class Id3V1Tag
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Year { get; set; }
        public string Comment { get; set; }
        public string Genre { get; set; }

        public string TrackNr { get; set; }
        public bool IsID3V1_1Compliant { get; set; }
    }
}