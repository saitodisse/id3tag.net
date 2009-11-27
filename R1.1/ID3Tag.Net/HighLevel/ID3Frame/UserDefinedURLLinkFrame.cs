using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using Id3Tag.LowLevel;

namespace Id3Tag.HighLevel.Id3Frame
{
	/// <summary>
	/// This frame is intended for URL links concerning the audiofile in a similar way to the 
	/// other "W"-frames. The frame body consists of a description of the string, represented 
	/// as a terminated string, followed by the actual URL. The URL is always encoded with ISO-8859-1. 
	/// There may be more than one "WXXX" frame in each tag, but only one with the same description.
	/// </summary>
	public class UserDefinedUrlLinkFrame : EncodedTextFrame
	{
		/// <summary>
		/// Creates a new UserDefinedUrlLinkFrame
		/// </summary>
		public UserDefinedUrlLinkFrame()
			: this(String.Empty, String.Empty, Encoding.ASCII)
		{}

		/// <summary>
		/// Creates a new UserDefinedUrlLinkFrame
		/// </summary>
		/// <param name="description">the Description</param>
		/// <param name="url">The URL</param>
		/// <param name="encoding">The text encoding type.</param>
		[SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "URL validation is not performed.")]
		public UserDefinedUrlLinkFrame(string description, string url, Encoding encoding)
		{
			Descriptor.Id = "WXXX";
			Description = description;
			Url = url;
			TextEncoding = encoding;
		}

		/// <summary>
		/// The description
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// The URL.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "URL validation is not performed.")]
		public string Url { get; set; }

		/// <summary>
		/// The frame type.
		/// </summary>
		public override FrameType Type
		{
			get { return FrameType.UserDefinedUrlLink; }
		}

		/// <summary>
		/// Convert the values to a raw frame.
		/// </summary>
		/// <returns>the raw frame.</returns>
		public override RawFrame Convert(TagVersion version)
		{
			FrameOptions options = Descriptor.Options;

			byte[] payload;
			using (var writer = new FrameDataWriter())
			{
				writer.WriteEncodingByte(TextEncoding);
				writer.WritePreamble(TextEncoding);
				writer.WriteString(Description, TextEncoding, true);
				writer.WriteString(Url, Encoding.GetEncoding(28591));
				payload = writer.ToArray();
			}

			return RawFrame.CreateFrame(Descriptor.Id, options, payload, version);
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
				Url = reader.ReadVariableString(Encoding.GetEncoding(28591));
			}
		}

		/// <summary>
		/// Overwrite ToString
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return String.Format(
				CultureInfo.InvariantCulture,
				"User-Definded URL : Encoding = {0}, Description = {1}, URL : {2} ",
				TextEncoding,
				Description,
				Url);
		}
	}
}