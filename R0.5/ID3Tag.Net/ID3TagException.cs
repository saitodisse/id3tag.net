using System;

namespace ID3Tag
{
    /// <summary>
    /// Base exception for all ID3Tag specific exceptions.
    /// </summary>
    public class ID3TagException : ApplicationException
    {
        /// <summary>
        /// Creates a new instance of ID3TagException.
        /// </summary>
        /// <param name="message">the message.</param>
        public ID3TagException(string message) : base(message)
        {
        }

        /// <summary>
        /// Creates a new instance of ID3TagException.
        /// </summary>
        /// <param name="message">the message.</param>
        /// <param name="innerEx">the inner exception.</param>
        public ID3TagException(string message, Exception innerEx) : base(message, innerEx)
        {
        }
    }
}