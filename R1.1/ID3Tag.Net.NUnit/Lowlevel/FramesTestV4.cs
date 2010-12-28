using Id3Tag.LowLevel;
using NUnit.Framework;

namespace Id3Tag.Net.NUnit.Lowlevel
{
    [TestFixture]
    public class FramesTestV4 : Test
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
                                 0x31, 0x32, 0x33, 0x34, 0x00, 0x00, 0x00, 0x0B, 0x00, 0x08,
                                 0x49, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59
                             };

            byte[] completeTag = GetCompleteV4Tag(frames);
            Read(completeTag);

            Assert.AreEqual(m_TagInfo.Frames.Count, 1);

            RawFrame frame = m_TagInfo.Frames[0];
            Assert.AreEqual(frame.Id, "1234");
            Assert.AreEqual(frame.Payload.Count, 11);

            Assert.IsFalse(frame.Options.FileAlterPreservation);
            Assert.IsFalse(frame.Options.TagAlterPreservation);
            Assert.IsFalse(frame.Options.ReadOnly);
            Assert.IsTrue(frame.Options.Compression);
            Assert.IsFalse(frame.Options.Encryption);
            Assert.IsFalse(frame.Options.GroupingIdentify);
            Assert.IsFalse(frame.Options.Unsynchronisation);
            Assert.IsFalse(frame.Options.DataLengthIndicator);
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

            byte[] completeTag = GetCompleteV4Tag(frames);
            Read(completeTag);

            Assert.AreEqual(m_TagInfo.Frames.Count, 1);

            RawFrame frame = m_TagInfo.Frames[0];
            Assert.AreEqual(frame.Id, "1234");
            Assert.AreEqual(frame.Payload.Count, 11);

            Assert.IsFalse(frame.Options.FileAlterPreservation);
            Assert.IsFalse(frame.Options.TagAlterPreservation);
            Assert.IsFalse(frame.Options.ReadOnly);
            Assert.IsFalse(frame.Options.Compression);
            Assert.IsFalse(frame.Options.Encryption);
            Assert.IsFalse(frame.Options.GroupingIdentify);
            Assert.IsFalse(frame.Options.Unsynchronisation);
            Assert.IsTrue(frame.Options.DataLengthIndicator);
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

            byte[] completeTag = GetCompleteV4Tag(frames);
            Read(completeTag);

            Assert.AreEqual(m_TagInfo.Frames.Count, 1);

            RawFrame frame = m_TagInfo.Frames[0];
            Assert.AreEqual(frame.Id, "1234");
            Assert.AreEqual(frame.Payload.Count, 11);

            Assert.IsFalse(frame.Options.FileAlterPreservation);
            Assert.IsFalse(frame.Options.TagAlterPreservation);
            Assert.IsFalse(frame.Options.ReadOnly);
            Assert.IsFalse(frame.Options.Compression);
            Assert.IsTrue(frame.Options.Encryption);
            Assert.IsFalse(frame.Options.GroupingIdentify);
            Assert.IsFalse(frame.Options.Unsynchronisation);
            Assert.IsFalse(frame.Options.DataLengthIndicator);
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

            byte[] completeTag = GetCompleteV4Tag(frames);
            Read(completeTag);

            Assert.AreEqual(m_TagInfo.Frames.Count, 1);

            RawFrame frame = m_TagInfo.Frames[0];
            Assert.AreEqual(frame.Id, "1234");
            Assert.AreEqual(frame.Payload.Count, 11);

            Assert.IsTrue(frame.Options.FileAlterPreservation);
            Assert.IsFalse(frame.Options.TagAlterPreservation);
            Assert.IsFalse(frame.Options.ReadOnly);
            Assert.IsFalse(frame.Options.Compression);
            Assert.IsFalse(frame.Options.Encryption);
            Assert.IsFalse(frame.Options.GroupingIdentify);
            Assert.IsFalse(frame.Options.Unsynchronisation);
            Assert.IsFalse(frame.Options.DataLengthIndicator);
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

            byte[] completeTag = GetCompleteV4Tag(frames);
            Read(completeTag);

            Assert.AreEqual(m_TagInfo.Frames.Count, 1);

            RawFrame frame = m_TagInfo.Frames[0];
            Assert.AreEqual(frame.Id, "1234");
            Assert.AreEqual(frame.Payload.Count, 11);

            Assert.IsFalse(frame.Options.FileAlterPreservation);
            Assert.IsFalse(frame.Options.TagAlterPreservation);
            Assert.IsFalse(frame.Options.ReadOnly);
            Assert.IsFalse(frame.Options.Compression);
            Assert.IsFalse(frame.Options.Encryption);
            Assert.IsTrue(frame.Options.GroupingIdentify);
            Assert.IsFalse(frame.Options.Unsynchronisation);
            Assert.IsFalse(frame.Options.DataLengthIndicator);
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

            byte[] completeTag = GetCompleteV4Tag(frames);
            Read(completeTag);

            Assert.AreEqual(m_TagInfo.Frames.Count, 1);

            RawFrame frame = m_TagInfo.Frames[0];
            Assert.AreEqual(frame.Id, "1234");
            Assert.AreEqual(frame.Payload.Count, 11);

            Assert.IsFalse(frame.Options.FileAlterPreservation);
            Assert.IsFalse(frame.Options.TagAlterPreservation);
            Assert.IsFalse(frame.Options.ReadOnly);
            Assert.IsFalse(frame.Options.Compression);
            Assert.IsFalse(frame.Options.Encryption);
            Assert.IsFalse(frame.Options.GroupingIdentify);
            Assert.IsFalse(frame.Options.Unsynchronisation);
            Assert.IsFalse(frame.Options.DataLengthIndicator);
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

            byte[] completeTag = GetCompleteV4Tag(frames);
            Read(completeTag);

            Assert.AreEqual(m_TagInfo.Frames.Count, 1);

            RawFrame frame = m_TagInfo.Frames[0];
            Assert.AreEqual(frame.Id, "1234");
            Assert.AreEqual(frame.Payload.Count, 11);

            Assert.IsFalse(frame.Options.FileAlterPreservation);
            Assert.IsFalse(frame.Options.TagAlterPreservation);
            Assert.IsTrue(frame.Options.ReadOnly);
            Assert.IsFalse(frame.Options.Compression);
            Assert.IsFalse(frame.Options.Encryption);
            Assert.IsFalse(frame.Options.GroupingIdentify);
            Assert.IsFalse(frame.Options.Unsynchronisation);
            Assert.IsFalse(frame.Options.DataLengthIndicator);
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

            byte[] completeTag = GetCompleteV4Tag(frames);
            Read(completeTag);

            Assert.AreEqual(m_TagInfo.Frames.Count, 1);

            RawFrame frame = m_TagInfo.Frames[0];
            Assert.AreEqual(frame.Id, "1234");
            Assert.AreEqual(frame.Payload.Count, 11);

            Assert.IsFalse(frame.Options.FileAlterPreservation);
            Assert.IsTrue(frame.Options.TagAlterPreservation);
            Assert.IsFalse(frame.Options.ReadOnly);
            Assert.IsFalse(frame.Options.Compression);
            Assert.IsFalse(frame.Options.Encryption);
            Assert.IsFalse(frame.Options.GroupingIdentify);
            Assert.IsFalse(frame.Options.Unsynchronisation);
            Assert.IsFalse(frame.Options.DataLengthIndicator);
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

            byte[] completeTag = GetCompleteV4Tag(frames);
            Read(completeTag);

            Assert.AreEqual(m_TagInfo.Frames.Count, 1);

            RawFrame frame = m_TagInfo.Frames[0];
            Assert.AreEqual(frame.Id, "1234");
            Assert.AreEqual(frame.Payload.Count, 11);

            Assert.IsFalse(frame.Options.FileAlterPreservation);
            Assert.IsFalse(frame.Options.TagAlterPreservation);
            Assert.IsFalse(frame.Options.ReadOnly);
            Assert.IsFalse(frame.Options.Compression);
            Assert.IsFalse(frame.Options.Encryption);
            Assert.IsFalse(frame.Options.GroupingIdentify);
            Assert.IsTrue(frame.Options.Unsynchronisation);
            Assert.IsFalse(frame.Options.DataLengthIndicator);
        }
    }
}