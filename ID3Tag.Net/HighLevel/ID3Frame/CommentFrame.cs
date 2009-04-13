using System;
using System.Text;
using ID3Tag.LowLevel;

namespace ID3Tag.HighLevel.ID3Frame
{
    public class CommentFrame : Frame
    {
        public CommentFrame()
        {
        }

        public CommentFrame(string language, string descriptor, string text, TextEncodingType type)
        {
            Descriptor.ID = "COMM";
            Language = language;
            ContentDescriptor = descriptor;
            Text = text;
            TextEncoding = type;
        }

        public TextEncodingType TextEncoding { get; set; }
        public string Language { get; set; }
        public string ContentDescriptor { get; set; }
        public string Text { get; set; }

        public override FrameType Type
        {
            get { return FrameType.Comment; }
        }

        public override RawFrame Convert()
        {
            var flagsBytes = Descriptor.GetFlagBytes();
            var dataBuilder = new StringBuilder();

            dataBuilder.Append(ContentDescriptor);
            dataBuilder.Append('\u0000');
            dataBuilder.Append(Text);

            var dataBytes = Converter.GetContentBytes(TextEncoding, dataBuilder.ToString());
            var languageBytes = Converter.GetContentBytes(TextEncodingType.ISO_8859_1, Language);
            var payloadBytes = new byte[4 + dataBytes.Length];

            payloadBytes[0] = System.Convert.ToByte(TextEncoding);
            Array.Copy(languageBytes, 0, payloadBytes, 1, 3);
            Array.Copy(dataBytes, 0, payloadBytes, 4, dataBytes.Length);

            var rawFrame = RawFrame.CreateFrame(Descriptor.ID, flagsBytes, payloadBytes);
            return rawFrame;
        }

        public override void Import(RawFrame rawFrame)
        {
            ImportRawFrameHeader(rawFrame);

            /*
             *  ID = "COMM"
             *  TextEncoding    xx
             *  Language        xx xx xx
             *  Short Content   (xx xx ... xx) (00 / 00 00)
             *  Text            (xx xx ... xx)
             */

            var payload = rawFrame.Payload;
            var languageBytes = new byte[3];
            var textBytes = new byte[payload.Length - 4];

            TextEncoding = (TextEncodingType) payload[0];
            Array.Copy(payload, 1, languageBytes, 0, 3);
            var languageChars = Converter.Extract(TextEncodingType.ISO_8859_1, languageBytes, false);
            Language = new string(languageChars);

            Array.Copy(payload, 4, textBytes, 0, textBytes.Length);
            var chars = Converter.Extract(TextEncoding, textBytes);

            string descriptor;
            string text;
            Converter.DecodeDescriptionValuePairs(chars, out descriptor, out text);
            ContentDescriptor = descriptor;
            Text = text;
        }

        public override string ToString()
        {
            var builder = new StringBuilder("Comment : ");

            builder.AppendFormat("Encoding = {0} ", TextEncoding);
            builder.AppendFormat("Language = {0} ", Language);
            builder.AppendFormat("Descriptor = {0} ", ContentDescriptor);
            builder.AppendFormat("Text = {0}", Text);

            return builder.ToString();
        }
    }
}