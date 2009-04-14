using System;
using System.Text;
using ID3Tag.LowLevel;

namespace ID3Tag.HighLevel.ID3Frame
{
    /// <summary>
    /// This frame is used to contain information from a software producer that its program uses 
    /// and does not fit into the other frames. The frame consists of an 'Owner identifier' string 
    /// and the binary data. The 'Owner identifier' is a null-terminated string with a URL 
    /// containing an email address, or a link to a location where an email address can be found, 
    /// that belongs to the organisation responsible for the frame. Questions regarding the 
    /// frame should be sent to the indicated email address. The tag may contain more than 
    /// one "PRIV" frame but only with different contents. It is recommended to keep the 
    /// number of "PRIV" frames as low as possible.
    /// </summary>
    public class PrivateFrame : Frame
    {
        public PrivateFrame()
        {
            Owner = String.Empty;
            Data = new byte[0];
        }

        public PrivateFrame(string owner, byte[] data)
        {
            Descriptor.ID = "PRIV";
            Owner = owner;
            Data = data;
        }

        public string Owner { get; set; }
        public byte[] Data { get; set; }

        public override FrameType Type
        {
            get { return FrameType.Private; }
        }

        public override RawFrame Convert()
        {
            var flagBytes = Descriptor.GetFlagBytes();
            var ownerBytes = Converter.GetContentBytes(TextEncodingType.ISO_8859_1, Owner);

            var payloadBytes = new byte[ownerBytes.Length + 1 + Data.Length];
            Array.Copy(ownerBytes, 0, payloadBytes, 0, ownerBytes.Length);
            Array.Copy(Data, 0, payloadBytes, ownerBytes.Length + 1, Data.Length);

            var rawFrame = RawFrame.CreateFrame(Descriptor.ID, flagBytes, payloadBytes);
            return rawFrame;
        }

        public override void Import(RawFrame rawFrame)
        {
            ImportRawFrameHeader(rawFrame);

            //
            //  <text> 00 <data>
            //
            var payload = rawFrame.Payload;
            var items = Converter.SplitByteArray(payload);

            if (items.Count < 2)
            {
                throw new ID3TagException("Could not decode PrivateFrame : Payload decoding failed.");
            }

            var ownerBytes = items[0];
            var ownerChars = Converter.Extract(TextEncodingType.ISO_8859_1, ownerBytes, false);
            Owner = new string(ownerChars);

            Data = items[1];
        }

        public override string ToString()
        {
            var sb = new StringBuilder("PrivateFrame : ");

            sb.AppendFormat("Owner = {0} ", Owner);
            sb.AppendFormat("Data = {0}", Utils.BytesToString(Data));

            return sb.ToString();
        }
    }
}