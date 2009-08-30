using System.Text;
using ID3Tag.LowLevel;

namespace ID3Tag.HighLevel.ID3Frame
{
    /// <summary>
    /// Represents a frame that cannot be identified by the reader.
    /// </summary>
    public class UnknownFrame : Frame
    {
        /// <summary>
        /// Creates a new Unknown frame.
        /// </summary>
        public UnknownFrame()
        {
            Content = new byte[0];
        }

        /// <summary>
        /// The payload.
        /// </summary>
        public byte[] Content { get; set; }

        /// <summary>
        /// The frame type.
        /// </summary>
        public override FrameType Type
        {
            get { return FrameType.Unknown; }
        }

        /// <summary>
        /// Convert the Unknownframe.
        /// </summary>
        /// <returns>a RawFrame.</returns>
        public override RawFrame Convert(TagVersion version)
        {
            var flags = Descriptor.GetFlags();

            var frame = RawFrame.CreateFrame(Descriptor.ID, flags, Content, version);
            return frame;
        }

		/// <summary>
		/// Import the the raw frame data.
		/// </summary>
		/// <param name="rawFrame">the raw frame.</param>
		/// <param name="codePage">Default code page for Ansi encoding. Pass 0 to use default system encoding code page.</param>
        public override void Import(RawFrame rawFrame, int codePage)
        {
            ImportRawFrameHeader(rawFrame);
            Content = rawFrame.Payload;
        }

        /// <summary>
        /// Overwrite ToString.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder("UnknownFrame : ");

            sb.AppendFormat("ID : {0} ", Descriptor.ID);

            if (Content != null)
            {
                sb.Append(Utils.BytesToString(Content));
            }

            return sb.ToString();
        }
    }
}