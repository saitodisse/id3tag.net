using System.Text;

namespace Id3Tag.HighLevel
{
	/// <summary>
	/// Base class for all frames that contain encoded text
	/// </summary>
	public abstract class EncodedTextFrame : Frame
	{
		/// <summary>
		/// The text encoding
		/// </summary>
		public Encoding TextEncoding { get; set; }
	}
}