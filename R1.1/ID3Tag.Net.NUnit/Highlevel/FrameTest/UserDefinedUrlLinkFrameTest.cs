using System.Text;
using Id3Tag.HighLevel;
using Id3Tag.HighLevel.Id3Frame;
using NUnit.Framework;

namespace Id3Tag.Net.NUnit.Highlevel.FrameTest
{
    [TestFixture]
    public class UserDefinedUrlLinkFrameTest : Test
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
        public void ConverterISO88591_Test()
        {
            var frame = new UserDefinedUrlLinkFrame();

            frame.Description = "ABCD";
            frame.TextEncoding = Encoding.Default;
            frame.Url = "EFGH";
            frame.Descriptor.Id = "WXXX";

            var refPayloadBytes = new byte[] {0x00, 0x41, 0x42, 0x43, 0x44, 0x00, 0x45, 0x46, 0x47, 0x48};
            var rawFrame = frame.Convert(TagVersion.Id3V23);
            Assert.AreEqual(rawFrame.Id, "WXXX");
            Assert.AreEqual(rawFrame.Payload, refPayloadBytes);
        }

        [Test]
        public void ConverterUTF16_Test()
        {
            var frame = new UserDefinedUrlLinkFrame();

            frame.Description = "ABCD";
            frame.TextEncoding = Encoding.Unicode;
            frame.Url = "EFGH";
            frame.Descriptor.Id = "WXXX";

            var refPayloadBytes = new byte[]
                                      {
                                          0x01, 0xFF, 0xFE,
                                          0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x44, 0x00, 0x00, 0x00,
                                          0x45, 0x46, 0x47, 0x48
                                      };
            var rawFrame = frame.Convert(TagVersion.Id3V23);
            Assert.AreEqual(rawFrame.Id, "WXXX");
            Assert.AreEqual(rawFrame.Payload, refPayloadBytes);
        }

        [Test]
        public void ConverterUTF16BE_Test()
        {
            var frame = new UserDefinedUrlLinkFrame();

            frame.Description = "ABCD";
            frame.TextEncoding = Encoding.BigEndianUnicode;
            frame.Url = "EFGH";
            frame.Descriptor.Id = "WXXX";

            var refPayloadBytes = new byte[]
                                      {
                                          0x02,
                                          0x00, 0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x44, 0x00, 0x00,
                                          0x45, 0x46, 0x47, 0x48
                                      };
            var rawFrame = frame.Convert(TagVersion.Id3V23);
            Assert.AreEqual(rawFrame.Id, "WXXX");
            Assert.AreEqual(rawFrame.Payload, refPayloadBytes);
        }

        [Test]
        public void ConverterUTF8_Test()
        {
            var frame = new UserDefinedUrlLinkFrame();

            frame.Description = "ABCD";
            frame.TextEncoding = Encoding.UTF8;
            frame.Url = "EFGH";
            frame.Descriptor.Id = "WXXX";

            var refPayloadBytes = new byte[]
                                      {
                                          0x03,
                                          0x41, 0x42, 0x43, 0x44, 0x00,
                                          0x45, 0x46, 0x47, 0x48
                                      };
            var rawFrame = frame.Convert(TagVersion.Id3V23);
            Assert.AreEqual(rawFrame.Id, "WXXX");
            Assert.AreEqual(rawFrame.Payload, refPayloadBytes);
        }

        [Test]
        public void CreateTest()
        {
            var frame = new UserDefinedUrlLinkFrame("ABCD", "EFGH", Encoding.Unicode);

            Assert.AreEqual(frame.Descriptor.Id, "WXXX");
            Assert.AreEqual(frame.Description, "ABCD");
            Assert.AreEqual(frame.Url, "EFGH");
            Assert.AreEqual(frame.TextEncoding.CodePage, Encoding.Unicode.CodePage);
        }

        [Test]
        public void UserDefinedURLLinkDetection_UTF16BE_Test()
        {
            var frames = new byte[]
                             {
                                 // WXXX
                                 0x57, 0x58, 0x58, 0x58, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00,
                                 0x02,
                                 0x00, 0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x44, 0x00, 0x00,
                                 0x45, 0x46, 0x47, 0x48
                             };

            var completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            var tagContainer = m_TagController.Decode(m_TagInfo);
            var tag = tagContainer.GetId3V23Descriptor();

            Assert.AreEqual(tag.MajorVersion, 3);
            Assert.AreEqual(tag.Revision, 0);
            Assert.AreEqual(tagContainer.Count, 1);

            var f1 = tagContainer[0];
            var text1 = FrameUtilities.ConvertToUserDefinedUrlLinkFrame(f1);

            Assert.AreEqual(text1.Descriptor.Id, "WXXX");
            Assert.AreEqual(text1.TextEncoding.CodePage, Encoding.BigEndianUnicode.CodePage);
            Assert.AreEqual(text1.Type, FrameType.UserDefinedUrlLink);
            Assert.AreEqual(text1.Description, "ABCD");
            Assert.AreEqual(text1.Url, "EFGH");
        }

        [Test]
        public void UserDefinedURLLinkDetection_UTF8_Test()
        {
            var frames = new byte[]
                             {
                                 // WXXX
                                 0x57, 0x58, 0x58, 0x58, 0x00, 0x00, 0x00, 0x0A, 0x00, 0x00,
                                 0x03,
                                 0x41, 0x42, 0x43, 0x44, 0x00,
                                 0x45, 0x46, 0x47, 0x48
                             };

            var completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            var tagContainer = m_TagController.Decode(m_TagInfo);
            var tag = tagContainer.GetId3V23Descriptor();

            Assert.AreEqual(tag.MajorVersion, 3);
            Assert.AreEqual(tag.Revision, 0);
            Assert.AreEqual(tagContainer.Count, 1);

            var f1 = tagContainer[0];
            var text1 = FrameUtilities.ConvertToUserDefinedUrlLinkFrame(f1);

            Assert.AreEqual(text1.Descriptor.Id, "WXXX");
            Assert.AreEqual(text1.TextEncoding.CodePage, Encoding.UTF8.CodePage);
            Assert.AreEqual(text1.Type, FrameType.UserDefinedUrlLink);
            Assert.AreEqual(text1.Description, "ABCD");
            Assert.AreEqual(text1.Url, "EFGH");
        }

        [Test]
        public void UserDefinedURLLinkDetectionTest1()
        {
            var frames = new byte[]
                             {
                                 // WXXX
                                 0x57, 0x58, 0x58, 0x58, 0x00, 0x00, 0x00, 0x0A, 0x00, 0x00,
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
            var text1 = FrameUtilities.ConvertToUserDefinedUrlLinkFrame(f1);

            Assert.AreEqual(text1.Descriptor.Id, "WXXX");
            Assert.AreEqual(text1.TextEncoding.CodePage, Encoding.Default.CodePage);
            Assert.AreEqual(text1.Type, FrameType.UserDefinedUrlLink);
            Assert.AreEqual(text1.Description, "ABCD");
            Assert.AreEqual(text1.Url, "EFGH");
        }

        [Test]
        public void UserDefinedURLLinkDetectionTest1_BigEndian()
        {
            var frames = new byte[]
                             {
                                 // WXXX
                                 0x57, 0x58, 0x58, 0x58, 0x00, 0x00, 0x00, 0x011, 0x00, 0x00,
                                 0x01, 0xFE, 0xFF,
                                 0x00, 0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x44, 0x00, 0x00,
                                 0x45, 0x46, 0x47, 0x48
                             };

            var completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            var tagContainer = m_TagController.Decode(m_TagInfo);
            var tag = tagContainer.GetId3V23Descriptor();

            Assert.AreEqual(tag.MajorVersion, 3);
            Assert.AreEqual(tag.Revision, 0);
            Assert.AreEqual(tagContainer.Count, 1);

            var f1 = tagContainer[0];
            var text1 = FrameUtilities.ConvertToUserDefinedUrlLinkFrame(f1);

            Assert.AreEqual(text1.Descriptor.Id, "WXXX");
            Assert.AreEqual(text1.TextEncoding.CodePage, Encoding.BigEndianUnicode.CodePage);
            Assert.AreEqual(text1.Type, FrameType.UserDefinedUrlLink);
            Assert.AreEqual(text1.Description, "ABCD");
            Assert.AreEqual(text1.Url, "EFGH");
        }

        [Test]
        public void UserDefinedURLLinkDetectionTest1_LittleEndian()
        {
            var frames = new byte[]
                             {
                                 // WXXX
                                 0x57, 0x58, 0x58, 0x58, 0x00, 0x00, 0x00, 0x011, 0x00, 0x00,
                                 0x01, 0xFF, 0xFE,
                                 0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x44, 0x00, 0x00, 0x00,
                                 0x45, 0x46, 0x47, 0x48
                             };

            var completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            var tagContainer = m_TagController.Decode(m_TagInfo);
            var tag = tagContainer.GetId3V23Descriptor();

            Assert.AreEqual(tag.MajorVersion, 3);
            Assert.AreEqual(tag.Revision, 0);
            Assert.AreEqual(tagContainer.Count, 1);

            var f1 = tagContainer[0];
            var text1 = FrameUtilities.ConvertToUserDefinedUrlLinkFrame(f1);

            Assert.AreEqual(text1.Descriptor.Id, "WXXX");
            Assert.AreEqual(text1.TextEncoding.CodePage, Encoding.Unicode.CodePage);
            Assert.AreEqual(text1.Type, FrameType.UserDefinedUrlLink);
            Assert.AreEqual(text1.Description, "ABCD");
            Assert.AreEqual(text1.Url, "EFGH");
        }
    }
}