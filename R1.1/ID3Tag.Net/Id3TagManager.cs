using System;
using System.IO;
using Id3Tag.HighLevel;
using Id3Tag.LowLevel;

namespace Id3Tag
{
	/// <summary>
	/// Provides simple interface to ID3 Tag V1 and V2 functionality
	/// </summary>
	public class Id3TagManager : IId3TagManager
	{
		/// <summary>
		/// Reads ID3 v1 tag from the the specified file path.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns>ID3 v1 Tag</returns>
		public Id3V1Tag ReadV1Tag(string path)
		{
			return ReadV1Tag(path, 0);
		}

		/// <summary>
		/// Reads ID3 v1 tag from the the specified file path.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="codePage">Code page of the tag text.</param>
		/// <returns>ID3 v1 Tag</returns>
		public Id3V1Tag ReadV1Tag(string path, int codePage)
		{
			#region Params Check

			if (String.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}

			if (!File.Exists(path))
			{
				throw new FileNotFoundException("File does not exist.", path);
			}

			#endregion

			var file = new FileInfo(path);
			IId3V1Controller id3Converter = Id3TagFactory.CreateId3V1Controller();
			return id3Converter.Read(file, codePage);
		}

		/// <summary>
		/// Updates ID3 v1 tag in the same file.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="tag">The tag.</param>
		public void WriteV1Tag(string path, Id3V1Tag tag)
		{
			WriteV1Tag(path, tag, 0);
		}

		/// <summary>
		/// Updates ID3 v1 tag in the same file.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="tag">The tag.</param>
		/// <param name="codePage">The code page.</param>
		public void WriteV1Tag(string path, Id3V1Tag tag, int codePage)
		{
			#region Params Check

			if (String.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}

			if (!File.Exists(path))
			{
				throw new FileNotFoundException("File does not exist.", path);
			}

			if (tag == null)
			{
				throw new ArgumentNullException("tag");
			}

			#endregion

			IId3V1Controller controller = Id3TagFactory.CreateId3V1Controller();
			WriteTag(path, (input, output) => controller.Write(tag, input, output, codePage));
		}

		/// <summary>
		/// Saves ID3 v1 tag to new file.
		/// </summary>
		/// <param name="sourcePath">The path.</param>
		/// <param name="targetPath">The target sourcePath.</param>
		/// <param name="tag">The tag.</param>
		public void WriteV1Tag(string sourcePath, string targetPath, Id3V1Tag tag)
		{
			WriteV1Tag(sourcePath, targetPath, tag, 0);
		}

		/// <summary>
		/// Saves ID3 v1 tag to new file.
		/// </summary>
		/// <param name="sourcePath">The path.</param>
		/// <param name="targetPath">The target sourcePath.</param>
		/// <param name="tag">The tag.</param>
		/// <param name="codePage">The code page for tag text.</param>
		public void WriteV1Tag(string sourcePath, string targetPath, Id3V1Tag tag, int codePage)
		{
			#region Params Check

			if (String.IsNullOrEmpty(sourcePath))
			{
				throw new ArgumentNullException("sourcePath");
			}

			if (!File.Exists(sourcePath))
			{
				throw new FileNotFoundException("File does not exist.", sourcePath);
			}

			if (String.IsNullOrEmpty(targetPath))
			{
				throw new ArgumentNullException("targetPath");
			}

			if (File.Exists(targetPath))
			{
				throw new FileNotFoundException("File already exists.", targetPath);
			}

			if (tag == null)
			{
				throw new ArgumentNullException("tag");
			}

			#endregion

			IId3V1Controller controller = Id3TagFactory.CreateId3V1Controller();
			WriteTag(sourcePath, targetPath, (input, output) => controller.Write(tag, input, output, codePage));
		}

		/// <summary>
		/// Reads ID3 v2 tag from the the specified file path.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns>ID3 v2 Tag</returns>
		public TagContainer ReadV2Tag(string path)
		{
			return ReadV2Tag(path, 0);
		}

		/// <summary>
		/// Reads ID3 v2 Tag tag from the the specified file path.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="codePage">The default code page for non-unicode encoded text.</param>
		/// <returns>ID3 v2 Tag</returns>
		public TagContainer ReadV2Tag(string path, int codePage)
		{
			#region Params Check

			if (String.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}

			if (!File.Exists(path))
			{
				throw new FileNotFoundException("File does not exist.", path);
			}

			#endregion

			var file = new FileInfo(path);
			IIOController ioController = Id3TagFactory.CreateIOController();
			ITagController tagController = Id3TagFactory.CreateTagController();
			Id3TagInfo tagData = ioController.Read(file);
			return tagController.Decode(tagData, codePage);
		}

		/// <summary>
		/// Updates specified ID3 v2 tag in the same file.
		/// </summary>
		/// <param name="path">The file path.</param>
		/// <param name="tag">The tag to save.</param>
		public void WriteV2Tag(string path, TagContainer tag)
		{
			#region Params Check

			if (String.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}

			if (!File.Exists(path))
			{
				throw new FileNotFoundException("File does not exist.", path);
			}

			if (tag == null)
			{
				throw new ArgumentNullException("tag");
			}

			#endregion

			IIOController controller = Id3TagFactory.CreateIOController();
			WriteTag(path, (input, output) => controller.Write(tag, input, output));
		}

		/// <summary>
		/// Writes specified ID3 v2 tag to new file.
		/// </summary>
		/// <param name="sourcePath">The source file path.</param>
		/// <param name="targetPath">The target file path.</param>
		/// <param name="tag">The tag data.</param>
		public void WriteV2Tag(string sourcePath, string targetPath, TagContainer tag)
		{
			#region Params Check

			if (String.IsNullOrEmpty(sourcePath))
			{
				throw new ArgumentNullException("sourcePath");
			}

			if (!File.Exists(sourcePath))
			{
				throw new FileNotFoundException("File does not exist.", sourcePath);
			}

			if (String.IsNullOrEmpty(targetPath))
			{
				throw new ArgumentNullException("targetPath");
			}

			if (File.Exists(targetPath))
			{
				throw new FileNotFoundException("File already exists.", targetPath);
			}

			if (tag == null)
			{
				throw new ArgumentNullException("tag");
			}

			#endregion

			IIOController controller = Id3TagFactory.CreateIOController();
			WriteTag(sourcePath, targetPath, (input, output) => controller.Write(tag, input, output));
		}


		/// <summary>
		/// Checks if specified file has ID3 tags.
		/// </summary>
		/// <param name="path">The path to file.</param>
		/// <returns>Status of the tags in the file</returns>
		public FileState GetTagsStatus(string path)
		{
			#region Params Check

			if (String.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}

			if (!File.Exists(path))
			{
				throw new FileNotFoundException("File does not exist.", path);
			}

			#endregion

			var file = new FileInfo(path);
			IIOController ioController = Id3TagFactory.CreateIOController();
			return ioController.DetermineTagStatus(file);
		}

        //public void RemoveV2Tag(string path)
        //{
        //    #region Params Check

        //    if (String.IsNullOrEmpty(path))
        //    {
        //        throw new ArgumentNullException("path");
        //    }

        //    if (!File.Exists(path))
        //    {
        //        throw new FileNotFoundException("File does not exist.", path);
        //    }

        //    #endregion

        //    string tempPath = Path.GetTempFileName();


        //}

		/// <summary>
		/// Writes the tag data generically.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="copyDataDelegate">The delegate.</param>
		private static void WriteTag(string path, CopyDataDelegate copyDataDelegate)
		{
			// This implementation writes new content to a temp file and then replaces
			// the original file from the temp file

			string tempPath = Path.GetTempFileName();

			try
			{
				WriteTag(path, tempPath, copyDataDelegate);

				// Save old file as bak
				// Rename new file as old
				// Delete bak file
				string backupPath = path + ".bak";
				if (File.Exists(backupPath))
				{
					File.Delete(backupPath);
				}

				File.Move(path, backupPath);
				File.Move(tempPath, path);
				File.Delete(backupPath);
			}
			catch
			{
				if (File.Exists(tempPath))
				{
					File.Delete(tempPath);
				}

				throw;
			}
		}

		/// <summary>
		/// Writes the data from source file to target file generically.
		/// </summary>
		/// <param name="sourcePath">The source path.</param>
		/// <param name="targetPath">The target path.</param>
		/// <param name="copyDataDelegate">The delegate.</param>
		private static void WriteTag(string sourcePath, string targetPath, CopyDataDelegate copyDataDelegate)
		{
			using (FileStream inputStream = File.Open(sourcePath, FileMode.Open, FileAccess.ReadWrite))
			{
				using (FileStream outputStream = File.OpenWrite(targetPath))
				{
					// Invoke the code that copies sound data from inputStream to outputStream
					// and injects new tag data
					copyDataDelegate.Invoke(inputStream, outputStream);
				}
			}
		}

		#region Nested type: CopyDataDelegate

		private delegate void CopyDataDelegate(FileStream source, FileStream target);

		#endregion

    }
}