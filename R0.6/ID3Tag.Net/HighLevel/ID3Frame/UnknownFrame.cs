using System;
using ID3Tag.LowLevel;

namespace ID3Tag.HighLevel.ID3Frame
{
	/// <summary>
	/// Represents a frame that cannot be identified by the reader.
	/// </summary>
	public class UnknownFrame : Frame
	{
		/// <summary>
		/// Creates a new Unknown frame.
		/// </summary>
		public UnknownFrame()
		{
			Content = new byte[0];
		}

		/// <summary>
		/// The payload.
		/// </summary>
		public byte[] Content { get; set; }

		/// <summary>
		/// The frame type.
		/// </summary>
		public override FrameType Type
		{
			get { return FrameType.Unknown; }
		}

		/// <summary>
		/// Convert the Unknownframe.
		/// </summary>
		/// <returns>a RawFrame.</returns>
		public override RawFrame Convert(TagVersion version)
		{
			FrameFlags flags = Descriptor.GetFlags();
			return RawFrame.CreateFrame(Descriptor.ID, flags, Content, version);
		}

		/// <summary>
		/// Import the the raw frame data.
		/// </summary>
		/// <param name="rawFrame">the raw frame.</param>
		/// <param name="codePage">Default code page for Ansi encoding. Pass 0 to use default system encoding code page.</param>
		public override void Import(RawFrame rawFrame, int codePage)
		{
			ImportRawFrameHeader(rawFrame);
			Content = rawFrame.Payload;
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public override string ToString()
		{
			return String.Format("Unknown : ID = {0}, Content = {1}", Descriptor.ID, Utils.BytesToString(Content));
		}
	}
}