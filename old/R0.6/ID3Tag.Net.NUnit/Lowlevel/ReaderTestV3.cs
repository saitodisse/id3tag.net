﻿using System;
using NUnit.Framework;

namespace ID3Tag.Net.NUnit.Lowlevel
{
    [TestFixture]
    public class ReaderTestV3 : Test
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
            Assert.IsFalse(frame1.Flag.Compression);
            Assert.IsFalse(frame1.Flag.Encryption);
            Assert.IsFalse(frame1.Flag.FileAlterPreservation);
            Assert.IsFalse(frame1.Flag.GroupingIdentify);
            Assert.IsFalse(frame1.Flag.ReadOnly);
            Assert.IsFalse(frame1.Flag.TagAlterPreservation);
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
            Assert.IsFalse(frame1.Flag.Compression);
            Assert.IsFalse(frame1.Flag.Encryption);
            Assert.IsFalse(frame1.Flag.FileAlterPreservation);
            Assert.IsFalse(frame1.Flag.GroupingIdentify);
            Assert.IsFalse(frame1.Flag.ReadOnly);
            Assert.IsFalse(frame1.Flag.TagAlterPreservation);
            // frame 2
            Assert.AreEqual(frame2.ID, "5678");
            Assert.AreEqual(frame2.Payload.Length, 4);
            Assert.IsTrue(frame2.Flag.Compression);
            Assert.IsTrue(frame2.Flag.Encryption);
            Assert.IsTrue(frame2.Flag.FileAlterPreservation);
            Assert.IsTrue(frame2.Flag.GroupingIdentify);
            Assert.IsTrue(frame2.Flag.ReadOnly);
            Assert.IsTrue(frame2.Flag.TagAlterPreservation);
        }

        [Test]
        public void UnsynchronisationTest1()
        {
            // ID,ID,ID,ID,S,S,S,S,F,F,
            // D,....,D

            var frames = new byte[]
                             {
                                 0x49, 0x44, 0x33, 0x03, 0x00, 0x80, 0x00, 0x00, 0x00, 0x00,
                                 0x50, 0x43, 0x4E, 0x54, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00,
                                 0x12, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF
                             };

            var size = CalculateSize(frames.Length - 10);
            Array.Copy(size, 0, frames, 6, 4);

            Read(frames);

            Assert.AreEqual(m_TagInfo.Frames.Count, 1);
            Assert.AreEqual(m_TagInfo.Frames[0].ID, "PCNT");

            var payload = m_TagInfo.Frames[0].Payload;
            Assert.AreEqual(payload.Length, 5);
            Assert.AreEqual(payload[0], 0x12);
            Assert.AreEqual(payload[1], 0xFF);
            Assert.AreEqual(payload[2], 0xFF);
            Assert.AreEqual(payload[3], 0xFF);
            Assert.AreEqual(payload[4], 0xFF);
        }

        [Test]
        public void UnsynchronisationTest2()
        {
            // ID,ID,ID,ID,S,S,S,S,F,F,
            // D,....,D

            var frames = new byte[]
                             {
                                 0x49, 0x44, 0x33, 0x03, 0x00, 0x80, 0x00, 0x00, 0x00, 0x00,
                                 0x50, 0x43, 0x4E, 0x54, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00,
                                 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x02
                             };

            var size = CalculateSize(frames.Length - 10);
            Array.Copy(size, 0, frames, 6, 4);

            Read(frames);

            Assert.AreEqual(m_TagInfo.Frames.Count, 1);
            Assert.AreEqual(m_TagInfo.Frames[0].ID, "PCNT");

            var payload = m_TagInfo.Frames[0].Payload;
            Assert.AreEqual(payload.Length, 5);
            Assert.AreEqual(payload[0], 0xFF);
            Assert.AreEqual(payload[1], 0xFF);
            Assert.AreEqual(payload[2], 0xFF);
            Assert.AreEqual(payload[3], 0x02);
        }
    }
}