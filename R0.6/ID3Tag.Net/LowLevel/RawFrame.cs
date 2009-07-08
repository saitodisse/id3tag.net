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

        /// <summary>
        /// The TagAlterPreservation flag.
        /// </summary>
        public bool TagAlterPreservation { get; private set; }

        /// <summary>
        /// The FileAlterPreservation flag.
        /// </summary>
        public bool FileAlterPreservation { get; private set; }

        /// <summary>
        /// The ReadOnly flag.
        /// </summary>
        public bool ReadOnly { get; private set; }

        /// <summary>
        /// The Compression flag.
        /// </summary>
        public bool Compression { get; private set; }

        /// <summary>
        /// The Encryption flag.
        /// </summary>
        public bool Encryption { get; private set; }

        /// <summary>
        /// The GroupingIdentify flag.
        /// </summary>
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