using System;

namespace ID3Tag
{
    /// <summary>
    /// This exception is thrown if no valid ID3 header is found.
    /// </summary>
    public class ID3HeaderNotFoundException : ID3TagException
    {
        /// <summary>
        /// Creates a new instance of ID3HeaderNotFoundException.
        /// </summary>
        public ID3HeaderNotFoundException()
            : base("ID3v2 Header not found")
        {
        }

        /// <summary>
        /// Creates a new instance of ID3HeaderNotFoundException.
        /// </summary>
        /// <param name="message">the message.</param>
        /// <param name="ex">the previous exception.</param>
        public ID3HeaderNotFoundException(string message, Exception ex)
            : base(message, ex)
        {
        }

        /// <summary>
        /// Creates a new instance of ID3HeaderNotFoundException.
        /// </summary>
        /// <param name="message">the message.</param>
        public ID3HeaderNotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates a new instance of ID3HeaderNotFoundException.
        /// </summary>
        /// <param name="ex">the previous exception.</param>
        public ID3HeaderNotFoundException(Exception ex)
            : base("ID3v2 Header not found", ex)
        {
        }
    }
}