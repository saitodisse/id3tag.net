using System;
using System.Runtime.Serialization;

namespace ID3Tag
{
	/// <summary>
	/// Base exception for all ID3Tag specific exceptions.
	/// </summary>
	[Serializable]
	public class ID3TagException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ID3TagException"/> class.
		/// </summary>
		public ID3TagException()
		{}

		/// <summary>
		/// Creates a new instance of ID3TagException.
		/// </summary>
		/// <param name="message">the message.</param>
		public ID3TagException(string message) : base(message)
		{}

		/// <summary>
		/// Creates a new instance of ID3TagException.
		/// </summary>
		/// <param name="message">the message.</param>
		/// <param name="innerEx">the inner exception.</param>
		public ID3TagException(string message, Exception innerEx) : base(message, innerEx)
		{}

		/// <summary>
		/// Initializes a new instance of the <see cref="ID3TagException"/> class.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		protected ID3TagException(SerializationInfo info, StreamingContext context) : base(info, context)
		{}
	}
}