using System;
using Id3Tag.HighLevel;
using Id3Tag.HighLevel.Id3Frame;
using NUnit.Framework;

namespace Id3Tag.Net.NUnit.Highlevel.FrameTest
{
    [TestFixture]
    public class UniqueFileIdentifierTest : Test
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            m_TagController = Id3TagFactory.CreateTagController();
            m_Controller = Id3TagFactory.CreateIOController();
        }

        #endregion

        [Test]
        public void ConstructorTest1()
        {
            var frame = new UniqueFileIdentifierFrame();
            Assert.AreEqual(frame.Descriptor.Id, "UFID");
            Assert.IsNotNull(frame.Identifier);
            Assert.IsEmpty(frame.Owner);
            Assert.AreEqual(frame.Type, FrameType.UniqueFileIdentifier);
        }

        [Test]
        public void ConstructorTest2()
        {
            var bytes = new byte[] {0x10, 0x11, 0x12, 0x13, 0x14};
            var frame = new UniqueFileIdentifierFrame("owner", bytes);
            Assert.AreEqual(frame.Descriptor.Id, "UFID");
            Assert.AreEqual(frame.Owner, "owner");
            Assert.IsTrue(ComparePayload(frame.Identifier, bytes));
            Assert.AreEqual(frame.Type, FrameType.UniqueFileIdentifier);
        }

        [Test]
        public void ConvertTest1()
        {
            const string owner = "1234";
            var identifier = new byte[] {0x10, 0x11, 0x12};
            var frame = new UniqueFileIdentifierFrame(owner, identifier);

            var rawFrame = frame.Convert(TagVersion.Id3V23);
            Assert.AreEqual(rawFrame.Id, "UFID");

            var payload = rawFrame.Payload;
            var refBytes = new byte[] {0x31, 0x32, 0x33, 0x34, 0x00, 0x10, 0x11, 0x12};

            Assert.IsTrue(ComparePayload(payload, refBytes));
        }

        [Test]
        public void ConvertTest2()
        {
            var owner = String.Empty;
            var identifier = new byte[] {0x10, 0x11, 0x12};
            var frame = new UniqueFileIdentifierFrame(owner, identifier);

            var rawFrame = frame.Convert(TagVersion.Id3V23);
            Assert.AreEqual(rawFrame.Id, "UFID");

            var payload = rawFrame.Payload;
            var refBytes = new byte[] {0x00, 0x10, 0x11, 0x12};

            Assert.IsTrue(ComparePayload(payload, refBytes));
        }

        [Test]
        public void ImportTest1()
        {
            var frames = new byte[]
                             {
                                 0x55, 0x46, 0x49, 0x44, 0x00, 0x00, 0x00, 0x0A, 0x00, 0x00,
                                 0x31, 0x32, 0x33, 0x34, 0x00, 0x10, 0x11, 0x12, 0x13, 0x14
                             };

            var completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            var tagContainer = m_TagController.Decode(m_TagInfo);
            Assert.AreEqual(tagContainer.Count, 1);

            var frame = tagContainer[0];
            var ufid = FrameUtilities.ConvertToUniqueIdentifierFrame(frame);

            Assert.AreEqual(ufid.Descriptor.Id, "UFID");
            Assert.AreEqual(ufid.Owner, "1234");

            var refBytes = new byte[] {0x10, 0x11, 0x12, 0x13, 0x14};
            Assert.IsTrue(ComparePayload(refBytes, ufid.Identifier));
        }

        [Test]
        public void ImportTest2()
        {
            var frames = new byte[]
                             {
                                 0x55, 0x46, 0x49, 0x44, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00,
                                 0x00, 0x10, 0x11, 0x12, 0x13, 0x14
                             };

            var completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            var tagContainer = m_TagController.Decode(m_TagInfo);
            Assert.AreEqual(tagContainer.Count, 1);

            var frame = tagContainer[0];
            var ufid = FrameUtilities.ConvertToUniqueIdentifierFrame(frame);

            Assert.AreEqual(ufid.Descriptor.Id, "UFID");
            Assert.AreEqual(ufid.Owner, "");

            var refBytes = new byte[] {0x10, 0x11, 0x12, 0x13, 0x14};
            Assert.IsTrue(ComparePayload(refBytes, ufid.Identifier));
        }

        [Test]
        public void ImportTest3()
        {
            var frames = new byte[]
                             {
                                 0x55, 0x46, 0x49, 0x44, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00,
                                 0x31, 0x32, 0x33, 0x34, 0x00
                             };

            var completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            var tagContainer = m_TagController.Decode(m_TagInfo);
            Assert.AreEqual(tagContainer.Count, 1);

            var frame = tagContainer[0];
            var ufid = FrameUtilities.ConvertToUniqueIdentifierFrame(frame);

            Assert.AreEqual(ufid.Descriptor.Id, "UFID");
            Assert.AreEqual(ufid.Owner, "1234");

            var refBytes = new byte[0];
            Assert.IsTrue(ComparePayload(refBytes, ufid.Identifier));
        }
    }
}