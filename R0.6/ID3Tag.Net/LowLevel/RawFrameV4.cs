using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ID3Tag.LowLevel
{
    public sealed class RawFrameV4 : RawFrame
    {
        public RawFrameV4(string id, byte[] flags, byte[] payload)
            : base(id,payload)
        {
            DecodeFlags(flags);
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

        private void DecodeFlags(byte[] flags)
        {
            Flag = new FrameFlags();

            var isTagAlterPreservation = (flags[0] & 0x40) == 0x40;
            var isFileAlterPreservation = (flags[0] & 0x20) == 0x20;
            var isReadOnly = (flags[0] & 0x10) == 0x10;
            var isGroupingIdentity = (flags[1] & 0x40) == 0x40;
            var isCompression = (flags[1] & 0x08) == 0x08;
            var isEncryption = (flags[1] & 0x04) == 0x04;
            var isUnsync = (flags[1] & 0x02) == 0x02;
            var isDataLengthIndicator = (flags[1] & 0x01) == 0x01;

            Flag.TagAlterPreservation = isTagAlterPreservation;
            Flag.FileAlterPreservation = isFileAlterPreservation;
            Flag.ReadOnly = isReadOnly;
            Flag.GroupingIdentify = isGroupingIdentity;
            Flag.Compression = isCompression;
            Flag.Encryption = isEncryption;
            Flag.Unsynchronisation = isUnsync;
            Flag.DataLengthIndicator = isDataLengthIndicator;
        }

        internal override byte[] EncodeFlags()
        {
            var flagsByte = new byte[2];

            // Decode the flags
            if (Flag.TagAlterPreservation)
            {
                flagsByte[0] |= 0x40;
            }

            if (Flag.FileAlterPreservation)
            {
                flagsByte[0] |= 0x20;
            }

            if (Flag.ReadOnly)
            {
                flagsByte[0] |= 0x10;
            }

            if (Flag.GroupingIdentify)
            {
                flagsByte[1] |= 0x40;
            }

            if (Flag.Compression)
            {
                flagsByte[1] |= 0x08;
            }

            if (Flag.Encryption)
            {
                flagsByte[1] |= 0x04;
            }

            if (Flag.Unsynchronisation)
            {
                flagsByte[1] |= 0x02;
            }

            if (Flag.DataLengthIndicator)
            {
                flagsByte[1] |= 0x01;
            }

            return flagsByte;
        }
    }
}
