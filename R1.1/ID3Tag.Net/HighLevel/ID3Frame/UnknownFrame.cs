using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Id3Tag.LowLevel;

namespace Id3Tag.HighLevel.Id3Frame
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
			Content = new ReadOnlyCollection<byte>(new byte[0]);
		}

		/// <summary>
		/// The payload.
		/// </summary>
		public ReadOnlyCollection<byte> Content { get; private set; }

		/// <summary>
		/// Sets the content from binary data.
		/// </summary>
		/// <param name="value">The value.</param>
		public void SetContent(IList<byte> value)
		{
			Content = new ReadOnlyCollection<byte>(value);
		}

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
			FrameOptions options = Descriptor.Options;
			return RawFrame.CreateFrame(Descriptor.Id, options, Content, version);
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
			return String.Format(
				CultureInfo.InvariantCulture,
				"Unknown : ID = {0}, Content = {1}",
				Descriptor.Id,
				Utils.BytesToString(Content));
		}
	}
}