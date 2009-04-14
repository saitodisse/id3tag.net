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
    public class TextFrame : Frame
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
        public TextFrame(string id, string content, TextEncodingType type)
        {
            Descriptor.ID = id;
            Content = content;
            TextEncoding = type;
        }

        /// <summary>
        /// The text encoding.
        /// </summary>
        public TextEncodingType TextEncoding { get; set; }
        /// <summary>
        /// The content.
        /// </summary>
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