using System;
using ID3Tag.LowLevel;

namespace ID3Tag.HighLevel.ID3Frame
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
        public PlayCounterFrame(long playCounter)
        {
            Descriptor.ID = "PCNT";
            Counter = playCounter;
        }

        /// <summary>
        /// The play counter of the audio file.
        /// </summary>
        public long Counter { get; set; }

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
            var flag = Descriptor.GetFlags();

            // Read and convert to MSB!
            var counterBytes = BitConverter.GetBytes(Counter);
            Array.Reverse(counterBytes);

            var counterLength = GetRealCounterLength(counterBytes);
            if (counterLength <= 4)
            {
                // There are at least 4 bytes!
                counterLength = 4;
            }

            var payload = new byte[counterLength];
            Array.Copy(counterBytes, counterBytes.Length - counterLength, payload, 0, counterLength);

            var rawFrame = RawFrame.CreateFrame(Descriptor.ID, flag, payload, version);
            return rawFrame;
        }

        /// <summary>
        /// Converts a raw frame to PlayCounter frame.
        /// </summary>s
        /// <param name="rawFrame">the raw frame.</param>
        public override void Import(RawFrame rawFrame)
        {
            ImportRawFrameHeader(rawFrame);

            var payload = rawFrame.Payload;
            if (payload.Length == 0)
            {
                Counter = 0;
            }
            else
            {
                /*
                 * This lib supports 8 bytes of the play counter values. The frames supports up to 8 bytes with a least 4 bytes.
                 * We must add zero bytes for the byte conversion.
                 */

                var counterBytes = new byte[8];
                if (payload.Length < 8)
                {
                    Array.Copy(payload, 0, counterBytes, 8 - payload.Length, payload.Length);
                }
                else
                {
                    counterBytes = payload;
                }

                // Convert To LSB and decode!
                Array.Reverse(counterBytes);
                Counter = BitConverter.ToInt64(counterBytes, 0);
            }
        }

        /// <summary>
        /// Overwrites ToString
        /// </summary>
        /// <returns>the ToString representation</returns>
        public override string ToString()
        {
            return String.Format("PlayCounter : Counter = {0}", Counter);
        }

        private static int GetRealCounterLength(byte[] bytes)
        {
            // MSB                 LSB
            //
            // xx xx xx xx xx xx xx xx
            //          12 00 1A 00 45
            //

            var length = bytes.Length;
            var realLength = 0;
            for (var i = 0; i < length; i++)
            {
                if (bytes[i] != 0)
                {
                    realLength = length - i;
                    break;
                }
            }

            return realLength;
        }
    }
}