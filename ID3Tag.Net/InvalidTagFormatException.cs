using System;

namespace ID3Tag
{
    /// <summary>
    /// This exception is thrown if the tag format is invalid.
    /// </summary>
    public class InvalidTagFormatException : ID3TagException
    {
        /// <summary>
        /// Creates a new instance of InvalidTagFormatException.
        /// </summary>
        /// <param name="message">the message.</param>
        public InvalidTagFormatException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates a new instance of InvalidTagFormatException.
        /// </summary>
        /// <param name="message">the message.</param>
        /// <param name="innerEx">the previous exception.</param>
        public InvalidTagFormatException(string message, Exception innerEx)
            : base(message, innerEx)
        {
        }
    }
}