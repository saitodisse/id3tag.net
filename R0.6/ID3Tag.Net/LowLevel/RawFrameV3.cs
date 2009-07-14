using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ID3Tag.LowLevel
{
    public class RawFrameV3 : RawFrame
    {
        public RawFrameV3(string id, byte[] flags, byte[] payload)
            : base(id,flags,payload)
        {
        }

        internal override byte[] EncodeFlags()
        {
            var flagsByte = new byte[2];

            // Decode the flags
            if (Flag.TagAlterPreservation)
            {
                flagsByte[0] |= 0x80;
            }

            if (Flag.FileAlterPreservation)
            {
                flagsByte[0] |= 0x40;
            }

            if (Flag.ReadOnly)
            {
                flagsByte[0] |= 0x20;
            }

            if (Flag.Compression)
            {
                flagsByte[1] |= 0x80;
            }

            if (Flag.Encryption)
            {
                flagsByte[1] |= 0x40;
            }

            if (Flag.GroupingIdentify)
            {
                flagsByte[1] |= 0x20;
            }

            return flagsByte;
        }

        protected override void DecodeFlags(byte[] flags)
        {
            Flag.TagAlterPreservation = (flags[0] & 0x80) == 0x80;
            Flag.FileAlterPreservation = (flags[0] & 0x40) == 0x40;
            Flag.ReadOnly = (flags[0] & 0x20) == 0x20;
            Flag.Compression = (flags[1] & 0x80) == 0x80;
            Flag.Encryption = (flags[1] & 0x40) == 0x40;
            Flag.GroupingIdentify = (flags[1] & 0x20) == 0x20;
        }
    }
}
