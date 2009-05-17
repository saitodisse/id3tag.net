namespace ID3Tag.HighLevel
{
    /// <summary>
    /// Represents the ID3V1 Tag block.
    /// </summary>
    public class Id3V1Tag
    {
        /// <summary>
        /// The Title.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// The Artist.
        /// </summary>
        public string Artist { get; set; }
        /// <summary>
        /// The Album.
        /// </summary>
        public string Album { get; set; }
        /// <summary>
        /// The Year.
        /// </summary>
        public string Year { get; set; }
        /// <summary>
        /// The Comment.
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// The Genre.
        /// </summary>
        public string Genre { get; set; }
        /// <summary>
        /// The Track Nr.
        /// </summary>
        public string TrackNr { get; set; }
        /// <summary>
        /// True if ID3V1.1 is supported otherwise false
        /// </summary>
        public bool IsID3V1_1Compliant { get; set; }
    }
}