using System.IO;
using System.Text;
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

			var textFrame = new TextFrame("TALB", "My Albun", Encoding.Default);
			var userDefineTextFrame = new UserDefinedTextFrame("my comment", "so", Encoding.Default);
			var linkFrame = new UserDefinedURLLinkFrame("id3tag", "id3tag.codeplex.com", Encoding.Default);
			var pictureFrame = new PictureFrame(
				Encoding.Default,
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

			var textFrame = new TextFrame("TALB", "My Albun", Encoding.Unicode);
			var userDefineTextFrame = new UserDefinedTextFrame("my comment", "so", Encoding.Unicode);
			var linkFrame = new UserDefinedURLLinkFrame("id3tag", "id3tag.codeplex.com", Encoding.Unicode);
			var pictureFrame = new PictureFrame(
				Encoding.Unicode,
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

			var textFrame = new TextFrame("TALB", "My Albun", Encoding.Default);
			var userDefineTextFrame = new UserDefinedTextFrame("my comment", "so", Encoding.Default);
			var linkFrame = new UserDefinedURLLinkFrame("id3tag", "id3tag.codeplex.com", Encoding.Default);
			var pictureFrame = new PictureFrame(
				Encoding.BigEndianUnicode,
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

			var textFrame = new TextFrame("TALB", "My Albun", Encoding.Default);
			var userDefineTextFrame = new UserDefinedTextFrame("my comment", "so", Encoding.Default);
			var linkFrame = new UserDefinedURLLinkFrame("id3tag", "id3tag.codeplex.com", Encoding.Default);
			var pictureFrame = new PictureFrame(
				Encoding.UTF8,
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

			var textFrame = new TextFrame("TALB", "My Albun", Encoding.BigEndianUnicode);
			var userDefineTextFrame = new UserDefinedTextFrame("my comment", "so", Encoding.Unicode);
			var linkFrame = new UserDefinedURLLinkFrame("id3tag", "id3tag.codeplex.com", Encoding.Unicode);
			var pictureFrame = new PictureFrame(
				Encoding.Unicode,
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

			var textFrame = new TextFrame("TALB", "My Albun", Encoding.UTF8);
			var userDefineTextFrame = new UserDefinedTextFrame("my comment", "so", Encoding.Unicode);
			var linkFrame = new UserDefinedURLLinkFrame("id3tag", "id3tag.codeplex.com", Encoding.Unicode);
			var pictureFrame = new PictureFrame(
				Encoding.Unicode,
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

			var textFrame = new TextFrame("TALB", "My Albun", Encoding.Default);
			var userDefineTextFrame = new UserDefinedTextFrame("my comment", "so", Encoding.BigEndianUnicode);
			var linkFrame = new UserDefinedURLLinkFrame("id3tag", "id3tag.codeplex.com", Encoding.Unicode);
			var pictureFrame = new PictureFrame(
				Encoding.Unicode,
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

			var textFrame = new TextFrame("TALB", "My Albun", Encoding.Default);
			var userDefineTextFrame = new UserDefinedTextFrame("my comment", "so", Encoding.UTF8);
			var linkFrame = new UserDefinedURLLinkFrame("id3tag", "id3tag.codeplex.com", Encoding.Unicode);
			var pictureFrame = new PictureFrame(
				Encoding.Unicode,
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

			var textFrame = new TextFrame("TALB", "My Albun", Encoding.Default);
			var userDefineTextFrame = new UserDefinedTextFrame("my comment", "so", Encoding.Default);
			var linkFrame = new UserDefinedURLLinkFrame("id3tag", "id3tag.codeplex.com", Encoding.BigEndianUnicode);
			var pictureFrame = new PictureFrame(
				Encoding.Unicode,
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

			var textFrame = new TextFrame("TALB", "My Albun", Encoding.Default);
			var userDefineTextFrame = new UserDefinedTextFrame("my comment", "so", Encoding.Default);
			var linkFrame = new UserDefinedURLLinkFrame("id3tag", "id3tag.codeplex.com", Encoding.UTF8);
			var pictureFrame = new PictureFrame(
				Encoding.Unicode,
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