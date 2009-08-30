using ID3Tag.HighLevel;
using ID3Tag.HighLevel.ID3Frame;
using NUnit.Framework;

namespace ID3Tag.Net.NUnit.Highlevel.FrameTest
{
    [TestFixture]
    public class CommentFrameTest : Test
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            m_TagController = Id3TagFactory.CreateTagController();
            m_Controller = Id3TagFactory.CreateIoController();
        }

        #endregion

        [Test]
        public void CommentFrameDetectionTest()
        {
            var frames = new byte[]
                             {
                                 // COMM
                                 0x43, 0x4F, 0x4D, 0x4D, 0x00, 0x00, 0x00, 0x0D, 0x00, 0x00,
                                 0x00, 0x65, 0x6E, 0x67, 0x41, 0x42, 0x43, 0x44, 0x00, 0x45,
                                 0x46, 0x47, 0x48
                             };

            var completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            var tagContainer = m_TagController.Decode(m_TagInfo);
            Assert.AreEqual(tagContainer.Count, 1);

            var commentFrame = FrameUtils.ConvertToCommentFrame(tagContainer[0]);

            Assert.AreEqual(commentFrame.Descriptor.ID, "COMM");
            Assert.AreEqual(commentFrame.ContentDescriptor, "ABCD");
            Assert.AreEqual(commentFrame.TextEncoding, TextEncodingType.Ansi);
            Assert.AreEqual(commentFrame.Text, "EFGH");
            Assert.AreEqual(commentFrame.Text.Length, 4);
        }

        [Test]
        public void CommentFrameDetectionUTF16BigEndianTest()
        {
            var frames = new byte[]
                             {
                                 // COMM
                                 0x43, 0x4F, 0x4D, 0x4D, 0x00, 0x00, 0x00, 0x18, 0x00, 0x00,
                                 0x01, 0x65, 0x6E, 0x67, 0xFE, 0xFF, 0x00, 0x41, 0x00, 0x42,
                                 0x00, 0x43, 0x00, 0x44, 0x00, 0x00, 0x00, 0x45, 0x00, 0x46,
                                 0x00, 0x47, 0x00, 0x48
                             };

            var completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            var tagContainer = m_TagController.Decode(m_TagInfo);
            Assert.AreEqual(tagContainer.Count, 1);

            var commentFrame = FrameUtils.ConvertToCommentFrame(tagContainer[0]);

            Assert.AreEqual(commentFrame.Language, "eng");
            Assert.AreEqual(commentFrame.Descriptor.ID, "COMM");
            Assert.AreEqual(commentFrame.ContentDescriptor, "ABCD");
            Assert.AreEqual(commentFrame.TextEncoding, TextEncodingType.UTF16);
            Assert.AreEqual(commentFrame.Text, "EFGH");
            Assert.AreEqual(commentFrame.Text.Length, 4);
        }

        [Test]
        public void CommentFrameDetectionUTF16BigEndianTypeTest()
        {
            var frames = new byte[]
                             {
                                 // COMM
                                 0x43, 0x4F, 0x4D, 0x4D, 0x00, 0x00, 0x00, 0x16, 0x00, 0x00,
                                 0x02, 0x65, 0x6E, 0x67, 0x00, 0x41, 0x00, 0x42,
                                 0x00, 0x43, 0x00, 0x44, 0x00, 0x00, 0x00, 0x45, 0x00, 0x46,
                                 0x00, 0x47, 0x00, 0x48
                             };

            var completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            var tagContainer = m_TagController.Decode(m_TagInfo);
            Assert.AreEqual(tagContainer.Count, 1);

            var commentFrame = FrameUtils.ConvertToCommentFrame(tagContainer[0]);

            Assert.AreEqual(commentFrame.Language, "eng");
            Assert.AreEqual(commentFrame.Descriptor.ID, "COMM");
            Assert.AreEqual(commentFrame.ContentDescriptor, "ABCD");
            Assert.AreEqual(commentFrame.TextEncoding, TextEncodingType.UTF16_BE);
            Assert.AreEqual(commentFrame.Text, "EFGH");
            Assert.AreEqual(commentFrame.Text.Length, 4);
        }

        [Test]
        public void CommentFrameDetectionUTF16LittleEndianTest()
        {
            var frames = new byte[]
                             {
                                 // COMM
                                 0x43, 0x4F, 0x4D, 0x4D, 0x00, 0x00, 0x00, 0x18, 0x00, 0x00,
                                 0x01, 0x65, 0x6E, 0x67, 0xFF, 0xFE, 0x41, 0x00, 0x42, 0x00,
                                 0x43, 0x00, 0x44, 0x00, 0x00, 0x00, 0x45, 0x00, 0x46, 0x00,
                                 0x47, 0x00, 0x48, 0x00
                             };

            var completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            var tagContainer = m_TagController.Decode(m_TagInfo);
            Assert.AreEqual(tagContainer.Count, 1);

            var commentFrame = FrameUtils.ConvertToCommentFrame(tagContainer[0]);

            Assert.AreEqual(commentFrame.Language, "eng");
            Assert.AreEqual(commentFrame.Descriptor.ID, "COMM");
            Assert.AreEqual(commentFrame.ContentDescriptor, "ABCD");
            Assert.AreEqual(commentFrame.TextEncoding, TextEncodingType.UTF16);
            Assert.AreEqual(commentFrame.Text, "EFGH");
            Assert.AreEqual(commentFrame.Text.Length, 4);
        }

        [Test]
        public void CommentFrameDetectionUTF8Test()
        {
            var frames = new byte[]
                             {
                                 // COMM
                                 0x43, 0x4F, 0x4D, 0x4D, 0x00, 0x00, 0x00, 0x0D, 0x00, 0x00,
                                 0x03, 0x65, 0x6E, 0x67, 0x41, 0x42, 0x43, 0x44,
                                 0x00, 0x45, 0x46, 0x47, 0x48
                             };

            var completeTag = GetCompleteV3Tag(frames);
            Read(completeTag);

            var tagContainer = m_TagController.Decode(m_TagInfo);
            Assert.AreEqual(tagContainer.Count, 1);

            var commentFrame = FrameUtils.ConvertToCommentFrame(tagContainer[0]);

            Assert.AreEqual(commentFrame.Language, "eng");
            Assert.AreEqual(commentFrame.Descriptor.ID, "COMM");
            Assert.AreEqual(commentFrame.ContentDescriptor, "ABCD");
            Assert.AreEqual(commentFrame.TextEncoding, TextEncodingType.UTF8);
            Assert.AreEqual(commentFrame.Text, "EFGH");
            Assert.AreEqual(commentFrame.Text.Length, 4);
        }

        [Test]
        public void ConvertISO8859_1Test()
        {
            var commentFrame = new CommentFrame();
            commentFrame.Descriptor.ID = "COMM";
            commentFrame.TextEncoding = TextEncodingType.Ansi;
            commentFrame.ContentDescriptor = "ABCD";
            commentFrame.Text = "EFGH";
            commentFrame.Language = "eng";

            var rawFrame = commentFrame.Convert(TagVersion.Id3V23);
            Assert.AreEqual(rawFrame.ID, "COMM");

            var refBytes = new byte[]
                               {
                                   0x00, 0x65, 0x6E, 0x67, 0x41, 0x42, 0x43, 0x44, 0x00, 0x45,
                                   0x46, 0x47, 0x48
                               };

            Assert.IsTrue(ComparePayload(rawFrame.Payload, refBytes));
        }

        [Test]
        public void ConvertUTF16BETest()
        {
            var commentFrame = new CommentFrame();
            commentFrame.Descriptor.ID = "COMM";
            commentFrame.TextEncoding = TextEncodingType.UTF16_BE;
            commentFrame.ContentDescriptor = "ABCD";
            commentFrame.Text = "EFGH";
            commentFrame.Language = "eng";

            var rawFrame = commentFrame.Convert(TagVersion.Id3V23);
            Assert.AreEqual(rawFrame.ID, "COMM");

            var refBytes = new byte[]
                               {
                                   0x02, 0x65, 0x6E, 0x67, 0x00, 0x41, 0x00, 0x42,
                                   0x00, 0x43, 0x00, 0x44, 0x00, 0x00, 0x00, 0x45, 0x00, 0x46,
                                   0x00, 0x47, 0x00, 0x48
                               };

            Assert.IsTrue(ComparePayload(rawFrame.Payload, refBytes));
        }

        [Test]
        public void ConvertUTF16Test()
        {
            var commentFrame = new CommentFrame();
            commentFrame.Descriptor.ID = "COMM";
            commentFrame.TextEncoding = TextEncodingType.UTF16;
            commentFrame.ContentDescriptor = "ABCD";
            commentFrame.Text = "EFGH";
            commentFrame.Language = "eng";

            var rawFrame = commentFrame.Convert(TagVersion.Id3V23);
            Assert.AreEqual(rawFrame.ID, "COMM");

            var refBytes = new byte[]
                               {
                                   0x01, 0x65, 0x6E, 0x67, 0xFF, 0xFE, 0x41, 0x00, 0x42, 0x00,
                                   0x43, 0x00, 0x44, 0x00, 0x00, 0x00, 0x45, 0x00, 0x46, 0x00,
                                   0x47, 0x00, 0x48, 0x00
                               };

            Assert.IsTrue(ComparePayload(rawFrame.Payload, refBytes));
        }

        [Test]
        public void ConvertUTF8Test()
        {
            var commentFrame = new CommentFrame();
            commentFrame.Descriptor.ID = "COMM";
            commentFrame.TextEncoding = TextEncodingType.UTF8;
            commentFrame.ContentDescriptor = "ABCD";
            commentFrame.Text = "EFGH";
            commentFrame.Language = "eng";

            var rawFrame = commentFrame.Convert(TagVersion.Id3V23);
            Assert.AreEqual(rawFrame.ID, "COMM");

            var refBytes = new byte[]
                               {
                                   0x03, 0x65, 0x6E, 0x67, 0x41, 0x42,
                                   0x43, 0x44, 0x00, 0x45, 0x46, 0x47,
                                   0x48
                               };

            Assert.IsTrue(ComparePayload(rawFrame.Payload, refBytes));
        }

        [Test]
        public void CreateTest()
        {
            var frame = new CommentFrame("ENG", "ABCD", "EFGH", TextEncodingType.UTF16, 0);

            Assert.AreEqual(frame.Descriptor.ID, "COMM");
            Assert.AreEqual(frame.Language, "ENG");
            Assert.AreEqual(frame.ContentDescriptor, "ABCD");
            Assert.AreEqual(frame.Text, "EFGH");
            Assert.AreEqual(frame.TextEncoding, TextEncodingType.UTF16);
        }
    }
}