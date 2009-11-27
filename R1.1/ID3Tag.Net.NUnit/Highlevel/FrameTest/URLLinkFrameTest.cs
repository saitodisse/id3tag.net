using Id3Tag.HighLevel;
using Id3Tag.HighLevel.Id3Frame;
using NUnit.Framework;

namespace Id3Tag.Net.NUnit.Highlevel.FrameTest
{
    [TestFixture]
    public class URLLinkFrameTest : Test
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
            var urlLinkFrame = new UrlLinkFrame
                                   {
                                       Url = "ABCD",
                                       Descriptor = {Id = "W123"}
                                   };

            var rawFrame = urlLinkFrame.Convert(TagVersion.Id3V23);

            Assert.AreEqual(rawFrame.Id, "W123");
            Assert.AreEqual(rawFrame.Payload.Count, 4);
            var refBytes = new byte[] {0x41, 0x42, 0x43, 0x44};
            Assert.IsTrue(ComparePayload(rawFrame.Payload, refBytes));
        }

        [Test]
        public void CreateTest()
        {
            var frame = new UrlLinkFrame("W123", "URL");

            Assert.AreEqual(frame.Descriptor.Id, "W123");
            Assert.AreEqual(frame.Url, "URL");
        }

        [Test]
        public void URLLinkDetectionTest1()
        {
            var frames = new byte[]
                             {
                                 // W123
                                 0x57, 0x31, 0x32, 0x33, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00,
                                 0x00, 0x41, 0x42, 0x43, 0x44,
                             };

            var completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            var tagContainer = m_TagController.Decode(m_TagInfo);
            var tag = tagContainer.GetId3V23Descriptor();

            Assert.AreEqual(tag.MajorVersion, 3);
            Assert.AreEqual(tag.Revision, 0);
            Assert.AreEqual(tagContainer.Count, 1);

            var f1 = tagContainer[0];
            var text1 = FrameUtilities.ConvertToUrlLinkFrame(f1);

            Assert.AreEqual(text1.Descriptor.Id, "W123");
            Assert.AreEqual(text1.Type, FrameType.UrlLink);
        }
    }
}