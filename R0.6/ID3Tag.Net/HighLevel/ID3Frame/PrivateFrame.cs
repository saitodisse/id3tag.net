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
        /// <summary>
        /// Creates a new instance of PrivateFrame.
        /// </summary>
        public PrivateFrame() : this(String.Empty, new byte[0])
        {
        }

        /// <summary>
        /// Creates a new instance of PrivateFrame.
        /// </summary>
        /// <param name="owner">the owner.</param>
        /// <param name="data">the data.</param>
        public PrivateFrame(string owner, byte[] data)
        {
            Descriptor.ID = "PRIV";
            Owner = owner;
            Data = data;
        }

        /// <summary>
        /// The owner.
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// The data.
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// The frame type.
        /// </summary>
        public override FrameType Type
        {
            get { return FrameType.Private; }
        }

        /// <summary>
        /// Convert the values to a raw frame.
        /// </summary>
        /// <returns>the raw frame.</returns>
        public override RawFrame Convert()
        {
            var flagBytes = Descriptor.GetFlagBytes();
            var ownerBytes = Converter.GetContentBytes(TextEncodingType.ISO_8859_1, Owner);

            var payloadBytes = new byte[ownerBytes.Length + 1 + Data.Length];
            Array.Copy(ownerBytes, 0, payloadBytes, 0, ownerBytes.Length);
            Array.Copy(Data, 0, payloadBytes, ownerBytes.Length + 1, Data.Length);

            var rawFrame = RawFrame.CreateV3Frame(Descriptor.ID, flagBytes, payloadBytes);
            return rawFrame;
        }

        /// <summary>
        /// Import the raw frame.
        /// </summary>
        /// <param name="rawFrame">the raw frame.</param>
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

        /// <summary>
        /// Overwrite ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder("PrivateFrame : ");

            sb.AppendFormat("Owner = {0} ", Owner);
            sb.AppendFormat("Data = {0}", Utils.BytesToString(Data));

            return sb.ToString();
        }
    }
}