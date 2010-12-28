using System.Text;
using Id3Tag.HighLevel;
using Id3Tag.HighLevel.Id3Frame;
using Id3Tag.LowLevel;
using NUnit.Framework;

namespace Id3Tag.Net.NUnit.Highlevel.FrameTest
{
    [TestFixture]
    public class UserDefinedTextFrameTest : Test
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
        public void Convert_ISO8859_1_Test()
        {
            var frame = new UserDefinedTextFrame();
            frame.TextEncoding = Encoding.Default;
            frame.Descriptor.Id = "TXXX";
            frame.Description = "ABCD";
            frame.Value = "EFGH";

            RawFrame rawFrame = frame.Convert(TagVersion.Id3V23);

            Assert.AreEqual(rawFrame.Id, "TXXX");
            Assert.AreEqual(rawFrame.Payload.Count, 10);
            Assert.AreEqual(rawFrame.Payload[0], 0); // ISO coding
        }

        [Test]
        public void Convert_UTF16BE_Test()
        {
            var frame = new UserDefinedTextFrame();
            frame.TextEncoding = Encoding.BigEndianUnicode;
            frame.Descriptor.Id = "TXXX";
            frame.Description = "ABCD";
            frame.Value = "EFGH";

            RawFrame rawFrame = frame.Convert(TagVersion.Id3V23);

            Assert.AreEqual(rawFrame.Id, "TXXX");

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
        public void Convert_UTF16_Test()
        {
            var frame = new UserDefinedTextFrame();
            frame.TextEncoding = Encoding.Unicode;
            frame.Descriptor.Id = "TXXX";
            frame.Description = "ABCD";
            frame.Value = "EFGH";

            RawFrame rawFrame = frame.Convert(TagVersion.Id3V23);

            Assert.AreEqual(rawFrame.Id, "TXXX");
            Assert.AreEqual(rawFrame.Payload[0], 1); // ISO coding
            Assert.AreEqual(rawFrame.Payload[1], 0xFF); // BOM 1
            Assert.AreEqual(rawFrame.Payload[2], 0xFE); // BOM 2
            Assert.AreEqual(rawFrame.Payload.Count, 21);
        }

        [Test]
        public void Convert_UTF8_Test()
        {
            var frame = new UserDefinedTextFrame();
            frame.TextEncoding = Encoding.UTF8;
            frame.Descriptor.Id = "TXXX";
            frame.Description = "ABCD";
            frame.Value = "EFGH";

            RawFrame rawFrame = frame.Convert(TagVersion.Id3V23);

            Assert.AreEqual(rawFrame.Id, "TXXX");

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

            Assert.AreEqual(frame.Descriptor.Id, "TXXX");
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

            byte[] completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            TagContainer tagContainer = m_TagController.Decode(m_TagInfo);
            TagDescriptorV3 tag = tagContainer.GetId3V23Descriptor();

            Assert.AreEqual(tag.MajorVersion, 3);
            Assert.AreEqual(tag.Revision, 0);
            Assert.AreEqual(tagContainer.Count, 1);

            IFrame f1 = tagContainer[0];
            UserDefinedTextFrame userDefined1 = FrameUtilities.ConvertToUserDefinedText(f1);

            Assert.AreEqual(userDefined1.Descriptor.Id, "TXXX");
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

            byte[] completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);
            TagContainer tagContainer = m_TagController.Decode(m_TagInfo);

            Assert.AreEqual(tagContainer.Count, 1);

            UserDefinedTextFrame userDefinedTextFrame = FrameUtilities.ConvertToUserDefinedText(tagContainer[0]);
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

            byte[] completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);
            TagContainer tagContainer = m_TagController.Decode(m_TagInfo);

            Assert.AreEqual(tagContainer.Count, 1);

            UserDefinedTextFrame userDefinedTextFrae = FrameUtilities.ConvertToUserDefinedText(tagContainer[0]);
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

            byte[] completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);
            TagContainer tagContainer = m_TagController.Decode(m_TagInfo);

            Assert.AreEqual(tagContainer.Count, 1);

            UserDefinedTextFrame userDefinedTextFrae = FrameUtilities.ConvertToUserDefinedText(tagContainer[0]);
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
                                 0x54, 0x58, 0x58, 0x58, 0x00, 0x00, 0x00, 0x0A, 0x00, 0x00,
                                 // UTF 8
                                 0x03,
                                 // Data..
                                 0x41, 0x42, 0x43, 0x44,
                                 0x00, 0x45, 0x46, 0x47, 0x48
                             };

            byte[] completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);
            TagContainer tagContainer = m_TagController.Decode(m_TagInfo);

            Assert.AreEqual(tagContainer.Count, 1);

            UserDefinedTextFrame userDefinedTextFrame = FrameUtilities.ConvertToUserDefinedText(tagContainer[0]);
            Assert.AreEqual(userDefinedTextFrame.Type, FrameType.UserDefinedText);
            Assert.AreEqual(userDefinedTextFrame.TextEncoding.CodePage, Encoding.UTF8.CodePage);
            Assert.AreEqual(userDefinedTextFrame.Description, "ABCD");
            Assert.AreEqual(userDefinedTextFrame.Value, "EFGH");
        }
    }
}