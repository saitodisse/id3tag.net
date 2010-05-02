using System;
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
		public PictureFrame() : this(Encoding.ASCII, String.Empty, String.Empty, PictureType.Other, new byte[0])
		{}

		/// <summary>
		/// Creates a new instance of PictureFrame.
		/// </summary>
		/// <param name="encoding">the text encoding</param>
		/// <param name="mimeType">the MIME type</param>
		/// <param name="description">the description</param>
		/// <param name="picture">the picture type</param>
		/// <param name="data">the picture bytes</param>
		public PictureFrame(Encoding encoding, string mimeType, string description, PictureType picture, byte[] data)
		{
		    Descriptor.ID = "APIC";
			TextEncoding = encoding;
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
			FrameFlags flag = Descriptor.GetFlags();

			byte[] payload;
			using (var writer = new FrameDataWriter())
			{
				writer.WriteEncodingByte(TextEncoding);
				writer.WriteString(MimeType, Encoding.ASCII, true);
				writer.WriteByte((byte)PictureCoding);
				writer.WritePreamble(TextEncoding);
				writer.WriteString(Description, TextEncoding, true);
				writer.WriteBytes(PictureData);
				payload = writer.ToArray();
			}

			return RawFrame.CreateFrame("APIC", flag, payload, version);
		}

		/*
           <Header for 'Attached picture', ID: "APIC"> 
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

			if (rawFrame.Payload.Length == 0)
			{
				throw new ID3TagException("Frame has no payload.");
			}

			using (var reader = new FrameDataReader(rawFrame.Payload))
			{
				var encodingByte = reader.ReadByte();
				MimeType = reader.ReadVariableString(Encoding.ASCII);
				PictureCoding = (PictureType)reader.ReadByte();
				TextEncoding = reader.ReadEncoding(encodingByte, codePage);
				Description = reader.ReadVariableString(TextEncoding);
				PictureData = reader.ReadBytes();
			}
		}

		/// <summary>
		/// Overwrites ToString.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return
				String.Format(
					"Picture : Encoding = {0}, MIME = {1}, Type = {2}, Description = {3}, Data = {4}",
					TextEncoding.EncodingName,
					MimeType,
					PictureCoding,
					Description,
					Utils.BytesToString(PictureData));
		}
	}
}