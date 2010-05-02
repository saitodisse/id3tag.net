using System;
using System.Text;
using ID3Tag.LowLevel;

namespace ID3Tag.HighLevel.ID3Frame
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
        public UnsynchronisedLyricFrame() 
            : this("ENG","","",Encoding.ASCII)
        {
        }

        public UnsynchronisedLyricFrame(string language, string descriptor, string lyrics, Encoding encoding)
        {
            Descriptor.ID = "USLT";
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

        public override RawFrame Convert(TagVersion version)
        {
            var flags = Descriptor.GetFlags();
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

            return RawFrame.CreateFrame(Descriptor.ID, flags, payload, version);
        }

        public override void Import(RawFrame rawFrame, int codePage)
        {
            ImportRawFrameHeader(rawFrame);

            using (var reader = new FrameDataReader(rawFrame.Payload))
            {
                var encodingByte = reader.ReadByte();
                Language = reader.ReadFixedString(Encoding.ASCII, 3);
                TextEncoding = reader.ReadEncoding(encodingByte, codePage);
                ContentDescriptor = reader.ReadVariableString(TextEncoding);
                Lyrics = reader.ReadVariableString(TextEncoding);
            }
        }

        public override FrameType Type
        {
            get { return FrameType.UnsynchronisedLyric; }
        }

        public override string ToString()
        {
            return String.Format("Unsynchronised Lyrics [USLT] : Language = {0} Descriptor = {1} ", Language, ContentDescriptor);
        }
    }
}