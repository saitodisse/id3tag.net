using System;
using System.Runtime.Serialization;

namespace Id3Tag
{
    /// <summary>
    /// This exception is thrown if the tag format is invalid.
    /// </summary>
    [Serializable]
    public class InvalidTagFormatException : Id3TagException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTagFormatException"/> class.
        /// </summary>
        public InvalidTagFormatException()
        {
        }

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

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTagFormatException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        protected InvalidTagFormatException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}