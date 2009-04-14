using System;
using System.Text;
using ID3Tag.LowLevel;

namespace ID3Tag.HighLevel.ID3Frame
{

    public class UserDefinedTextFrame : Frame
    {
        public UserDefinedTextFrame()
        {
        }

        public UserDefinedTextFrame(string description, string value, TextEncodingType type)
        {
            Descriptor.ID = "TXXX";
            Description = description;
            Value = value;
            TextEncoding = type;
        }

        public TextEncodingType TextEncoding { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }

        public override FrameType Type
        {
            get { return FrameType.UserDefinedText; }
        }

        public override RawFrame Convert()
        {
            var flagBytes = Descriptor.GetFlagBytes();
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

            var rawFrame = RawFrame.CreateFrame(Descriptor.ID, flagBytes, payloadBytes);
            return rawFrame;
        }

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

        public override string ToString()
        {
            return String.Format("UserDefinedTextFrame : Coding : {0}, Description = {1}, Value = {2}", TextEncoding,
                                 Description, Value);
        }
    }
}