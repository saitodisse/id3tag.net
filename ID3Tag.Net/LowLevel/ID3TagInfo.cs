using System.Collections.Generic;

namespace ID3Tag.LowLevel
{
    public class Id3TagInfo
    {
        private readonly List<RawFrame> m_FrameList;

        internal Id3TagInfo()
        {
            m_FrameList = new List<RawFrame>();
            SetDefault();
        }

        public int MajorVersion { get; internal set; }
        public int Revision { get; internal set; }
        public bool UnsynchronisationFlag { get; internal set; }
        public bool ExtendedHeaderAvailable { get; internal set; }
        public bool Experimental { get; internal set; }
        public ExtendedTagHeader ExtendHeader { get; internal set; }

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