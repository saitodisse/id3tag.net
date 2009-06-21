using ID3Tag.Factory;
using NUnit.Framework;

namespace ID3Tag.Net.NUnit.Highlevel
{
    [TestFixture]
    public class TagDecoderTest : Test
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
        public void ExtendedHeaderTest()
        {
            var headerBytes = new byte[]
                                  {
                                      0x49, 0x44, 0x33, 0x03, 0x00, 0xE0, 0x00, 0x00, 0x02, 0x01,
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
            Assert.IsTrue(m_TagInfo.UnsynchronisationFlag);
            Assert.IsTrue(m_TagInfo.ExtendedHeaderAvailable);

            var extendedHeader = m_TagInfo.ExtendHeader;
            Assert.IsTrue(extendedHeader.CRCDataPresent);
            Assert.AreEqual(extendedHeader.PaddingSize, 10);
            Assert.IsNotNull(extendedHeader.CRC);

            var refCRCBytes = new byte[] {0x71, 0xB4, 0x00, 0x0F};
            Assert.IsTrue(ComparePayload(extendedHeader.CRC, refCRCBytes));

            // Validate TagContainer
            var tagContainer = m_TagController.Decode(m_TagInfo);
            Assert.AreEqual(tagContainer.Tag.MajorVersion, 3);
            Assert.AreEqual(tagContainer.Tag.Revision, 0);
            Assert.IsTrue(tagContainer.Tag.ExperimentalIndicator);
            Assert.IsTrue(tagContainer.Tag.Unsynchronisation);

            Assert.IsTrue(tagContainer.Tag.ExtendedHeader);
            Assert.AreEqual(tagContainer.Tag.PaddingSize, 10);
            Assert.IsTrue(tagContainer.Tag.CrcDataPresent);
            Assert.IsTrue(ComparePayload(tagContainer.Tag.Crc, refCRCBytes));
        }

        [Test]
        public void WithoutExtendedHeaderTest()
        {
            var headerBytes = new byte[]
                                  {
                                      0x49, 0x44, 0x33, 0x03, 0x00, 0x20, 0x00, 0x00, 0x02, 0x01,
                                      0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0A
                                  };
            Read(headerBytes);

            Assert.AreEqual(m_TagInfo.MajorVersion, 3);
            Assert.AreEqual(m_TagInfo.Revision, 0);
            Assert.IsTrue(m_TagInfo.Experimental);
            Assert.IsFalse(m_TagInfo.UnsynchronisationFlag);
            Assert.IsFalse(m_TagInfo.ExtendedHeaderAvailable);

            var extendedHeader = m_TagInfo.ExtendHeader;
            Assert.IsNull(extendedHeader);

            // Validate TagContainer
            var tagContainer = m_TagController.Decode(m_TagInfo);

            Assert.AreEqual(tagContainer.Tag.MajorVersion, 3);
            Assert.AreEqual(tagContainer.Tag.Revision, 0);
            Assert.IsTrue(tagContainer.Tag.ExperimentalIndicator);
            Assert.IsFalse(tagContainer.Tag.Unsynchronisation);
            Assert.IsFalse(tagContainer.Tag.ExtendedHeader);

            Assert.AreEqual(tagContainer.Tag.PaddingSize, 0);
            Assert.IsFalse(tagContainer.Tag.CrcDataPresent);
            Assert.AreEqual(tagContainer.Tag.Crc.Length, 0);
        }
    }
}