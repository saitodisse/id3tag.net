using System;
using System.Text;
using ID3Tag.LowLevel;

namespace ID3Tag.HighLevel.ID3Frame
{
    /// <summary>
    /// Represents a Popularitmeter frame
    /// </summary>
    public class PopularimeterFrame : Frame
    {
        public PopularimeterFrame()
        {
        }

        /// <summary>
        /// Creates a new Popularimeter frame.
        /// </summary>
        /// <param name="mail">The mail adress</param>
        /// <param name="rating">The rating</param>
        /// <param name="counter">The playcounter</param>
        public PopularimeterFrame(string mail, byte rating, int counter)
        {
            eMail = mail;
            Rating = rating;
            PlayCounter = counter;
        }

        /// <summary>
        /// The payload.
        /// The Rating.
        /// Byte-Value with
        /// 0 = Unrated
        /// 1 = Worst Rating
        /// ...
        /// 255 = Best Rating
        /// </summary>
        public byte Rating { get; set; }

        /// <summary>
        /// Identifies the source of the rating.
        /// </summary>
        public string eMail { get; set; }

        /// <summary>
        /// Specifies how often the file has been played by the source  of the rating.
        /// </summary>
        public Int32 PlayCounter { get; set; }

        /// <summary>
        /// The frame type.
        /// </summary>
        public override FrameType Type
        {
            get { return FrameType.Popularimeter; }
        }

        /// <summary>
        /// Convert the Popularimeterframe.
        /// </summary>
        /// <returns>a RawFrame.</returns>
        public override RawFrame Convert()
        {
            var flagBytes = Descriptor.GetFlagBytes();
            var sb = new StringBuilder();
            sb.Append(eMail);
            sb.Append('\0');
            var mailbytes = Converter.GetContentBytes(TextEncodingType.ISO_8859_1, sb.ToString());
            var playcounterbytes = BitConverter.GetBytes(PlayCounter);
            var counterlength = 4;
            while (playcounterbytes[counterlength - 4] == 0)
            {
                counterlength--;
            }
            var payload = new byte[mailbytes.Length + 1 + counterlength];
            Array.Copy(mailbytes, payload, mailbytes.Length);
            payload[mailbytes.Length] = Rating;
            Array.Copy(playcounterbytes, counterlength - 4, payload, mailbytes.Length + 1, counterlength);
            var frame = RawFrame.CreateFrame(Descriptor.ID, flagBytes, payload);
            return frame;
        }
        ///<summary>
        /// 
        /// </summary>
        /// <param name="rawFrame">the raw frame.</param>
        public override void Import(RawFrame rawFrame)
        {
            /*
             * ID = "POPM"
            * Email to user   <text string> $00
             * Rating          $xx
             * Counter         $xx xx xx xx (xx ...)
             */
            ImportRawFrameHeader(rawFrame);

            var payload = rawFrame.Payload;
            if (payload.Length > 0)
            {
                var chars = Converter.Extract(TextEncodingType.ISO_8859_1, payload, true);
                var pointer = chars.Length;
                eMail = new string(chars);

                if (payload.Length > pointer)
                {
                    Rating = payload[++pointer]; //email-characters + 00 Termination (+ 1 because Length is 1-based)
                    if (payload.Length > ++pointer) //Playcounter present?
                    {
                        var bytesleft = payload.Length - pointer;
                        pointer += (bytesleft > 4) ? bytesleft - 4 : 0;
                            // only read the last 4 bytes (come on who listens to a song mor than Int32.Max times??
                        var readlength = (bytesleft < 4) ? bytesleft : 4;
                        var counterbytes = new byte[4] {0, 0, 0, 0};
                        Array.Copy(payload, pointer, counterbytes, 0, readlength);
                        PlayCounter = BitConverter.ToInt32(counterbytes, 0);
                    }
                }
            }
        }

        /// <summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder("PopularimeterFrame : ");

            sb.AppendFormat("ID : {0} ", Descriptor.ID);
            sb.AppendFormat("Email : {0}, ", eMail);
            sb.AppendFormat("Rating : {0}, ", Rating);
            sb.AppendFormat("Playcounter : {0}", PlayCounter);

            return sb.ToString();
        }
    }
}