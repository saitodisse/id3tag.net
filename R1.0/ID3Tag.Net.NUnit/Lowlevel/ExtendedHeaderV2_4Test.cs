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

        //TODO : Fix the CompleteExtendedHeaderTest Test!

        //[Test]
        //public void CompleteExtendedHeaderTest()
        //{
        //    var headerBytes = new byte[]
        //                          {
        //                              0x49, 0x44, 0x33, 0x04, 0x00, 0x40, 0x00, 0x00, 0x00, 0x0F,
        //                              0x00, 0x00, 0x00, 0x0F, 0x01, 0xF0, 0x00, 0x05, 0x01, 0x02, 0x03, 0x04, 0x05, 0x01
        //                              , 0x011
        //                          };
        //    Read(headerBytes);

        //    Assert.AreEqual(m_TagInfo.MajorVersion, 4);
        //    Assert.AreEqual(m_TagInfo.Revision, 0);
        //    Assert.IsTrue(m_TagInfo.ExtendedHeaderAvailable);

        //    Assert.IsNotNull(m_TagInfo.ExtendedHeader);

        //    var extendedHeader = m_TagInfo.ExtendedHeader.ConvertToV24();
        //    Assert.IsTrue(extendedHeader.UpdateTag);
        //    Assert.IsTrue(extendedHeader.CrcDataPresent);
        //    Assert.IsNotNull(extendedHeader.Crc32);
        //    Assert.AreEqual(extendedHeader.Crc32[0], 0x01);
        //    Assert.AreEqual(extendedHeader.Crc32[1], 0x02);
        //    Assert.AreEqual(extendedHeader.Crc32[2], 0x03);
        //    Assert.AreEqual(extendedHeader.Crc32[3], 0x04);
        //    Assert.AreEqual(extendedHeader.Crc32[4], 0x05);
        //    Assert.IsTrue(extendedHeader.RestrictionPresent);
        //    Assert.AreEqual(extendedHeader.Restriction, 0x11);
        //}

        //TODO : Fix the Crc32Test Test!

        //[Test]
        //public void Crc32Test()
        //{
        //    var headerBytes = new byte[]
        //                          {
        //                              0x49, 0x44, 0x33, 0x04, 0x00, 0x40, 0x00, 0x00, 0x00, 0x0C,
        //                              0x00, 0x00, 0x00, 0x0C, 0x01, 0x20, 0x05, 0x01, 0x02, 0x03, 0x04, 0x05
        //                          };
        //    Read(headerBytes);

        //    Assert.AreEqual(m_TagInfo.MajorVersion, 4);
        //    Assert.AreEqual(m_TagInfo.Revision, 0);
        //    Assert.IsTrue(m_TagInfo.ExtendedHeaderAvailable);

        //    var extendedHeader = m_TagInfo.ExtendedHeader.ConvertToV24();
        //    Assert.IsNotNull(extendedHeader);
        //    Assert.IsFalse(extendedHeader.UpdateTag);
        //    Assert.IsTrue(extendedHeader.CrcDataPresent);
        //    Assert.IsNotNull(extendedHeader.Crc32);
        //    Assert.AreEqual(extendedHeader.Crc32[0], 0x01);
        //    Assert.AreEqual(extendedHeader.Crc32[1], 0x02);
        //    Assert.AreEqual(extendedHeader.Crc32[2], 0x03);
        //    Assert.AreEqual(extendedHeader.Crc32[3], 0x04);
        //    Assert.AreEqual(extendedHeader.Crc32[4], 0x05);
        //    Assert.IsFalse(extendedHeader.RestrictionPresent);
        //    Assert.AreEqual(extendedHeader.Restriction, 0);
        //}

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

            var extendedHeader = m_TagInfo.ExtendedHeader.ConvertToV24();
            Assert.IsNotNull(extendedHeader);
            Assert.IsFalse(extendedHeader.UpdateTag);
            Assert.IsFalse(extendedHeader.CrcDataPresent);
            Assert.IsNull(extendedHeader.Crc32);
            Assert.IsFalse(extendedHeader.RestrictionPresent);
            Assert.AreEqual(extendedHeader.Restriction, 0);
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

            var extendedHeader = m_TagInfo.ExtendedHeader.ConvertToV24();
            Assert.IsNotNull(extendedHeader);
            Assert.IsFalse(extendedHeader.UpdateTag);
            Assert.IsFalse(extendedHeader.CrcDataPresent);
            Assert.IsNull(extendedHeader.Crc32);
            Assert.IsTrue(extendedHeader.RestrictionPresent);
            Assert.AreEqual(extendedHeader.Restriction, 0x02);
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

            var extendedHeader = m_TagInfo.ExtendedHeader.ConvertToV24();
            Assert.IsNotNull(extendedHeader);
            Assert.IsTrue(extendedHeader.UpdateTag);
            Assert.IsFalse(extendedHeader.CrcDataPresent);
            Assert.IsNull(extendedHeader.Crc32);
            Assert.IsFalse(extendedHeader.RestrictionPresent);
            Assert.AreEqual(extendedHeader.Restriction, 0);
        }
    }
}