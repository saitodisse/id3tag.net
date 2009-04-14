using ID3Tag.LowLevel;

namespace ID3Tag.HighLevel
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
        RawFrame Convert();
        /// <summary>
        /// Imports a raw frame values into a high level frame.
        /// </summary>
        /// <param name="rawFrame">the raw frame.</param>
        void Import(RawFrame rawFrame);
    }
}