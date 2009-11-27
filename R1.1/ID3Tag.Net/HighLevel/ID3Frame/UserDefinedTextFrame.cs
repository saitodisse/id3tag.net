using System;
using System.Globalization;
using System.Text;
using Id3Tag.LowLevel;

namespace Id3Tag.HighLevel.Id3Frame
{
	/// <summary>
	/// This frame is intended for one-string text information concerning the 
	/// audiofile in a similar way to the other "T"-frames. The frame body consists 
	/// of a description of the string, represented as a terminated string, 
	/// followed by the actual string. There may be more than one "TXXX" frame in each tag, 
	/// but only one with the same description.
	/// </summary>
	public class UserDefinedTextFrame : EncodedTextFrame
	{
		/// <summary>
		/// Creates a new UserDefindedTextFrame.
		/// </summary>
		public UserDefinedTextFrame()
            : this(String.Empty,String.Empty,Encoding.ASCII)
		{
		}

		/// <summary>
		/// Creates a new UserDefindedTextFrame.
		/// </summary>
		/// <param name="description">the description.</param>
		/// <param name="value">the value.</param>
		/// <param name="encoding">the text encoding.</param>
		public UserDefinedTextFrame(string description, string value, Encoding encoding)
		{
			Descriptor.Id = "TXXX";
			Description = description;
			Value = value;
			TextEncoding = encoding;
		}

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
			FrameOptions options = Descriptor.Options;

			byte[] payload;
			using (var writer = new FrameDataWriter())
			{
				writer.WriteEncodingByte(TextEncoding);
				writer.WritePreamble(TextEncoding);
				writer.WriteString(Description, TextEncoding, true);
				writer.WriteString(Value, TextEncoding);
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
				<Header for 'User defined text information frame', ID: "TXXX"> 
				Text encoding : $xx
				Description   : <text string according to encoding> $00 (00)
				Value         : <text string according to encoding>
			*/

			using (var reader = new FrameDataReader(rawFrame.Payload))
			{
				byte encodingByte = reader.ReadByte();
				TextEncoding = reader.ReadEncoding(encodingByte, codePage);
				Description = reader.ReadVariableString(TextEncoding);
				Value = reader.ReadVariableString(TextEncoding);
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
				"User-Defined Text : Encoding = {0}, Description = {1}, Value = {2}",
				TextEncoding.EncodingName,
				Description,
				Value);
		}
	}
}