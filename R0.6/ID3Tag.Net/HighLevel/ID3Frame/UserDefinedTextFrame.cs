using System;
using System.Text;
using ID3Tag.LowLevel;

namespace ID3Tag.HighLevel.ID3Frame
{
    /// <summary>
    /// This frame is intended for one-string text information concerning the 
    /// audiofile in a similar way to the other "T"-frames. The frame body consists 
    /// of a description of the string, represented as a terminated string, 
    /// followed by the actual string. There may be more than one "TXXX" frame in each tag, 
    /// but only one with the same description.
    /// </summary>
    public class UserDefinedTextFrame : Frame
    {
        /// <summary>
        /// Creates a new UserDefindedTextFrame.
        /// </summary>
        public UserDefinedTextFrame()
        {
            Descriptor.ID = "TXXX";
        }

        /// <summary>
        /// Creates a new UserDefindedTextFrame.
        /// </summary>
        /// <param name="description">the description.</param>
        /// <param name="value">the value.</param>
        /// <param name="type">the text encoding.</param>
        public UserDefinedTextFrame(string description, string value, TextEncodingType type)
        {
            Descriptor.ID = "TXXX";
            Description = description;
            Value = value;
            TextEncoding = type;
        }

        /// <summary>
        /// The text encoding.
        /// </summary>
        public TextEncodingType TextEncoding { get; set; }

        /// <summary>
        /// The description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The frame type.
        /// </summary>
        public override FrameType Type
        {
            get { return FrameType.UserDefinedText; }
        }

        /// <summary>
        /// Convert the values to a raw frame.
        /// </summary>
        /// <returns>a raw frame.</returns>
        public override RawFrame Convert(TagVersion version)
        {
            var flag= Descriptor.GetFlags();
            var encodingByte = System.Convert.ToByte(TextEncoding);
            var contentBuilder = new StringBuilder();
            contentBuilder.Append(Description);
            contentBuilder.Append('\u0000');
            contentBuilder.Append(Value);

            var contentBytes = Converter.GetContentBytes(TextEncoding, contentBuilder.ToString());

            //
            //  Payload :  XX Y1 Y2 ... Yn 00 Z1 Z2 ... Zn
            //
            // ( XX = Endoding Type, Yx = Data, Zx = Data )
            //

            var payloadBytes = new byte[contentBytes.Length + 1];
            payloadBytes[0] = System.Convert.ToByte(encodingByte);
            Array.Copy(contentBytes, 0, payloadBytes, 1, contentBytes.Length);

            var rawFrame = RawFrame.CreateFrame(Descriptor.ID, flag, payloadBytes,version);
            return rawFrame;
        }

        /// <summary>
        /// Import the raw frame.
        /// </summary>
        /// <param name="rawFrame">the raw frame.</param>
        public override void Import(RawFrame rawFrame)
        {
            ImportRawFrameHeader(rawFrame);
            TextEncoding = (TextEncodingType) rawFrame.Payload[0];

            var contentLength = rawFrame.Payload.Length - 1;
            var content = new byte[contentLength];
            Array.Copy(rawFrame.Payload, 1, content, 0, contentLength);

            /*
             *  TextEncoding        XX
             *  Desc                Data (00 | 0000)
             *  Value               Data
             * 
             */

            var chars = Converter.Extract(TextEncoding, content);

            string descriptionText;
            string valueText;
            Converter.DecodeDescriptionValuePairs(chars, out descriptionText, out valueText);

            Description = descriptionText;
            Value = valueText;
        }

        /// <summary>
        /// Overwrite ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("UserDefinedTextFrame : Coding : {0}, Description = {1}, Value = {2}", TextEncoding,
                                 Description, Value);
        }
    }
}