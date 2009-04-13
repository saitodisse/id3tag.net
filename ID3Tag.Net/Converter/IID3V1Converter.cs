using System.IO;
using ID3Tag.HighLevel;

namespace ID3Tag.Converter
{
    public interface IId3V1Converter
    {
        TagContainer Convert(Stream inputStream);
        TagContainer Convert(FileInfo file);
    }
}