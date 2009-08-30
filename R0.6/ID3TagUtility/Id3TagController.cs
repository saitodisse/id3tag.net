using System;
using System.IO;
using System.Windows;
using ID3Tag;
using ID3Tag.HighLevel;
using ID3Tag.HighLevel.ID3Frame;
using ID3Tag.LowLevel;

namespace ID3TagUtility
{
	public class Id3TagController
	{
		/// <summary>
		/// Reads a tag from a file.
		/// </summary>
		/// <param name="filename">the filename.</param>
		/// <returns>the tag.</returns>
		public TagContainer ReadTag(string filename)
		{
			var file = new FileInfo(filename);

			//
			//  Create the controller from the factory.
			//
			IIoController ioController = Id3TagFactory.CreateIoController();
			ITagController tagController = Id3TagFactory.CreateTagController();

			TagContainer tag = null;
			try
			{
				//
				// Read the raw tag ...
				//
				Id3TagInfo tagInfo = ioController.Read(file);
				//
				//  ... and decode the frames.
				//
				tag = tagController.Decode(tagInfo);
			}
			catch (ID3IOException ioException)
			{
				MessageBox.Show("IO Exception caught : " + ioException.Message);
			}
			catch (ID3HeaderNotFoundException headerNotFoundException)
			{
				MessageBox.Show("ID3 header not found : " + headerNotFoundException.Message);
			}
			catch (ID3TagException tagException)
			{
				MessageBox.Show("ID3TagException caught : " + tagException.Message);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Unknown exception caught : " + ex.Message);
			}

			return tag;
		}

		/// <summary>
		/// Writes a tag to file.
		/// </summary>
		/// <param name="tagController"></param>
		/// <param name="sourceFile">the source file.</param>
		/// <param name="targetFile">the target file.</param>
		/// <remarks>the old tags will be removed.</remarks>
		public void WriteTag(TagContainer tagController, string sourceFile, string targetFile)
		{
			FileStream inputStream = null;
			FileStream outputStream = null;
			try
			{
				IIoController ioController = Id3TagFactory.CreateIoController();

				// Write the tag.
				inputStream = File.Open(sourceFile, FileMode.Open);
				outputStream = File.OpenWrite(targetFile);
				ioController.Write(tagController, inputStream, outputStream);
			}
			catch (ID3IOException ioException)
			{
				MessageBox.Show("IO Exception caught : " + ioException.Message);
			}
			catch (ID3TagException tagException)
			{
				MessageBox.Show("ID3TagException caught : " + tagException.Message);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Unknown exception caught : " + ex.Message);
			}
			finally
			{
				if (inputStream != null)
				{
					inputStream.Close();
					inputStream.Dispose();
				}

				if (outputStream != null)
				{
					outputStream.Close();
					outputStream.Dispose();
				}
			}
		}

		public FileState ReadTagStatus(string filename)
		{
			//
			//  Create the controller from the factory.
			//

			var file = new FileInfo(filename);
			IIoController ioController = Id3TagFactory.CreateIoController();
			FileState state = null;
			try
			{
				state = ioController.DetermineTagStatus(file);
			}
			catch (ID3IOException ioException)
			{
				MessageBox.Show("IO Exception caught : " + ioException.Message);
			}
			catch (ID3TagException tagException)
			{
				MessageBox.Show("ID3TagException caught : " + tagException.Message);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Unknown exception caught : " + ex.Message);
			}

			return state;
		}

		public TagContainer BuildTag(ID3V2TagData data)
		{
			TagContainer tagController;
			if (data.Version == TagVersion.Id3V23)
			{
				var tagContainerV3 = new TagContainerV3();
				// Configure the tag header.
				tagContainerV3.Tag.SetHeaderFlags(data.Unsynchronisation, data.ExtendedHeader, data.ExperimentalIndicator);

				if (data.ExtendedHeader)
				{
					tagContainerV3.Tag.SetExtendedHeader(data.PaddingSize, data.CrCPresent);
					tagContainerV3.Tag.SetCrc32(data.Crc);
				}

				tagController = tagContainerV3;
			}
			else
			{
				var tagContainerV4 = new TagContainerV4();

				tagContainerV4.Tag.SetHeaderFlags(false, false, false, true);

				//TODO: Read the data from parameter!

				tagController = tagContainerV4;
			}

			// OK. Build the frames.
			var albumFrame = new TextFrame("TALB", data.Album, data.EncodingType, 0);
			var artistFrame = new TextFrame("TPE2", data.Artist, data.EncodingType, 0);
			var yearFrame = new TextFrame("TYER", data.Year, data.EncodingType, 0);
			var titleFrame = new TextFrame("TIT2", data.Title, data.EncodingType, 0);
			var textComment = new UserDefinedTextFrame("Your comment", data.Comment, data.EncodingType, 0);
			var comment = new CommentFrame("ENG", "Your Comment", data.Comment, data.EncodingType, 0);
			var encoder = new TextFrame("TENC", data.Encoder, data.EncodingType, 0);

			tagController.Add(albumFrame);
			tagController.Add(artistFrame);
			tagController.Add(yearFrame);
			tagController.Add(titleFrame);
			tagController.Add(textComment);
			tagController.Add(comment);
			tagController.Add(encoder);

			if (data.PictureFrameEnabled)
			{
				if (File.Exists(data.PictureFile))
				{
					WritePictureFrame(data, tagController);
				}
				else
				{
					MessageBox.Show("Picture file not found.");
				}
			}

			return tagController;
		}

		public Id3V1Tag ReadId3V1Tag(string filename, int codePage)
		{
			Id3V1Tag container = null;

			try
			{
				var file = new FileInfo(filename);
				IId3V1Controller id3Converter = Id3TagFactory.CreateId3V1Controller();

				container = id3Converter.Read(file, codePage);
			}
			catch (ID3IOException ioException)
			{
				MessageBox.Show("IO Exception caught : " + ioException.Message);
			}
			catch (ID3TagException tagException)
			{
				MessageBox.Show("ID3TagException caught : " + tagException.Message);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Unknown exception caught : " + ex.Message);
			}

			return container;
		}

		public void WriteId3V1Tag(Id3V1Tag tag, string sourceFile, string targetFile, int codePage)
		{
			FileStream inputStream = null;
			FileStream outputStream = null;
			try
			{
				IId3V1Controller id3V1Controller = Id3TagFactory.CreateId3V1Controller();

				// Write the tag.
				inputStream = File.Open(sourceFile, FileMode.Open);
				outputStream = File.OpenWrite(targetFile);

				id3V1Controller.Write(tag, inputStream, outputStream, codePage);
			}
			catch (ID3IOException ioException)
			{
				MessageBox.Show("IO Exception caught : " + ioException.Message);
			}
			catch (ID3TagException tagException)
			{
				MessageBox.Show("ID3TagException caught : " + tagException.Message);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Unknown exception caught : " + ex.Message);
			}
			finally
			{
				if (inputStream != null)
				{
					inputStream.Close();
					inputStream.Dispose();
				}

				if (outputStream != null)
				{
					outputStream.Close();
					outputStream.Dispose();
				}
			}
		}

		private static void WritePictureFrame(ID3V2TagData data, TagContainer container)
		{
			using (FileStream stream = File.Open(data.PictureFile, FileMode.Open))
			{
				//
				//  Read the picture.
				//
				int byteCount = Convert.ToInt32(stream.Length);
				var pictureData = new byte[byteCount];
				stream.Read(pictureData, 0, byteCount);

				//
				//  Add the picture frame.
				//  
				var pictureFrame = new PictureFrame(
					TextEncodingType.Ansi,
					0,
					"image/jpg",
					"Other",
					PictureType.Other,
					pictureData);
				container.Add(pictureFrame);
			}
		}
	}
}