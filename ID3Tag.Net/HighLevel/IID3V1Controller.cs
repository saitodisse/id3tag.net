using System.IO;

namespace ID3Tag.HighLevel
{
    /// <summary>
    /// Converts the ID3v1 tags to ID3v2 tags.
    /// </summary>
    public interface IId3V1Controller
    {
        /// <summary>
        /// Reads the ID3v1 tag.
        /// </summary>
        /// <param name="inputStream">the input stream (i.e. the file.)</param>
        /// <returns>An ID3v1 container.</returns>
        Id3V1Tag Read(Stream inputStream);

        /// <summary>
        /// Reads the ID3v1 tag.
        /// </summary>
        /// <param name="file">the file.</param>
        /// <returns>An ID3v1 container.</returns>
        Id3V1Tag Read(FileInfo file);

        /// <summary>
        /// Writes a new ID3v1 tag
        /// </summary>
        /// <param name="tag">the tag.</param>
        /// <param name="input">the audio input stream.</param>
        /// <param name="output">the target stream.</param>
        //void Write(Id3V1Tag tag, Stream input, Stream output);
    }
}