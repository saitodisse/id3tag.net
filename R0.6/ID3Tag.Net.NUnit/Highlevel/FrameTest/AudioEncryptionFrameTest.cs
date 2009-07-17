using ID3Tag.Factory;
using ID3Tag.HighLevel;
using ID3Tag.HighLevel.ID3Frame;
using NUnit.Framework;

namespace ID3Tag.Net.NUnit.Highlevel.FrameTest
{
    [TestFixture]
    public class AudioEncryptionFrameTest : Test
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
        public void ConvertTest()
        {
            var bytes = new byte[] {0x20, 0x21, 0x22};
            var frame = new AudioEncryptionFrame("ABCD", 0x00FF, 0xFF00, bytes);

            Assert.AreEqual(frame.Descriptor.ID, "AENC");
            Assert.AreEqual(frame.Owner, "ABCD");
            Assert.AreEqual(frame.PreviewStart, 0x00FF);
            Assert.AreEqual(frame.PreviewLength, 0xFF00);
            Assert.IsTrue(ComparePayload(bytes, frame.Encryption));

            var refBytes = new byte[]
                               {
                                   0x41, 0x42, 0x43, 0x44, 0x00, 0x00, 0xFF, 0xFF, 0x00, 0x20, 0x21, 0x22
                               };

            var rawFrame = frame.Convert();

            Assert.AreEqual(rawFrame.ID, "AENC");
            Assert.IsTrue(ComparePayload(rawFrame.Payload, refBytes));
        }

        [Test]
        public void CreateTest()
        {
            var bytes = new byte[] {0x20, 0x21, 0x22};
            var frame = new AudioEncryptionFrame("ABCD", 0, 0xFF, bytes);

            Assert.AreEqual(frame.Descriptor.ID, "AENC");
            Assert.AreEqual(frame.Owner, "ABCD");
            Assert.AreEqual(frame.PreviewStart, 0);
            Assert.AreEqual(frame.PreviewLength, 0xFF);
            Assert.IsTrue(ComparePayload(bytes, frame.Encryption));
        }

        [Test]
        public void CreateTest2()
        {
            var frame = new AudioEncryptionFrame();
            Assert.AreEqual(frame.Descriptor.ID, "AENC");
            Assert.IsNotNull(frame.Owner);
            Assert.IsNotNull(frame.Encryption);
        }

        [Test]
        public void ImportTest()
        {
            var refBytes = new byte[]
                               {
                                   0x41, 0x45, 0x4E, 0x43, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00,
                                   0x41, 0x42, 0x43, 0x44, 0x00, 0x00, 0xFF, 0xFF, 0x00, 0x20, 0x21, 0x22
                               };

            var completeTag = GetCompleteV3Tag(refBytes);
            Read(completeTag);

            var tagContainer = m_TagController.Decode(m_TagInfo);
            Assert.AreEqual(tagContainer.Count, 1);

            var frame = FrameUtils.ConvertToAudioEncryptionFrame(tagContainer[0]);
            Assert.AreEqual(frame.Descriptor.ID, "AENC");
            Assert.AreEqual(frame.Owner, "ABCD");
            Assert.AreEqual(frame.PreviewStart, 0xFF);
            Assert.AreEqual(frame.PreviewLength, 0xFF00);

            Assert.AreEqual(frame.Encryption.Length, 3);
            Assert.AreEqual(frame.Encryption[0], 0x20);
            Assert.AreEqual(frame.Encryption[1], 0x21);
            Assert.AreEqual(frame.Encryption[2], 0x22);
        }
    }
}