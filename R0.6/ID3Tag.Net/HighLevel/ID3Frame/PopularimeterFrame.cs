using System;
using System.Text;
using ID3Tag.LowLevel;

namespace ID3Tag.HighLevel.ID3Frame
{
	/// <summary>
	/// The purpose of this frame is to specify how good an audio file is. 
	/// Many interesting applications could be found to this frame such as a playlist that features 
	/// better audiofiles more often than others or it could be used to profile a person's taste and 
	/// find other 'good' files by comparing people's profiles. The frame is very simple. 
	/// It contains the email address to the user, one rating byte and a four byte play counter, intended to 
	/// be increased with one for every time the file is played. The email is a terminated string. 
	/// The rating is 1-255 where 1 is worst and 255 is best. 0 is unknown. 
	/// If no personal counter is wanted it may be omitted. When the counter reaches all one's, one byte 
	/// is inserted in front of the counter thus making the counter eight bits 
	/// bigger in the same away as the play counter ("PCNT"). 
	/// There may be more than one "POPM" frame in each tag, but only one with the same email address.
	/// </summary>
	public class PopularimeterFrame : Frame
	{
		/// <summary>
		/// Creates a new PopularityMeterFrame.
		/// </summary>
		public PopularimeterFrame() : this(String.Empty, 0, 0)
		{}

		/// <summary>
		/// Creates a new Popularimeter frame.
		/// </summary>
		/// <param name="mail">The mail adress</param>
		/// <param name="rating">The rating</param>
		/// <param name="counter">The playcounter</param>
		public PopularimeterFrame(string mail, byte rating, ulong counter)
		{
			Descriptor.ID = "POPM";
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
		public UInt64 PlayCounter { get; set; }

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
		public override RawFrame Convert(TagVersion version)
		{
			FrameFlags flag = Descriptor.GetFlags();

			byte[] payload;
			using (var writer = new FrameDataWriter())
			{
				writer.WriteString(eMail, Encoding.ASCII, true);
				writer.WriteByte(Rating);
				writer.WriteUInt64(PlayCounter);
				payload = writer.ToArray();
			}

			RawFrame frame = RawFrame.CreateFrame(Descriptor.ID, flag, payload, version);
			return frame;
		}

		/// <summary>
		/// Import the raw content to a high level frame.
		/// </summary>
		/// <param name="rawFrame">the raw frame.</param>
		/// <param name="codePage">Default code page for Ansi encoding. Pass 0 to use default system encoding code page.</param>
		public override void Import(RawFrame rawFrame, int codePage)
		{
			/*
             * ID = "POPM"
             * Email to user   <text string> $00
             * Rating          $xx
             * Counter         $xx xx xx xx (xx ...)
             */
			ImportRawFrameHeader(rawFrame);

			using (var reader = new FrameDataReader(rawFrame.Payload))
			{
				eMail = reader.ReadVariableString(Encoding.ASCII);
				Rating = reader.ReadByte();
				PlayCounter = reader.ReadUInt64();
			}
		}

		/// <summary>
		/// Returns a string with details.
		/// </summary>
		/// <returns>the string.</returns>
		public override string ToString()
		{
			return String.Format("Popularimeter : ID = {0}, Email = {1},  Rating = {2}, Playcounter = {3}",
				Descriptor.ID,
				eMail,
				Rating,
				PlayCounter);
		}
	}
}