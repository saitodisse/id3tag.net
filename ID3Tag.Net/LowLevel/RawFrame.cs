using System.Text;

namespace ID3Tag.LowLevel
{
    public class RawFrame
    {
        private RawFrame(string id, byte[] flags, byte[] payload)
        {
            ID = id;
            Payload = payload;

            AnalyseFrameTags(flags);
        }

        public byte[] Payload { get; private set; }
        public string ID { get; private set; }
        public bool TagAlterPreservation { get; private set; }
        public bool FileAlterPreservation { get; private set; }
        public bool ReadOnly { get; private set; }
        public bool Compression { get; private set; }
        public bool Encryption { get; private set; }
        public bool GroupingIdentify { get; private set; }

        private void AnalyseFrameTags(byte[] flags)
        {
            TagAlterPreservation = (flags[0] & 0x80) == 0x80;
            FileAlterPreservation = (flags[0] & 0x40) == 0x40;
            ReadOnly = (flags[0] & 0x20) == 0x20;
            Compression = (flags[1] & 0x80) == 0x80;
            Encryption = (flags[1] & 0x40) == 0x40;
            GroupingIdentify = (flags[1] & 0x20) == 0x20;
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
            if (TagAlterPreservation)
            {
                flagsByte[0] |= 0x80;
            }

            if (FileAlterPreservation)
            {
                flagsByte[0] |= 0x40;
            }

            if (ReadOnly)
            {
                flagsByte[0] |= 0x20;
            }

            if (Compression)
            {
                flagsByte[1] |= 0x80;
            }

            if (Encryption)
            {
                flagsByte[1] |= 0x40;
            }

            if (GroupingIdentify)
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