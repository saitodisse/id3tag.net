﻿using Id3Tag.LowLevel;
using NUnit.Framework;

namespace Id3Tag.Net.NUnit.Lowlevel
{
    [TestFixture]
    public class ExtendedHeaderV2_3Test : Test
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            m_Controller = Id3TagFactory.CreateIOController();
        }

        #endregion

        [Test]
        public void ExtendedHeaderPropertyTest1()
        {
            var headerBytes = new byte[]
                                  {
                                      0x49, 0x44, 0x33, 0x03, 0x00, 0xE0, 0x00, 0x00, 0x00, 0x0A,
                                      0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0A
                                  };
            Read(headerBytes);

            Assert.AreEqual(m_TagInfo.MajorVersion, 3);
            Assert.AreEqual(m_TagInfo.Revision, 0);
            Assert.IsTrue(m_TagInfo.Experimental);
            Assert.IsTrue(m_TagInfo.Unsynchronised);
            Assert.IsTrue(m_TagInfo.ExtendedHeaderAvailable);

            ExtendedTagHeaderV3 extendedHeader = m_TagInfo.ExtendedHeader.ConvertToV23();
            Assert.IsFalse(extendedHeader.CrcDataPresent);
            Assert.AreEqual(extendedHeader.PaddingSize, 10);
            Assert.IsNull(extendedHeader.Crc32);
        }

        [Test]
        [ExpectedException(typeof (Id3TagException))]
        public void ExtendedHeaderPropertyTest2()
        {
            var headerBytes = new byte[]
                                  {
                                      0x49, 0x44, 0x33, 0x03, 0x00, 0xE0, 0x00, 0x00, 0x02, 0x01,
                                      0x00, 0x00, 0x00, 0x0A, 0x80, 0x00, 0x00, 0x00, 0x00, 0x0A, 0x30, 0x31, 0x32, 0x33
                                      ,
                                      // TALB 
                                      0x54, 0x41, 0x4C, 0x42, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00,
                                      0x00, 0x30, 0x31, 0x32, 0x33
                                  };

            //
            //  The CRC32 value does not match. Excpeption !
            //
            Read(headerBytes);

            //Assert.AreEqual(m_TagInfo.MajorVersion, 3);
            //Assert.AreEqual(m_TagInfo.Revision, 0);
            //Assert.IsTrue(m_TagInfo.Experimental);
            //Assert.IsTrue(m_TagInfo.Unsynchronised);
            //Assert.IsTrue(m_TagInfo.ExtendedHeaderAvailable);

            //var extendedHeader = m_TagInfo.ExtendHeaderV3;
            //Assert.IsTrue(extendedHeader.CRCDataPresent);
            //Assert.AreEqual(extendedHeader.PaddingSize, 10);
            //Assert.IsNotNull(extendedHeader.CRC);
            //Assert.AreEqual(extendedHeader.CRC.Length, 4);
            //Assert.AreEqual(extendedHeader.CRC[0], 0x30);
            //Assert.AreEqual(extendedHeader.CRC[1], 0x31);
            //Assert.AreEqual(extendedHeader.CRC[2], 0x32);
            //Assert.AreEqual(extendedHeader.CRC[3], 0x33);
        }

        [Test]
        public void ExtendedHeaderPropertyTest3()
        {
            var headerBytes = new byte[]
                                  {
                                      0x49, 0x44, 0x33, 0x03, 0x00, 0xE0, 0x00, 0x00, 0x00, 0x1D,
                                      0x00, 0x00, 0x00, 0x0A, 0x80, 0x00, 0x00, 0x00, 0x00, 0x0A, 0x71, 0xB4, 0x00, 0x0F
                                      ,
                                      // TALB 
                                      0x54, 0x41, 0x4C, 0x42, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00,
                                      0x00, 0x30, 0x31, 0x32, 0x33
                                  };

            Read(headerBytes);
            Assert.AreEqual(m_TagInfo.MajorVersion, 3);
            Assert.AreEqual(m_TagInfo.Revision, 0);
            Assert.IsTrue(m_TagInfo.Experimental);
            Assert.IsTrue(m_TagInfo.Unsynchronised);
            Assert.IsTrue(m_TagInfo.ExtendedHeaderAvailable);

            ExtendedTagHeaderV3 extendedHeader = m_TagInfo.ExtendedHeader.ConvertToV23();
            Assert.IsTrue(extendedHeader.CrcDataPresent);
            Assert.AreEqual(extendedHeader.PaddingSize, 10);
            Assert.IsNotNull(extendedHeader.Crc32);
            Assert.AreEqual(extendedHeader.Crc32.Count, 4);
            Assert.AreEqual(extendedHeader.Crc32[0], 0x71);
            Assert.AreEqual(extendedHeader.Crc32[1], 0xB4);
            Assert.AreEqual(extendedHeader.Crc32[2], 0x00);
            Assert.AreEqual(extendedHeader.Crc32[3], 0x0F);
        }

        [Test]
        public void HeaderPropertyTest()
        {
            var headerBytes = new byte[]
                                  {
                                      0x49, 0x44, 0x33, 0x03, 0x00, 0xE0, 0x00, 0x00, 0x00, 0x0A,
                                      0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                                  };
            Read(headerBytes);

            Assert.AreEqual(m_TagInfo.MajorVersion, 3);
            Assert.AreEqual(m_TagInfo.Revision, 0);
            Assert.IsTrue(m_TagInfo.Experimental);
            Assert.IsTrue(m_TagInfo.Unsynchronised);
            Assert.IsTrue(m_TagInfo.ExtendedHeaderAvailable);

            // Test the flag, only.
        }
    }
}