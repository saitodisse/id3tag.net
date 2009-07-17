using ID3Tag.Factory;
using ID3Tag.HighLevel;
using ID3Tag.HighLevel.ID3Frame;
using NUnit.Framework;

namespace ID3Tag.Net.NUnit.Highlevel.FrameTest
{
    [TestFixture]
    public class PopularitymeterFrameTest : Test
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
            var popFrame = new PopularimeterFrame("ABC", 0xF, 0xFF);

            var rawFrame = popFrame.Convert();

            Assert.AreEqual(rawFrame.ID, "POPM");
            var payload = rawFrame.Payload;
            var refBytes = new byte[] {0x41, 0x42, 0x43, 0x00, 0x0F, 0x00, 0x00, 0x00, 0xFF};

            Assert.IsTrue(ComparePayload(refBytes, payload));
        }

        [Test]
        public void ConvertTest2()
        {
            var popFrame = new PopularimeterFrame("ABC", 0xF, 0xFFFF);

            var rawFrame = popFrame.Convert();

            Assert.AreEqual(rawFrame.ID, "POPM");
            var payload = rawFrame.Payload;
            var refBytes = new byte[] {0x41, 0x42, 0x43, 0x00, 0x0F, 0x00, 0x00, 0xFF, 0xFF};

            Assert.IsTrue(ComparePayload(refBytes, payload));
        }

        [Test]
        public void ConvertTest3()
        {
            var popFrame = new PopularimeterFrame("ABC", 0xF, 0xFFFFFF);

            var rawFrame = popFrame.Convert();

            Assert.AreEqual(rawFrame.ID, "POPM");
            var payload = rawFrame.Payload;
            var refBytes = new byte[] {0x41, 0x42, 0x43, 0x00, 0x0F, 0x00, 0xFF, 0xFF, 0xFF};

            Assert.IsTrue(ComparePayload(refBytes, payload));
        }

        [Test]
        public void ConvertTest4()
        {
            var popFrame = new PopularimeterFrame("ABC", 0xF, 0x7FFFFFFF);

            var rawFrame = popFrame.Convert();

            Assert.AreEqual(rawFrame.ID, "POPM");
            var payload = rawFrame.Payload;
            var refBytes = new byte[] {0x41, 0x42, 0x43, 0x00, 0x0F, 0x7F, 0xFF, 0xFF, 0xFF};

            Assert.IsTrue(ComparePayload(refBytes, payload));
        }

        [Test]
        public void CreateTest()
        {
            var popFrame = new PopularimeterFrame();

            Assert.AreEqual(popFrame.Descriptor.ID, "POPM");
            Assert.IsNotNull(popFrame.eMail);
            Assert.AreEqual(popFrame.PlayCounter, 0);
            Assert.AreEqual(popFrame.Rating, 0);
        }

        [Test]
        public void CreateTest2()
        {
            const string mail = "Mail";
            const int playCounter = 3;
            const int rating = 0xFF;

            var popFrame = new PopularimeterFrame(mail, rating, playCounter);

            Assert.AreEqual(popFrame.Descriptor.ID, "POPM");
            Assert.AreEqual(popFrame.eMail, mail);
            Assert.AreEqual(popFrame.PlayCounter, playCounter);
            Assert.AreEqual(popFrame.Rating, rating);
        }

        [Test]
        public void CreateTest3()
        {
            var popFrame = new PopularimeterFrame("", 0xF, 0x7FFFFFFF);

            var rawFrame = popFrame.Convert();

            Assert.AreEqual(rawFrame.ID, "POPM");
            var payload = rawFrame.Payload;
            var refBytes = new byte[] {0x00, 0x0F, 0x7F, 0xFF, 0xFF, 0xFF};

            Assert.IsTrue(ComparePayload(refBytes, payload));
        }

        [Test]
        public void ImportTest1()
        {
            var frames = new byte[]
                             {
                                 0x50, 0x4F, 0x50, 0x4D, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00,
                                 0x41, 0x42, 0x43, 0x00, 0x0F, 0x00, 0x00, 0x00, 0xFF
                             };

            var completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            var tagContainer = m_TagController.Decode(m_TagInfo);
            Assert.AreEqual(tagContainer.Count, 1);

            var pop = FrameUtils.ConvertToPopularimeterFrame(tagContainer[0]);

            Assert.AreEqual(pop.Descriptor.ID, "POPM");
            Assert.AreEqual(pop.eMail, "ABC");
            Assert.AreEqual(pop.Rating, 15);
            Assert.AreEqual(pop.PlayCounter, 0xFF);
        }

        [Test]
        public void ImportTest2()
        {
            var frames = new byte[]
                             {
                                 0x50, 0x4F, 0x50, 0x4D, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00,
                                 0x00, 0x0F, 0x00, 0x00, 0x00, 0xFF
                             };

            var completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            var tagContainer = m_TagController.Decode(m_TagInfo);
            Assert.AreEqual(tagContainer.Count, 1);

            var pop = FrameUtils.ConvertToPopularimeterFrame(tagContainer[0]);

            Assert.AreEqual(pop.Descriptor.ID, "POPM");
            Assert.AreEqual(pop.eMail, "");
            Assert.AreEqual(pop.Rating, 15);
            Assert.AreEqual(pop.PlayCounter, 0xFF);
        }
    }
}