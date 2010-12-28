using Id3Tag.LowLevel;
using NUnit.Framework;

namespace Id3Tag.Net.NUnit.Lowlevel
{
    [TestFixture]
    public class FramesTestV3 : Test
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            m_Controller = Id3TagFactory.CreateIOController();
        }

        #endregion

        [Test]
        public void CompressionTest()
        {
            // ID,ID,ID,ID,S,S,S,S,F,F,
            // D,....,D

            var frames = new byte[]
                             {
                                 0x31, 0x32, 0x33, 0x34, 0x00, 0x00, 0x00, 0x0B, 0x00, 0x80,
                                 0x49, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59
                             };

            byte[] completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            Assert.AreEqual(m_TagInfo.Frames.Count, 1);

            RawFrame frame = m_TagInfo.Frames[0];
            Assert.AreEqual(frame.Id, "1234");
            Assert.AreEqual(frame.Payload.Count, 11);
            Assert.IsTrue(frame.Options.Compression);
        }

        [Test]
        public void EncryptionTest()
        {
            // ID,ID,ID,ID,S,S,S,S,F,F,
            // D,....,D

            var frames = new byte[]
                             {
                                 0x31, 0x32, 0x33, 0x34, 0x00, 0x00, 0x00, 0x0B, 0x00, 0x40,
                                 0x49, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59
                             };

            byte[] completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            Assert.AreEqual(m_TagInfo.Frames.Count, 1);

            RawFrame frame = m_TagInfo.Frames[0];
            Assert.AreEqual(frame.Id, "1234");
            Assert.AreEqual(frame.Payload.Count, 11);
            Assert.IsTrue(frame.Options.Encryption);
        }

        [Test]
        public void FileAlterPreservationTest()
        {
            // ID,ID,ID,ID,S,S,S,S,F,F,
            // D,....,D

            var frames = new byte[]
                             {
                                 0x31, 0x32, 0x33, 0x34, 0x00, 0x00, 0x00, 0x0B, 0x40, 0x00,
                                 0x49, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59
                             };

            byte[] completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            Assert.AreEqual(m_TagInfo.Frames.Count, 1);

            RawFrame frame = m_TagInfo.Frames[0];
            Assert.AreEqual(frame.Id, "1234");
            Assert.AreEqual(frame.Payload.Count, 11);
            Assert.IsTrue(frame.Options.FileAlterPreservation);
        }

        [Test]
        public void FrameDetectionTest1()
        {
            // ID,ID,ID,ID,S,S,S,S,F,F,
            // D,....,D

            var frames = new byte[]
                             {
                                 0x31, 0x32, 0x33, 0x34, 0x00, 0x00, 0x00, 0x0B, 0x00, 0x00,
                                 0x49, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59
                             };

            byte[] completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            Assert.AreEqual(m_TagInfo.Frames.Count, 1);

            RawFrame frame = m_TagInfo.Frames[0];
            Assert.AreEqual(frame.Id, "1234");
            Assert.AreEqual(frame.Payload.Count, 11);
        }

        [Test]
        public void FrameDetectionTest2()
        {
            // ID,ID,ID,ID,S,S,S,S,F,F,
            // D,....,D

            var frames = new byte[]
                             {
                                 0x54, 0x41, 0x4C, 0x42, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00,
                                 0x54, 0x50, 0x51, 0x52,
                                 0x54, 0x49, 0x54, 0x32, 0x00, 0x00, 0x00, 0x004, 0xE0, 0xE0,
                                 0x53, 0x54, 0x55, 0x56
                             };

            byte[] completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            RawFrame frame1 = m_TagInfo.Frames[0];
            RawFrame frame2 = m_TagInfo.Frames[1];

            Assert.AreEqual(m_TagInfo.Frames.Count, 2);
            // frame 1
            Assert.AreEqual(frame1.Id, "TALB");
            Assert.AreEqual(frame1.Payload.Count, 4);
            Assert.IsFalse(frame1.Options.Compression);
            Assert.IsFalse(frame1.Options.Encryption);
            Assert.IsFalse(frame1.Options.FileAlterPreservation);
            Assert.IsFalse(frame1.Options.GroupingIdentify);
            Assert.IsFalse(frame1.Options.ReadOnly);
            Assert.IsFalse(frame1.Options.TagAlterPreservation);
            // frame 2
            Assert.AreEqual(frame2.Id, "TIT2");
            Assert.AreEqual(frame2.Payload.Count, 4);
            Assert.IsTrue(frame2.Options.Compression);
            Assert.IsTrue(frame2.Options.Encryption);
            Assert.IsTrue(frame2.Options.FileAlterPreservation);
            Assert.IsTrue(frame2.Options.GroupingIdentify);
            Assert.IsTrue(frame2.Options.ReadOnly);
            Assert.IsTrue(frame2.Options.TagAlterPreservation);
        }

        [Test]
        public void GroupIdentityTest()
        {
            // ID,ID,ID,ID,S,S,S,S,F,F,
            // D,....,D

            var frames = new byte[]
                             {
                                 0x31, 0x32, 0x33, 0x34, 0x00, 0x00, 0x00, 0x0B, 0x00, 0x20,
                                 0x49, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59
                             };

            byte[] completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            Assert.AreEqual(m_TagInfo.Frames.Count, 1);

            RawFrame frame = m_TagInfo.Frames[0];
            Assert.AreEqual(frame.Id, "1234");
            Assert.AreEqual(frame.Payload.Count, 11);
            Assert.IsTrue(frame.Options.GroupingIdentify);
        }

        [Test]
        public void NullByteTest1()
        {
            // ID,ID,ID,ID,S,S,S,S,F,F,
            // D,....,D

            var frames = new byte[]
                             {
                                 0x31, 0x32, 0x33, 0x34, 0x00, 0x00, 0x00, 0x0B, 0x00, 0x20,
                                 0x49, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59,
                                 // No valid frame header!
                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 //,0x00
                             };

            /*
             *  Der letzte Frame Header ist nicht mehr komplett, weil er nur aus 9 Bytes besteht.
             *  Der Algorithmus ignoriert ihn, weil er am Ende des Streams ist.
             */

            byte[] completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            Assert.AreEqual(m_TagInfo.Frames.Count, 1);

            RawFrame frame = m_TagInfo.Frames[0];
            Assert.AreEqual(frame.Id, "1234");
            Assert.AreEqual(frame.Payload.Count, 11);
            Assert.IsTrue(frame.Options.GroupingIdentify);
        }

        [Test]
        public void NullByteTest2()
        {
            var frames = new byte[]
                             {
                                 0x31, 0x32, 0x33, 0x34, 0x00, 0x00, 0x00, 0x0B, 0x00, 0x20,
                                 0x49, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59,
                                 // No valid frame header!
                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                 0x35, 0x36, 0x37, 0x80, 0x00, 0x00, 0x00, 0x0B, 0x00, 0x20,
                                 0x49, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59,
                             };

            byte[] completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            Assert.AreEqual(m_TagInfo.Frames.Count, 1);

            RawFrame frame = m_TagInfo.Frames[0];
            Assert.AreEqual(frame.Id, "1234");
            Assert.AreEqual(frame.Payload.Count, 11);
            Assert.IsTrue(frame.Options.GroupingIdentify);
        }

        [Test]
        public void ReadOnlyTest()
        {
            // ID,ID,ID,ID,S,S,S,S,F,F,
            // D,....,D

            var frames = new byte[]
                             {
                                 0x31, 0x32, 0x33, 0x34, 0x00, 0x00, 0x00, 0x0B, 0x20, 0x00,
                                 0x49, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59
                             };

            byte[] completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            Assert.AreEqual(m_TagInfo.Frames.Count, 1);

            RawFrame frame = m_TagInfo.Frames[0];
            Assert.AreEqual(frame.Id, "1234");
            Assert.AreEqual(frame.Payload.Count, 11);
            Assert.IsTrue(frame.Options.ReadOnly);
        }

        [Test]
        public void TagAlterPreservationTest()
        {
            // ID,ID,ID,ID,S,S,S,S,F,F,
            // D,....,D

            var frames = new byte[]
                             {
                                 0x31, 0x32, 0x33, 0x34, 0x00, 0x00, 0x00, 0x0B, 0x80, 0x00,
                                 0x49, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59
                             };

            byte[] completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            Assert.AreEqual(m_TagInfo.Frames.Count, 1);

            RawFrame frame = m_TagInfo.Frames[0];
            Assert.AreEqual(frame.Id, "1234");
            Assert.AreEqual(frame.Payload.Count, 11);
            Assert.IsTrue(frame.Options.TagAlterPreservation);
        }
    }
}