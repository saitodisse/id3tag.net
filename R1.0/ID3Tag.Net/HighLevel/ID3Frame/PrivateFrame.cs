using System;
using System.Text;
using ID3Tag.LowLevel;

namespace ID3Tag.HighLevel.ID3Frame
{
	/// <summary>
	/// This frame is used to contain information from a software producer that its program uses 
	/// and does not fit into the other frames. The frame consists of an 'Owner identifier' string 
	/// and the binary data. The 'Owner identifier' is a null-terminated string with a URL 
	/// containing an email address, or a link to a location where an email address can be found, 
	/// that belongs to the organisation responsible for the frame. Questions regarding the 
	/// frame should be sent to the indicated email address. The tag may contain more than 
	/// one "PRIV" frame but only with different contents. It is recommended to keep the 
	/// number of "PRIV" frames as low as possible.
	/// </summary>
	public class PrivateFrame : Frame
	{
		/// <summary>
		/// Creates a new instance of PrivateFrame.
		/// </summary>
		public PrivateFrame() : this(String.Empty, new byte[0])
		{}

		/// <summary>
		/// Creates a new instance of PrivateFrame.
		/// </summary>
		/// <param name="owner">the owner.</param>
		/// <param name="data">the data.</param>
		public PrivateFrame(string owner, byte[] data)
		{
			Descriptor.ID = "PRIV";
			Owner = owner;
			Data = data;
		}

		/// <summary>
		/// The owner.
		/// </summary>
		public string Owner { get; set; }

		/// <summary>
		/// The data.
		/// </summary>
		public byte[] Data { get; set; }

		/// <summary>
		/// The frame type.
		/// </summary>
		public override FrameType Type
		{
			get { return FrameType.Private; }
		}

		/// <summary>
		/// Convert the values to a raw frame.
		/// </summary>
		/// <returns>the raw frame.</returns>
		public override RawFrame Convert(TagVersion version)
		{
			FrameFlags flag = Descriptor.GetFlags();

			byte[] payload;
			using (var writer = new FrameDataWriter())
			{
				writer.WriteString(Owner, Encoding.GetEncoding(28591), true);
				writer.WriteBytes(Data);
				payload = writer.ToArray();
			}

			return RawFrame.CreateFrame(Descriptor.ID, flag, payload, version);
		}

		/// <summary>
		/// Import the raw frame.
		/// </summary>
		/// <param name="rawFrame">the raw frame.</param>
		/// <param name="codePage">Default code page for Ansi encoding. Pass 0 to use default system encoding code page.</param>
		public override void Import(RawFrame rawFrame, int codePage)
		{
			ImportRawFrameHeader(rawFrame);

			//
			//  <text> 00 <data>
			//

			using (var reader = new FrameDataReader(rawFrame.Payload))
			{
				Owner = reader.ReadVariableString(Encoding.GetEncoding(28591));
				Data = reader.ReadBytes();
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
			return String.Format("Private : Owner = {0}, Data = {1}", Owner, Utils.BytesToString(Data));
		}
	}
}