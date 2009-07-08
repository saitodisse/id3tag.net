using ID3Tag.Factory;
using NUnit.Framework;

namespace ID3Tag.Net.NUnit.Lowlevel
{
    [TestFixture]
    public class ExtendedHeaderV2_4Test : Test
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            m_Controller = Id3TagFactory.CreateIoController();
        }

        #endregion

        [Test]
        public void CompleteExtendedHeaderTest()
        {
            var headerBytes = new byte[]
                                  {
                                      0x49, 0x44, 0x33, 0x04, 0x00, 0x40, 0x00, 0x00, 0x00, 0x0F,
                                      0x00, 0x00, 0x00, 0x0F, 0x01, 0xF0, 0x00, 0x05, 0x01, 0x02, 0x03, 0x04, 0x05, 0x01
                                      , 0x011
                                  };
            Read(headerBytes);

            Assert.AreEqual(m_TagInfo.MajorVersion, 4);
            Assert.AreEqual(m_TagInfo.Revision, 0);
            Assert.IsTrue(m_TagInfo.ExtendedHeaderAvailable);

            Assert.IsNotNull(m_TagInfo.ExtendHeaderV4);
            Assert.IsTrue(m_TagInfo.ExtendHeaderV4.UpdateTag);
            Assert.IsTrue(m_TagInfo.ExtendHeaderV4.CrcDataPresent);
            Assert.IsNotNull(m_TagInfo.ExtendHeaderV4.Crc32);
            Assert.AreEqual(m_TagInfo.ExtendHeaderV4.Crc32[0], 0x01);
            Assert.AreEqual(m_TagInfo.ExtendHeaderV4.Crc32[1], 0x02);
            Assert.AreEqual(m_TagInfo.ExtendHeaderV4.Crc32[2], 0x03);
            Assert.AreEqual(m_TagInfo.ExtendHeaderV4.Crc32[3], 0x04);
            Assert.AreEqual(m_TagInfo.ExtendHeaderV4.Crc32[4], 0x05);
            Assert.IsTrue(m_TagInfo.ExtendHeaderV4.RestrictionPresent);
            Assert.AreEqual(m_TagInfo.ExtendHeaderV4.Restriction, 0x11);
        }

        [Test]
        public void Crc32Test()
        {
            var headerBytes = new byte[]
                                  {
                                      0x49, 0x44, 0x33, 0x04, 0x00, 0x40, 0x00, 0x00, 0x00, 0x0C,
                                      0x00, 0x00, 0x00, 0x0C, 0x01, 0x20, 0x05, 0x01, 0x02, 0x03, 0x04, 0x05
                                  };
            Read(headerBytes);

            Assert.AreEqual(m_TagInfo.MajorVersion, 4);
            Assert.AreEqual(m_TagInfo.Revision, 0);
            Assert.IsTrue(m_TagInfo.ExtendedHeaderAvailable);
            Assert.IsNotNull(m_TagInfo.ExtendHeaderV4);
            Assert.IsFalse(m_TagInfo.ExtendHeaderV4.UpdateTag);
            Assert.IsTrue(m_TagInfo.ExtendHeaderV4.CrcDataPresent);
            Assert.IsNotNull(m_TagInfo.ExtendHeaderV4.Crc32);
            Assert.AreEqual(m_TagInfo.ExtendHeaderV4.Crc32[0], 0x01);
            Assert.AreEqual(m_TagInfo.ExtendHeaderV4.Crc32[1], 0x02);
            Assert.AreEqual(m_TagInfo.ExtendHeaderV4.Crc32[2], 0x03);
            Assert.AreEqual(m_TagInfo.ExtendHeaderV4.Crc32[3], 0x04);
            Assert.AreEqual(m_TagInfo.ExtendHeaderV4.Crc32[4], 0x05);
            Assert.IsFalse(m_TagInfo.ExtendHeaderV4.RestrictionPresent);
            Assert.AreEqual(m_TagInfo.ExtendHeaderV4.Restriction, 0);
        }

        [Test]
        public void ExtendedHeaderTest1()
        {
            var headerBytes = new byte[]
                                  {
                                      0x49, 0x44, 0x33, 0x04, 0x00, 0x40, 0x00, 0x00, 0x00, 0x06,
                                      0x00, 0x00, 0x00, 0x06, 0x01, 0x00
                                  };
            Read(headerBytes);

            Assert.AreEqual(m_TagInfo.MajorVersion, 4);
            Assert.AreEqual(m_TagInfo.Revision, 0);
            Assert.IsTrue(m_TagInfo.ExtendedHeaderAvailable);

            Assert.IsNotNull(m_TagInfo.ExtendHeaderV4);
            Assert.IsFalse(m_TagInfo.ExtendHeaderV4.UpdateTag);
            Assert.IsFalse(m_TagInfo.ExtendHeaderV4.CrcDataPresent);
            Assert.IsNull(m_TagInfo.ExtendHeaderV4.Crc32);
            Assert.IsFalse(m_TagInfo.ExtendHeaderV4.RestrictionPresent);
            Assert.AreEqual(m_TagInfo.ExtendHeaderV4.Restriction, 0);
        }

        [Test]
        public void RestrictionTest()
        {
            var headerBytes = new byte[]
                                  {
                                      0x49, 0x44, 0x33, 0x04, 0x00, 0x40, 0x00, 0x00, 0x00, 0x08,
                                      0x00, 0x00, 0x00, 0x08, 0x01, 0x10, 0x01, 0x02
                                  };
            Read(headerBytes);

            Assert.AreEqual(m_TagInfo.MajorVersion, 4);
            Assert.AreEqual(m_TagInfo.Revision, 0);
            Assert.IsTrue(m_TagInfo.ExtendedHeaderAvailable);

            Assert.IsNotNull(m_TagInfo.ExtendHeaderV4);
            Assert.IsFalse(m_TagInfo.ExtendHeaderV4.UpdateTag);
            Assert.IsFalse(m_TagInfo.ExtendHeaderV4.CrcDataPresent);
            Assert.IsNull(m_TagInfo.ExtendHeaderV4.Crc32);
            Assert.IsTrue(m_TagInfo.ExtendHeaderV4.RestrictionPresent);
            Assert.AreEqual(m_TagInfo.ExtendHeaderV4.Restriction, 0x02);
        }

        [Test]
        public void UpdateTest()
        {
            var headerBytes = new byte[]
                                  {
                                      0x49, 0x44, 0x33, 0x04, 0x00, 0x40, 0x00, 0x00, 0x00, 0x08,
                                      0x00, 0x00, 0x00, 0x08, 0x01, 0x40, 0x00, 0x00
                                  };
            Read(headerBytes);

            Assert.AreEqual(m_TagInfo.MajorVersion, 4);
            Assert.AreEqual(m_TagInfo.Revision, 0);
            Assert.IsTrue(m_TagInfo.ExtendedHeaderAvailable);
            Assert.IsNotNull(m_TagInfo.ExtendHeaderV4);
            Assert.IsTrue(m_TagInfo.ExtendHeaderV4.UpdateTag);
            Assert.IsFalse(m_TagInfo.ExtendHeaderV4.CrcDataPresent);
            Assert.IsNull(m_TagInfo.ExtendHeaderV4.Crc32);
            Assert.IsFalse(m_TagInfo.ExtendHeaderV4.RestrictionPresent);
            Assert.AreEqual(m_TagInfo.ExtendHeaderV4.Restriction, 0);
        }
    }
}