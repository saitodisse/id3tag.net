using System.Text;

namespace ID3Tag.LowLevel
{
    /// <summary>
    /// Represents a raw ID3 tag frame.
    /// </summary>
    public abstract class RawFrame
    {
        protected RawFrame(string id, byte[] flags, byte[] payload)
        {
            ID = id;
            Payload = payload;
            Flag = new FrameFlags();

            DecodeFlags(flags);
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

        protected abstract void DecodeFlags(byte[] flags);
        internal abstract byte[] EncodeFlags();

        #region Create a new ID3 Tag Frame

        internal byte[] GetIDBytes()
        {
            // Write the Frame ID
            var asciiEncoding = new ASCIIEncoding();
            var idBytes = asciiEncoding.GetBytes(ID);

            return idBytes;
        }

        internal static RawFrame CreateFrame(string frameID, byte[] flags, byte[] payload)
        {
            var f = new RawFrameV3(frameID, flags, payload);
            return f;
        }

        #endregion
    }
}