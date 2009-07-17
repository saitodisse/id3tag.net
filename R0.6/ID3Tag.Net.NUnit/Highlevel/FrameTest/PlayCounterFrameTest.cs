using ID3Tag.Factory;
using ID3Tag.HighLevel;
using ID3Tag.HighLevel.ID3Frame;
using NUnit.Framework;

namespace ID3Tag.Net.NUnit.Highlevel.FrameTest
{
    [TestFixture]
    public class PlayCounterFrameTest : Test
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
        public void ConvertTest1()
        {
            var playCounterFrame = new PlayCounterFrame();
            Assert.AreEqual(playCounterFrame.Descriptor.ID, "PCNT");
            Assert.AreEqual(playCounterFrame.Counter, 0);

            var rawFrame = playCounterFrame.Convert();
            Assert.AreEqual(rawFrame.ID, "PCNT");

            var payloadBytes = rawFrame.Payload;
            var refBytes = new byte[] {0x00, 0x00, 0x00, 0x00};

            Assert.IsTrue(ComparePayload(refBytes, payloadBytes));
        }

        [Test]
        public void ConvertTest2()
        {
            const long counterValue = 1;

            var playCounterFrame = new PlayCounterFrame(counterValue);
            Assert.AreEqual(playCounterFrame.Descriptor.ID, "PCNT");
            Assert.AreEqual(playCounterFrame.Counter, counterValue);

            var rawFrame = playCounterFrame.Convert();
            Assert.AreEqual(rawFrame.ID, "PCNT");

            var payloadBytes = rawFrame.Payload;
            var refBytes = new byte[] {0x00, 0x00, 0x00, 0x01};

            Assert.IsTrue(ComparePayload(refBytes, payloadBytes));
        }

        [Test]
        public void ConvertTest3()
        {
            const long counterValue = 0xFFFF;

            var playCounterFrame = new PlayCounterFrame(counterValue);
            Assert.AreEqual(playCounterFrame.Descriptor.ID, "PCNT");
            Assert.AreEqual(playCounterFrame.Counter, counterValue);

            var rawFrame = playCounterFrame.Convert();
            Assert.AreEqual(rawFrame.ID, "PCNT");

            var payloadBytes = rawFrame.Payload;
            var refBytes = new byte[] {0x00, 0x00, 0xFF, 0xFF};

            Assert.IsTrue(ComparePayload(refBytes, payloadBytes));
        }

        [Test]
        public void ConvertTest4()
        {
            const long counterValue = 0x01FFFF;

            var playCounterFrame = new PlayCounterFrame(counterValue);
            Assert.AreEqual(playCounterFrame.Descriptor.ID, "PCNT");
            Assert.AreEqual(playCounterFrame.Counter, counterValue);

            var rawFrame = playCounterFrame.Convert();
            Assert.AreEqual(rawFrame.ID, "PCNT");

            var payloadBytes = rawFrame.Payload;
            var refBytes = new byte[] {0x00, 0x01, 0xFF, 0xFF};

            Assert.IsTrue(ComparePayload(refBytes, payloadBytes));
        }

        [Test]
        public void ConvertTest5()
        {
            const long counterValue = 0xFFFFFFFF;

            var playCounterFrame = new PlayCounterFrame(counterValue);
            Assert.AreEqual(playCounterFrame.Descriptor.ID, "PCNT");
            Assert.AreEqual(playCounterFrame.Counter, counterValue);

            var rawFrame = playCounterFrame.Convert();
            Assert.AreEqual(rawFrame.ID, "PCNT");

            var payloadBytes = rawFrame.Payload;
            var refBytes = new byte[] {0xFF, 0xFF, 0xFF, 0xFF};

            Assert.IsTrue(ComparePayload(refBytes, payloadBytes));
        }

        [Test]
        public void ConvertTest6()
        {
            const long counterValue = 0x01FFFFFFFF;

            var playCounterFrame = new PlayCounterFrame(counterValue);
            Assert.AreEqual(playCounterFrame.Descriptor.ID, "PCNT");
            Assert.AreEqual(playCounterFrame.Counter, counterValue);

            var rawFrame = playCounterFrame.Convert();
            Assert.AreEqual(rawFrame.ID, "PCNT");

            var payloadBytes = rawFrame.Payload;
            var refBytes = new byte[] {0x01, 0xFF, 0xFF, 0xFF, 0xFF};

            Assert.IsTrue(ComparePayload(refBytes, payloadBytes));
        }

        [Test]
        public void ConvertTest7()
        {
            const long counterValue = 0x0100FFFFFF;

            var playCounterFrame = new PlayCounterFrame(counterValue);
            Assert.AreEqual(playCounterFrame.Descriptor.ID, "PCNT");
            Assert.AreEqual(playCounterFrame.Counter, counterValue);

            var rawFrame = playCounterFrame.Convert();
            Assert.AreEqual(rawFrame.ID, "PCNT");

            var payloadBytes = rawFrame.Payload;
            var refBytes = new byte[] {0x01, 0x00, 0xFF, 0xFF, 0xFF};

            Assert.IsTrue(ComparePayload(refBytes, payloadBytes));
        }

        [Test]
        public void ConvertTest8()
        {
            const long counterValue = 0x7FFFFFFFFFFFFFFF;

            // This must be a very very good song ;-)

            var playCounterFrame = new PlayCounterFrame(counterValue);
            Assert.AreEqual(playCounterFrame.Descriptor.ID, "PCNT");
            Assert.AreEqual(playCounterFrame.Counter, counterValue);

            var rawFrame = playCounterFrame.Convert();
            Assert.AreEqual(rawFrame.ID, "PCNT");

            var payloadBytes = rawFrame.Payload;
            var refBytes = new byte[] {0x7F, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF};

            Assert.IsTrue(ComparePayload(refBytes, payloadBytes));
        }

        [Test]
        public void ImportTest1()
        {
            var frames = new byte[]
                             {
                                 0x50, 0x43, 0x4E, 0x54, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00,
                                 0x00, 0x00, 0x00, 0x00
                             };

            var completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            var tagContainer = m_TagController.Decode(m_TagInfo);
            Assert.AreEqual(tagContainer.Count, 1);

            var playCounter = FrameUtils.ConvertToPlayCounterFrame(tagContainer[0]);

            Assert.AreEqual(playCounter.Descriptor.ID, "PCNT");
            Assert.AreEqual(playCounter.Counter, 0);
        }

        [Test]
        public void ImportTest2()
        {
            var frames = new byte[]
                             {
                                 0x50, 0x43, 0x4E, 0x54, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00,
                                 0x00, 0x00, 0x00, 0x01
                             };

            var completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            var tagContainer = m_TagController.Decode(m_TagInfo);
            Assert.AreEqual(tagContainer.Count, 1);

            var playCounter = FrameUtils.ConvertToPlayCounterFrame(tagContainer[0]);

            Assert.AreEqual(playCounter.Descriptor.ID, "PCNT");
            Assert.AreEqual(playCounter.Counter, 1);
        }

        [Test]
        public void ImportTest3()
        {
            var frames = new byte[]
                             {
                                 0x50, 0x43, 0x4E, 0x54, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00,
                                 0x00, 0x00, 0x00, 0x00, 0x01
                             };

            var completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            var tagContainer = m_TagController.Decode(m_TagInfo);
            Assert.AreEqual(tagContainer.Count, 1);

            var playCounter = FrameUtils.ConvertToPlayCounterFrame(tagContainer[0]);

            Assert.AreEqual(playCounter.Descriptor.ID, "PCNT");
            Assert.AreEqual(playCounter.Counter, 1);
        }

        [Test]
        public void ImportTest4()
        {
            var frames = new byte[]
                             {
                                 0x50, 0x43, 0x4E, 0x54, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00,
                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x01
                             };

            var completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            var tagContainer = m_TagController.Decode(m_TagInfo);
            Assert.AreEqual(tagContainer.Count, 1);

            var playCounter = FrameUtils.ConvertToPlayCounterFrame(tagContainer[0]);

            Assert.AreEqual(playCounter.Descriptor.ID, "PCNT");
            Assert.AreEqual(playCounter.Counter, 1);
        }

        [Test]
        public void ImportTest5()
        {
            var frames = new byte[]
                             {
                                 0x50, 0x43, 0x4E, 0x54, 0x00, 0x00, 0x00, 0x07, 0x00, 0x00,
                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01
                             };

            var completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            var tagContainer = m_TagController.Decode(m_TagInfo);
            Assert.AreEqual(tagContainer.Count, 1);

            var playCounter = FrameUtils.ConvertToPlayCounterFrame(tagContainer[0]);

            Assert.AreEqual(playCounter.Descriptor.ID, "PCNT");
            Assert.AreEqual(playCounter.Counter, 1);
        }

        [Test]
        public void ImportTest6()
        {
            var frames = new byte[]
                             {
                                 0x50, 0x43, 0x4E, 0x54, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00,
                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01
                             };

            var completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            var tagContainer = m_TagController.Decode(m_TagInfo);
            Assert.AreEqual(tagContainer.Count, 1);

            var playCounter = FrameUtils.ConvertToPlayCounterFrame(tagContainer[0]);

            Assert.AreEqual(playCounter.Descriptor.ID, "PCNT");
            Assert.AreEqual(playCounter.Counter, 1);
        }
    }
}