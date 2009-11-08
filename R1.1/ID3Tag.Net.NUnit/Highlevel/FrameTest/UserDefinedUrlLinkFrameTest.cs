using System.Text;
using ID3Tag.HighLevel;
using ID3Tag.HighLevel.ID3Frame;
using NUnit.Framework;

namespace ID3Tag.Net.NUnit.Highlevel.FrameTest
{
    [TestFixture]
    public class UserDefinedUrlLinkFrameTest : Test
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
        public void ConverterISO88591_Test()
        {
            var frame = new UserDefinedURLLinkFrame();

            frame.Description = "ABCD";
            frame.TextEncoding = Encoding.Default;
            frame.URL = "EFGH";
            frame.Descriptor.ID = "WXXX";

            var refPayloadBytes = new byte[] {0x00, 0x41, 0x42, 0x43, 0x44, 0x00, 0x45, 0x46, 0x47, 0x48};
            var rawFrame = frame.Convert(TagVersion.Id3V23);
            Assert.AreEqual(rawFrame.ID, "WXXX");
            Assert.AreEqual(rawFrame.Payload, refPayloadBytes);
        }

        [Test]
        public void ConverterUTF16_Test()
        {
            var frame = new UserDefinedURLLinkFrame();

            frame.Description = "ABCD";
            frame.TextEncoding = Encoding.Unicode;
            frame.URL = "EFGH";
            frame.Descriptor.ID = "WXXX";

            var refPayloadBytes = new byte[]
                                      {
                                          0x01, 0xFF, 0xFE,
                                          0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x44, 0x00, 0x00, 0x00,
                                          0x45, 0x46, 0x47, 0x48
                                      };
            var rawFrame = frame.Convert(TagVersion.Id3V23);
            Assert.AreEqual(rawFrame.ID, "WXXX");
            Assert.AreEqual(rawFrame.Payload, refPayloadBytes);
        }

        [Test]
        public void ConverterUTF16BE_Test()
        {
            var frame = new UserDefinedURLLinkFrame();

            frame.Description = "ABCD";
            frame.TextEncoding = Encoding.BigEndianUnicode;
            frame.URL = "EFGH";
            frame.Descriptor.ID = "WXXX";

            var refPayloadBytes = new byte[]
                                      {
                                          0x02,
                                          0x00, 0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x44, 0x00, 0x00,
                                          0x45, 0x46, 0x47, 0x48
                                      };
            var rawFrame = frame.Convert(TagVersion.Id3V23);
            Assert.AreEqual(rawFrame.ID, "WXXX");
            Assert.AreEqual(rawFrame.Payload, refPayloadBytes);
        }

        [Test]
        public void ConverterUTF8_Test()
        {
            var frame = new UserDefinedURLLinkFrame();

            frame.Description = "ABCD";
            frame.TextEncoding = Encoding.UTF8;
            frame.URL = "EFGH";
            frame.Descriptor.ID = "WXXX";

            var refPayloadBytes = new byte[]
                                      {
                                          0x03,
                                          0x41, 0x42, 0x43, 0x44, 0x00,
                                          0x45, 0x46, 0x47, 0x48
                                      };
            var rawFrame = frame.Convert(TagVersion.Id3V23);
            Assert.AreEqual(rawFrame.ID, "WXXX");
            Assert.AreEqual(rawFrame.Payload, refPayloadBytes);
        }

        [Test]
        public void CreateTest()
        {
            var frame = new UserDefinedURLLinkFrame("ABCD", "EFGH", Encoding.Unicode);

            Assert.AreEqual(frame.Descriptor.ID, "WXXX");
            Assert.AreEqual(frame.Description, "ABCD");
            Assert.AreEqual(frame.URL, "EFGH");
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
            var text1 = FrameUtils.ConvertToUserDefinedURLLinkFrame(f1);

            Assert.AreEqual(text1.Descriptor.ID, "WXXX");
            Assert.AreEqual(text1.TextEncoding.CodePage, Encoding.BigEndianUnicode.CodePage);
            Assert.AreEqual(text1.Type, FrameType.UserDefindedURLLink);
            Assert.AreEqual(text1.Description, "ABCD");
            Assert.AreEqual(text1.URL, "EFGH");
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
            var text1 = FrameUtils.ConvertToUserDefinedURLLinkFrame(f1);

            Assert.AreEqual(text1.Descriptor.ID, "WXXX");
            Assert.AreEqual(text1.TextEncoding.CodePage, Encoding.UTF8.CodePage);
            Assert.AreEqual(text1.Type, FrameType.UserDefindedURLLink);
            Assert.AreEqual(text1.Description, "ABCD");
            Assert.AreEqual(text1.URL, "EFGH");
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
            var text1 = FrameUtils.ConvertToUserDefinedURLLinkFrame(f1);

            Assert.AreEqual(text1.Descriptor.ID, "WXXX");
            Assert.AreEqual(text1.TextEncoding.CodePage, Encoding.Default.CodePage);
            Assert.AreEqual(text1.Type, FrameType.UserDefindedURLLink);
            Assert.AreEqual(text1.Description, "ABCD");
            Assert.AreEqual(text1.URL, "EFGH");
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
            var text1 = FrameUtils.ConvertToUserDefinedURLLinkFrame(f1);

            Assert.AreEqual(text1.Descriptor.ID, "WXXX");
            Assert.AreEqual(text1.TextEncoding.CodePage, Encoding.BigEndianUnicode.CodePage);
            Assert.AreEqual(text1.Type, FrameType.UserDefindedURLLink);
            Assert.AreEqual(text1.Description, "ABCD");
            Assert.AreEqual(text1.URL, "EFGH");
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
            var text1 = FrameUtils.ConvertToUserDefinedURLLinkFrame(f1);

            Assert.AreEqual(text1.Descriptor.ID, "WXXX");
            Assert.AreEqual(text1.TextEncoding.CodePage, Encoding.Unicode.CodePage);
            Assert.AreEqual(text1.Type, FrameType.UserDefindedURLLink);
            Assert.AreEqual(text1.Description, "ABCD");
            Assert.AreEqual(text1.URL, "EFGH");
        }
    }
}