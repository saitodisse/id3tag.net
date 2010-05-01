using System.IO;

namespace Id3Tag.HighLevel
{
    /// <summary>
    /// Converts the ID3v1 tags to ID3v2 tags.
    /// </summary>
    public interface IId3V1Controller
    {
		/// <summary>
		/// Reads the ID3v1 tag using specified code page for text encoding.
		/// </summary>
		/// <param name="inputStream">the input stream (i.e. the file.)</param>
		/// <param name="codePage">The code page for text encoding.</param>
		/// <returns>An ID3v1 container.</returns>
        Id3V1Tag Read(Stream inputStream, int codePage);

		/// <summary>
		/// Reads the ID3v1 tag using default code page for text encoding.
		/// </summary>
		/// <param name="file">the file.</param>
		/// <returns>An ID3v1 container.</returns>
		Id3V1Tag Read(FileInfo file);

		/// <summary>
		/// Reads the ID3v1 tag using default code page for text encoding.
		/// </summary>
		/// <param name="inputStream">the input stream (i.e. the file.)</param>
		/// <returns>An ID3v1 container.</returns>
		Id3V1Tag Read(Stream inputStream);

		/// <summary>
		/// Reads the ID3v1 tag using specified code page for text encoding.
		/// </summary>
		/// <param name="file">the file.</param>
		/// <param name="codePage">The code page.</param>
		/// <returns>An ID3v1 container.</returns>
		Id3V1Tag Read(FileInfo file, int codePage);

		/// <summary>
		/// Writes a new ID3v1 tag using default code page for text encoding.
		/// </summary>
		/// <param name="tag">the tag.</param>
		/// <param name="input">the audio input stream.</param>
		/// <param name="output">the target stream.</param>
        void Write(Id3V1Tag tag, Stream input, Stream output);

		/// <summary>
		/// Writes a new ID3v1 tag using specified code page for text encoding.
		/// </summary>
		/// <param name="tag">the tag.</param>
		/// <param name="input">the audio input stream.</param>
		/// <param name="output">the target stream.</param>
		/// <param name="codePage">The code page for text encoding.</param>
		void Write(Id3V1Tag tag, Stream input, Stream output, int codePage);

        /// <summary>
        /// Removes ID3v1 tag.
        /// </summary>
        /// <param name="input">the input stream</param>
        /// <param name="output">the output stream</param>
        void Remove(Stream input, Stream output);
	}
}