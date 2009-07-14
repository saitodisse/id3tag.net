using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ID3Tag.LowLevel
{
    public class RawFrameV4 : RawFrame
    {
        public RawFrameV4(string id, byte[] flags, byte[] payload)
            : base(id,flags,payload)
        {
        }

        protected override void DecodeFlags(byte[] flags)
        {
            // TODO: Decode the flags...
        }

        internal override byte[] EncodeFlags()
        {
            // TODO: Encode the flags...
            return null;
        }
    }
}
