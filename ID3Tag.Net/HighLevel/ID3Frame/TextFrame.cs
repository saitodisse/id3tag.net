using System;
using System.Text;
using ID3Tag.LowLevel;

namespace ID3Tag.HighLevel.ID3Frame
{
    public class TextFrame : Frame
    {
        public TextFrame()
        {
        }

        public TextFrame(string id, string content, TextEncodingType type)
        {
            Descriptor.ID = id;
            Content = content;
            TextEncoding = type;
        }

        public TextEncodingType TextEncoding { get; set; }
        public string Content { get; set; }

        public override FrameType Type
        {
            get { return FrameType.Text; }
        }

        public override RawFrame Convert()
        {
            var flagBytes = Descriptor.GetFlagBytes();
            var contentBytes = Converter.GetContentBytes(TextEncoding, Content);
            var payloadBytes = new byte[contentBytes.Length + 1];

            //
            //  Payload :  XX Y1 Y2 ... Yn  ( XX = Endoding Type, Yx = Data )
            //
            payloadBytes[0] = System.Convert.ToByte(TextEncoding);
            Array.Copy(contentBytes, 0, payloadBytes, 1, contentBytes.Length);

            var rawFrame = RawFrame.CreateFrame(Descriptor.ID, flagBytes, payloadBytes);
            return rawFrame;
        }

        public override void Import(RawFrame rawFrame)
        {
            ImportRawFrameHeader(rawFrame);

            if (rawFrame.Payload.Length > 0)
            {
                TextEncoding = (TextEncodingType) rawFrame.Payload[0];
            }

            if (rawFrame.Payload.Length > 1)
            {
                var contentLength = rawFrame.Payload.Length - 1;
                var content = new byte[contentLength];
                Array.Copy(rawFrame.Payload, 1, content, 0, contentLength);

                var chars = Converter.Extract(TextEncoding, content, true);
                Content = new string(chars);
            }
            else
            {
                Content = String.Empty;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder("TextFrame : ");
            sb.AppendFormat("ID : {0} ", Descriptor.ID);
            sb.AppendFormat("Encoding : {0} , Text = {1}", TextEncoding, Content);

            return sb.ToString();
        }
    }
}