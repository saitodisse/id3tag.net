namespace Id3Tag.HighLevel
{
    /// <summary>
    /// Represents the ID3V1 Tag block.
    /// </summary>
    public class Id3V1Tag
    {
        /// <summary>
        /// Creates a new ID3V1 Tag with default settings.
        /// </summary>
        public Id3V1Tag()
        {
            Title = string.Empty;
            Artist = string.Empty;
            Album = string.Empty;
            Year = string.Empty;
            Comment = string.Empty;
            GenreIdentifier = 0;
            TrackNumber = string.Empty;
            IsId3V1Dot1Compliant = false;
        }

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
        public string Genre
        {
            get
            {
                var genreID = GenreIdentifier;
                var value = "Unknown";
                if (HighLevel.Genre.Instance.ContainsGenre(genreID))
                {
                    value = HighLevel.Genre.Instance.GetGenre(genreID);
                }

                return value;
            }
        }

        /// <summary>
        ///  The ID3v1 Genre Identifier  ( 0 = Blues )
        /// </summary>
        public int GenreIdentifier { get; set; }

        /// <summary>
        /// The Track Nr.
        /// </summary>
        public string TrackNumber { get; set; }

        /// <summary>
        /// True if ID3V1.1 is supported otherwise false
        /// </summary>
        public bool IsId3V1Dot1Compliant { get; set; }
    }
}