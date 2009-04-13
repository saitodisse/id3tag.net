using ID3Tag.Converter;
using ID3Tag.HighLevel;
using ID3Tag.LowLevel;

namespace ID3Tag.Factory
{
    public static class Id3TagFactory
    {
        public static IIoController CreateIoController()
        {
            return new IoController();
        }

        public static ITagController CreateTagController()
        {
            return new TagController();
        }

        public static IId3V1Converter CreateId3V1Converter()
        {
            return new Id3V1Converter();
        }
    }
}