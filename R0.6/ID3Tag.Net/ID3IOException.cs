using System;
using System.Runtime.Serialization;

namespace ID3Tag
{
	/// <summary>
	/// This exception is thrown if an internal I/O error occured.
	/// </summary>
	[Serializable]
	public class ID3IOException : ID3TagException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ID3IOException"/> class.
		/// </summary>
		public ID3IOException()
		{}

		/// <summary>
		/// Creates a new instance of ID3IOException.
		/// </summary>
		/// <param name="message">the message.</param>
		public ID3IOException(string message) : base(message)
		{}

		/// <summary>
		/// Creates a new instance of ID3IOException.
		/// </summary>
		/// <param name="message">the message.</param>
		/// <param name="innerEx">the previous exception.</param>
		public ID3IOException(string message, Exception innerEx)
			: base(message, innerEx)
		{}

		/// <summary>
		/// Initializes a new instance of the <see cref="ID3TagException"/> class.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		protected ID3IOException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{}
	}
}