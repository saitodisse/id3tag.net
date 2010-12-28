using System.Collections.Generic;

namespace Id3Tag.LowLevel
{
    /// <summary>
    /// Contains Version 2.4 specifics of <see cref="RawFrame"/>
    /// </summary>
    public sealed class RawFrameV4 : RawFrame
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RawFrameV4"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="options">The options.</param>
        /// <param name="payload">The payload.</param>
        public RawFrameV4(string id, byte[] options, IList<byte> payload)
            : base(id, payload)
        {
            DecodeFlags(options);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RawFrameV4"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="options">The options.</param>
        /// <param name="payload">The payload.</param>
        public RawFrameV4(string id, FrameOptions options, IList<byte> payload)
            : base(id, payload)
        {
            Options = options;
        }

        /*
         * 
         *      %0abc 0000 %0h00 kmnp
         *      
         *      a = Tag Alter Preservartion
         *      b = File Alter Preservation
         *      c = Read Only
         *      h = Grouping Identity
         *      k = Compression
         *      m = Encryption
         *      n = Unsynchronisation
         *      p = Data Length Indicator
         */

        private void DecodeFlags(byte[] options)
        {
            Options = new FrameOptions();

            bool isTagAlterPreservation = (options[0] & 0x40) == 0x40;
            bool isFileAlterPreservation = (options[0] & 0x20) == 0x20;
            bool isReadOnly = (options[0] & 0x10) == 0x10;
            bool isGroupingIdentity = (options[1] & 0x40) == 0x40;
            bool isCompression = (options[1] & 0x08) == 0x08;
            bool isEncryption = (options[1] & 0x04) == 0x04;
            bool isUnsync = (options[1] & 0x02) == 0x02;
            bool isDataLengthIndicator = (options[1] & 0x01) == 0x01;

            Options.TagAlterPreservation = isTagAlterPreservation;
            Options.FileAlterPreservation = isFileAlterPreservation;
            Options.ReadOnly = isReadOnly;
            Options.GroupingIdentify = isGroupingIdentity;
            Options.Compression = isCompression;
            Options.Encryption = isEncryption;
            Options.Unsynchronisation = isUnsync;
            Options.DataLengthIndicator = isDataLengthIndicator;
        }

        internal override byte[] EncodeFlags()
        {
            var flagsByte = new byte[2];

            // Decode the flags
            if (Options.TagAlterPreservation)
            {
                flagsByte[0] |= 0x40;
            }

            if (Options.FileAlterPreservation)
            {
                flagsByte[0] |= 0x20;
            }

            if (Options.ReadOnly)
            {
                flagsByte[0] |= 0x10;
            }

            if (Options.GroupingIdentify)
            {
                flagsByte[1] |= 0x40;
            }

            if (Options.Compression)
            {
                flagsByte[1] |= 0x08;
            }

            if (Options.Encryption)
            {
                flagsByte[1] |= 0x04;
            }

            if (Options.Unsynchronisation)
            {
                flagsByte[1] |= 0x02;
            }

            if (Options.DataLengthIndicator)
            {
                flagsByte[1] |= 0x01;
            }

            return flagsByte;
        }
    }
}