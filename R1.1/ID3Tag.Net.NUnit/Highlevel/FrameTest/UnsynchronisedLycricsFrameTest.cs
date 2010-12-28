using System.Text;
using Id3Tag.HighLevel;
using Id3Tag.HighLevel.Id3Frame;
using Id3Tag.LowLevel;
using NUnit.Framework;

namespace Id3Tag.Net.NUnit.Highlevel.FrameTest
{
    [TestFixture]
    public class UnsynchronisedLycricsFrameTest : Test
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
        public void CreateTest1()
        {
            var frame = new UnsynchronisedLyricFrame();
            Assert.AreEqual(frame.Descriptor.Id, "USLT");
            Assert.AreEqual(frame.Language, "ENG");
            Assert.AreEqual(frame.ContentDescriptor, "");
            Assert.AreEqual(frame.Lyrics, "");
            Assert.AreEqual(frame.Type, FrameType.UnsynchronisedLyric);
            Assert.AreEqual(frame.TextEncoding, Encoding.ASCII);
        }

        [Test]
        public void CreateTest2()
        {
            const string descriptor = "Descriptor";
            const string lyrics = "lala";
            const string language = "language";

            var frame = new UnsynchronisedLyricFrame(language, descriptor, lyrics, Encoding.ASCII);
            Assert.AreEqual(frame.Descriptor.Id, "USLT");
            Assert.AreEqual(frame.Language, language);
            Assert.AreEqual(frame.ContentDescriptor, descriptor);
            Assert.AreEqual(frame.Lyrics, lyrics);
            Assert.AreEqual(frame.Type, FrameType.UnsynchronisedLyric);
            Assert.AreEqual(frame.TextEncoding, Encoding.ASCII);
        }

        [Test]
        public void EmptyDescriptorTest()
        {
            var frames = new byte[]
                             {
                                 // USLT
                                 0x55, 0x53, 0x4C, 0x54, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00,
                                 0x00, 0x41, 0x42, 0x43, 0x00, 0x44, 0x45, 0x46
                             };

            byte[] completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            TagContainer tagContainer = m_TagController.Decode(m_TagInfo);
            Assert.AreEqual(tagContainer.Count, 1);

            UnsynchronisedLyricFrame uslt = FrameUtilities.ConvertToUnsynchronisedLyricsFrame(tagContainer[0]);

            Assert.AreEqual(uslt.Descriptor.Id, "USLT");
            Assert.AreEqual(uslt.Language, "ABC");
            Assert.AreEqual(uslt.ContentDescriptor, "");
            Assert.AreEqual(uslt.Lyrics, "DEF");
        }

        [Test]
        public void EmptyLyricsTest()
        {
            var frames = new byte[]
                             {
                                 // USLT
                                 0x55, 0x53, 0x4C, 0x54, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00,
                                 0x00, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x00
                             };

            byte[] completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            TagContainer tagContainer = m_TagController.Decode(m_TagInfo);
            Assert.AreEqual(tagContainer.Count, 1);

            UnsynchronisedLyricFrame uslt = FrameUtilities.ConvertToUnsynchronisedLyricsFrame(tagContainer[0]);

            Assert.AreEqual(uslt.Descriptor.Id, "USLT");
            Assert.AreEqual(uslt.Language, "ABC");
            Assert.AreEqual(uslt.ContentDescriptor, "DEF");
            Assert.AreEqual(uslt.Lyrics, "");
        }

        [Test]
        public void ImportAndConvertTest()
        {
            var frames = new byte[]
                             {
                                 // USLT
                                 0x55, 0x53, 0x4C, 0x54, 0x00, 0x00, 0x00, 0x0B, 0x00, 0x00,
                                 0x00, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x00, 0x47, 0x48,
                                 0x49
                             };

            byte[] completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            TagContainer tagContainer = m_TagController.Decode(m_TagInfo);
            Assert.AreEqual(tagContainer.Count, 1);

            UnsynchronisedLyricFrame uslt = FrameUtilities.ConvertToUnsynchronisedLyricsFrame(tagContainer[0]);

            Assert.AreEqual(uslt.Descriptor.Id, "USLT");
            Assert.AreEqual(uslt.Language, "ABC");
            Assert.AreEqual(uslt.ContentDescriptor, "DEF");
            Assert.AreEqual(uslt.Lyrics, "GHI");

            RawFrame rawFrame = uslt.Convert(TagVersion.Id3V23);


            Assert.AreEqual(rawFrame.Id, "USLT");
            ComparePayload(rawFrame.Payload, frames);
        }
    }
}