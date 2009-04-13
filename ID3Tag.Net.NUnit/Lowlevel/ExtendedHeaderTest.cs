using ID3Tag.Factory;
using NUnit.Framework;

namespace ID3Tag.Net.NUnit.Lowlevel
{
    [TestFixture]
    public class ExtendedHeaderTest : Test
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            m_Controller = Id3TagFactory.CreateIoController();
        }

        #endregion

        [Test]
        public void ExtendedHeaderPropertyTest1()
        {
            var headerBytes = new byte[]
                                  {
                                      0x49, 0x44, 0x33, 0x03, 0x00, 0xE0, 0x00, 0x00, 0x02, 0x01,
                                      0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0A
                                  };
            Read(headerBytes);

            Assert.AreEqual(m_TagInfo.MajorVersion, 3);
            Assert.AreEqual(m_TagInfo.Revision, 0);
            Assert.IsTrue(m_TagInfo.Experimental);
            Assert.IsTrue(m_TagInfo.UnsynchronisationFlag);
            Assert.IsTrue(m_TagInfo.ExtendedHeaderAvailable);

            var extendedHeader = m_TagInfo.ExtendHeader;
            Assert.IsFalse(extendedHeader.CRCDataPresent);
            Assert.AreEqual(extendedHeader.PaddingSize, 10);
            Assert.IsNull(extendedHeader.CRC);
        }

        [Test]
        public void ExtendedHeaderPropertyTest2()
        {
            var headerBytes = new byte[]
                                  {
                                      0x49, 0x44, 0x33, 0x03, 0x00, 0xE0, 0x00, 0x00, 0x02, 0x01,
                                      0x00, 0x00, 0x00, 0x0A, 0x80, 0x00, 0x00, 0x00, 0x00, 0x0A, 0x30, 0x31, 0x32, 0x33
                                  };
            Read(headerBytes);

            Assert.AreEqual(m_TagInfo.MajorVersion, 3);
            Assert.AreEqual(m_TagInfo.Revision, 0);
            Assert.IsTrue(m_TagInfo.Experimental);
            Assert.IsTrue(m_TagInfo.UnsynchronisationFlag);
            Assert.IsTrue(m_TagInfo.ExtendedHeaderAvailable);

            var extendedHeader = m_TagInfo.ExtendHeader;
            Assert.IsTrue(extendedHeader.CRCDataPresent);
            Assert.AreEqual(extendedHeader.PaddingSize, 10);
            Assert.IsNotNull(extendedHeader.CRC);
            Assert.AreEqual(extendedHeader.CRC.Length, 4);
            Assert.AreEqual(extendedHeader.CRC[0], 0x30);
            Assert.AreEqual(extendedHeader.CRC[1], 0x31);
            Assert.AreEqual(extendedHeader.CRC[2], 0x32);
            Assert.AreEqual(extendedHeader.CRC[3], 0x33);
        }

        [Test]
        public void HeaderPropertyTest()
        {
            var headerBytes = new byte[]
                                  {
                                      0x49, 0x44, 0x33, 0x03, 0x00, 0xE0, 0x00, 0x00, 0x02, 0x01,
                                      0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                                  };
            Read(headerBytes);

            Assert.AreEqual(m_TagInfo.MajorVersion, 3);
            Assert.AreEqual(m_TagInfo.Revision, 0);
            Assert.IsTrue(m_TagInfo.Experimental);
            Assert.IsTrue(m_TagInfo.UnsynchronisationFlag);
            Assert.IsTrue(m_TagInfo.ExtendedHeaderAvailable);

            // Test the flag, only.
        }
    }
}