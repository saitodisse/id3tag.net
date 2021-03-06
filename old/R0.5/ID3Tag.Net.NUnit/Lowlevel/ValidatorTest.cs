﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ID3Tag.Factory;
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

        [Test]
        public void ValidationOKTest()
        {
            /*
             *  T___
             *  TXXX
             *  WXXX
             *  APIC
             */

            var tagContainer = new TagContainer();
            tagContainer.Tag.SetExtendedHeader(0,false);
            tagContainer.Tag.SetHeaderFlags(false,false,false);
            tagContainer.Tag.SetVersion(3,0);

            var textFrame = new TextFrame("TALB", "My Albun", TextEncodingType.ISO_8859_1);
            var userDefineTextFrame = new UserDefinedTextFrame("my comment", "so", TextEncodingType.ISO_8859_1);
            var linkFrame = new UserDefinedURLLinkFrame("id3tag", "id3tag.codeplex.com", TextEncodingType.ISO_8859_1);
            var pictureFrame = new PictureFrame(TextEncodingType.ISO_8859_1, "image/jpeg", "la", PictureType.Other,
                                                new byte[] {0x11, 0x12, 0x13, 0x14});

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

            var tagContainer = new TagContainer();
            tagContainer.Tag.SetExtendedHeader(0, false);
            tagContainer.Tag.SetHeaderFlags(false, false, false);
            tagContainer.Tag.SetVersion(3, 0);

            var textFrame = new TextFrame("TALB", "My Albun", TextEncodingType.UTF16);
            var userDefineTextFrame = new UserDefinedTextFrame("my comment", "so", TextEncodingType.UTF16);
            var linkFrame = new UserDefinedURLLinkFrame("id3tag", "id3tag.codeplex.com", TextEncodingType.UTF16);
            var pictureFrame = new PictureFrame(TextEncodingType.UTF16, "image/jpeg", "la", PictureType.Other,
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
        public void ValidationHeaderFailed1()
        {
            /*
             *  T___
             *  TXXX
             *  WXXX
             *  APIC
             */

            var tagContainer = new TagContainer();
            tagContainer.Tag.SetExtendedHeader(0, false);
            tagContainer.Tag.SetHeaderFlags(false, false, false);
            tagContainer.Tag.SetVersion(4, 0);

            var textFrame = new TextFrame("TALB", "My Albun", TextEncodingType.UTF16);
            var userDefineTextFrame = new UserDefinedTextFrame("my comment", "so", TextEncodingType.UTF16);
            var linkFrame = new UserDefinedURLLinkFrame("id3tag", "id3tag.codeplex.com", TextEncodingType.UTF16);
            var pictureFrame = new PictureFrame(TextEncodingType.UTF16, "image/jpeg", "la", PictureType.Other,
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
        public void ValidationHeaderFailed2()
        {
            /*
             *  T___
             *  TXXX
             *  WXXX
             *  APIC
             */

            var tagContainer = new TagContainer();
            tagContainer.Tag.SetExtendedHeader(0, false);
            tagContainer.Tag.SetHeaderFlags(false, false, false);
            tagContainer.Tag.SetVersion(3, 1);

            var textFrame = new TextFrame("TALB", "My Albun", TextEncodingType.UTF16);
            var userDefineTextFrame = new UserDefinedTextFrame("my comment", "so", TextEncodingType.UTF16);
            var linkFrame = new UserDefinedURLLinkFrame("id3tag", "id3tag.codeplex.com", TextEncodingType.UTF16);
            var pictureFrame = new PictureFrame(TextEncodingType.UTF16, "image/jpeg", "la", PictureType.Other,
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

            var tagContainer = new TagContainer();
            tagContainer.Tag.SetExtendedHeader(0, false);
            tagContainer.Tag.SetHeaderFlags(false, false, false);
            tagContainer.Tag.SetVersion(3, 0);

            var textFrame = new TextFrame("TALB", "My Albun", TextEncodingType.UTF16_BE);
            var userDefineTextFrame = new UserDefinedTextFrame("my comment", "so", TextEncodingType.UTF16);
            var linkFrame = new UserDefinedURLLinkFrame("id3tag", "id3tag.codeplex.com", TextEncodingType.UTF16);
            var pictureFrame = new PictureFrame(TextEncodingType.UTF16, "image/jpeg", "la", PictureType.Other,
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

            var tagContainer = new TagContainer();
            tagContainer.Tag.SetExtendedHeader(0, false);
            tagContainer.Tag.SetHeaderFlags(false, false, false);
            tagContainer.Tag.SetVersion(3, 0);

            var textFrame = new TextFrame("TALB", "My Albun", TextEncodingType.UTF8);
            var userDefineTextFrame = new UserDefinedTextFrame("my comment", "so", TextEncodingType.UTF16);
            var linkFrame = new UserDefinedURLLinkFrame("id3tag", "id3tag.codeplex.com", TextEncodingType.UTF16);
            var pictureFrame = new PictureFrame(TextEncodingType.UTF16, "image/jpeg", "la", PictureType.Other,
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

            var tagContainer = new TagContainer();
            tagContainer.Tag.SetExtendedHeader(0, false);
            tagContainer.Tag.SetHeaderFlags(false, false, false);
            tagContainer.Tag.SetVersion(3, 0);

            var textFrame = new TextFrame("TALB", "My Albun", TextEncodingType.ISO_8859_1);
            var userDefineTextFrame = new UserDefinedTextFrame("my comment", "so", TextEncodingType.UTF16_BE);
            var linkFrame = new UserDefinedURLLinkFrame("id3tag", "id3tag.codeplex.com", TextEncodingType.UTF16);
            var pictureFrame = new PictureFrame(TextEncodingType.UTF16, "image/jpeg", "la", PictureType.Other,
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

            var tagContainer = new TagContainer();
            tagContainer.Tag.SetExtendedHeader(0, false);
            tagContainer.Tag.SetHeaderFlags(false, false, false);
            tagContainer.Tag.SetVersion(3, 0);

            var textFrame = new TextFrame("TALB", "My Albun", TextEncodingType.ISO_8859_1);
            var userDefineTextFrame = new UserDefinedTextFrame("my comment", "so", TextEncodingType.UTF8);
            var linkFrame = new UserDefinedURLLinkFrame("id3tag", "id3tag.codeplex.com", TextEncodingType.UTF16);
            var pictureFrame = new PictureFrame(TextEncodingType.UTF16, "image/jpeg", "la", PictureType.Other,
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

            var tagContainer = new TagContainer();
            tagContainer.Tag.SetExtendedHeader(0, false);
            tagContainer.Tag.SetHeaderFlags(false, false, false);
            tagContainer.Tag.SetVersion(3, 0);

            var textFrame = new TextFrame("TALB", "My Albun", TextEncodingType.ISO_8859_1);
            var userDefineTextFrame = new UserDefinedTextFrame("my comment", "so", TextEncodingType.ISO_8859_1);
            var linkFrame = new UserDefinedURLLinkFrame("id3tag", "id3tag.codeplex.com", TextEncodingType.UTF16_BE);
            var pictureFrame = new PictureFrame(TextEncodingType.UTF16, "image/jpeg", "la", PictureType.Other,
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

            var tagContainer = new TagContainer();
            tagContainer.Tag.SetExtendedHeader(0, false);
            tagContainer.Tag.SetHeaderFlags(false, false, false);
            tagContainer.Tag.SetVersion(3, 0);

            var textFrame = new TextFrame("TALB", "My Albun", TextEncodingType.ISO_8859_1);
            var userDefineTextFrame = new UserDefinedTextFrame("my comment", "so", TextEncodingType.ISO_8859_1);
            var linkFrame = new UserDefinedURLLinkFrame("id3tag", "id3tag.codeplex.com", TextEncodingType.UTF8);
            var pictureFrame = new PictureFrame(TextEncodingType.UTF16, "image/jpeg", "la", PictureType.Other,
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

            var tagContainer = new TagContainer();
            tagContainer.Tag.SetExtendedHeader(0, false);
            tagContainer.Tag.SetHeaderFlags(false, false, false);
            tagContainer.Tag.SetVersion(3, 0);

            var textFrame = new TextFrame("TALB", "My Albun", TextEncodingType.ISO_8859_1);
            var userDefineTextFrame = new UserDefinedTextFrame("my comment", "so", TextEncodingType.ISO_8859_1);
            var linkFrame = new UserDefinedURLLinkFrame("id3tag", "id3tag.codeplex.com", TextEncodingType.ISO_8859_1);
            var pictureFrame = new PictureFrame(TextEncodingType.UTF16_BE, "image/jpeg", "la", PictureType.Other,
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

            var tagContainer = new TagContainer();
            tagContainer.Tag.SetExtendedHeader(0, false);
            tagContainer.Tag.SetHeaderFlags(false, false, false);
            tagContainer.Tag.SetVersion(3, 0);

            var textFrame = new TextFrame("TALB", "My Albun", TextEncodingType.ISO_8859_1);
            var userDefineTextFrame = new UserDefinedTextFrame("my comment", "so", TextEncodingType.ISO_8859_1);
            var linkFrame = new UserDefinedURLLinkFrame("id3tag", "id3tag.codeplex.com", TextEncodingType.ISO_8859_1);
            var pictureFrame = new PictureFrame(TextEncodingType.UTF8, "image/jpeg", "la", PictureType.Other,
                                                new byte[] { 0x11, 0x12, 0x13, 0x14 });

            tagContainer.Add(textFrame);
            tagContainer.Add(userDefineTextFrame);
            tagContainer.Add(linkFrame);
            tagContainer.Add(pictureFrame);

            WriteTagContainer(tagContainer);

            // OK... passed!
        }

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
    }
}
