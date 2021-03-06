﻿using System.Text;
using ID3Tag.HighLevel;
using ID3Tag.HighLevel.ID3Frame;
using NUnit.Framework;

namespace ID3Tag.Net.NUnit.Highlevel.FrameTest
{
    [TestFixture]
    public class PictureFrameTest : Test
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            m_TagController = Id3TagFactory.CreateTagController();
            m_Controller = Id3TagFactory.CreateIoController();
        }

        #endregion

        [Test]
        public void ConstructorTest()
        {
            var encoding = Encoding.Default;
            const string mime = "ABCD";
            const string description = "EFGH";
            const PictureType pictureType = PictureType.Other;
            var pictureData = new byte[] {0x20, 0x21, 0x22, 0x23};

            var frame = new PictureFrame(encoding, mime, description, pictureType, pictureData);

            Assert.AreEqual(frame.TextEncoding.CodePage, encoding.CodePage);
            Assert.AreEqual(frame.MimeType, mime);
            Assert.AreEqual(frame.Description, description);
            Assert.AreEqual(frame.PictureCoding, pictureType);
            Assert.IsTrue(ComparePayload(frame.PictureData, pictureData));
        }

        [Test]
        public void ConvertISO8859()
        {
            var encoding = Encoding.Default;
            const string mimeType = "ABCD";
            const PictureType pictureType = PictureType.CoverFront;
            const string description = "EFGH";
            var data = new byte[] {0x20, 0x21, 0x22, 0x23};

            var pictureFrame = new PictureFrame(encoding, mimeType, description, pictureType, data);
            var rawFrame = pictureFrame.Convert(TagVersion.Id3V23);

            var refBytes = new byte[]
                               {
                                   0x00, 0x41, 0x42, 0x43, 0x44, 0x00, 0x03, 0x45, 0x46, 0x47, 0x48, 0x00,
                                   0x20, 0x21, 0x22, 0x23
                               };

            Assert.IsTrue(ComparePayload(rawFrame.Payload, refBytes));
        }

        [Test]
        public void ConvertUTF16()
        {
            var encoding = Encoding.Unicode;
            const string mimeType = "ABCD";
            const PictureType pictureType = PictureType.CoverFront;
            const string description = "EFGH";
            var data = new byte[] {0x20, 0x21, 0x22, 0x23};

            var pictureFrame = new PictureFrame(encoding, mimeType, description, pictureType, data);
            var rawFrame = pictureFrame.Convert(TagVersion.Id3V23);

            var refBytes = new byte[]
                               {
                                   0x01, 0x41, 0x42, 0x43, 0x44, 0x00, 0x03, 0xFF, 0xFE, 0x45, 0x00, 0x46, 0x00, 0x47,
                                   0x00, 0x48, 0x00, 0x00, 0x00,
                                   0x20, 0x21, 0x22, 0x23
                               };

            Assert.IsTrue(ComparePayload(rawFrame.Payload, refBytes));
        }

        [Test]
        public void ConvertUTF16BE()
        {
            var encoding = new UnicodeEncoding(true, false);
            const string mimeType = "ABCD";
            const PictureType pictureType = PictureType.CoverFront;
            const string description = "EFGH";
            var data = new byte[] {0x20, 0x21, 0x22, 0x23};

            var pictureFrame = new PictureFrame(encoding, mimeType, description, pictureType, data);
            var rawFrame = pictureFrame.Convert(TagVersion.Id3V23);

            var refBytes = new byte[]
                               {
                                   0x02, 0x41, 0x42, 0x43, 0x44, 0x00, 0x03, 0x00, 0x45, 0x00, 0x46, 0x00, 0x47, 0x00,
                                   0x48, 0x00, 0x00,
                                   0x20, 0x21, 0x22, 0x23
                               };

            Assert.IsTrue(ComparePayload(rawFrame.Payload, refBytes));
        }

        [Test]
        public void ConvertUTF8()
        {
            var encoding = new UTF8Encoding();
            const string mimeType = "ABCD";
            const PictureType pictureType = PictureType.CoverFront;
            const string description = "EFGH";
            var data = new byte[] {0x20, 0x21, 0x22, 0x23};

            var pictureFrame = new PictureFrame(encoding, mimeType, description, pictureType, data);
            var rawFrame = pictureFrame.Convert(TagVersion.Id3V23);

            var refBytes = new byte[]
                               {
                                   0x03, 0x41, 0x42, 0x43, 0x44, 0x00, 0x03, 0x45, 0x46, 0x47, 0x48, 0x00,
                                   0x20, 0x21, 0x22, 0x23
                               };

            Assert.IsTrue(ComparePayload(rawFrame.Payload, refBytes));
        }

        [Test]
        public void PictureFrameDetectionTest2Utf16BigEndian()
        {
            var bytes = new byte[]
                            {
                                0x41, 0x50, 0x49, 0x43, 0x00, 0x00, 0x00, 0x17, 0x00, 0x00,
                                0x01, 0x41, 0x42, 0x43, 0x44, 0x00, 0x03, 0xFE, 0xFF, 0x00, 0x45, 0x00, 0x46, 0x00, 0x47
                                , 0x00, 0x48, 0x00, 0x00,
                                0x20, 0x21, 0x22, 0x23
                            };

            var completeTag = GetCompleteV3Tag(bytes);
            Read(completeTag);

            var tagContainer = m_TagController.Decode(m_TagInfo);
            var tag = tagContainer.GetId3V23Descriptor();

            Assert.AreEqual(tag.MajorVersion, 3);
            Assert.AreEqual(tag.Revision, 0);
            Assert.AreEqual(tagContainer.Count, 1);

            var frame = FrameUtils.ConvertToPictureFrame(tagContainer[0]);

            Assert.AreEqual(frame.Type, FrameType.Picture);
            Assert.AreEqual(frame.TextEncoding.CodePage, 1201);
            Assert.AreEqual(frame.MimeType, "ABCD");
            Assert.AreEqual(frame.PictureCoding, PictureType.CoverFront);
            Assert.AreEqual(frame.Description, "EFGH");
            Assert.IsTrue(ComparePayload(frame.PictureData, new byte[] {0x20, 0x21, 0x22, 0x23}));
        }

        [Test]
        public void PictureFrameDetectionTest2Utf16LittleEndian()
        {
            var bytes = new byte[]
                            {
                                0x41, 0x50, 0x49, 0x43, 0x00, 0x00, 0x00, 0x17, 0x00, 0x00,
                                0x01, 0x41, 0x42, 0x43, 0x44, 0x00, 0x03, 0xFF, 0xFE, 0x45, 0x00, 0x46, 0x00, 0x47, 0x00
                                , 0x48, 0x00, 0x00, 0x00,
                                0x20, 0x21, 0x22, 0x23
                            };

            var completeTag = GetCompleteV3Tag(bytes);
            Read(completeTag);

            var tagContainer = m_TagController.Decode(m_TagInfo);
            var tag = tagContainer.GetId3V23Descriptor();

            Assert.AreEqual(tag.MajorVersion, 3);
            Assert.AreEqual(tag.Revision, 0);
            Assert.AreEqual(tagContainer.Count, 1);

            var frame = FrameUtils.ConvertToPictureFrame(tagContainer[0]);

            Assert.AreEqual(frame.Type, FrameType.Picture);
            Assert.AreEqual(frame.TextEncoding, Encoding.Unicode);
            Assert.AreEqual(frame.MimeType, "ABCD");
            Assert.AreEqual(frame.PictureCoding, PictureType.CoverFront);
            Assert.AreEqual(frame.Description, "EFGH");
            Assert.IsTrue(ComparePayload(frame.PictureData, new byte[] {0x20, 0x21, 0x22, 0x23}));
        }

        [Test]
        public void PictureFrameDetectionTestIso8859()
        {
            var bytes = new byte[]
                            {
                                0x41, 0x50, 0x49, 0x43, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                                0x00, 0x41, 0x42, 0x43, 0x44, 0x00, 0x03, 0x45, 0x46, 0x47, 0x48, 0x00,
                                0x20, 0x21, 0x22, 0x23
                            };

            var completeTag = GetCompleteV3Tag(bytes);
            Read(completeTag);

            var tagContainer = m_TagController.Decode(m_TagInfo);
            var tag = tagContainer.GetId3V23Descriptor();

            Assert.AreEqual(tag.MajorVersion, 3);
            Assert.AreEqual(tag.Revision, 0);
            Assert.AreEqual(tagContainer.Count, 1);

            var frame = FrameUtils.ConvertToPictureFrame(tagContainer[0]);

            Assert.AreEqual(frame.Type, FrameType.Picture);
            Assert.AreEqual(frame.TextEncoding.CodePage, Encoding.Default.CodePage);
            Assert.AreEqual(frame.MimeType, "ABCD");
            Assert.AreEqual(frame.PictureCoding, PictureType.CoverFront);
            Assert.AreEqual(frame.Description, "EFGH");
            Assert.IsTrue(ComparePayload(frame.PictureData, new byte[] {0x20, 0x21, 0x22, 0x23}));
        }

        [Test]
        public void PictureFrameDetectionUTF16BE()
        {
            var bytes = new byte[]
                            {
                                0x41, 0x50, 0x49, 0x43, 0x00, 0x00, 0x00, 0x15, 0x00, 0x00,
                                0x02, 0x41, 0x42, 0x43, 0x44, 0x00, 0x03, 0x00, 0x45, 0x00, 0x46, 0x00, 0x47, 0x00, 0x48
                                , 0x00, 0x00,
                                0x20, 0x21, 0x22, 0x23
                            };

            var completeTag = GetCompleteV3Tag(bytes);
            Read(completeTag);

            var tagContainer = m_TagController.Decode(m_TagInfo);
            var tag = tagContainer.GetId3V23Descriptor();

            Assert.AreEqual(tag.MajorVersion, 3);
            Assert.AreEqual(tag.Revision, 0);
            Assert.AreEqual(tagContainer.Count, 1);

            var frame = FrameUtils.ConvertToPictureFrame(tagContainer[0]);

            Assert.AreEqual(frame.Type, FrameType.Picture);
            Assert.AreEqual(frame.TextEncoding.CodePage, Encoding.BigEndianUnicode.CodePage);
            Assert.AreEqual(frame.MimeType, "ABCD");
            Assert.AreEqual(frame.PictureCoding, PictureType.CoverFront);
            Assert.AreEqual(frame.Description, "EFGH");
            Assert.IsTrue(ComparePayload(frame.PictureData, new byte[] {0x20, 0x21, 0x22, 0x23}));
        }

        [Test]
        public void PictureFrameDetectionUTF8()
        {
            var bytes = new byte[]
                            {
                                0x41, 0x50, 0x49, 0x43, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                                0x03, 0x41, 0x42, 0x43, 0x44, 0x00, 0x03, 0x45, 0x46, 0x47, 0x48, 0x00,
                                0x20, 0x21, 0x22, 0x23
                            };

            var completeTag = GetCompleteV3Tag(bytes);
            Read(completeTag);

            var tagContainer = m_TagController.Decode(m_TagInfo);
            var tag = tagContainer.GetId3V23Descriptor();

            Assert.AreEqual(tag.MajorVersion, 3);
            Assert.AreEqual(tag.Revision, 0);
            Assert.AreEqual(tagContainer.Count, 1);

            var frame = FrameUtils.ConvertToPictureFrame(tagContainer[0]);

            Assert.AreEqual(frame.Type, FrameType.Picture);
            Assert.AreEqual(frame.TextEncoding.CodePage, Encoding.UTF8.CodePage);
            Assert.AreEqual(frame.MimeType, "ABCD");
            Assert.AreEqual(frame.PictureCoding, PictureType.CoverFront);
            Assert.AreEqual(frame.Description, "EFGH");
            Assert.IsTrue(ComparePayload(frame.PictureData, new byte[] {0x20, 0x21, 0x22, 0x23}));
        }
    }
}