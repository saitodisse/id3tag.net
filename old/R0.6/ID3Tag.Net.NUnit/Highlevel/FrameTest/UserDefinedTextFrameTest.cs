﻿using System.Text;
using ID3Tag.HighLevel;
using ID3Tag.HighLevel.ID3Frame;
using NUnit.Framework;

namespace ID3Tag.Net.NUnit.Highlevel.FrameTest
{
    [TestFixture]
    public class UserDefinedTextFrameTest : Test
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
        public void Convert_ISO8859_1_Test()
        {
            var frame = new UserDefinedTextFrame();
            frame.TextEncoding = Encoding.Default;
            frame.Descriptor.ID = "TXXX";
            frame.Description = "ABCD";
            frame.Value = "EFGH";

            var rawFrame = frame.Convert(TagVersion.Id3V23);

            Assert.AreEqual(rawFrame.ID, "TXXX");
            Assert.AreEqual(rawFrame.Payload.Length, 10);
            Assert.AreEqual(rawFrame.Payload[0], 0); // ISO coding
        }

        [Test]
        public void Convert_UTF16_Test()
        {
            var frame = new UserDefinedTextFrame();
            frame.TextEncoding = Encoding.Unicode;
            frame.Descriptor.ID = "TXXX";
            frame.Description = "ABCD";
            frame.Value = "EFGH";

            var rawFrame = frame.Convert(TagVersion.Id3V23);

            Assert.AreEqual(rawFrame.ID, "TXXX");
            Assert.AreEqual(rawFrame.Payload[0], 1); // ISO coding
            Assert.AreEqual(rawFrame.Payload[1], 0xFF); // BOM 1
            Assert.AreEqual(rawFrame.Payload[2], 0xFE); // BOM 2
            Assert.AreEqual(rawFrame.Payload.Length, 21);
        }

        [Test]
        public void Convert_UTF16BE_Test()
        {
            var frame = new UserDefinedTextFrame();
            frame.TextEncoding = Encoding.BigEndianUnicode;
            frame.Descriptor.ID = "TXXX";
            frame.Description = "ABCD";
            frame.Value = "EFGH";

            var rawFrame = frame.Convert(TagVersion.Id3V23);

            Assert.AreEqual(rawFrame.ID, "TXXX");

            var refBytes = new byte[]
                               {
                                   0x02,
                                   0x00, 0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x44,
                                   0x00, 0x00,
                                   0x00, 0x45, 0x00, 0x46, 0x00, 0x47, 0x00, 0x48
                               };

            Assert.IsTrue(ComparePayload(rawFrame.Payload, refBytes));
        }

        [Test]
        public void Convert_UTF8_Test()
        {
            var frame = new UserDefinedTextFrame();
            frame.TextEncoding = Encoding.UTF8;
            frame.Descriptor.ID = "TXXX";
            frame.Description = "ABCD";
            frame.Value = "EFGH";

            var rawFrame = frame.Convert(TagVersion.Id3V23);

            Assert.AreEqual(rawFrame.ID, "TXXX");

            var refBytes = new byte[]
                               {
                                   0x03,
                                   0x41, 0x42, 0x43, 0x44,
                                   0x00,
                                   0x45, 0x46, 0x47, 0x48
                               };

            Assert.IsTrue(ComparePayload(rawFrame.Payload, refBytes));
        }

        [Test]
        public void CreateTest()
        {
            var frame = new UserDefinedTextFrame("ABCD", "EFGH", Encoding.Unicode);

            Assert.AreEqual(frame.Descriptor.ID, "TXXX");
            Assert.AreEqual(frame.Description, "ABCD");
            Assert.AreEqual(frame.Value, "EFGH");
            Assert.AreEqual(frame.TextEncoding, Encoding.Unicode);
        }

        [Test]
        public void UserDefinedTextFrameDetectionTest1()
        {
            var frames = new byte[]
                             {
                                 // TXXX
                                 0x54, 0x58, 0x58, 0x58, 0x00, 0x00, 0x00, 0x0A, 0x00, 0x00,
                                 0x00, 0x41, 0x42, 0x43, 0x44, 0x00, 0x45, 0x46, 0x47, 0x48
                             };

            var completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            var tagContainer = m_TagController.Decode(m_TagInfo);
            var tag = tagContainer.GetId3V23Descriptor();

            Assert.AreEqual(tag.MajorVersion, 3);
            Assert.AreEqual(tag.Revision, 0);
            Assert.AreEqual(tagContainer.Count, 1);

            var f1 = tagContainer[0];
            var userDefined1 = FrameUtils.ConvertToUserDefinedText(f1);

            Assert.AreEqual(userDefined1.Descriptor.ID, "TXXX");
            Assert.AreEqual(userDefined1.Type, FrameType.UserDefinedText);
            Assert.AreEqual(userDefined1.TextEncoding, Encoding.Default);
            Assert.AreEqual(userDefined1.Description, "ABCD");
            Assert.AreEqual(userDefined1.Value, "EFGH");
        }

        [Test]
        public void UserDefinedTextFrameUTF16BETest()
        {
            var frames = new byte[]
                             {
                                 // TXXX
                                 0x54, 0x58, 0x58, 0x58, 0x00, 0x00, 0x00, 0x13, 0x00, 0x00,
                                 // UTF 16 BE
                                 0x02,
                                 // Data..
                                 0x00, 0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x44,
                                 0x00, 0x00, 0x00, 0x45, 0x00, 0x46, 0x00, 0x47, 0x00, 0x48
                             };

            var completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);
            var tagContainer = m_TagController.Decode(m_TagInfo);

            Assert.AreEqual(tagContainer.Count, 1);

            var userDefinedTextFrame = FrameUtils.ConvertToUserDefinedText(tagContainer[0]);
            Assert.AreEqual(userDefinedTextFrame.Type, FrameType.UserDefinedText);
            Assert.AreEqual(userDefinedTextFrame.TextEncoding.CodePage, Encoding.BigEndianUnicode.CodePage);
            Assert.AreEqual(userDefinedTextFrame.Description, "ABCD");
            Assert.AreEqual(userDefinedTextFrame.Value, "EFGH");
        }

        [Test]
        public void UserDefinedTextFrameUTF16WithBigEndianTest()
        {
            var frames = new byte[]
                             {
                                 // TXXX
                                 0x54, 0x58, 0x58, 0x58, 0x00, 0x00, 0x00, 0x15, 0x00, 0x00,
                                 // UTF 16 BEEncoding
                                 0x01,
                                 // Data..
                                 0xFE, 0xFF, 0x00, 0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x44,
                                 0x00, 0x00, 0x00, 0x45, 0x00, 0x46, 0x00, 0x47, 0x00, 0x48
                             };

            var completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);
            var tagContainer = m_TagController.Decode(m_TagInfo);

            Assert.AreEqual(tagContainer.Count, 1);

            var userDefinedTextFrae = FrameUtils.ConvertToUserDefinedText(tagContainer[0]);
            Assert.AreEqual(userDefinedTextFrae.Type, FrameType.UserDefinedText);
            Assert.AreEqual(userDefinedTextFrae.TextEncoding.CodePage, Encoding.BigEndianUnicode.CodePage);
            Assert.AreEqual(userDefinedTextFrae.Description, "ABCD");
            Assert.AreEqual(userDefinedTextFrae.Value, "EFGH");
        }

        [Test]
        public void UserDefinedTextFrameUTF16WithLittleEndianTest()
        {
            var frames = new byte[]
                             {
                                 // TXXX
                                 0x54, 0x58, 0x58, 0x58, 0x00, 0x00, 0x00, 0x15, 0x00, 0x00,
                                 // UTF 16 Encoding
                                 0x01,
                                 // Data..
                                 0xFF, 0xFE, 0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x44, 0x00,
                                 0x00, 0x00, 0x45, 0x00, 0x46, 0x00, 0x47, 0x00, 0x48, 0x00
                             };

            var completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);
            var tagContainer = m_TagController.Decode(m_TagInfo);

            Assert.AreEqual(tagContainer.Count, 1);

            var userDefinedTextFrae = FrameUtils.ConvertToUserDefinedText(tagContainer[0]);
            Assert.AreEqual(userDefinedTextFrae.Type, FrameType.UserDefinedText);
            Assert.AreEqual(userDefinedTextFrae.TextEncoding, Encoding.Unicode);
            Assert.AreEqual(userDefinedTextFrae.Description, "ABCD");
            Assert.AreEqual(userDefinedTextFrae.Value, "EFGH");
        }

        [Test]
        public void UserDefinedTextFrameUTF8Test()
        {
            var frames = new byte[]
                             {
                                 // TXXX
                                 0x54, 0x58, 0x58, 0x58, 0x00, 0x00, 0x00, 0x13, 0x00, 0x00,
                                 // UTF 8
                                 0x03,
                                 // Data..
                                 0x41, 0x42, 0x43, 0x44,
                                 0x00, 0x45, 0x46, 0x47, 0x48
                             };

            var completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);
            var tagContainer = m_TagController.Decode(m_TagInfo);

            Assert.AreEqual(tagContainer.Count, 1);

            var userDefinedTextFrame = FrameUtils.ConvertToUserDefinedText(tagContainer[0]);
            Assert.AreEqual(userDefinedTextFrame.Type, FrameType.UserDefinedText);
            Assert.AreEqual(userDefinedTextFrame.TextEncoding.CodePage, Encoding.UTF8.CodePage);
            Assert.AreEqual(userDefinedTextFrame.Description, "ABCD");
            Assert.AreEqual(userDefinedTextFrame.Value, "EFGH");
        }
    }
}