using System;

namespace ID3Tag
{
    public class InvalidTagFormatException : ID3TagException
    {
        public InvalidTagFormatException(string message)
            : base(message)
        {
        }

        public InvalidTagFormatException(string message, Exception innerEx)
            : base(message, innerEx)
        {
        }
    }
}