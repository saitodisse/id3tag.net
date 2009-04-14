using System;

namespace ID3Tag
{
    /// <summary>
    /// This exception is thrown if an internal I/O error occured.
    /// </summary>
    public class ID3IOException : ID3TagException
    {
        /// <summary>
        /// Creates a new instance of ID3IOException.
        /// </summary>
        /// <param name="message">the message.</param>
        public ID3IOException(string message) : base(message)
        {
        }

        /// <summary>
        /// Creates a new instance of ID3IOException.
        /// </summary>
        /// <param name="message">the message.</param>
        /// <param name="innerEx">the previous exception.</param>
        public ID3IOException(string message, Exception innerEx)
            : base(message, innerEx)
        {
        }
    }
}