﻿using System.Text;
using Id3Tag.HighLevel;
using Id3Tag.HighLevel.Id3Frame;
using Id3Tag.LowLevel;
using NUnit.Framework;

namespace Id3Tag.Net.NUnit.Highlevel.FrameTest
{
    [TestFixture]
    public class TextFrameTest : Test
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            m_TagController = Id3TagFactory.CreateTagController();
            m_Controller = Id3TagFactory.CreateIOController();
        }

        #endregion

        [Test]
        public void ConstructorTest()
        {
            var textFrame = new TextFrame("TALB", "ABCD", Encoding.Unicode);

            Assert.AreEqual(textFrame.Descriptor.Id, "TALB");
            Assert.AreEqual(textFrame.Content, "ABCD");
            Assert.AreEqual(textFrame.TextEncoding, Encoding.Unicode);
        }

        [Test]
        public void ConverterISO88591_Test()
        {
            var textFrame = new TextFrame
                                {
                                    TextEncoding = Encoding.Default,
                                    Content = "ABCD",
                                    Descriptor = {Id = "TALB"}
                                };

            RawFrame rawFrame = textFrame.Convert(TagVersion.Id3V23);

            Assert.AreEqual(rawFrame.Id, "TALB");
            Assert.AreEqual(rawFrame.Payload[0], 0); // ISO_8859 coding
            Assert.AreEqual(rawFrame.Payload.Count, 5);
        }

        [Test]
        public void ConverterUTF16BE_Test()
        {
            var textFrame = new TextFrame
                                {
                                    TextEncoding = new UnicodeEncoding(true, false),
                                    Content = "ABCD",
                                    Descriptor = {Id = "TALB"}
                                };

            RawFrame rawFrame = textFrame.Convert(TagVersion.Id3V23);

            Assert.AreEqual(rawFrame.Id, "TALB");

            var refBytes = new byte[]
                               {
                                   0x02, 0x00, 0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x44
                               };

            Assert.IsTrue(ComparePayload(rawFrame.Payload, refBytes));
        }

        [Test]
        public void ConverterUTF16_Test()
        {
            var textFrame = new TextFrame
                                {
                                    TextEncoding = Encoding.Unicode,
                                    Content = "ABCD",
                                    Descriptor = {Id = "TALB"}
                                };

            RawFrame rawFrame = textFrame.Convert(TagVersion.Id3V23);

            Assert.AreEqual(rawFrame.Id, "TALB");
            Assert.AreEqual(rawFrame.Payload[0], 1); // UTF 16
            Assert.AreEqual(rawFrame.Payload[1], 0xFF); // BOM 1
            Assert.AreEqual(rawFrame.Payload[2], 0xFE); // BOM 2
            Assert.AreEqual(rawFrame.Payload.Count, 11);
        }

        [Test]
        public void ConverterUTF8_Test()
        {
            var textFrame = new TextFrame
                                {
                                    TextEncoding = new UTF8Encoding(),
                                    Content = "ABCD",
                                    Descriptor = {Id = "TALB"}
                                };

            RawFrame rawFrame = textFrame.Convert(TagVersion.Id3V23);

            Assert.AreEqual(rawFrame.Id, "TALB");

            var refBytes = new byte[]
                               {
                                   0x03, 0x41, 0x42, 0x43, 0x44
                               };

            Assert.IsTrue(ComparePayload(rawFrame.Payload, refBytes));
        }

        [Test]
        public void TextFrameDetectionTest1()
        {
            var frames = new byte[]
                             {
                                 // TALB
                                 0x54, 0x41, 0x4C, 0x42, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00,
                                 0x00, 0x41, 0x42, 0x43, 0x44,
                                 // TIT2
                                 0x54, 0x49, 0x54, 0x32, 0x00, 0x00, 0x00, 0x007, 0xE0, 0xE0,
                                 0x00, 0x45, 0x46, 0x47, 0x48, 0x00, 048
                             };

            byte[] completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            TagContainer tagContainer = m_TagController.Decode(m_TagInfo);
            TagDescriptorV3 tag = tagContainer.GetId3V23Descriptor();

            Assert.AreEqual(tag.MajorVersion, 3);
            Assert.AreEqual(tag.Revision, 0);
            Assert.AreEqual(tagContainer.Count, 2);

            IFrame f1 = tagContainer[0];
            IFrame f2 = tagContainer[1];
            TextFrame text1 = FrameUtilities.ConvertToText(f1);
            TextFrame text2 = FrameUtilities.ConvertToText(f2);

            Assert.AreEqual(text1.Descriptor.Id, "TALB");
            Assert.AreEqual(text1.Type, FrameType.Text);
            Assert.AreEqual(text1.TextEncoding, Encoding.Default);
            Assert.AreEqual(text1.Content, "ABCD");

            Assert.AreEqual(text2.Descriptor.Id, "TIT2");
            Assert.AreEqual(text2.Type, FrameType.Text);
            Assert.AreEqual(text2.TextEncoding, Encoding.Default);
            Assert.AreEqual(text2.Content, "EFGH");
        }

        [Test]
        public void TextFrameDetectionTest2_UTF16()
        {
            var frames = new byte[]
                             {
                                 // TALB
                                 0x54, 0x41, 0x4C, 0x42, 0x00, 0x00, 0x00, 0x0B, 0x00, 0x00,
                                 0x01, 0xFF, 0xFE, 0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x44, 0x00,
                                 // TIT2
                                 0x54, 0x49, 0x54, 0x32, 0x00, 0x00, 0x00, 0x0F, 0xE0, 0xE0,
                                 0x01, 0xFF, 0xFE, 0x45, 0x00, 0x46, 0x00, 0x47, 0x00, 0x48, 0x00, 0x00, 0x00, 0x49,
                                 0x00
                             };

            byte[] completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            TagContainer tagContainer = m_TagController.Decode(m_TagInfo);
            TagDescriptorV3 tag = tagContainer.GetId3V23Descriptor();

            Assert.AreEqual(tag.MajorVersion, 3);
            Assert.AreEqual(tag.Revision, 0);
            Assert.AreEqual(tagContainer.Count, 2);

            IFrame f1 = tagContainer[0];
            IFrame f2 = tagContainer[1];
            TextFrame text1 = FrameUtilities.ConvertToText(f1);
            TextFrame text2 = FrameUtilities.ConvertToText(f2);

            Assert.AreEqual(text1.Descriptor.Id, "TALB");
            Assert.AreEqual(text1.Type, FrameType.Text);
            Assert.AreEqual(text1.TextEncoding, Encoding.Unicode);
            Assert.AreEqual(text1.Content, "ABCD");

            Assert.AreEqual(text2.Descriptor.Id, "TIT2");
            Assert.AreEqual(text2.Type, FrameType.Text);
            Assert.AreEqual(text2.TextEncoding, Encoding.Unicode);
            Assert.AreEqual(text2.Content, "EFGH");
        }

        [Test]
        public void TextFrameUTF16WithBigEndianTest()
        {
            var frames = new byte[]
                             {
                                 // TALB
                                 0x54, 0x41, 0x4C, 0x42, 0x00, 0x00, 0x00, 0x13, 0x00, 0x00,
                                 // UTF 16 BE Encoding
                                 0x01,
                                 // Data..
                                 0xFE, 0xFF, 0x00, 0x74, 0x00, 0x65, 0x00, 0x73, 0x00,
                                 0x74, 0x00, 0x2E, 0x00, 0x6D, 0x00, 0x70, 0x00, 0x33
                             };

            byte[] completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);
            TagContainer tagContainer = m_TagController.Decode(m_TagInfo);

            Assert.AreEqual(tagContainer.Count, 1);

            TextFrame textFrame = FrameUtilities.ConvertToText(tagContainer[0]);
            Assert.AreEqual(textFrame.TextEncoding.CodePage, Encoding.BigEndianUnicode.CodePage);
            Assert.AreEqual(textFrame.Content, "test.mp3");
        }

        [Test]
        public void TextFrameUTF16WithBigEndianTypeTest()
        {
            var frames = new byte[]
                             {
                                 // TALB
                                 0x54, 0x41, 0x4C, 0x42, 0x00, 0x00, 0x00, 0x11, 0x00, 0x00,
                                 // UTF 16 BE Encoding
                                 0x02,
                                 // Data..
                                 0x00, 0x74, 0x00, 0x65, 0x00, 0x73, 0x00,
                                 0x74, 0x00, 0x2E, 0x00, 0x6D, 0x00, 0x70, 0x00, 0x33
                             };

            byte[] completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);
            TagContainer tagContainer = m_TagController.Decode(m_TagInfo);

            Assert.AreEqual(tagContainer.Count, 1);

            TextFrame textFrame = FrameUtilities.ConvertToText(tagContainer[0]);
            Assert.AreEqual(textFrame.TextEncoding.CodePage, Encoding.BigEndianUnicode.CodePage);
            Assert.AreEqual(textFrame.Content, "test.mp3");
        }

        [Test]
        public void TextFrameUTF16WithLittleEndianTest()
        {
            var frames = new byte[]
                             {
                                 // TALB
                                 0x54, 0x41, 0x4C, 0x42, 0x00, 0x00, 0x00, 0x13, 0x00, 0x00,
                                 // UTF 16 Encoding
                                 0x01,
                                 // Data..
                                 0xFF, 0xFE, 0x74, 0x00, 0x65, 0x00, 0x73, 0x00, 0x74,
                                 0x00, 0x2E, 0x00, 0x6D, 0x00, 0x70, 0x00, 0x33, 0x00
                             };

            byte[] completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);
            TagContainer tagContainer = m_TagController.Decode(m_TagInfo);

            Assert.AreEqual(tagContainer.Count, 1);

            TextFrame textFrame = FrameUtilities.ConvertToText(tagContainer[0]);
            Assert.AreEqual(textFrame.TextEncoding, Encoding.Unicode);
            Assert.AreEqual(textFrame.Content, "test.mp3");
        }

        [Test]
        public void TextFrameUTF8Test()
        {
            var frames = new byte[]
                             {
                                 // TALB
                                 0x54, 0x41, 0x4C, 0x42, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00,
                                 // UTF 8 Encoding
                                 0x03,
                                 // Data..
                                 0x74, 0x65, 0x73, 0x74, 0x2E, 0x6D, 0x70, 0x33
                             };

            byte[] completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);
            TagContainer tagContainer = m_TagController.Decode(m_TagInfo);

            Assert.AreEqual(tagContainer.Count, 1);

            TextFrame textFrame = FrameUtilities.ConvertToText(tagContainer[0]);
            Assert.AreEqual(textFrame.TextEncoding.CodePage, Encoding.UTF8.CodePage);
            Assert.AreEqual(textFrame.Content, "test.mp3");
        }
    }
}