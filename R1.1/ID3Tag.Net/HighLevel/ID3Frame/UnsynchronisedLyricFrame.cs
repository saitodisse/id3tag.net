using System;
using System.Globalization;
using System.Text;
using Id3Tag.LowLevel;

namespace Id3Tag.HighLevel.Id3Frame
{
	/// <summary>
	/// This frame contains the lyrics of the song or a text transcription of
	///other vocal activities. The head includes an encoding descriptor and
	///a content descriptor. The body consists of the actual text. The
	///'Content descriptor' is a terminated string. If no descriptor is
	///entered, 'Content descriptor' is $00 (00) only. Newline characters
	///are allowed in the text. There may be more than one 'Unsynchronised
	/// lyrics/text transcription' frame in each tag, but only one with the
	///same language and content descriptor.
	/// </summary>
	public class UnsynchronisedLyricFrame : EncodedTextFrame
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UnsynchronisedLyricFrame"/> class.
		/// </summary>
		public UnsynchronisedLyricFrame()
			: this("ENG", "", "", Encoding.ASCII)
		{}

		/// <summary>
		/// Initializes a new instance of the <see cref="UnsynchronisedLyricFrame"/> class.
		/// </summary>
		/// <param name="language">The language.</param>
		/// <param name="descriptor">The descriptor.</param>
		/// <param name="lyrics">The lyrics.</param>
		/// <param name="encoding">The encoding.</param>
		public UnsynchronisedLyricFrame(string language, string descriptor, string lyrics, Encoding encoding)
		{
			Descriptor.Id = "USLT";
			Language = language;
			ContentDescriptor = descriptor;
			Lyrics = lyrics;
			TextEncoding = encoding;
		}

		//   <Header for 'Unsynchronised lyrics/text transcription', ID: "USLT">
		//    Text encoding        $xx
		//    Language             $xx xx xx
		//    Content descriptor   <text string according to encoding> $00 (00)
		//    Lyrics/text          <full text string according to encoding>

		/// <summary>
		/// The languange
		/// </summary>
		public string Language { get; set; }

		/// <summary>
		/// The content descriptor
		/// </summary>
		public string ContentDescriptor { get; set; }

		/// <summary>
		/// The lyrics
		/// </summary>
		public string Lyrics { get; set; }

		/// <summary>
		/// Gets the frame type.
		/// </summary>
		/// <value></value>
		public override FrameType Type
		{
			get { return FrameType.UnsynchronisedLyric; }
		}

		/// <summary>
		/// Convert this frame to <see cref="RawFrame"/>.
		/// </summary>
		/// <param name="version"></param>
		/// <returns>the raw frame.</returns>
		public override RawFrame Convert(TagVersion version)
		{
			FrameOptions options = Descriptor.Options;
			byte[] payload;

			using (var writer = new FrameDataWriter())
			{
				writer.WriteEncodingByte(TextEncoding);
				writer.WriteString(Language, Encoding.ASCII, 3);
				writer.WritePreamble(TextEncoding);
				writer.WriteString(ContentDescriptor, TextEncoding, true);
				writer.WriteString(Lyrics, TextEncoding);

				payload = writer.ToArray();
			}

			return RawFrame.CreateFrame(Descriptor.Id, options, payload, version);
		}

		/// <summary>
		/// Imports values from the <see cref="RawFrame"/>.
		/// </summary>
		/// <param name="rawFrame">the raw frame.</param>
		/// <param name="codePage">Default code page for Ansi encoding. Pass 0 to use default system encoding code page.</param>
		public override void Import(RawFrame rawFrame, int codePage)
		{
			ImportRawFrameHeader(rawFrame);

			using (var reader = new FrameDataReader(rawFrame.Payload))
			{
				byte encodingByte = reader.ReadByte();
				Language = reader.ReadFixedString(Encoding.ASCII, 3);
				TextEncoding = reader.ReadEncoding(encodingByte, codePage);
				ContentDescriptor = reader.ReadVariableString(TextEncoding);
				Lyrics = reader.ReadVariableString(TextEncoding);
			}
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public override string ToString()
		{
			return String.Format(
				CultureInfo.InvariantCulture,
				"Unsynchronised Lyrics [USLT] : Language = {0} Descriptor = {1} ",
				Language,
				ContentDescriptor);
		}
	}
}