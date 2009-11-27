using System;
using System.Runtime.Serialization;

namespace Id3Tag
{
	/// <summary>
	/// Base exception for all Id3Tag specific exceptions.
	/// </summary>
	[Serializable]
	public class Id3TagException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Id3TagException"/> class.
		/// </summary>
		public Id3TagException()
		{}

		/// <summary>
		/// Creates a new instance of Id3TagException.
		/// </summary>
		/// <param name="message">the message.</param>
		public Id3TagException(string message) : base(message)
		{}

		/// <summary>
		/// Creates a new instance of Id3TagException.
		/// </summary>
		/// <param name="message">the message.</param>
		/// <param name="innerEx">the inner exception.</param>
		public Id3TagException(string message, Exception innerEx) : base(message, innerEx)
		{}

		/// <summary>
		/// Initializes a new instance of the <see cref="Id3TagException"/> class.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		protected Id3TagException(SerializationInfo info, StreamingContext context) : base(info, context)
		{}
	}
}