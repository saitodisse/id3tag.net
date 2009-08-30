using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ID3Tag.HighLevel
{
	/// <summary>
	/// Base class for all frames that contain encoded text
	/// </summary>
	public abstract class EncodedTextFrame : Frame
	{
		/// <summary>
		/// The text encoding
		/// </summary>
		public TextEncodingType TextEncoding { get; set; }

		/// <summary>
		/// Gets or sets the default code page for <see cref="TextEncodingType.Ansi" />.
		/// </summary>
		/// <value>The code page.</value>
		public int CodePage { get; set; }
	}
}
