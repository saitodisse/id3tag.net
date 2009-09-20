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
		/// <param name="encoding">the text encoding.</param>
		public UserDefinedTextFrame(string description, string value, Encoding encoding)
		{
			Descriptor.ID = "TXXX";
			Description = description;
			Value = value;
			TextEncoding = encoding;
		}

		/// <summary>
		/// The text encoding.
		/// </summary>
		public Encoding TextEncoding { get; set; }

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
			FrameFlags flag = Descriptor.GetFlags();

			byte[] payload;
			using (var writer = new FrameDataWriter())
			{
				writer.WriteEncodingByte(TextEncoding);
				writer.WritePreamble(TextEncoding);
				writer.WriteString(Description, TextEncoding, true);
				writer.WriteString(Value, TextEncoding);
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
				"User-Defined Text : Encoding = {0}, Description = {1}, Value = {2}",
				TextEncoding.EncodingName,
				Description,
				Value);
		}
	}
}