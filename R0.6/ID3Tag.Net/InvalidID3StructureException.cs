using System;

namespace ID3Tag
{
    /// <summary>
    /// This exception is thrown if the ID3 structure is invalid.
    /// </summary>
    public class InvalidID3StructureException : ID3TagException
    {
        /// <summary>
        /// Creates a new instance of ID3TagException.
        /// </summary>
        /// <param name="message">the message.</param>
        public InvalidID3StructureException(string message) : base(message)
        {
        }

        /// <summary>
        /// Creates a new instance of ID3TagException.
        /// </summary>
        /// <param name="message">the message.</param>
        /// <param name="innerEx">the inner exception.</param>
        public InvalidID3StructureException(string message, Exception innerEx)
            : base(message, innerEx)
        {
        }
    }
}