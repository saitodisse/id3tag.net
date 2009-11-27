using System;
using System.Globalization;
using System.Text;
using Id3Tag.LowLevel;

namespace Id3Tag.HighLevel.Id3Frame
{
	/// <summary>
	/// The text information frames are the most important frames, containing information like artist, 
	/// album and more. There may only be one text information frame of its kind in an tag. 
	/// If the textstring is followed by a termination ($00 (00)) all the following information 
	/// should be ignored and not be displayed. All text frame identifiers begin with "T". 
	/// Only text frame identifiers begin with "T", with the exception of the "TXXX" frame. 
	/// </summary>
	public class TextFrame : EncodedTextFrame
	{
	    /// <summary>
	    /// Creates a new TextFrame.
	    /// </summary>
	    public TextFrame()
	        : this("T???", String.Empty, Encoding.ASCII)
		{}

		/// <summary>
		/// Creates a new TextFrame.
		/// </summary>
		/// <param name="id">the frame id.</param>
		/// <param name="content">the content.</param>
		/// <param name="encoding">the text encoding.</param>
		public TextFrame(string id, string content, Encoding encoding)
		{
			Descriptor.Id = id;
			Content = content;
			TextEncoding = encoding;
		}

		/// <summary>
		/// The content.
		/// </summary>
		public string Content { get; set; }

		/// <summary>
		/// The frame type.
		/// </summary>
		public override FrameType Type
		{
			get { return FrameType.Text; }
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
				writer.WriteString(Content, TextEncoding);
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

			//
			//  XX            - Encoding Byte
			//  Y1 Y2 ... Yn  - Text
			//

			using (var reader = new FrameDataReader(rawFrame.Payload))
			{
				byte encodingByte = reader.ReadByte();
				TextEncoding = reader.ReadEncoding(encodingByte, codePage);
				Content = reader.ReadVariableString(TextEncoding).TrimEnd(Char.MinValue);
			}
		}

		/// <summary>
		/// Overwrites ToString.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return String.Format(
				CultureInfo.InvariantCulture, 
				"Text [{0}] : Encoding = {1} , Text = {2}",
				Descriptor.Id,
				TextEncoding.EncodingName,
				Content);
		}
	}
}