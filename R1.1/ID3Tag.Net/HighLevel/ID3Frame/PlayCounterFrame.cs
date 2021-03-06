﻿using System;
using System.Globalization;
using Id3Tag.LowLevel;

namespace Id3Tag.HighLevel.Id3Frame
{
    /// <summary>
    /// This is simply a counter of the number of times a file has been played. The value is 
    /// increased by one every time the file begins to play. There may only be one "PCNT" frame in each tag. 
    /// When the counter reaches all one's, one byte is inserted in front of the 
    /// counter thus making the counter eight bits bigger. 
    /// The counter must be at least 32-bits long to begin with.
    /// </summary>
    public class PlayCounterFrame : Frame
    {
        /// <summary>
        /// Creates a new instance of PlayCounterFrame
        /// </summary>
        public PlayCounterFrame()
            : this(0)
        {
        }

        /// <summary>
        /// Creates a new instance of PlayCounterFrame
        /// </summary>
        /// <param name="playCounter">the play counter</param>
        public PlayCounterFrame(ulong playCounter)
        {
            Descriptor.Id = "PCNT";
            Counter = playCounter;
        }

        /// <summary>
        /// The play counter of the audio file.
        /// </summary>
        public ulong Counter { get; set; }

        /// <summary>
        /// Defines the frame type.
        /// </summary>
        public override FrameType Type
        {
            get { return FrameType.PlayCounter; }
        }

        /// <summary>
        /// Converts a play counter frame to raw frame.
        /// </summary>
        /// <returns>the raw frame.</returns>
        public override RawFrame Convert(TagVersion version)
        {
            FrameOptions flag = Descriptor.Options;

            byte[] payload;

            using (var writer = new FrameDataWriter(8))
            {
                writer.WriteUInt64(Counter);
                payload = writer.ToArray();
            }

            RawFrame rawFrame = RawFrame.CreateFrame(Descriptor.Id, flag, payload, version);
            return rawFrame;
        }

        /// <summary>
        /// Converts a raw frame to PlayCounter frame.
        /// </summary>
        /// <param name="rawFrame">the raw frame.</param>
        /// <param name="codePage">Default code page for Ansi encoding. Pass 0 to use default system encoding code page.</param>
        /// s
        public override void Import(RawFrame rawFrame, int codePage)
        {
            /*
				ID = "PCNT"
				Counter         $xx xx xx xx (xx ...)
			*/

            ImportRawFrameHeader(rawFrame);

            using (var reader = new FrameDataReader(rawFrame.Payload))
            {
                Counter = reader.ReadUInt64();
            }
        }

        /// <summary>
        /// Overwrites ToString
        /// </summary>
        /// <returns>the ToString representation</returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "Play Counter : Value = {0}", Counter);
        }
    }
}