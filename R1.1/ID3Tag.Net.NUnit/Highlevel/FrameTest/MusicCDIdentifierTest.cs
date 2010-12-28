using Id3Tag.HighLevel;
using Id3Tag.HighLevel.Id3Frame;
using Id3Tag.LowLevel;
using NUnit.Framework;

namespace Id3Tag.Net.NUnit.Highlevel.FrameTest
{
    [TestFixture]
    public class MusicCDIdentifierTest : Test
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
        public void ConvertTest()
        {
            var refBytes = new byte[] {0x41, 0x42, 0x43, 0x44};

            var idFrame = new MusicCDIdentifierFrame();
            idFrame.SetToc(refBytes);
            idFrame.Descriptor.Id = "MCDI";

            RawFrame rawFrame = idFrame.Convert(TagVersion.Id3V23);
            Assert.AreEqual(rawFrame.Id, "MCDI");
            Assert.IsTrue(ComparePayload(rawFrame.Payload, refBytes));
        }

        [Test]
        public void CreateTest()
        {
            var frame = new MusicCDIdentifierFrame(new byte[] {0xA0, 0xB0});

            Assert.AreEqual(frame.Descriptor.Id, "MCDI");
            Assert.AreEqual(frame.Toc.Count, 2);
            Assert.AreEqual(frame.Toc[0], 0xA0);
            Assert.AreEqual(frame.Toc[1], 0xB0);
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

            byte[] completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            TagContainer tagContainer = m_TagController.Decode(m_TagInfo);
            Assert.AreEqual(tagContainer.Count, 1);

            MusicCDIdentifierFrame musicIdFrame = FrameUtilities.ConvertToMusicCDIdentifierFrame(tagContainer[0]);
            Assert.AreEqual(musicIdFrame.Descriptor.Id, "MCDI");

            var refBytes = new byte[] {0x41, 0x42, 0x43, 0x44};
            Assert.IsTrue(ComparePayload(musicIdFrame.Toc, refBytes));
        }
    }
}