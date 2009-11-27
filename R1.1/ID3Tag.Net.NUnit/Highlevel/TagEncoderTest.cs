using System.Text;
using Id3Tag.HighLevel;
using Id3Tag.HighLevel.Id3Frame;
using NUnit.Framework;

namespace Id3Tag.Net.NUnit.Highlevel
{
    [TestFixture]
    public class TagEncoderTest : Test
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
        public void ExtendedHeaderEncoderTest()
        {
            var crc = new byte[] {0x20, 0x21, 0x22, 0x23};

            var tagContainer = new TagContainerV3();
            tagContainer.Tag.SetHeaderOptions(true, true, true);
            tagContainer.Tag.SetExtendedHeader(10, true);
            tagContainer.Tag.SetCrc32(crc);

            var titleFrame = new TextFrame();
            titleFrame.TextEncoding = Encoding.Unicode;
            titleFrame.Descriptor.Id = "TIT2";
            titleFrame.Content = "ABCD";

            tagContainer.Add(titleFrame);

            var Id3Tag = m_TagController.Encode(tagContainer);

            Assert.AreEqual(Id3Tag.MajorVersion, 3);
            Assert.AreEqual(Id3Tag.Revision, 0);
            Assert.IsTrue(Id3Tag.Unsynchronised);
            Assert.IsTrue(Id3Tag.Unsynchronised);
            Assert.IsTrue(Id3Tag.ExtendedHeaderAvailable);

            var extendedHeader = Id3Tag.ExtendedHeader.ConvertToV23();
            Assert.IsNotNull(extendedHeader);
            Assert.IsTrue(ComparePayload(extendedHeader.Crc32, crc));
            Assert.AreEqual(Id3Tag.Frames.Count, 1);
        }

        [Test]
        public void SimpleEncoderTest()
        {
            var tagContainer = new TagContainerV3();
            tagContainer.Tag.SetHeaderOptions(false, false, false);

            var titleFrame = new TextFrame();
            titleFrame.TextEncoding = Encoding.Unicode;
            titleFrame.Descriptor.Id = "TIT2";
            titleFrame.Content = "ABCD";

            tagContainer.Add(titleFrame);

            var Id3Tag = m_TagController.Encode(tagContainer);

            Assert.AreEqual(Id3Tag.MajorVersion, 3);
            Assert.AreEqual(Id3Tag.Revision, 0);
            Assert.IsFalse(Id3Tag.Unsynchronised);
            Assert.IsFalse(Id3Tag.Unsynchronised);
            Assert.IsFalse(Id3Tag.ExtendedHeaderAvailable);
            Assert.IsNull(Id3Tag.ExtendedHeader);
            Assert.AreEqual(Id3Tag.Frames.Count, 1);
        }
    }
}