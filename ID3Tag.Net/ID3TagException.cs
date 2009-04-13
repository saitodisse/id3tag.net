using System;

namespace ID3Tag
{
    public class ID3TagException : ApplicationException
    {
        public ID3TagException(string message) : base(message)
        {
        }

        public ID3TagException(string message, Exception innerEx) : base(message, innerEx)
        {
        }
    }
}