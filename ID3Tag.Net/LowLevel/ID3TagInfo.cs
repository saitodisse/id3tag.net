using System.Collections.Generic;

namespace ID3Tag.LowLevel
{
    /// <summary>
    /// Represents a low level ID3Tag.
    /// </summary>
    public class Id3TagInfo
    {
        private readonly List<RawFrame> m_FrameList;

        internal Id3TagInfo()
        {
            m_FrameList = new List<RawFrame>();
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
        public bool UnsynchronisationFlag { get; internal set; }

        /// <summary>
        /// True if the extended header is available, otherwise false.
        /// </summary>
        public bool ExtendedHeaderAvailable { get; internal set; }

        /// <summary>
        /// The experminental flag.
        /// </summary>
        public bool Experimental { get; internal set; }

        /// <summary>
        /// The extended header.
        /// </summary>
        public ExtendedTagHeaderV3 ExtendHeaderV3 { get; internal set; }

        /// <summary>
        /// The frames of the tag in raw format.
        /// </summary>
        public List<RawFrame> Frames
        {
            get { return m_FrameList; }
        }

        #region Private

        private void SetDefault()
        {
            MajorVersion = 0;
            Revision = 0;
            UnsynchronisationFlag = false;
            ExtendedHeaderAvailable = false;
            Experimental = false;
        }

        #endregion
    }
}