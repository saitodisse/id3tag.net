using ID3Tag.HighLevel;
using ID3Tag.HighLevel.ID3Frame;
using NUnit.Framework;

namespace ID3Tag.Net.NUnit.Highlevel
{
    [TestFixture]
    public class TagEncoderTest : Test
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
        public void ExtendedHeaderEncoderTest()
        {
            var crc = new byte[] {0x20, 0x21, 0x22, 0x23};

            var tagContainer = new TagContainerV3();
            tagContainer.Tag.SetHeaderFlags(true, true, true);
            tagContainer.Tag.SetExtendedHeader(10, true);
            tagContainer.Tag.SetCrc32(crc);

            var titleFrame = new TextFrame();
            titleFrame.TextEncoding = TextEncodingType.UTF16;
            titleFrame.Descriptor.ID = "TIT2";
            titleFrame.Content = "ABCD";

            tagContainer.Add(titleFrame);

            var id3Tag = m_TagController.Encode(tagContainer);

            Assert.AreEqual(id3Tag.MajorVersion, 3);
            Assert.AreEqual(id3Tag.Revision, 0);
            Assert.IsTrue(id3Tag.UnsynchronisationFlag);
            Assert.IsTrue(id3Tag.UnsynchronisationFlag);
            Assert.IsTrue(id3Tag.ExtendedHeaderAvailable);

            var extendedHeader = id3Tag.ExtendedHeader.ConvertToV23();
            Assert.IsNotNull(extendedHeader);
            Assert.IsTrue(ComparePayload(extendedHeader.Crc32, crc));
            Assert.AreEqual(id3Tag.Frames.Count, 1);
        }

        [Test]
        public void SimpleEncoderTest()
        {
            var tagContainer = new TagContainerV3();
            tagContainer.Tag.SetHeaderFlags(false, false, false);

            var titleFrame = new TextFrame();
            titleFrame.TextEncoding = TextEncodingType.UTF16;
            titleFrame.Descriptor.ID = "TIT2";
            titleFrame.Content = "ABCD";

            tagContainer.Add(titleFrame);

            var id3Tag = m_TagController.Encode(tagContainer);

            Assert.AreEqual(id3Tag.MajorVersion, 3);
            Assert.AreEqual(id3Tag.Revision, 0);
            Assert.IsFalse(id3Tag.UnsynchronisationFlag);
            Assert.IsFalse(id3Tag.UnsynchronisationFlag);
            Assert.IsFalse(id3Tag.ExtendedHeaderAvailable);
            Assert.IsNull(id3Tag.ExtendedHeader);
            Assert.AreEqual(id3Tag.Frames.Count, 1);
        }
    }
}