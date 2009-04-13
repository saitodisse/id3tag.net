using ID3Tag.LowLevel;

namespace ID3Tag.HighLevel
{
    public interface ITagController
    {
        TagContainer Decode(Id3TagInfo info);
        Id3TagInfo Encode(TagContainer container);
    }
}