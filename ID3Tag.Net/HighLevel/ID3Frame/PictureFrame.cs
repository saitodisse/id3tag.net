using System;
using System.Collections.Generic;
using System.Text;
using ID3Tag.LowLevel;

namespace ID3Tag.HighLevel.ID3Frame
{
    /// <summary>
    /// This frame contains a picture directly related to the audio file. Image format is the MIME type and 
    /// subtype for the image. In the event that the MIME media type name is omitted, "image/" will be implied.
    /// The "image/png" or "image/jpeg" picture format should be used when interoperability is wanted. 
    /// Description is a short description of the picture, represented as a terminated textstring. 
    /// The description has a maximum length of 64 characters, but may be empty. There may be several 
    /// pictures attached to one file, each in their individual "APIC" frame, but only one with the 
    /// same content descriptor. There may only be one picture with the picture type declared as 
    /// picture type $01 and $02 respectively. There is the possibility to put only a link to the image 
    /// file by using the 'MIME type' "-->" and having a complete URL instead of picture data. 
    /// The use of linked files should however be used sparingly since there is the risk of separation of files.
    /// </summary>
    public class PictureFrame : Frame
    {
        /// <summary>
        /// Creates a new instance of PictureFrame.
        /// </summary>
        public PictureFrame()
            : this(TextEncodingType.ISO_8859_1, String.Empty, String.Empty, PictureType.Other, new byte[0])
        {
        }

        /// <summary>
        /// Creates a new instance of PictureFrame.
        /// </summary>
        /// <param name="encoding">the text encoding</param>
        /// <param name="mimeType">the MIME type</param>
        /// <param name="description">the description</param>
        /// <param name="picture">the picture type</param>
        /// <param name="data">the picture bytes</param>
        public PictureFrame(TextEncodingType encoding, string mimeType, string description, PictureType picture,
                            byte[] data)
        {
            TextEncoding = encoding;
            MimeType = mimeType;
            Description = description;
            PictureCoding = picture;
            PictureData = data;
        }

        /// <summary>
        /// The Text Encoding.
        /// </summary>
        public TextEncodingType TextEncoding { get; set; }

        /// <summary>
        /// The MIME Type.
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// The Description of the frame.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The Picture coding.
        /// </summary>
        public PictureType PictureCoding { get; set; }

        /// <summary>
        /// The Picture bytes.
        /// </summary>
        public byte[] PictureData { get; set; }

        public override FrameType Type
        {
            get { return FrameType.Picture; }
        }

        public override RawFrame Convert()
        {
            var flagBytes = Descriptor.GetFlagBytes();
            var bytes = new List<byte>();

            var textEncodingByte = (byte) TextEncoding;
            var mimeBytes = Converter.GetContentBytes(TextEncodingType.ISO_8859_1, MimeType);
            var pictureEncodingByte = (byte) PictureCoding;
            var descriptionBytes = Converter.GetContentBytes(TextEncoding, Description);

            bytes.Add(textEncodingByte);
            bytes.AddRange(mimeBytes);
            bytes.Add(0x00);
            bytes.Add(pictureEncodingByte);
            bytes.AddRange(descriptionBytes);

            var length = Converter.GetTerminationCharLength(TextEncoding);
            for (var i = 0; i < length; i++)
            {
                bytes.Add(0x00);
            }

            bytes.AddRange(PictureData);

            var rawFrame = RawFrame.CreateFrame("APIC", flagBytes, bytes.ToArray());
            return rawFrame;
        }

        /*
         * <Header for 'Attached picture', ID: "APIC"> 
            Text encoding   $xx
            MIME type       <text string> $00
            Picture type    $xx
            Description     <text string according to encoding> $00 (00)
            Picture data    <binary data>
         */

        public override void Import(RawFrame rawFrame)
        {
            ImportRawFrameHeader(rawFrame);

            var payload = rawFrame.Payload;
            if (payload.Length == 0)
            {
                throw new ID3TagException("Frame does not have a payload.");
            }

            //
            //  Get the TextEncoding.
            //
            var encodingByte = payload[0];
            TextEncoding = (TextEncodingType) encodingByte;

            //
            //  Get the Mime.
            //
            var mimeBytes = new List<byte>();
            var curPos = 0;
            for (curPos = 1; curPos < payload.Length; curPos++)
            {
                var curByte = payload[curPos];
                if (curByte == 0x00)
                {
                    break;
                }

                mimeBytes.Add(curByte);
            }

            var charBytes = Converter.Extract(TextEncodingType.ISO_8859_1, mimeBytes);
            MimeType = new string(charBytes);

            //
            //  Get the PictureType.
            //
            var pictureTypeByte = payload[curPos + 1];
            PictureCoding = (PictureType) pictureTypeByte;

            //
            //  Get the description
            //
            curPos += 2;
            var descriptionBytes = new List<byte>();
            var increment = Converter.GetTerminationCharLength(TextEncoding);
            for (; curPos < payload.Length; curPos += increment)
            {
                var isTerminatedSymbol = Converter.DetermineTerminateSymbol(payload, curPos, increment);
                if (isTerminatedSymbol)
                {
                    curPos += increment;
                    break;
                }

                var values = new byte[increment];
                Array.Copy(payload, curPos, values, 0, increment);
                descriptionBytes.AddRange(values);
            }

            var descriptionChars = Converter.Extract(TextEncoding, descriptionBytes.ToArray());
            Description = new string(descriptionChars);

            //
            //  Get the payload.
            //
            var pictureDataLength = payload.Length - curPos;
            var pictureDataBytes = new byte[pictureDataLength];

            Array.Copy(payload, curPos, pictureDataBytes, 0, pictureDataLength);
            PictureData = pictureDataBytes;
        }

        /// <summary>
        /// Overwrites ToString.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var builder = new StringBuilder("PictureFrame : ");

            builder.AppendFormat("Encoding : {0}, MIME Type : {1} ", TextEncoding, MimeType);
            builder.AppendFormat("Picture Type : {0} ", PictureCoding);
            builder.AppendFormat("Description : {0} ", Description);
            builder.AppendFormat("Data = {0}", Utils.BytesToString(PictureData));

            return builder.ToString();
        }
    }
}