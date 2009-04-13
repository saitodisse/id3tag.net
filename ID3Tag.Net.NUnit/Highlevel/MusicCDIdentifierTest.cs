using ID3Tag.Factory;
using ID3Tag.HighLevel;
using ID3Tag.HighLevel.ID3Frame;
using NUnit.Framework;

namespace ID3Tag.Net.NUnit.Highlevel
{
    [TestFixture]
    public class MusicCDIdentifierTest : Test
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
            var refBytes = new byte[] {0x41, 0x42, 0x43, 0x44};

            var idFrame = new MusicCdIdentifierFrame();
            idFrame.TOC = refBytes;
            idFrame.Descriptor.ID = "MCDI";

            var rawFrame = idFrame.Convert();
            Assert.AreEqual(rawFrame.ID, "MCDI");
            Assert.IsTrue(ComparePayload(rawFrame.Payload, refBytes));
        }

        [Test]
        public void CreateTest()
        {
            var frame = new MusicCdIdentifierFrame(new byte[] {0xA0, 0xB0});

            Assert.AreEqual(frame.Descriptor.ID, "MCDI");
            Assert.AreEqual(frame.TOC.Length, 2);
            Assert.AreEqual(frame.TOC[0], 0xA0);
            Assert.AreEqual(frame.TOC[1], 0xB0);
        }

        [Test]
        public void MusicCDIdentiferDetectionTest()
        {
            var frames = new byte[]
                             {
                                 // MCDI
                                 0x4D, 0x43, 0x44, 0x49, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00,
                                 0x41, 0x42, 0x43, 0x44
                             };

            var completeTag = GetCompleteTag(frames);
            Read(completeTag);

            var tagContainer = m_TagController.Decode(m_TagInfo);
            Assert.AreEqual(tagContainer.Count, 1);

            var musicIdFrame = FrameUtils.ConvertToMusicCDIdentifierFrame(tagContainer[0]);
            Assert.AreEqual(musicIdFrame.Descriptor.ID, "MCDI");

            var refBytes = new byte[] {0x41, 0x42, 0x43, 0x44};
            Assert.IsTrue(ComparePayload(musicIdFrame.TOC, refBytes));
        }
    }
}