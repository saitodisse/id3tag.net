using System;
using System.Runtime.Serialization;

namespace ID3Tag
{
	/// <summary>
	/// This exception is thrown if no valid ID3 header is found.
	/// </summary>
	[Serializable]
	public class ID3HeaderNotFoundException : ID3TagException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ID3HeaderNotFoundException"/> class.
		/// </summary>
		public ID3HeaderNotFoundException()
			: base("ID3v2 Header not found")
		{}

		/// <summary>
		/// Initializes a new instance of the <see cref="ID3HeaderNotFoundException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="ex">The inner exception.</param>
		public ID3HeaderNotFoundException(string message, Exception ex)
			: base(message, ex)
		{}

		/// <summary>
		/// Initializes a new instance of the <see cref="ID3HeaderNotFoundException"/> class.
		/// </summary>
		/// <param name="message">the message.</param>
		public ID3HeaderNotFoundException(string message)
			: base(message)
		{}

		/// <summary>
		/// Initializes a new instance of the <see cref="ID3HeaderNotFoundException"/> class.
		/// </summary>
		/// <param name="ex">The inner exception.</param>
		public ID3HeaderNotFoundException(Exception ex)
			: base("ID3v2 Header not found", ex)
		{}

		/// <summary>
		/// Initializes a new instance of the <see cref="ID3HeaderNotFoundException"/> class.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		protected ID3HeaderNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{}
	}
}