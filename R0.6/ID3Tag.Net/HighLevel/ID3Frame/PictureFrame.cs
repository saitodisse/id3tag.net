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
    public class PictureFrame : EncodedTextFrame
    {
        /// <summary>
        /// Creates a new instance of PictureFrame.
        /// </summary>
        public PictureFrame()
            : this(TextEncodingType.Ansi, 0, String.Empty, String.Empty, PictureType.Other, new byte[0])
        {
        }

		/// <summary>
		/// Creates a new instance of PictureFrame.
		/// </summary>
		/// <param name="encoding">the text encoding</param>
		/// <param name="codePage">The codepage for text encoding = 0.</param>
		/// <param name="mimeType">the MIME type</param>
		/// <param name="description">the description</param>
		/// <param name="picture">the picture type</param>
		/// <param name="data">the picture bytes</param>
        public PictureFrame(TextEncodingType encoding, int codePage, string mimeType, string description, PictureType picture,
                            byte[] data)
        {
            TextEncoding = encoding;
			CodePage = codePage;
            MimeType = mimeType;
            Description = description;
            PictureCoding = picture;
            PictureData = data;
        }

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

        /// <summary>
        /// Defines the frame type.
        /// </summary>
        public override FrameType Type
        {
            get { return FrameType.Picture; }
        }

        /// <summary>
        /// Converts the picture frame to a raw frame.
        /// </summary>
        /// <returns>the raw frame.</returns>
        public override RawFrame Convert(TagVersion version)
        {
            var flag = Descriptor.GetFlags();
            var bytes = new List<byte>();

            var textEncodingByte = (byte) TextEncoding;
			var mimeBytes = Converter.GetContentBytes(TextEncodingType.Ansi, 28591, MimeType);
            var pictureEncodingByte = (byte) PictureCoding;
            var descriptionBytes = Converter.GetContentBytes(TextEncoding, CodePage, Description);

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

            var rawFrame = RawFrame.CreateFrame("APIC", flag, bytes.ToArray(), version);
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

		/// <summary>
		/// Import a raw frame
		/// </summary>
		/// <param name="rawFrame">the raw frame</param>
		/// <param name="codePage">Default code page for Ansi encoding. Pass 0 to use default system encoding code page.</param>
        public override void Import(RawFrame rawFrame, int codePage)
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
			CodePage = codePage;

            //
            //  Get the Mime.
            //
            var mimeBytes = new List<byte>();
            int curPos;
            for (curPos = 1; curPos < payload.Length; curPos++)
            {
                var curByte = payload[curPos];
                if (curByte == 0x00)
                {
                    break;
                }

                mimeBytes.Add(curByte);
            }

			var charBytes = Converter.Extract(TextEncodingType.Ansi, 28591, mimeBytes);
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

            var descriptionChars = Converter.Extract(TextEncoding, codePage, descriptionBytes.ToArray());
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