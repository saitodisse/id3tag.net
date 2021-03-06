﻿using ID3Tag.LowLevel;

namespace ID3Tag.HighLevel
{
    /// <summary>
    /// This class implements the IFrame interface and provides infrastructure code for the
    /// concrete Frame implementations.
    /// </summary>
    public abstract class Frame : IFrame
    {
        internal Frame()
        {
            Descriptor = new FrameDescriptor();
        }

        #region IFrame Members

        /// <summary>
        /// Convert the values from the high level frame to a raw frame.
        /// </summary>
        /// <returns>the raw frame.</returns>
        public abstract RawFrame Convert();

        /// <summary>
        /// Import the raw content to a high level frame.
        /// </summary>
        /// <param name="rawFrame">the raw frame.</param>
        public abstract void Import(RawFrame rawFrame);

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
        /// <param name="rawFrame"></param>
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