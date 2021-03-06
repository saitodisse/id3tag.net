using System.IO;
using ID3Tag.HighLevel;

namespace ID3Tag.LowLevel
{
    /// <summary>
    /// Represents a low level tag controller.
    /// </summary>
    public interface IIoController
    {
        /// <summary>
        /// Read the tag from file.
        /// </summary>
        /// <param name="file">the file.</param>
        /// <returns>a low level tag.</returns>
        Id3TagInfo Read(FileInfo file);

        /// <summary>
        /// Read the tag from an input stream.
        /// </summary>
        /// <param name="inputStream">the input stream.</param>
        /// <returns>a low level tag.</returns>
        Id3TagInfo Read(Stream inputStream);

        /// <summary>
        /// Writes a high level tag container to a output stream.
        /// </summary>
        /// <param name="tagContainer">the tag.</param>
        /// <param name="input">the input stream (i.e. audio file)</param>
        /// <param name="output">the output stream (temp. file)</param>
        void Write(TagContainer tagContainer, Stream input, Stream output);

        /// <summary>
        /// Determine the ID3Tag status.
        /// </summary>
        /// <param name="audioStream">the audio stream.</param>
        /// <returns>the ID3Tag state</returns>
        FileState DetermineTagStatus(Stream audioStream);

        /// <summary>
        /// Determine the ID3Tag status.
        /// </summary>
        /// <param name="file">the file.</param>
        /// <returns>the ID3Tag state</returns>
        FileState DetermineTagStatus(FileInfo file);
    }
}