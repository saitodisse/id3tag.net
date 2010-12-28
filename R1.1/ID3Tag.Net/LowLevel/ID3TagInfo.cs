using System.Collections.ObjectModel;

namespace Id3Tag.LowLevel
{
    /// <summary>
    /// Represents a low level Id3Tag.
    /// </summary>
    public class Id3TagInfo
    {
        private readonly Collection<RawFrame> m_FrameList;

        internal Id3TagInfo()
        {
            m_FrameList = new Collection<RawFrame>();
            SetDefault();
        }

        /// <summary>
        /// The major version.
        /// </summary>
        public int MajorVersion { get; internal set; }

        /// <summary>
        /// The revision.
        /// </summary>
        public int Revision { get; internal set; }

        /// <summary>
        /// The unsynchronisation flag.
        /// </summary>
        public bool Unsynchronised { get; internal set; }

        /// <summary>
        /// True if the extended header is available, otherwise false.
        /// </summary>
        public bool ExtendedHeaderAvailable { get; internal set; }

        /// <summary>
        /// The experminental flag.
        /// </summary>
        public bool Experimental { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has footer (ID3v2.4 only).
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has footer; otherwise, <c>false</c>.
        /// </value>
        public bool HasFooter { get; internal set; }

        /// <summary>
        /// The Extended Header of the tag
        /// </summary>
        public ExtendedHeader ExtendedHeader { get; set; }

        /// <summary>
        /// The frames of the tag in raw format.
        /// </summary>
        public Collection<RawFrame> Frames
        {
            get { return m_FrameList; }
        }

        #region Private

        private void SetDefault()
        {
            MajorVersion = 0;
            Revision = 0;
            Unsynchronised = false;
            ExtendedHeaderAvailable = false;
            Experimental = false;
            HasFooter = false;
        }

        #endregion
    }
}