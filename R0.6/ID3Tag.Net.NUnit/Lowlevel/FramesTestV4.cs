using NUnit.Framework;

namespace ID3Tag.Net.NUnit.Lowlevel
{
    [TestFixture]
    public class FramesTestV4 : Test
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            m_Controller = Id3TagFactory.CreateIoController();
        }

        #endregion

        [Test]
        public void CompressionTest()
        {
            // ID,ID,ID,ID,S,S,S,S,F,F,
            // D,....,D

            var frames = new byte[]
                             {
                                 0x31, 0x32, 0x33, 0x34, 0x00, 0x00, 0x00, 0x0B, 0x00, 0x08,
                                 0x49, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59
                             };

            var completeTag = GetCompleteV4Tag(frames);
            Read(completeTag);

            Assert.AreEqual(m_TagInfo.Frames.Count, 1);

            var frame = m_TagInfo.Frames[0];
            Assert.AreEqual(frame.ID, "1234");
            Assert.AreEqual(frame.Payload.Length, 11);

            Assert.IsFalse(frame.Flag.FileAlterPreservation);
            Assert.IsFalse(frame.Flag.TagAlterPreservation);
            Assert.IsFalse(frame.Flag.ReadOnly);
            Assert.IsTrue(frame.Flag.Compression);
            Assert.IsFalse(frame.Flag.Encryption);
            Assert.IsFalse(frame.Flag.GroupingIdentify);
            Assert.IsFalse(frame.Flag.Unsynchronisation);
            Assert.IsFalse(frame.Flag.DataLengthIndicator);
        }

        [Test]
        public void DataLengthIndicTest()
        {
            // ID,ID,ID,ID,S,S,S,S,F,F,
            // D,....,D

            var frames = new byte[]
                             {
                                 0x31, 0x32, 0x33, 0x34, 0x00, 0x00, 0x00, 0x0B, 0x00, 0x01,
                                 0x49, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59
                             };

            var completeTag = GetCompleteV4Tag(frames);
            Read(completeTag);

            Assert.AreEqual(m_TagInfo.Frames.Count, 1);

            var frame = m_TagInfo.Frames[0];
            Assert.AreEqual(frame.ID, "1234");
            Assert.AreEqual(frame.Payload.Length, 11);

            Assert.IsFalse(frame.Flag.FileAlterPreservation);
            Assert.IsFalse(frame.Flag.TagAlterPreservation);
            Assert.IsFalse(frame.Flag.ReadOnly);
            Assert.IsFalse(frame.Flag.Compression);
            Assert.IsFalse(frame.Flag.Encryption);
            Assert.IsFalse(frame.Flag.GroupingIdentify);
            Assert.IsFalse(frame.Flag.Unsynchronisation);
            Assert.IsTrue(frame.Flag.DataLengthIndicator);
        }

        [Test]
        public void EncryptionTest()
        {
            // ID,ID,ID,ID,S,S,S,S,F,F,
            // D,....,D

            var frames = new byte[]
                             {
                                 0x31, 0x32, 0x33, 0x34, 0x00, 0x00, 0x00, 0x0B, 0x00, 0x04,
                                 0x49, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59
                             };

            var completeTag = GetCompleteV4Tag(frames);
            Read(completeTag);

            Assert.AreEqual(m_TagInfo.Frames.Count, 1);

            var frame = m_TagInfo.Frames[0];
            Assert.AreEqual(frame.ID, "1234");
            Assert.AreEqual(frame.Payload.Length, 11);

            Assert.IsFalse(frame.Flag.FileAlterPreservation);
            Assert.IsFalse(frame.Flag.TagAlterPreservation);
            Assert.IsFalse(frame.Flag.ReadOnly);
            Assert.IsFalse(frame.Flag.Compression);
            Assert.IsTrue(frame.Flag.Encryption);
            Assert.IsFalse(frame.Flag.GroupingIdentify);
            Assert.IsFalse(frame.Flag.Unsynchronisation);
            Assert.IsFalse(frame.Flag.DataLengthIndicator);
        }

        [Test]
        public void FileAlterTest()
        {
            // ID,ID,ID,ID,S,S,S,S,F,F,
            // D,....,D

            var frames = new byte[]
                             {
                                 0x31, 0x32, 0x33, 0x34, 0x00, 0x00, 0x00, 0x0B, 0x20, 0x00,
                                 0x49, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59
                             };

            var completeTag = GetCompleteV4Tag(frames);
            Read(completeTag);

            Assert.AreEqual(m_TagInfo.Frames.Count, 1);

            var frame = m_TagInfo.Frames[0];
            Assert.AreEqual(frame.ID, "1234");
            Assert.AreEqual(frame.Payload.Length, 11);

            Assert.IsTrue(frame.Flag.FileAlterPreservation);
            Assert.IsFalse(frame.Flag.TagAlterPreservation);
            Assert.IsFalse(frame.Flag.ReadOnly);
            Assert.IsFalse(frame.Flag.Compression);
            Assert.IsFalse(frame.Flag.Encryption);
            Assert.IsFalse(frame.Flag.GroupingIdentify);
            Assert.IsFalse(frame.Flag.Unsynchronisation);
            Assert.IsFalse(frame.Flag.DataLengthIndicator);
        }

        [Test]
        public void GroupingIdTest()
        {
            // ID,ID,ID,ID,S,S,S,S,F,F,
            // D,....,D

            var frames = new byte[]
                             {
                                 0x31, 0x32, 0x33, 0x34, 0x00, 0x00, 0x00, 0x0B, 0x00, 0x40,
                                 0x49, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59
                             };

            var completeTag = GetCompleteV4Tag(frames);
            Read(completeTag);

            Assert.AreEqual(m_TagInfo.Frames.Count, 1);

            var frame = m_TagInfo.Frames[0];
            Assert.AreEqual(frame.ID, "1234");
            Assert.AreEqual(frame.Payload.Length, 11);

            Assert.IsFalse(frame.Flag.FileAlterPreservation);
            Assert.IsFalse(frame.Flag.TagAlterPreservation);
            Assert.IsFalse(frame.Flag.ReadOnly);
            Assert.IsFalse(frame.Flag.Compression);
            Assert.IsFalse(frame.Flag.Encryption);
            Assert.IsTrue(frame.Flag.GroupingIdentify);
            Assert.IsFalse(frame.Flag.Unsynchronisation);
            Assert.IsFalse(frame.Flag.DataLengthIndicator);
        }

        [Test]
        public void NothingTest()
        {
            // ID,ID,ID,ID,S,S,S,S,F,F,
            // D,....,D

            var frames = new byte[]
                             {
                                 0x31, 0x32, 0x33, 0x34, 0x00, 0x00, 0x00, 0x0B, 0x00, 0x00,
                                 0x49, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59
                             };

            var completeTag = GetCompleteV4Tag(frames);
            Read(completeTag);

            Assert.AreEqual(m_TagInfo.Frames.Count, 1);

            var frame = m_TagInfo.Frames[0];
            Assert.AreEqual(frame.ID, "1234");
            Assert.AreEqual(frame.Payload.Length, 11);

            Assert.IsFalse(frame.Flag.FileAlterPreservation);
            Assert.IsFalse(frame.Flag.TagAlterPreservation);
            Assert.IsFalse(frame.Flag.ReadOnly);
            Assert.IsFalse(frame.Flag.Compression);
            Assert.IsFalse(frame.Flag.Encryption);
            Assert.IsFalse(frame.Flag.GroupingIdentify);
            Assert.IsFalse(frame.Flag.Unsynchronisation);
            Assert.IsFalse(frame.Flag.DataLengthIndicator);
        }

        [Test]
        public void ReadOnlyTest()
        {
            // ID,ID,ID,ID,S,S,S,S,F,F,
            // D,....,D

            var frames = new byte[]
                             {
                                 0x31, 0x32, 0x33, 0x34, 0x00, 0x00, 0x00, 0x0B, 0x10, 0x00,
                                 0x49, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59
                             };

            var completeTag = GetCompleteV4Tag(frames);
            Read(completeTag);

            Assert.AreEqual(m_TagInfo.Frames.Count, 1);

            var frame = m_TagInfo.Frames[0];
            Assert.AreEqual(frame.ID, "1234");
            Assert.AreEqual(frame.Payload.Length, 11);

            Assert.IsFalse(frame.Flag.FileAlterPreservation);
            Assert.IsFalse(frame.Flag.TagAlterPreservation);
            Assert.IsTrue(frame.Flag.ReadOnly);
            Assert.IsFalse(frame.Flag.Compression);
            Assert.IsFalse(frame.Flag.Encryption);
            Assert.IsFalse(frame.Flag.GroupingIdentify);
            Assert.IsFalse(frame.Flag.Unsynchronisation);
            Assert.IsFalse(frame.Flag.DataLengthIndicator);
        }

        [Test]
        public void TagAlterTest()
        {
            // ID,ID,ID,ID,S,S,S,S,F,F,
            // D,....,D

            var frames = new byte[]
                             {
                                 0x31, 0x32, 0x33, 0x34, 0x00, 0x00, 0x00, 0x0B, 0x40, 0x00,
                                 0x49, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59
                             };

            var completeTag = GetCompleteV4Tag(frames);
            Read(completeTag);

            Assert.AreEqual(m_TagInfo.Frames.Count, 1);

            var frame = m_TagInfo.Frames[0];
            Assert.AreEqual(frame.ID, "1234");
            Assert.AreEqual(frame.Payload.Length, 11);

            Assert.IsFalse(frame.Flag.FileAlterPreservation);
            Assert.IsTrue(frame.Flag.TagAlterPreservation);
            Assert.IsFalse(frame.Flag.ReadOnly);
            Assert.IsFalse(frame.Flag.Compression);
            Assert.IsFalse(frame.Flag.Encryption);
            Assert.IsFalse(frame.Flag.GroupingIdentify);
            Assert.IsFalse(frame.Flag.Unsynchronisation);
            Assert.IsFalse(frame.Flag.DataLengthIndicator);
        }

        [Test]
        public void UnsyncTest()
        {
            // ID,ID,ID,ID,S,S,S,S,F,F,
            // D,....,D

            var frames = new byte[]
                             {
                                 0x31, 0x32, 0x33, 0x34, 0x00, 0x00, 0x00, 0x0B, 0x00, 0x02,
                                 0x49, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59
                             };

            var completeTag = GetCompleteV4Tag(frames);
            Read(completeTag);

            Assert.AreEqual(m_TagInfo.Frames.Count, 1);

            var frame = m_TagInfo.Frames[0];
            Assert.AreEqual(frame.ID, "1234");
            Assert.AreEqual(frame.Payload.Length, 11);

            Assert.IsFalse(frame.Flag.FileAlterPreservation);
            Assert.IsFalse(frame.Flag.TagAlterPreservation);
            Assert.IsFalse(frame.Flag.ReadOnly);
            Assert.IsFalse(frame.Flag.Compression);
            Assert.IsFalse(frame.Flag.Encryption);
            Assert.IsFalse(frame.Flag.GroupingIdentify);
            Assert.IsTrue(frame.Flag.Unsynchronisation);
            Assert.IsFalse(frame.Flag.DataLengthIndicator);
        }
    }
}