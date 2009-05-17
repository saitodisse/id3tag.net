using System.IO;

namespace ID3Tag.HighLevel
{
    /// <summary>
    /// Converts the ID3v1 tags to ID3v2 tags.
    /// </summary>
    public interface IId3V1Controller
    {
        /// <summary>
        /// Converts ID3v1 tags to ID3v2.
        /// </summary>
        /// <param name="inputStream">the input stream (i.e. the file.)</param>
        /// <returns>a tag container.</returns>
        Id3V1Tag Read(Stream inputStream);

        /// <summary>
        /// Converts ID3v1 tags to ID3v2.
        /// </summary>
        /// <param name="file">the file.</param>
        /// <returns>a tag container.</returns>
        Id3V1Tag Read(FileInfo file);
    }
}