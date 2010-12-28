using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using Id3Tag.LowLevel;

namespace Id3Tag.HighLevel.Id3Frame
{
    /// <summary>
    /// This frame is intended for URL links concerning the audiofile in a similar way to 
    /// the other "W"-frames. The frame body consists of a description of the string, 
    /// represented as a terminated string, followed by the actual URL. The URL is always 
    /// encoded with ISO-8859-1. There may be more than one "WXXX" frame in each tag, but 
    /// only one with the same description.
    /// </summary>
    public class UrlLinkFrame : Frame
    {
        /// <summary>
        /// Creates a new instance of UrlLinkFrame.
        /// </summary>
        public UrlLinkFrame()
            : this("W???", String.Empty)
        {
        }

        /// <summary>
        /// Creates a new instance of UrlLinkFrame.
        /// </summary>
        /// <param name="id">the frame ID.</param>
        /// <param name="url">the url.</param>
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#",
            Justification = "URL validation rules do not apply.")]
        public UrlLinkFrame(string id, string url)
        {
            Descriptor.Id = id;
            Url = url;
        }

        /// <summary>
        /// The URL.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings",
            Justification = "URL validation rules do not apply.")]
        public string Url { get; set; }

        /// <summary>
        /// The frame Type.
        /// </summary>
        public override FrameType Type
        {
            get { return FrameType.UrlLink; }
        }

        /// <summary>
        /// Convert the URLLinkFrame.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns>the RawFrame.</returns>
        public override RawFrame Convert(TagVersion version)
        {
            FrameOptions flag = Descriptor.Options;

            byte[] payload;
            using (var writer = new FrameDataWriter())
            {
                writer.WriteString(Url, Encoding.ASCII);
                payload = writer.ToArray();
            }

            return RawFrame.CreateFrame(Descriptor.Id, flag, payload, version);
        }

        /// <summary>
        /// Import the raw frame data.
        /// </summary>
        /// <param name="rawFrame">the raw frame.</param>
        /// <param name="codePage">Not used</param>
        public override void Import(RawFrame rawFrame, int codePage)
        {
            ImportRawFrameHeader(rawFrame);

            using (var reader = new FrameDataReader(rawFrame.Payload))
            {
                Url = reader.ReadVariableString(Encoding.ASCII);
            }
        }

        /// <summary>
        /// Overwrite ToString.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "URL : URL = {0}", Url);
        }
    }
}