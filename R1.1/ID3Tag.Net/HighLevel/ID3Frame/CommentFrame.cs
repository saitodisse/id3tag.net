using System;
using System.Globalization;
using System.Text;
using Id3Tag.LowLevel;

namespace Id3Tag.HighLevel.Id3Frame
{
    /// <summary>
    /// This frame is indended for any kind of full text information that does not fit in any other frame. 
    /// It consists of a frame header followed by encoding, language and content descriptors and is 
    /// ended with the actual comment as a text string. Newline characters are allowed in the 
    /// comment text string. There may be more than one comment frame in each tag, but 
    /// only one with the same language and content descriptor.
    /// </summary>
    public class CommentFrame : EncodedTextFrame
    {
        /// <summary>
        /// Creates a new instance of CommentFrame.
        /// </summary>
        public CommentFrame()
            : this("ENG", "", "", Encoding.ASCII)
        {
        }

        /// <summary>
        /// Creates a new instance of CommentFrame.
        /// </summary>
        /// <param name="language">the language.</param>
        /// <param name="descriptor">the descriptor.</param>
        /// <param name="text">the text.</param>
        /// <param name="encoding">the text encoding.</param>
        public CommentFrame(string language, string descriptor, string text, Encoding encoding)
        {
            Descriptor.Id = "COMM";
            Language = language;
            ContentDescriptor = descriptor;
            Text = text;
            TextEncoding = encoding;
        }

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

        /// <summary>
        /// The frame type.
        /// </summary>
        public override FrameType Type
        {
            get { return FrameType.Comment; }
        }

        /// <summary>
        /// Convert the values to a raw frame.
        /// </summary>
        /// <returns>the raw frame.</returns>
        public override RawFrame Convert(TagVersion version)
        {
            FrameOptions options = Descriptor.Options;

            byte[] payload;
            using (var writer = new FrameDataWriter())
            {
                writer.WriteEncodingByte(TextEncoding);
                writer.WriteString(Language, Encoding.ASCII, 3);
                writer.WritePreamble(TextEncoding);
                writer.WriteString(ContentDescriptor, TextEncoding, true);
                writer.WriteString(Text, TextEncoding);
                payload = writer.ToArray();
            }

            return RawFrame.CreateFrame(Descriptor.Id, options, payload, version);
        }

        /// <summary>
        /// Import the raw frame.
        /// </summary>
        /// <param name="rawFrame">the raw frame.</param>
        /// <param name="codePage">Default code page for Ansi encoding. Pass 0 to use default system encoding code page.</param>
        public override void Import(RawFrame rawFrame, int codePage)
        {
            ImportRawFrameHeader(rawFrame);

            /*
				ID = "COMM"
				TextEncoding    xx
				Language        xx xx xx
				Short Content   (xx xx ... xx) (00 / 00 00)
				Text            (xx xx ... xx)
            */

            using (var reader = new FrameDataReader(rawFrame.Payload))
            {
                byte encodingByte = reader.ReadByte();
                Language = reader.ReadFixedString(Encoding.ASCII, 3);
                TextEncoding = reader.ReadEncoding(encodingByte, codePage);
                ContentDescriptor = reader.ReadVariableString(TextEncoding);
                Text = reader.ReadVariableString(TextEncoding);
            }
        }

        /// <summary>
        /// Overwrites ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format(
                CultureInfo.InvariantCulture,
                "Comment : Encoding = {0}, Language = {1}, Descriptor = {2}, Text = {3}",
                TextEncoding.EncodingName,
                Language,
                ContentDescriptor,
                Text);
        }
    }
}