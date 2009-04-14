using System;
using System.Text;
using ID3Tag.LowLevel;

namespace ID3Tag.HighLevel.ID3Frame
{
    /// <summary>
    /// This frame is indended for any kind of full text information that does not fit in any other frame. 
    /// It consists of a frame header followed by encoding, language and content descriptors and is 
    /// ended with the actual comment as a text string. Newline characters are allowed in the 
    /// comment text string. There may be more than one comment frame in each tag, but 
    /// only one with the same language and content descriptor.
    /// </summary>
    public class CommentFrame : Frame
    {
        /// <summary>
        /// Creates a new instance of CommentFrame.
        /// </summary>
        public CommentFrame()
        {
        }

        /// <summary>
        /// Creates a new instance of CommentFrame.
        /// </summary>
        /// <param name="descriptor">the descriptor.</param>
        /// <param name="language">the language.</param>
        /// <param name="text">the text.</param>
        /// <param name="type">the text encoding.</param>
        public CommentFrame(string language, string descriptor, string text, TextEncodingType type)
        {
            Descriptor.ID = "COMM";
            Language = language;
            ContentDescriptor = descriptor;
            Text = text;
            TextEncoding = type;
        }

        /// <summary>
        /// The text encoding.
        /// </summary>
        public TextEncodingType TextEncoding { get; set; }
        /// <summary>
        /// The language.
        /// </summary>
        public string Language { get; set; }
        /// <summary>
        /// The content descriptor.
        /// </summary>
        public string ContentDescriptor { get; set; }
        /// <summary>
        /// The text.
        /// </summary>
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

        /// <summary>
        /// Overwrites ToString
        /// </summary>
        /// <returns></returns>
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