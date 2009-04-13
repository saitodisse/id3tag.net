using System;
using ID3Tag.Factory;
using NUnit.Framework;

namespace ID3Tag.Net.NUnit.Lowlevel
{
    [TestFixture]
    public class ReaderTest : Test
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            m_Controller = Id3TagFactory.CreateIoController();
        }

        #endregion

        [Test]
        public void NullByteFrameTest()
        {
            // ID,ID,ID,ID,S,S,S,S,F,F,
            // D,....,D

            var frames = new byte[]
                             {
                                 0x49, 0x44, 0x33, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                 0x31, 0x32, 0x33, 0x34, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00,
                                 0x49, 0x50, 0x51, 0x52,
                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x004, 0x00, 0x00,
                                 0x00, 0x00, 0x00, 0x00,
                             };

            var size = CalculateSize(28);
            Array.Copy(size, 0, frames, 6, 4);

            Read(frames);

            var frame1 = m_TagInfo.Frames[0];

            Assert.AreEqual(m_TagInfo.Frames.Count, 1);
            // frame 1
            Assert.AreEqual(frame1.ID, "1234");
            Assert.AreEqual(frame1.Payload.Length, 4);
            Assert.IsFalse(frame1.Compression);
            Assert.IsFalse(frame1.Encryption);
            Assert.IsFalse(frame1.FileAlterPreservation);
            Assert.IsFalse(frame1.GroupingIdentify);
            Assert.IsFalse(frame1.ReadOnly);
            Assert.IsFalse(frame1.TagAlterPreservation);
        }

        [Test]
        public void ReadTagOnlyTest()
        {
            // ID,ID,ID,ID,S,S,S,S,F,F,
            // D,....,D

            var frames = new byte[]
                             {
                                 0x49, 0x44, 0x33, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                 0x31, 0x32, 0x33, 0x34, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00,
                                 0x49, 0x50, 0x51, 0x52,
                                 0x35, 0x36, 0x37, 0x38, 0x00, 0x00, 0x00, 0x004, 0xE0, 0xE0,
                                 0x53, 0x54, 0x55, 0x56,
                                 // Frame that does not belong to the tag!
                                 0x35, 0x36, 0x37, 0x38, 0x00, 0x00, 0x00, 0x004, 0xE0, 0xE0,
                                 0x53, 0x54, 0x55, 0x56,
                             };

            var size = CalculateSize(28);
            Array.Copy(size, 0, frames, 6, 4);

            Read(frames);

            var frame1 = m_TagInfo.Frames[0];
            var frame2 = m_TagInfo.Frames[1];

            Assert.AreEqual(m_TagInfo.Frames.Count, 2);
            // frame 1
            Assert.AreEqual(frame1.ID, "1234");
            Assert.AreEqual(frame1.Payload.Length, 4);
            Assert.IsFalse(frame1.Compression);
            Assert.IsFalse(frame1.Encryption);
            Assert.IsFalse(frame1.FileAlterPreservation);
            Assert.IsFalse(frame1.GroupingIdentify);
            Assert.IsFalse(frame1.ReadOnly);
            Assert.IsFalse(frame1.TagAlterPreservation);
            // frame 2
            Assert.AreEqual(frame2.ID, "5678");
            Assert.AreEqual(frame2.Payload.Length, 4);
            Assert.IsTrue(frame2.Compression);
            Assert.IsTrue(frame2.Encryption);
            Assert.IsTrue(frame2.FileAlterPreservation);
            Assert.IsTrue(frame2.GroupingIdentify);
            Assert.IsTrue(frame2.ReadOnly);
            Assert.IsTrue(frame2.TagAlterPreservation);
        }
    }
}