using System.IO;
using ID3Tag.HighLevel;
using ID3Tag.HighLevel.ID3Frame;
using NUnit.Framework;

namespace ID3Tag.Net.NUnit.Lowlevel
{
	[TestFixture]
	public class ValidatorTest : Test
	{
		#region Setup/Teardown

		[SetUp]
		public void Setup()
		{
			m_TagController = Id3TagFactory.CreateTagController();
			m_Controller = Id3TagFactory.CreateIoController();

			m_AudioData = new byte[0x10];
			FillData(m_AudioData);
		}

		#endregion

		private void WriteTagContainer(TagContainer tagContainer)
		{
			Stream dataStream = null;
			Stream tagStream = null;
			try
			{
				dataStream = new MemoryStream(m_AudioData);
				tagStream = new MemoryStream(64000);

				// Write the content to a byte stream.
				m_Controller.Write(tagContainer, dataStream, tagStream);
			}
			finally
			{
				if (dataStream != null)
				{
					dataStream.Close();
					dataStream.Dispose();
				}

				if (tagStream != null)
				{
					tagStream.Close();
					tagStream.Dispose();
				}
			}
		}

		[Test]
		public void ValidationOKTest()
		{
			/*
             *  T___
             *  TXXX
             *  WXXX
             *  APIC
             */

			var tagContainer = new TagContainerV3();
			tagContainer.Tag.SetExtendedHeader(0, false);
			tagContainer.Tag.SetHeaderFlags(false, false, false);

			var textFrame = new TextFrame("TALB", "My Albun", TextEncodingType.Ansi, 0);
			var userDefineTextFrame = new UserDefinedTextFrame("my comment", "so", TextEncodingType.Ansi, 0);
			var linkFrame = new UserDefinedURLLinkFrame("id3tag", "id3tag.codeplex.com", TextEncodingType.Ansi, 0);
			var pictureFrame = new PictureFrame(
				TextEncodingType.Ansi,
				0,
				"image/jpeg",
				"la",
				PictureType.Other,
				new byte[] { 0x11, 0x12, 0x13, 0x14 });

			tagContainer.Add(textFrame);
			tagContainer.Add(userDefineTextFrame);
			tagContainer.Add(linkFrame);
			tagContainer.Add(pictureFrame);

			WriteTagContainer(tagContainer);

			// OK... passed!
		}

		[Test]
		public void ValidationOKTest1()
		{
			/*
             *  T___
             *  TXXX
             *  WXXX
             *  APIC
             */

			var tagContainer = new TagContainerV3();
			tagContainer.Tag.SetExtendedHeader(0, false);
			tagContainer.Tag.SetHeaderFlags(false, false, false);

			var textFrame = new TextFrame("TALB", "My Albun", TextEncodingType.UTF16, 0);
			var userDefineTextFrame = new UserDefinedTextFrame("my comment", "so", TextEncodingType.UTF16, 0);
			var linkFrame = new UserDefinedURLLinkFrame("id3tag", "id3tag.codeplex.com", TextEncodingType.UTF16, 0);
			var pictureFrame = new PictureFrame(
				TextEncodingType.UTF16,
				0,
				"image/jpeg",
				"la",
				PictureType.Other,
				new byte[] { 0x11, 0x12, 0x13, 0x14 });

			tagContainer.Add(textFrame);
			tagContainer.Add(userDefineTextFrame);
			tagContainer.Add(linkFrame);
			tagContainer.Add(pictureFrame);

			WriteTagContainer(tagContainer);

			// OK... passed!
		}

		[Test]
		[ExpectedException(typeof(InvalidID3StructureException))]
		public void ValidationPictureFrameFailed1()
		{
			/*
             *  T___
             *  TXXX
             *  WXXX
             *  APIC
             */

			var tagContainer = new TagContainerV3();
			tagContainer.Tag.SetExtendedHeader(0, false);
			tagContainer.Tag.SetHeaderFlags(false, false, false);

			var textFrame = new TextFrame("TALB", "My Albun", TextEncodingType.Ansi, 0);
			var userDefineTextFrame = new UserDefinedTextFrame("my comment", "so", TextEncodingType.Ansi, 0);
			var linkFrame = new UserDefinedURLLinkFrame("id3tag", "id3tag.codeplex.com", TextEncodingType.Ansi, 0);
			var pictureFrame = new PictureFrame(
				TextEncodingType.UTF16_BE,
				0,
				"image/jpeg",
				"la",
				PictureType.Other,
				new byte[] { 0x11, 0x12, 0x13, 0x14 });

			tagContainer.Add(textFrame);
			tagContainer.Add(userDefineTextFrame);
			tagContainer.Add(linkFrame);
			tagContainer.Add(pictureFrame);

			WriteTagContainer(tagContainer);

			// OK... passed!
		}

		[Test]
		[ExpectedException(typeof(InvalidID3StructureException))]
		public void ValidationPictureFrameFailed2()
		{
			/*
             *  T___
             *  TXXX
             *  WXXX
             *  APIC
             */

			var tagContainer = new TagContainerV3();
			tagContainer.Tag.SetExtendedHeader(0, false);
			tagContainer.Tag.SetHeaderFlags(false, false, false);

			var textFrame = new TextFrame("TALB", "My Albun", TextEncodingType.Ansi, 0);
			var userDefineTextFrame = new UserDefinedTextFrame("my comment", "so", TextEncodingType.Ansi, 0);
			var linkFrame = new UserDefinedURLLinkFrame("id3tag", "id3tag.codeplex.com", TextEncodingType.Ansi, 0);
			var pictureFrame = new PictureFrame(
				TextEncodingType.UTF8,
				0,
				"image/jpeg",
				"la",
				PictureType.Other,
				new byte[] { 0x11, 0x12, 0x13, 0x14 });

			tagContainer.Add(textFrame);
			tagContainer.Add(userDefineTextFrame);
			tagContainer.Add(linkFrame);
			tagContainer.Add(pictureFrame);

			WriteTagContainer(tagContainer);

			// OK... passed!
		}

		[Test]
		[ExpectedException(typeof(InvalidID3StructureException))]
		public void ValidationTextFrameFailed1()
		{
			/*
             *  T___
             *  TXXX
             *  WXXX
             *  APIC
             */

			var tagContainer = new TagContainerV3();
			tagContainer.Tag.SetExtendedHeader(0, false);
			tagContainer.Tag.SetHeaderFlags(false, false, false);

			var textFrame = new TextFrame("TALB", "My Albun", TextEncodingType.UTF16_BE, 0);
			var userDefineTextFrame = new UserDefinedTextFrame("my comment", "so", TextEncodingType.UTF16, 0);
			var linkFrame = new UserDefinedURLLinkFrame("id3tag", "id3tag.codeplex.com", TextEncodingType.UTF16, 0);
			var pictureFrame = new PictureFrame(
				TextEncodingType.UTF16,
				0,
				"image/jpeg",
				"la",
				PictureType.Other,
				new byte[] { 0x11, 0x12, 0x13, 0x14 });

			tagContainer.Add(textFrame);
			tagContainer.Add(userDefineTextFrame);
			tagContainer.Add(linkFrame);
			tagContainer.Add(pictureFrame);

			WriteTagContainer(tagContainer);

			// OK... passed!
		}

		[Test]
		[ExpectedException(typeof(InvalidID3StructureException))]
		public void ValidationTextFrameFailed2()
		{
			/*
             *  T___
             *  TXXX
             *  WXXX
             *  APIC
             */

			var tagContainer = new TagContainerV3();
			tagContainer.Tag.SetExtendedHeader(0, false);
			tagContainer.Tag.SetHeaderFlags(false, false, false);

			var textFrame = new TextFrame("TALB", "My Albun", TextEncodingType.UTF8, 0);
			var userDefineTextFrame = new UserDefinedTextFrame("my comment", "so", TextEncodingType.UTF16, 0);
			var linkFrame = new UserDefinedURLLinkFrame("id3tag", "id3tag.codeplex.com", TextEncodingType.UTF16, 0);
			var pictureFrame = new PictureFrame(
				TextEncodingType.UTF16,
				0,
				"image/jpeg",
				"la",
				PictureType.Other,
				new byte[] { 0x11, 0x12, 0x13, 0x14 });

			tagContainer.Add(textFrame);
			tagContainer.Add(userDefineTextFrame);
			tagContainer.Add(linkFrame);
			tagContainer.Add(pictureFrame);

			WriteTagContainer(tagContainer);

			// OK... passed!
		}

		[Test]
		[ExpectedException(typeof(InvalidID3StructureException))]
		public void ValidationUserDefinedTextFrameFailed1()
		{
			/*
             *  T___
             *  TXXX
             *  WXXX
             *  APIC
             */

			var tagContainer = new TagContainerV3();
			tagContainer.Tag.SetExtendedHeader(0, false);
			tagContainer.Tag.SetHeaderFlags(false, false, false);

			var textFrame = new TextFrame("TALB", "My Albun", TextEncodingType.Ansi, 0);
			var userDefineTextFrame = new UserDefinedTextFrame("my comment", "so", TextEncodingType.UTF16_BE, 0);
			var linkFrame = new UserDefinedURLLinkFrame("id3tag", "id3tag.codeplex.com", TextEncodingType.UTF16, 0);
			var pictureFrame = new PictureFrame(
				TextEncodingType.UTF16,
				0,
				"image/jpeg",
				"la",
				PictureType.Other,
				new byte[] { 0x11, 0x12, 0x13, 0x14 });

			tagContainer.Add(textFrame);
			tagContainer.Add(userDefineTextFrame);
			tagContainer.Add(linkFrame);
			tagContainer.Add(pictureFrame);

			WriteTagContainer(tagContainer);

			// OK... passed!
		}

		[Test]
		[ExpectedException(typeof(InvalidID3StructureException))]
		public void ValidationUserDefinedTextFrameFailed2()
		{
			/*
             *  T___
             *  TXXX
             *  WXXX
             *  APIC
             */

			var tagContainer = new TagContainerV3();
			tagContainer.Tag.SetExtendedHeader(0, false);
			tagContainer.Tag.SetHeaderFlags(false, false, false);

			var textFrame = new TextFrame("TALB", "My Albun", TextEncodingType.Ansi, 0);
			var userDefineTextFrame = new UserDefinedTextFrame("my comment", "so", TextEncodingType.UTF8, 0);
			var linkFrame = new UserDefinedURLLinkFrame("id3tag", "id3tag.codeplex.com", TextEncodingType.UTF16, 0);
			var pictureFrame = new PictureFrame(
				TextEncodingType.UTF16,
				0,
				"image/jpeg",
				"la",
				PictureType.Other,
				new byte[] { 0x11, 0x12, 0x13, 0x14 });

			tagContainer.Add(textFrame);
			tagContainer.Add(userDefineTextFrame);
			tagContainer.Add(linkFrame);
			tagContainer.Add(pictureFrame);

			WriteTagContainer(tagContainer);

			// OK... passed!
		}

		[Test]
		[ExpectedException(typeof(InvalidID3StructureException))]
		public void ValidationUserDefinedURLFrameFailed1()
		{
			/*
             *  T___
             *  TXXX
             *  WXXX
             *  APIC
             */

			var tagContainer = new TagContainerV3();
			tagContainer.Tag.SetExtendedHeader(0, false);
			tagContainer.Tag.SetHeaderFlags(false, false, false);

			var textFrame = new TextFrame("TALB", "My Albun", TextEncodingType.Ansi, 0);
			var userDefineTextFrame = new UserDefinedTextFrame("my comment", "so", TextEncodingType.Ansi, 0);
			var linkFrame = new UserDefinedURLLinkFrame("id3tag", "id3tag.codeplex.com", TextEncodingType.UTF16_BE, 0);
			var pictureFrame = new PictureFrame(
				TextEncodingType.UTF16,
				0,
				"image/jpeg",
				"la",
				PictureType.Other,
				new byte[] { 0x11, 0x12, 0x13, 0x14 });

			tagContainer.Add(textFrame);
			tagContainer.Add(userDefineTextFrame);
			tagContainer.Add(linkFrame);
			tagContainer.Add(pictureFrame);

			WriteTagContainer(tagContainer);

			// OK... passed!
		}

		[Test]
		[ExpectedException(typeof(InvalidID3StructureException))]
		public void ValidationUserDefinedURLFrameFailed2()
		{
			/*
             *  T___
             *  TXXX
             *  WXXX
             *  APIC
             */

			var tagContainer = new TagContainerV3();
			tagContainer.Tag.SetExtendedHeader(0, false);
			tagContainer.Tag.SetHeaderFlags(false, false, false);

			var textFrame = new TextFrame("TALB", "My Albun", TextEncodingType.Ansi, 0);
			var userDefineTextFrame = new UserDefinedTextFrame("my comment", "so", TextEncodingType.Ansi, 0);
			var linkFrame = new UserDefinedURLLinkFrame("id3tag", "id3tag.codeplex.com", TextEncodingType.UTF8, 0);
			var pictureFrame = new PictureFrame(
				TextEncodingType.UTF16,
				0,
				"image/jpeg",
				"la",
				PictureType.Other,
				new byte[] { 0x11, 0x12, 0x13, 0x14 });

			tagContainer.Add(textFrame);
			tagContainer.Add(userDefineTextFrame);
			tagContainer.Add(linkFrame);
			tagContainer.Add(pictureFrame);

			WriteTagContainer(tagContainer);

			// OK... passed!
		}
	}
}