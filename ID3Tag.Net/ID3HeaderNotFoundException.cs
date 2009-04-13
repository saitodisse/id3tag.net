using System;

namespace ID3Tag
{
    public class ID3HeaderNotFoundException : ID3TagException
    {
        public ID3HeaderNotFoundException()
            : base("ID3v2 Header not found")
        {
        }

        public ID3HeaderNotFoundException(string message, Exception ex)
            : base(message, ex)
        {
        }

        public ID3HeaderNotFoundException(string message)
            : base(message)
        {
        }

        public ID3HeaderNotFoundException(Exception ex)
            : base("ID3v2 Header not found", ex)
        {
        }
    }
}