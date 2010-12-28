using Id3Tag.LowLevel;

namespace Id3Tag.HighLevel
{
    /// <summary>
    /// This class implements the IFrame interface and provides infrastructure code for the
    /// concrete Frame implementations.
    /// </summary>
    public abstract class Frame : IFrame
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Frame"/> class.
        /// </summary>
        internal Frame()
        {
            Descriptor = new FrameDescriptor();
        }

        #region IFrame Members

        /// <summary>
        /// Convert the values from the high level frame to a raw frame.
        /// </summary>
        /// <returns>the raw frame.</returns>
        public abstract RawFrame Convert(TagVersion version);

        /// <summary>
        /// Import the raw content to a high level frame.
        /// </summary>
        /// <param name="rawFrame">the raw frame.</param>
        /// <param name="codePage">Default code page for Ansi encoding. Pass 0 to use default system encoding code page.</param>
        public abstract void Import(RawFrame rawFrame, int codePage);

        /// <summary>
        /// The frame type.
        /// </summary>
        public abstract FrameType Type { get; }

        /// <summary>
        /// The description of the frame.
        /// </summary>
        public FrameDescriptor Descriptor { get; internal set; }

        #endregion

        /// <summary>
        /// Import the header flags from a raw frame.
        /// </summary>
        /// <param name="rawFrame">The raw frame.</param>
        protected void ImportRawFrameHeader(RawFrame rawFrame)
        {
            Descriptor.Id = rawFrame.Id;
            Descriptor.TagAlterPreservation = rawFrame.Options.TagAlterPreservation;
            Descriptor.FileAlterPreservation = rawFrame.Options.FileAlterPreservation;
            Descriptor.ReadOnly = rawFrame.Options.ReadOnly;
            Descriptor.Compression = rawFrame.Options.Compression;
            Descriptor.Encryption = rawFrame.Options.Encryption;
            Descriptor.GroupingIdentify = rawFrame.Options.GroupingIdentify;
            //
            // ID3v2.4 only
            //
            Descriptor.Unsynchronisation = rawFrame.Options.Unsynchronisation;
            Descriptor.DataLengthIndicator = rawFrame.Options.DataLengthIndicator;
        }
    }
}