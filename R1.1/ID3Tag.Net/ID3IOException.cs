using System;
using System.Runtime.Serialization;

namespace Id3Tag
{
	/// <summary>
	/// This exception is thrown if an internal I/O error occured.
	/// </summary>
	[Serializable]
	public class Id3IOException : Id3TagException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Id3IOException"/> class.
		/// </summary>
		public Id3IOException()
		{}

		/// <summary>
		/// Creates a new instance of Id3IOException.
		/// </summary>
		/// <param name="message">the message.</param>
		public Id3IOException(string message) : base(message)
		{}

		/// <summary>
		/// Creates a new instance of Id3IOException.
		/// </summary>
		/// <param name="message">the message.</param>
		/// <param name="innerEx">the previous exception.</param>
		public Id3IOException(string message, Exception innerEx)
			: base(message, innerEx)
		{}

		/// <summary>
		/// Initializes a new instance of the <see cref="Id3TagException"/> class.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		protected Id3IOException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{}
	}
}