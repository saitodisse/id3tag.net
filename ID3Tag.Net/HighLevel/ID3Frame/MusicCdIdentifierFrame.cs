using System.Text;
using ID3Tag.LowLevel;

namespace ID3Tag.HighLevel.ID3Frame
{
    public class MusicCdIdentifierFrame : Frame
    {
        public MusicCdIdentifierFrame()
        {
            TOC = new byte[0];
        }

        public MusicCdIdentifierFrame(byte[] toc)
        {
            Descriptor.ID = "MCDI";
            TOC = toc;
        }

        public byte[] TOC { get; set; }

        public override FrameType Type
        {
            get { return FrameType.MusicCDIdentifier; }
        }

        public override RawFrame Convert()
        {
            var flagBytes = Descriptor.GetFlagBytes();
            var frame = RawFrame.CreateFrame(Descriptor.ID, flagBytes, TOC);
            return frame;
        }

        public override void Import(RawFrame rawFrame)
        {
            ImportRawFrameHeader(rawFrame);

            TOC = rawFrame.Payload;
        }

        public override string ToString()
        {
            var sb = new StringBuilder("Music CD Identifier : ");

            sb.AppendFormat("ID : {0} ", Descriptor.ID);

            sb.Append("TOC : ");
            if (TOC != null)
            {
                sb.Append(Utils.BytesToString(TOC));
            }

            return sb.ToString();
        }
    }
}