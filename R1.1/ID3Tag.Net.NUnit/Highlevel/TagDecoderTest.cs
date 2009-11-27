using NUnit.Framework;

namespace Id3Tag.Net.NUnit.Highlevel
{
    [TestFixture]
    public class TagDecoderTest : Test
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
        public void ExtendedHeaderTest()
        {
            var headerBytes = new byte[]
                                  {
                                      0x49, 0x44, 0x33, 0x03, 0x00, 0xE0, 0x00, 0x00, 0x00, 0x1D,
                                      0x00, 0x00, 0x00, 0x0A, 0x80, 0x00, 0x00, 0x00, 0x00, 0x0A, 0x71, 0xB4, 0x00, 0x0F,
                                      // TALB 
                                      0x54, 0x41, 0x4C, 0x42, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00,
                                      0x00, 0x30, 0x31, 0x32, 0x33
                                  };
            Read(headerBytes);

            // Validate TagInfo
            Assert.AreEqual(m_TagInfo.MajorVersion, 3);
            Assert.AreEqual(m_TagInfo.Revision, 0);
            Assert.IsTrue(m_TagInfo.Experimental);
            Assert.IsTrue(m_TagInfo.Unsynchronised);
            Assert.IsTrue(m_TagInfo.ExtendedHeaderAvailable);

            var extendedHeader = m_TagInfo.ExtendedHeader.ConvertToV23();
            Assert.IsTrue(extendedHeader.CrcDataPresent);
            Assert.AreEqual(extendedHeader.PaddingSize, 10);
            Assert.IsNotNull(extendedHeader.Crc32);

            var refCRCBytes = new byte[] {0x71, 0xB4, 0x00, 0x0F};
            Assert.IsTrue(ComparePayload(extendedHeader.Crc32, refCRCBytes));

            // Validate TagContainer
            var tagContainer = m_TagController.Decode(m_TagInfo);
            var tag = tagContainer.GetId3V23Descriptor();
            Assert.AreEqual(tag.MajorVersion, 3);
            Assert.AreEqual(tag.Revision, 0);
            Assert.IsTrue(tag.ExperimentalIndicator);
            Assert.IsTrue(tag.Unsynchronisation);

            Assert.IsTrue(tag.ExtendedHeader);
            Assert.AreEqual(tag.PaddingSize, 10);
            Assert.IsTrue(tag.CrcDataPresent);
            Assert.IsTrue(ComparePayload(tag.Crc, refCRCBytes));
        }

        [Test]
        public void WithoutExtendedHeaderTest()
        {
            var headerBytes = new byte[]
                                  {
                                      0x49, 0x44, 0x33, 0x03, 0x00, 0x20, 0x00, 0x00, 0x00, 0x0A,
                                      0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0A
                                  };
            Read(headerBytes);

            Assert.AreEqual(m_TagInfo.MajorVersion, 3);
            Assert.AreEqual(m_TagInfo.Revision, 0);
            Assert.IsTrue(m_TagInfo.Experimental);
            Assert.IsFalse(m_TagInfo.Unsynchronised);
            Assert.IsFalse(m_TagInfo.ExtendedHeaderAvailable);

            Assert.IsNull(m_TagInfo.ExtendedHeader);

            // Validate TagContainer
            var tagContainer = m_TagController.Decode(m_TagInfo);
            var tag = tagContainer.GetId3V23Descriptor();

            Assert.AreEqual(tag.MajorVersion, 3);
            Assert.AreEqual(tag.Revision, 0);
            Assert.IsTrue(tag.ExperimentalIndicator);
            Assert.IsFalse(tag.Unsynchronisation);
            Assert.IsFalse(tag.ExtendedHeader);

            Assert.AreEqual(tag.PaddingSize, 0);
            Assert.IsFalse(tag.CrcDataPresent);
            Assert.AreEqual(tag.Crc.Count, 0);
        }
    }
}