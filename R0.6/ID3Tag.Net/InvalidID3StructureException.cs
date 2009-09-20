using System;
using System.Runtime.Serialization;

namespace ID3Tag
{
	/// <summary>
	/// This exception is thrown if the ID3 structure is invalid.
	/// </summary>
	[Serializable]
	public class InvalidID3StructureException : ID3TagException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidID3StructureException"/> class.
		/// </summary>
		public InvalidID3StructureException()
		{}

		/// <summary>
		/// Creates a new instance of ID3TagException.
		/// </summary>
		/// <param name="message">the message.</param>
		public InvalidID3StructureException(string message) : base(message)
		{}

		/// <summary>
		/// Creates a new instance of ID3TagException.
		/// </summary>
		/// <param name="message">the message.</param>
		/// <param name="innerEx">the inner exception.</param>
		public InvalidID3StructureException(string message, Exception innerEx)
			: base(message, innerEx)
		{}

		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidID3StructureException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// The <paramref name="info"/> parameter is null.
		/// </exception>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">
		/// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
		/// </exception>
		protected InvalidID3StructureException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{}
	}
}