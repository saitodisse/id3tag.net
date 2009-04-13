using System.Text;
using ID3Tag.LowLevel;

namespace ID3Tag.HighLevel.ID3Frame
{
    public class UrlLinkFrame : Frame
    {
        public UrlLinkFrame()
        {
        }

        public UrlLinkFrame(string id, string url)
        {
            Descriptor.ID = id;
            URL = url;
        }

        public string URL { get; set; }

        public override FrameType Type
        {
            get { return FrameType.URLLink; }
        }

        public override RawFrame Convert()
        {
            var flagBytes = Descriptor.GetFlagBytes();
            var payloadBytes = Converter.GetContentBytes(TextEncodingType.ISO_8859_1, URL);

            var rawFrame = RawFrame.CreateFrame(Descriptor.ID, flagBytes, payloadBytes);
            return rawFrame;
        }

        public override void Import(RawFrame rawFrame)
        {
            ImportRawFrameHeader(rawFrame);

            // Simple ISO8859-1 coding in the payload!

            var chars = Converter.Extract(TextEncodingType.ISO_8859_1, rawFrame.Payload, true);
            URL = new string(chars);
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder("URL Link Frame : ");

            stringBuilder.Append("URL : ");
            stringBuilder.Append(URL);

            return stringBuilder.ToString();
        }
    }
}