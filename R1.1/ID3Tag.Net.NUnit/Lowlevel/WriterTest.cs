using System.IO;
using System.Text;
using Id3Tag.HighLevel;
using Id3Tag.HighLevel.Id3Frame;
using Id3Tag.LowLevel;
using NUnit.Framework;

namespace Id3Tag.Net.NUnit.Lowlevel
{
    [TestFixture]
    public class WriterTest : Test
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            m_TagController = Id3TagFactory.CreateTagController();
            m_Controller = Id3TagFactory.CreateIOController();

            m_AudioData = new byte[0x10];
            FillData(m_AudioData);
        }

        #endregion

        private TagContainer WriteAndRead(TagContainer tagContainer1)
        {
            TagContainer tagContainer2;
            Stream dataStream = null;
            Stream tagStream = null;
            try
            {
                dataStream = new MemoryStream(m_AudioData);
                tagStream = new MemoryStream(64000);

                // Write the content to a byte stream.
                m_Controller.Write(tagContainer1, dataStream, tagStream);

                // Read the bytes again
                tagStream.Position = 0;
                Id3TagInfo Id3TagInfo = m_Controller.Read(tagStream);

                tagContainer2 = m_TagController.Decode(Id3TagInfo);
            }
            finally
            {
                if (dataStream != null)
                {
                    dataStream.Close();
                    dataStream.Dispose();
                }

                if (tagStream != null)
                {
                    tagStream.Close();
                    tagStream.Dispose();
                }
            }
            return tagContainer2;
        }

        private static void CompareContainer(TagContainer c1, TagContainer c2)
        {
            Assert.AreEqual(c1.Count, c2.Count);
            TagDescriptorV3 c1Tag = c1.GetId3V23Descriptor();
            TagDescriptorV3 c2Tag = c2.GetId3V23Descriptor();

            Assert.AreEqual(c1Tag.MajorVersion, c2Tag.MajorVersion);
            Assert.AreEqual(c1Tag.Revision, c2Tag.Revision);
            Assert.AreEqual(c1Tag.ExperimentalIndicator, c2Tag.ExperimentalIndicator);
            Assert.AreEqual(c1Tag.ExtendedHeader, c2Tag.ExtendedHeader);
            Assert.AreEqual(c1Tag.Unsynchronisation, c2Tag.Unsynchronisation);

            Assert.AreEqual(c1Tag.CrcDataPresent, c2Tag.CrcDataPresent);
            Assert.IsTrue(ComparePayload(c1Tag.Crc, c2Tag.Crc));
            Assert.AreEqual(c1Tag.PaddingSize, c2Tag.PaddingSize);
        }

        [Test]
        public void EncodeWithExtendedHeaderTest2()
        {
            //
            // Configure the tag
            //
            var tagContainer1 = new TagContainerV3();
            tagContainer1.Tag.SetHeaderOptions(true, true, true);
            tagContainer1.Tag.SetExtendedHeader(10, true);
            tagContainer1.Tag.SetCrc32(new byte[] {0x10, 0x20, 0x30, 0x40});

            var titleFrame = new TextFrame
                                 {
                                     Descriptor = {Id = "TIT2"},
                                     TextEncoding = Encoding.Unicode,
                                     Content = "Title1"
                                 };

            tagContainer1.Add(titleFrame);

            //
            // Write and read the tag again. 
            //
            TagContainer tagContainer2 = WriteAndRead(tagContainer1);

            //
            // Compare both container!
            //
            CompareContainer(tagContainer1, tagContainer2);
        }

        [Test]
        public void EncodeWithoutExtendedHeaderTest()
        {
            //
            // Configure the tag
            //
            var tagContainer1 = new TagContainerV3();
            tagContainer1.Tag.SetHeaderOptions(false, false, false);

            var titleFrame = new TextFrame
                                 {
                                     Descriptor = {Id = "TIT2"},
                                     TextEncoding = Encoding.Unicode,
                                     Content = "Title1"
                                 };

            tagContainer1.Add(titleFrame);

            //
            // Write and read the tag again. 
            //
            TagContainer tagContainer2 = WriteAndRead(tagContainer1);

            //
            // Compare both container!
            //
            CompareContainer(tagContainer1, tagContainer2);
        }

        [Test]
        public void SynchronizedTest1()
        {
            var tagContainer1 = new TagContainerV3();
            tagContainer1.Tag.SetHeaderOptions(true, false, false);

            const long counter = 0xFF00FF12;
            var playCounter = new PlayCounterFrame(counter);
            tagContainer1.Add(playCounter);

            TagContainer tagContainer2 = WriteAndRead(tagContainer1);
            CompareContainer(tagContainer1, tagContainer2);

            PlayCounterFrame playCounter2 = FrameUtilities.ConvertToPlayCounterFrame(tagContainer2[0]);
            Assert.AreEqual(playCounter2.Counter, counter);
        }

        [Test]
        public void SynchronizedTest2()
        {
            var tagContainer1 = new TagContainerV3();
            tagContainer1.Tag.SetHeaderOptions(true, false, false);

            const long counter = 0xFFFFFF12;
            var playCounter = new PlayCounterFrame(counter);
            tagContainer1.Add(playCounter);

            TagContainer tagContainer2 = WriteAndRead(tagContainer1);
            CompareContainer(tagContainer1, tagContainer2);

            PlayCounterFrame playCounter2 = FrameUtilities.ConvertToPlayCounterFrame(tagContainer2[0]);
            Assert.AreEqual(playCounter2.Counter, counter);
        }

        [Test]
        public void SynchronizedTest3()
        {
            var tagContainer1 = new TagContainerV3();
            tagContainer1.Tag.SetHeaderOptions(true, false, false);

            const long counter = 0xFFE0;
            var playCounter = new PlayCounterFrame(counter);
            tagContainer1.Add(playCounter);

            TagContainer tagContainer2 = WriteAndRead(tagContainer1);
            CompareContainer(tagContainer1, tagContainer2);

            PlayCounterFrame playCounter2 = FrameUtilities.ConvertToPlayCounterFrame(tagContainer2[0]);
            Assert.AreEqual(playCounter2.Counter, counter);
        }

        [Test]
        public void SynchronizedTest4()
        {
            var tagContainer1 = new TagContainerV3();
            tagContainer1.Tag.SetHeaderOptions(true, false, false);

            const long counter = 0xFFE1;
            var playCounter = new PlayCounterFrame(counter);
            tagContainer1.Add(playCounter);

            TagContainer tagContainer2 = WriteAndRead(tagContainer1);
            CompareContainer(tagContainer1, tagContainer2);

            PlayCounterFrame playCounter2 = FrameUtilities.ConvertToPlayCounterFrame(tagContainer2[0]);
            Assert.AreEqual(playCounter2.Counter, counter);
        }

        [Test]
        public void SynchronizedTest5()
        {
            var tagContainer1 = new TagContainerV3();
            tagContainer1.Tag.SetHeaderOptions(true, false, false);

            const long counter = 0x12FF;
            var playCounter = new PlayCounterFrame(counter);
            tagContainer1.Add(playCounter);

            TagContainer tagContainer2 = WriteAndRead(tagContainer1);
            CompareContainer(tagContainer1, tagContainer2);

            PlayCounterFrame playCounter2 = FrameUtilities.ConvertToPlayCounterFrame(tagContainer2[0]);
            Assert.AreEqual(playCounter2.Counter, counter);
        }
    }
}