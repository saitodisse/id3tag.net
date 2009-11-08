using System;
using System.Text;
using ID3Tag.LowLevel;

namespace ID3Tag.HighLevel.ID3Frame
{
	/// <summary>
	/// This frame is intended for URL links concerning the audiofile in a similar way to the 
	/// other "W"-frames. The frame body consists of a description of the string, represented 
	/// as a terminated string, followed by the actual URL. The URL is always encoded with ISO-8859-1. 
	/// There may be more than one "WXXX" frame in each tag, but only one with the same description.
	/// </summary>
	public class UserDefinedURLLinkFrame : EncodedTextFrame
	{
		/// <summary>
		/// Creates a new UserDefinedURLLinkFrame
		/// </summary>
		public UserDefinedURLLinkFrame()
            : this(String.Empty,String.Empty,Encoding.ASCII)
		{}

		/// <summary>
		/// Creates a new UserDefinedURLLinkFrame
		/// </summary>
		/// <param name="description">the Description</param>
		/// <param name="url">The URL</param>
		/// <param name="encoding">The text encoding type.</param>
		public UserDefinedURLLinkFrame(string description, string url, Encoding encoding)
		{
			Descriptor.ID = "WXXX";
			Description = description;
			URL = url;
			TextEncoding = encoding;
		}

		/// <summary>
		/// The description
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// The URL.
		/// </summary>
		public string URL { get; set; }

		/// <summary>
		/// The frame type.
		/// </summary>
		public override FrameType Type
		{
			get { return FrameType.UserDefindedURLLink; }
		}

		/// <summary>
		/// Convert the values to a raw frame.
		/// </summary>
		/// <returns>the raw frame.</returns>
		public override RawFrame Convert(TagVersion version)
		{
			FrameFlags flag = Descriptor.GetFlags();

			byte[] payload;
			using (var writer = new FrameDataWriter())
			{
				writer.WriteEncodingByte(TextEncoding);
				writer.WritePreamble(TextEncoding);
				writer.WriteString(Description, TextEncoding, true);
				writer.WriteString(URL, Encoding.GetEncoding(28591));
				payload = writer.ToArray();
			}

			return RawFrame.CreateFrame(Descriptor.ID, flag, payload, version);
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
             *  Text Encoding   xx
             *  Description     (xx xx .. xx) (00 / 00 00)
             *  URL             (xx xx ... xx)  als ISO8859-1 !
             */

			using (var reader = new FrameDataReader(rawFrame.Payload))
			{
				byte encodingByte = reader.ReadByte();
				TextEncoding = reader.ReadEncoding(encodingByte, codePage);
				Description = reader.ReadVariableString(TextEncoding);
				URL = reader.ReadVariableString(Encoding.GetEncoding(28591));
			}
		}

		/// <summary>
		/// Overwrite ToString
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return String.Format(
				"User-Definded URL : Encoding = {0}, Description = {1}, URL : {2} ", TextEncoding, Description, URL);
		}
	}
}