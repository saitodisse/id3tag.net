using System;

namespace ID3Tag
{
    public class ID3IOException : ID3TagException
    {
        public ID3IOException(string message) : base(message)
        {
        }

        public ID3IOException(string message, Exception innerEx)
            : base(message, innerEx)
        {
        }
    }
}