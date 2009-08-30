using System;
using System.Text;
using ID3Tag.LowLevel;

namespace ID3Tag.HighLevel.ID3Frame
{
    /// <summary>
    /// The text information frames are the most important frames, containing information like artist, 
    /// album and more. There may only be one text information frame of its kind in an tag. 
    /// If the textstring is followed by a termination ($00 (00)) all the following information 
    /// should be ignored and not be displayed. All text frame identifiers begin with "T". 
    /// Only text frame identifiers begin with "T", with the exception of the "TXXX" frame. 
    /// </summary>
    public class TextFrame : EncodedTextFrame
    {
        /// <summary>
        /// Creates a new TextFrame.
        /// </summary>
        public TextFrame()
        {
        }

		/// <summary>
		/// Creates a new TextFrame.
		/// </summary>
		/// <param name="id">the frame id.</param>
		/// <param name="content">the content.</param>
		/// <param name="type">the text encoding.</param>
		/// <param name="codePage">The code page.</param>
        public TextFrame(string id, string content, TextEncodingType type, int codePage)
        {
            Descriptor.ID = id;
            Content = content;
            TextEncoding = type;
        	CodePage = codePage;
        }

        /// <summary>
        /// The content.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// The frame type.
        /// </summary>
        public override FrameType Type
        {
            get { return FrameType.Text; }
        }

        /// <summary>
        /// Convert the values to a raw frame.
        /// </summary>
        /// <returns>the raw frame.</returns>
        public override RawFrame Convert(TagVersion version)
        {
            var flags = Descriptor.GetFlags();
            var contentBytes = Converter.GetContentBytes(TextEncoding, CodePage, Content);
            var payloadBytes = new byte[contentBytes.Length + 1];

            //
            //  Payload :  XX Y1 Y2 ... Yn  ( XX = Endoding Type, Yx = Data )
            //
            payloadBytes[0] = System.Convert.ToByte(TextEncoding);
            Array.Copy(contentBytes, 0, payloadBytes, 1, contentBytes.Length);

            var rawFrame = RawFrame.CreateFrame(Descriptor.ID, flags, payloadBytes, version);
            return rawFrame;
        }

		/// <summary>
		/// Import the raw frame.
		/// </summary>
		/// <param name="rawFrame">the raw frame.</param>
		/// <param name="codePage">Default code page for Ansi encoding. Pass 0 to use default system encoding code page.</param>
        public override void Import(RawFrame rawFrame, int codePage)
        {
            ImportRawFrameHeader(rawFrame);

            if (rawFrame.Payload.Length > 0)
            {
                TextEncoding = (TextEncodingType) rawFrame.Payload[0];
            	CodePage = codePage;
            }

            if (rawFrame.Payload.Length > 1)
            {
                var contentLength = rawFrame.Payload.Length - 1;
                var content = new byte[contentLength];
                Array.Copy(rawFrame.Payload, 1, content, 0, contentLength);

                var chars = Converter.Extract(TextEncoding, codePage, content, true);
                Content = new string(chars);
            }
            else
            {
                Content = String.Empty;
            }
        }

        /// <summary>
        /// Overwrites ToString.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder("TextFrame : ");
            sb.AppendFormat("ID : {0} ", Descriptor.ID);
            sb.AppendFormat("Encoding : {0} , Text = {1}", TextEncoding, Content);

            return sb.ToString();
        }
    }
}