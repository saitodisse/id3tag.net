using System.Text;
using Id3Tag.HighLevel;
using Id3Tag.HighLevel.Id3Frame;
using NUnit.Framework;

namespace Id3Tag.Net.NUnit.Highlevel.FrameTest
{
    [TestFixture]
    public class CommentFrameTest : Test
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

            var commentFrame = FrameUtilities.ConvertToCommentFrame(tagContainer[0]);

            Assert.AreEqual(commentFrame.Descriptor.Id, "COMM");
            Assert.AreEqual(commentFrame.ContentDescriptor, "ABCD");
            Assert.AreEqual(commentFrame.TextEncoding.CodePage, Encoding.Default.CodePage);
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

            var commentFrame = FrameUtilities.ConvertToCommentFrame(tagContainer[0]);

            Assert.AreEqual(commentFrame.Language, "eng");
            Assert.AreEqual(commentFrame.Descriptor.Id, "COMM");
            Assert.AreEqual(commentFrame.ContentDescriptor, "ABCD");
            Assert.AreEqual(commentFrame.TextEncoding.CodePage, Encoding.BigEndianUnicode.CodePage);
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

            var commentFrame = FrameUtilities.ConvertToCommentFrame(tagContainer[0]);

            Assert.AreEqual(commentFrame.Language, "eng");
            Assert.AreEqual(commentFrame.Descriptor.Id, "COMM");
            Assert.AreEqual(commentFrame.ContentDescriptor, "ABCD");
            Assert.AreEqual(commentFrame.TextEncoding.CodePage, Encoding.BigEndianUnicode.CodePage);
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

            var commentFrame = FrameUtilities.ConvertToCommentFrame(tagContainer[0]);

            Assert.AreEqual(commentFrame.Language, "eng");
            Assert.AreEqual(commentFrame.Descriptor.Id, "COMM");
            Assert.AreEqual(commentFrame.TextEncoding.CodePage, Encoding.Unicode.CodePage);
			Assert.AreEqual(commentFrame.ContentDescriptor, "ABCD");
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

            var commentFrame = FrameUtilities.ConvertToCommentFrame(tagContainer[0]);

            Assert.AreEqual(commentFrame.Language, "eng");
            Assert.AreEqual(commentFrame.Descriptor.Id, "COMM");
            Assert.AreEqual(commentFrame.ContentDescriptor, "ABCD");
            Assert.AreEqual(commentFrame.TextEncoding.CodePage, Encoding.UTF8.CodePage);
            Assert.AreEqual(commentFrame.Text, "EFGH");
            Assert.AreEqual(commentFrame.Text.Length, 4);
        }

        [Test]
        public void ConvertISO8859_1Test()
        {
            var commentFrame = new CommentFrame();
            commentFrame.Descriptor.Id = "COMM";
            commentFrame.TextEncoding = Encoding.Default;
            commentFrame.ContentDescriptor = "ABCD";
            commentFrame.Text = "EFGH";
            commentFrame.Language = "eng";

            var rawFrame = commentFrame.Convert(TagVersion.Id3V23);
            Assert.AreEqual(rawFrame.Id, "COMM");

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
            commentFrame.Descriptor.Id = "COMM";
            commentFrame.TextEncoding = new UnicodeEncoding(true, false) ;
            commentFrame.ContentDescriptor = "ABCD";
            commentFrame.Text = "EFGH";
            commentFrame.Language = "eng";

            var rawFrame = commentFrame.Convert(TagVersion.Id3V23);
            Assert.AreEqual(rawFrame.Id, "COMM");

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
            commentFrame.Descriptor.Id = "COMM";
            commentFrame.TextEncoding = Encoding.Unicode;
            commentFrame.ContentDescriptor = "ABCD";
            commentFrame.Text = "EFGH";
            commentFrame.Language = "eng";

            var rawFrame = commentFrame.Convert(TagVersion.Id3V23);
            Assert.AreEqual(rawFrame.Id, "COMM");

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
            commentFrame.Descriptor.Id = "COMM";
            commentFrame.TextEncoding = new UTF8Encoding();
            commentFrame.ContentDescriptor = "ABCD";
            commentFrame.Text = "EFGH";
            commentFrame.Language = "eng";

            var rawFrame = commentFrame.Convert(TagVersion.Id3V23);
            Assert.AreEqual(rawFrame.Id, "COMM");

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
            var frame = new CommentFrame("ENG", "ABCD", "EFGH", Encoding.Unicode);

            Assert.AreEqual(frame.Descriptor.Id, "COMM");
            Assert.AreEqual(frame.Language, "ENG");
            Assert.AreEqual(frame.ContentDescriptor, "ABCD");
            Assert.AreEqual(frame.Text, "EFGH");
            Assert.AreEqual(frame.TextEncoding, Encoding.Unicode);
        }
    }
}