using System.IO;
using ID3Tag.HighLevel;

namespace ID3Tag.LowLevel
{
    public interface IIoController
    {
        Id3TagInfo Read(FileInfo file);
        Id3TagInfo Read(Stream inputStream);
        void Write(TagContainer tagContainer, Stream input, Stream output);
    }
}