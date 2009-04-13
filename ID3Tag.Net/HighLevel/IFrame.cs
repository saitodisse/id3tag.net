using ID3Tag.LowLevel;

namespace ID3Tag.HighLevel
{
    public interface IFrame
    {
        FrameType Type { get; }
        FrameDescriptor Descriptor { get; }
        RawFrame Convert();
        void Import(RawFrame rawFrame);
    }
}