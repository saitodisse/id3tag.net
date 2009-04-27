using System;
using System.Collections.Generic;
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
    public class UserDefinedURLLinkFrame : Frame
    {
        /// <summary>
        /// Creates a new UserDefinedURLLinkFrame
        /// </summary>
        public UserDefinedURLLinkFrame()
        {
        }

        /// <summary>
        /// Creates a new UserDefinedURLLinkFrame
        /// </summary>
        /// <param name="description">the Description</param>
        /// <param name="url">The URL</param>
        /// <param name="type">The text encoding type.</param>
        public UserDefinedURLLinkFrame(string description, string url, TextEncodingType type)
        {
            Descriptor.ID = "WXXX";
            Description = description;
            URL = url;
            TextEncoding = type;
        }

        /// <summary>
        /// The text encoding
        /// </summary>
        public TextEncodingType TextEncoding { get; set; }

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
        public override RawFrame Convert()
        {
            var flagBytes = Descriptor.GetFlagBytes();
            var descrBytes = Converter.GetContentBytes(TextEncoding, Description);
            var urlBytes = Converter.GetContentBytes(TextEncodingType.ISO_8859_1, URL);
            var terminateCharLength = Converter.GetTerminationCharLength(TextEncoding);

            var payloadSize = 1 + descrBytes.Length + terminateCharLength + urlBytes.Length;
            var payload = new byte[payloadSize];

            payload[0] = System.Convert.ToByte(TextEncoding);
            Array.Copy(descrBytes, 0, payload, 1, descrBytes.Length);
            Array.Copy(urlBytes, 0, payload, 1 + terminateCharLength + descrBytes.Length, urlBytes.Length);

            var rawFrame = RawFrame.CreateFrame(Descriptor.ID, flagBytes, payload);
            return rawFrame;
        }

        /// <summary>
        /// Import the raw frame.
        /// </summary>
        /// <param name="rawFrame">the raw frame.</param>
        public override void Import(RawFrame rawFrame)
        {
            ImportRawFrameHeader(rawFrame);

            /*
             *  Text Encoding   xx
             *  Description     (xx xx .. xx) (00 / 00 00)
             *  URL             (xx xx ... xx)  als ISO8859-1 !
             */

            TextEncoding = (TextEncodingType) rawFrame.Payload[0];
            var dataBytes = new byte[rawFrame.Payload.Length - 1];
            Array.Copy(rawFrame.Payload, 1, dataBytes, 0, dataBytes.Length);

            int increment;
            switch (TextEncoding)
            {
                case TextEncodingType.ISO_8859_1:
                    increment = 1;
                    break;
                case TextEncodingType.UTF16:
                    increment = 2;
                    break;
                case TextEncodingType.UTF16_BE:
                    increment = 2;
                    break;
                case TextEncodingType.UTF8:
                    increment = 1;
                    break;
                default:
                    throw new ID3TagException("Unknown TextEncoding format (" + (byte) TextEncoding + ".)");
            }

            var descrBytes = new List<byte>();
            var urlBytes = new List<byte>();
            var urlPos = 0;

            //
            //  Determine the description bytes first
            //
            for (var i = 0; i < dataBytes.Length; i += increment)
            {
                var isTerminatedSymbol = DetermineTerminateSymbol(dataBytes, i, increment);
                if (isTerminatedSymbol)
                {
                    urlPos = i + increment;
                    break;
                }

                var values = new byte[increment];
                Array.Copy(dataBytes, i, values, 0, increment);
                descrBytes.AddRange(values);
            }

            //
            //  Get the URL bytes
            //
            for (var j = urlPos; j < dataBytes.Length; j++)
            {
                urlBytes.Add(dataBytes[j]);
            }

            var descrChars = Converter.Extract(TextEncoding, descrBytes.ToArray());
            Description = new string(descrChars);

            var urlChars = Converter.Extract(TextEncodingType.ISO_8859_1, urlBytes.ToArray(), false);
            URL = new string(urlChars);
        }

        private static bool DetermineTerminateSymbol(byte[] data, int pos, int increment)
        {
            var isNull = true;
            for (var i = 0; i < increment; i++)
            {
                if (data[pos + i] == 0x00)
                {
                    isNull &= true;
                }
                else
                {
                    isNull &= false;
                }
            }

            return isNull;
        }

        /// <summary>
        /// Overwrite ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var builder = new StringBuilder("UserDefindedURLLinkFrame : ");

            builder.AppendFormat("Encoding : {0} ", TextEncoding);
            builder.AppendFormat("Description : {0} ", Description);
            builder.AppendFormat("URL : {0}", URL);

            return builder.ToString();
        }
    }
}