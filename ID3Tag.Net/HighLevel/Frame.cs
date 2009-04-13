using ID3Tag.LowLevel;

namespace ID3Tag.HighLevel
{
    public abstract class Frame : IFrame
    {
        internal Frame()
        {
            Descriptor = new FrameDescriptor();
        }

        #region IFrame Members

        public abstract RawFrame Convert();

        public abstract void Import(RawFrame rawFrame);

        public abstract FrameType Type { get; }

        public FrameDescriptor Descriptor { get; internal set; }

        #endregion

        protected void ImportRawFrameHeader(RawFrame rawFrame)
        {
            Descriptor.ID = rawFrame.ID;
            Descriptor.TagAlterPreservation = rawFrame.TagAlterPreservation;
            Descriptor.FileAlterPreservation = rawFrame.FileAlterPreservation;
            Descriptor.ReadOnly = rawFrame.ReadOnly;
            Descriptor.Compression = rawFrame.Compression;
            Descriptor.Encryption = rawFrame.Encryption;
            Descriptor.GroupingIdentify = rawFrame.GroupingIdentify;
        }
    }
}