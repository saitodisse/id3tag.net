using ID3Tag.HighLevel;
using ID3Tag.HighLevel.ID3Frame;
using NUnit.Framework;

namespace ID3Tag.Net.NUnit.Highlevel.FrameTest
{
    [TestFixture]
    public class PrivateFrameText : Test
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
            var privateFrame = new PrivateFrame
                                   {
                                       Descriptor = {ID = "PRIV"},
                                       Owner = "ABCD",
                                       Data = new byte[] {0x45, 0x46, 0x47, 0x48}
                                   };

            var rawFrame = privateFrame.Convert(TagVersion.Id3V23);

            Assert.AreEqual(rawFrame.ID, "PRIV");
            Assert.AreEqual(rawFrame.Payload.Length, 9);

            var refBytes = new byte[] {0x41, 0x42, 0x43, 0x44, 0x00, 0x45, 0x46, 0x47, 0x48};
            Assert.IsTrue(ComparePayload(rawFrame.Payload, refBytes));
        }

        [Test]
        public void CreateTest()
        {
            var frame = new PrivateFrame("ABCD", new byte[] {0xA0, 0xB0});

            Assert.AreEqual(frame.Descriptor.ID, "PRIV");
            Assert.AreEqual(frame.Owner, "ABCD");
            Assert.AreEqual(frame.Data.Length, 2);
            Assert.AreEqual(frame.Data[0], 0xA0);
            Assert.AreEqual(frame.Data[1], 0xB0);
        }

        [Test]
        public void NullBytesProblemTest()
        {
            var frames = new byte[]
                             {
                                 // PRIV
                                 0x50, 0x52, 0x49, 0x56, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00,
                                 0x41, 0x42, 0x43, 0x44, 0x00, 0x45, 0x46, 0x47, 0x48, 0x00,
                                 0x00, 0x00, 0x00, 0x00, 0x00
                             };

            var completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            var tagContainer = m_TagController.Decode(m_TagInfo);
            Assert.AreEqual(tagContainer.Count, 1);

            var privFrame = FrameUtils.ConvertToPrivateFrame(tagContainer[0]);
            Assert.AreEqual(privFrame.Descriptor.ID, "PRIV");
            Assert.AreEqual(privFrame.Owner, "ABCD");
            Assert.AreEqual(privFrame.Data.Length, 4);
            Assert.AreEqual(privFrame.Data[0], 0x45);
            Assert.AreEqual(privFrame.Data[1], 0x46);
            Assert.AreEqual(privFrame.Data[2], 0x47);
            Assert.AreEqual(privFrame.Data[3], 0x48);
        }

        [Test]
        public void PrivateFrameDetectionTest1()
        {
            var frames = new byte[]
                             {
                                 // PRIV
                                 0x50, 0x52, 0x49, 0x56, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00,
                                 0x41, 0x42, 0x43, 0x44, 0x00, 0x45, 0x46, 0x47, 0x48
                             };

            var completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            var tagContainer = m_TagController.Decode(m_TagInfo);
            Assert.AreEqual(tagContainer.Count, 1);

            var privFrame = FrameUtils.ConvertToPrivateFrame(tagContainer[0]);
            Assert.AreEqual(privFrame.Descriptor.ID, "PRIV");
            Assert.AreEqual(privFrame.Owner, "ABCD");
            Assert.AreEqual(privFrame.Data.Length, 4);
            Assert.AreEqual(privFrame.Data[0], 0x45);
            Assert.AreEqual(privFrame.Data[1], 0x46);
            Assert.AreEqual(privFrame.Data[2], 0x47);
            Assert.AreEqual(privFrame.Data[3], 0x48);
        }
    }
}