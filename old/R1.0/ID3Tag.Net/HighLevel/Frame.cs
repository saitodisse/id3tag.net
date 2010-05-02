using ID3Tag.LowLevel;

namespace ID3Tag.HighLevel
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
            Descriptor.ID = rawFrame.ID;
            Descriptor.TagAlterPreservation = rawFrame.Flag.TagAlterPreservation;
            Descriptor.FileAlterPreservation = rawFrame.Flag.FileAlterPreservation;
            Descriptor.ReadOnly = rawFrame.Flag.ReadOnly;
            Descriptor.Compression = rawFrame.Flag.Compression;
            Descriptor.Encryption = rawFrame.Flag.Encryption;
            Descriptor.GroupingIdentify = rawFrame.Flag.GroupingIdentify;
            //
            // ID3v2.4 only
            //
            Descriptor.Unsynchronisation = rawFrame.Flag.Unsynchronisation;
            Descriptor.DataLengthIndicator = rawFrame.Flag.DataLengthIndicator;
        }
    }
}