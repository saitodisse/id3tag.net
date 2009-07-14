using System.Text;

namespace ID3Tag.LowLevel
{
    /// <summary>
    /// Represents a raw ID3 tag frame.
    /// </summary>
    public class RawFrame
    {
        private RawFrame(string id, byte[] flags, byte[] payload)
        {
            ID = id;
            Payload = payload;
            Flag = new FrameFlags();

            AnalyseFrameTags(flags);
        }

        /// <summary>
        /// The payload of the frame.
        /// </summary>
        public byte[] Payload { get; private set; }

        /// <summary>
        /// The frame ID.
        /// </summary>
        public string ID { get; private set; }

        public FrameFlags Flag { get; private set; }

        private void AnalyseFrameTags(byte[] flags)
        {
            Flag.TagAlterPreservation = (flags[0] & 0x80) == 0x80;
            Flag.FileAlterPreservation = (flags[0] & 0x40) == 0x40;
            Flag.ReadOnly = (flags[0] & 0x20) == 0x20;
            Flag.Compression = (flags[1] & 0x80) == 0x80;
            Flag.Encryption = (flags[1] & 0x40) == 0x40;
            Flag.GroupingIdentify = (flags[1] & 0x20) == 0x20;
        }

        #region Create a new ID3 Tag Frame

        internal byte[] GetIDBytes()
        {
            // Write the Frame ID
            var asciiEncoding = new ASCIIEncoding();
            var idBytes = asciiEncoding.GetBytes(ID);

            return idBytes;
        }

        internal byte[] GetFlags()
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

        internal static RawFrame CreateFrame(string frameID, byte[] flags, byte[] payload)
        {
            var f = new RawFrame(frameID, flags, payload);
            return f;
        }

        #endregion
    }
}