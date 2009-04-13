using System.Text;
using ID3Tag.LowLevel;

namespace ID3Tag.HighLevel.ID3Frame
{
    public class UnknownFrame : Frame
    {
        public UnknownFrame()
        {
            Content = new byte[0];
        }

        public byte[] Content { get; set; }

        public override FrameType Type
        {
            get { return FrameType.Unknown; }
        }

        public override RawFrame Convert()
        {
            var flagBytes = Descriptor.GetFlagBytes();
            var frame = RawFrame.CreateFrame(Descriptor.ID, flagBytes, Content);
            return frame;
        }

        public override void Import(RawFrame rawFrame)
        {
            ImportRawFrameHeader(rawFrame);

            Content = rawFrame.Payload;
        }

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