using Id3Tag.LowLevel;

namespace Id3Tag.HighLevel
{
    /// <summary>
    /// Represents an high level frame.
    /// </summary>
    public interface IFrame
    {
        /// <summary>
        /// The frame type.
        /// </summary>
        FrameType Type { get; }

        /// <summary>
        /// The descriptor.
        /// </summary>
        FrameDescriptor Descriptor { get; }

        /// <summary>
        /// Convert a high level frame into a raw frame.
        /// </summary>
        /// <returns>the raw frame.</returns>
        RawFrame Convert(TagVersion version);

		/// <summary>
		/// Imports a raw frame values into a high level frame.
		/// </summary>
		/// <param name="rawFrame">the raw frame.</param>
		/// <param name="codePage">Default code page for Ansi encoding. Pass 0 to use default system encoding code page.</param>
        void Import(RawFrame rawFrame, int codePage);
    }
}