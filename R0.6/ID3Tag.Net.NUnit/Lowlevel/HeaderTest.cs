using NUnit.Framework;

namespace ID3Tag.Net.NUnit.Lowlevel
{
    [TestFixture]
    public class HeaderTest : Test
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            m_Controller = Id3TagFactory.CreateIoController();
        }

        #endregion

        [Test]
        public void FooterTest1()
        {
            //TODO: ID3V2.4 Test - > In eigene Klasse verschieben!

            var headerBytes = new byte[]
                                  {
                                      0x49, 0x44, 0x33, 0x04, 0x00, 0x10, 0x00, 0x00, 0x02, 0x01,
                                      0x54, 0x41, 0x4C, 0x42, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, // TALB
                                      0x41, 0x42, 0x43, 0x44,
                                      0x33, 0x44, 0x49, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Footer
                                      0x54, 0x49, 0x54, 0x32, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, // TIT2
                                      0x45, 0x46, 0x47, 0x48,
                                  };
            Read(headerBytes);

            Assert.AreEqual(m_TagInfo.MajorVersion, 4);
            Assert.AreEqual(m_TagInfo.Revision, 0);
            Assert.IsFalse(m_TagInfo.UnsynchronisationFlag);
            Assert.IsFalse(m_TagInfo.Experimental);
            Assert.IsFalse(m_TagInfo.ExtendedHeaderAvailable);
            Assert.IsTrue(m_TagInfo.FooterFlag);

            Assert.AreEqual(m_TagInfo.Frames.Count, 1);
        }

        [Test]
        public void FooterTest2()
        {
            //TODO: ID3V2.4 Test - > In eigene Klasse verschieben!

            var headerBytes = new byte[] {0x49, 0x44, 0x33, 0x04, 0x00, 0x00, 0x00, 0x00, 0x02, 0x01};
            Read(headerBytes);

            Assert.AreEqual(m_TagInfo.MajorVersion, 4);
            Assert.AreEqual(m_TagInfo.Revision, 0);
            Assert.IsFalse(m_TagInfo.UnsynchronisationFlag);
            Assert.IsFalse(m_TagInfo.Experimental);
            Assert.IsFalse(m_TagInfo.ExtendedHeaderAvailable);
            Assert.IsFalse(m_TagInfo.FooterFlag);
        }

        [Test]
        public void HeaderPropertyTest1()
        {
            var headerBytes = new byte[] {0x49, 0x44, 0x33, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08};
            Read(headerBytes);

            Assert.AreEqual(m_TagInfo.MajorVersion, 3);
            Assert.AreEqual(m_TagInfo.Revision, 0);
            Assert.IsFalse(m_TagInfo.Experimental);
            Assert.IsFalse(m_TagInfo.UnsynchronisationFlag);
            Assert.IsFalse(m_TagInfo.ExtendedHeaderAvailable);
        }

        [Test]
        public void HeaderPropertyTest2()
        {
            var headerBytes = new byte[] {0x49, 0x44, 0x33, 0x03, 0x00, 0xA0, 0x00, 0x00, 0x02, 0x01};
            Read(headerBytes);

            Assert.AreEqual(m_TagInfo.MajorVersion, 3);
            Assert.AreEqual(m_TagInfo.Revision, 0);
            Assert.IsTrue(m_TagInfo.Experimental);
            Assert.IsTrue(m_TagInfo.UnsynchronisationFlag);
            Assert.IsFalse(m_TagInfo.ExtendedHeaderAvailable);
        }

        [Test]
        public void HeaderSizeTes3()
        {
            var headerBytes = new byte[] {0x49, 0x44, 0x33, 0x03, 0x00, 0xA0, 0x00, 0x00, 0x00, 0xFF};
            Read(headerBytes);

            Assert.AreEqual(m_TagInfo.MajorVersion, 3);
            Assert.AreEqual(m_TagInfo.Revision, 0);
            Assert.IsTrue(m_TagInfo.Experimental);
            Assert.IsTrue(m_TagInfo.UnsynchronisationFlag);
            Assert.IsFalse(m_TagInfo.ExtendedHeaderAvailable);
        }

        [Test]
        public void HeaderSizeTes4()
        {
            var headerBytes = new byte[] {0x49, 0x44, 0x33, 0x03, 0x00, 0xA0, 0x00, 0x00, 0xFF, 0xFF};
            Read(headerBytes);

            Assert.AreEqual(m_TagInfo.MajorVersion, 3);
            Assert.AreEqual(m_TagInfo.Revision, 0);
            Assert.IsTrue(m_TagInfo.Experimental);
            Assert.IsTrue(m_TagInfo.UnsynchronisationFlag);
            Assert.IsFalse(m_TagInfo.ExtendedHeaderAvailable);
        }

        [Test]
        public void HeaderSizeTes5()
        {
            var headerBytes = new byte[] {0x49, 0x44, 0x33, 0x03, 0x00, 0xA0, 0x00, 0xFF, 0xFF, 0xFF};
            Read(headerBytes);

            Assert.AreEqual(m_TagInfo.MajorVersion, 3);
            Assert.AreEqual(m_TagInfo.Revision, 0);
            Assert.IsTrue(m_TagInfo.Experimental);
            Assert.IsTrue(m_TagInfo.UnsynchronisationFlag);
            Assert.IsFalse(m_TagInfo.ExtendedHeaderAvailable);
        }

        [Test]
        public void HeaderSizeTest1()
        {
            var headerBytes = new byte[] {0x49, 0x44, 0x33, 0x03, 0x00, 0xA0, 0x00, 0x00, 0x00, 0x00};
            Read(headerBytes);
        }

        [Test]
        [ExpectedException(typeof (ID3HeaderNotFoundException))]
        public void InvalidHeaderTest1()
        {
            var headerBytes = new byte[] {0x49, 0x44, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08};
            Read(headerBytes);
        }

        [Test]
        [ExpectedException(typeof (ID3HeaderNotFoundException))]
        public void InvalidHeaderTest2()
        {
            var headerBytes = new byte[] {0x49, 0x00, 0x33, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08};
            Read(headerBytes);
        }

        [Test]
        [ExpectedException(typeof (ID3HeaderNotFoundException))]
        public void InvalidHeaderTest3()
        {
            var headerBytes = new byte[] {0x00, 0x44, 0x33, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08};
            Read(headerBytes);
        }

        [Test]
        [ExpectedException(typeof (ID3HeaderNotFoundException))]
        public void InvalidHeaderTest4()
        {
            var headerBytes = new byte[] {0x54, 0x41, 0x47, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08};
            Read(headerBytes);
        }
    }
}