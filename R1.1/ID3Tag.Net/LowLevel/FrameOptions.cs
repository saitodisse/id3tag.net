namespace Id3Tag.LowLevel
{
    /// <summary>
    /// Identifies applicable Frame Flags (Options)
    /// </summary>
    public class FrameOptions
    {
        /// <summary>
        /// The TagAlterPreservation option.
        /// </summary>
        public bool TagAlterPreservation { get; internal set; }

        /// <summary>
        /// The FileAlterPreservation option.
        /// </summary>
        public bool FileAlterPreservation { get; internal set; }

        /// <summary>
        /// The ReadOnly option.
        /// </summary>
        public bool ReadOnly { get; internal set; }

        /// <summary>
        /// The Compression option.
        /// </summary>
        public bool Compression { get; internal set; }

        /// <summary>
        /// The Encryption option.
        /// </summary>
        public bool Encryption { get; internal set; }

        /// <summary>
        /// The GroupingIdentify option.
        /// </summary>
        public bool GroupingIdentify { get; internal set; }

        /// <summary>
        /// The Unsynchronisation option ( ID3V.2.4 only! )
        /// </summary>
        public bool Unsynchronisation { get; internal set; }

        /// <summary>
        /// The DataLengthIndicator option ( ID3v2.4. only! )
        /// </summary>
        public bool DataLengthIndicator { get; internal set; }
    }
}