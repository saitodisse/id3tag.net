using Id3Tag.HighLevel;

namespace Id3Tag
{
	/// <summary>
	/// Defines ID3 Tag Manager Functionality
	/// </summary>
	public interface IId3TagManager
	{
		/// <summary>
		/// Reads ID3 v1 tag from the the specified file path.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns>ID3 v1 Tag</returns>
		Id3V1Tag ReadV1Tag(string path);

		/// <summary>
		/// Reads ID3 v1 tag from the the specified file path.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="codePage">Code page of the tag text.</param>
		/// <returns>ID3 v1 Tag</returns>
		Id3V1Tag ReadV1Tag(string path, int codePage);

		/// <summary>
		/// Updates ID3 v1 tag in the same file.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="tag">The tag.</param>
		void WriteV1Tag(string path, Id3V1Tag tag);

		/// <summary>
		/// Updates ID3 v1 tag in the same file.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="tag">The tag.</param>
		/// <param name="codePage">The code page.</param>
		void WriteV1Tag(string path, Id3V1Tag tag, int codePage);

		/// <summary>
		/// Saves ID3 v1 tag to new file.
		/// </summary>
		/// <param name="sourcePath">The path.</param>
		/// <param name="targetPath">The target sourcePath.</param>
		/// <param name="tag">The tag.</param>
		void WriteV1Tag(string sourcePath, string targetPath, Id3V1Tag tag);

		/// <summary>
		/// Saves ID3 v1 tag to new file.
		/// </summary>
		/// <param name="sourcePath">The path.</param>
		/// <param name="targetPath">The target sourcePath.</param>
		/// <param name="tag">The tag.</param>
		/// <param name="codePage">The code page for tag text.</param>
		void WriteV1Tag(string sourcePath, string targetPath, Id3V1Tag tag, int codePage);

		/// <summary>
		/// Reads ID3 v2 tag from the the specified file path.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns>ID3 v2 Tag</returns>
		TagContainer ReadV2Tag(string path);

		/// <summary>
		/// Reads ID3 v2 Tag tag from the the specified file path.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="codePage">The default code page for non-unicode encoded text.</param>
		/// <returns>ID3 v2 Tag</returns>
		TagContainer ReadV2Tag(string path, int codePage);

		/// <summary>
		/// Updates specified ID3 v2 tag in the same file.
		/// </summary>
		/// <param name="path">The file path.</param>
		/// <param name="tag">The tag to save.</param>
		void WriteV2Tag(string path, TagContainer tag);

		/// <summary>
		/// Writes specified ID3 v2 tag to new file.
		/// </summary>
		/// <param name="sourcePath">The source file path.</param>
		/// <param name="targetPath">The target file path.</param>
		/// <param name="tag">The tag data.</param>
		void WriteV2Tag(string sourcePath, string targetPath, TagContainer tag);

		/// <summary>
		/// Checks if specified file has ID3 tags.
		/// </summary>
		/// <param name="path">The path to file.</param>
		/// <returns>Status of the tags in the file</returns>
		FileState GetTagsStatus(string path);
	}
}