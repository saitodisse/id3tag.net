using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Id3Tag.HighLevel;

namespace Id3Tag.LowLevel
{
    /// <summary>
    /// Represents a raw ID3 tag frame.
    /// </summary>
    public abstract class RawFrame
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RawFrame"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="payload">The payload.</param>
        protected RawFrame(string id, IList<byte> payload)
        {
            Id = id;
            Payload = new ReadOnlyCollection<byte>(payload);
        }

        /// <summary>
        /// The payload of the frame.
        /// </summary>
        public ReadOnlyCollection<byte> Payload { get; private set; }

        /// <summary>
        /// The frame ID.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        /// <value>The options.</value>
        public FrameOptions Options { get; protected set; }

        internal abstract byte[] EncodeFlags();

        #region Create a new ID3 Tag Frame

        internal byte[] GetIdBytes()
        {
            // Write the Frame ID
            var asciiEncoding = new ASCIIEncoding();
            byte[] idBytes = asciiEncoding.GetBytes(Id);

            return idBytes;
        }

        internal static RawFrame CreateV3Frame(string frameId, byte[] flags, byte[] payload)
        {
            var f = new RawFrameV3(frameId, flags, payload);
            return f;
        }

/*
        internal static RawFrame CreateV3Frame(string frameID, FrameOptions flags, byte[] payload)
        {
            var f = new RawFrameV3(frameID, flags, payload);
            return f;
        }
*/

        internal static RawFrame CreateV4Frame(string frameId, byte[] flags, byte[] payload)
        {
            var f = new RawFrameV4(frameId, flags, payload);
            return f;
        }

/*
        internal static RawFrame CreateV4Frame(string frameID, FrameOptions flags, byte[] payload)
        {
            var f = new RawFrameV4(frameID, flags, payload);
            return f;
        }
*/

        internal static RawFrame CreateFrame(string frameId, FrameOptions options, IList<byte> payload,
                                             TagVersion version)
        {
            RawFrame frame;
            switch (version)
            {
                case TagVersion.Id3V23:
                    frame = new RawFrameV3(frameId, options, payload);
                    break;
                case TagVersion.Id3V24:
                    frame = new RawFrameV4(frameId, options, payload);
                    break;
                default:
                    throw new Id3TagException("Unknown Tag Version found!");
            }

            return frame;
        }

        #endregion
    }
}