using System.IO;
using ID3Tag.Factory;
using ID3Tag.HighLevel;
using ID3Tag.HighLevel.ID3Frame;
using NUnit.Framework;

namespace ID3Tag.Net.NUnit.Lowlevel
{
    [TestFixture]
    public class WriterTest : Test
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            m_TagController = Id3TagFactory.CreateTagController();
            m_Controller = Id3TagFactory.CreateIoController();

            m_AudioData = new byte[0x10];
            FillData(m_AudioData);
        }

        #endregion

        private byte[] m_AudioData;

        private static void FillData(byte[] array)
        {
            byte curValue = 0;

            for (var i = 0; i < array.Length; i++)
            {
                array[i] = curValue;

                if (curValue < 0xFF)
                {
                    curValue++;
                }
                else
                {
                    curValue = 0;
                }
            }
        }

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
                var id3TagInfo = m_Controller.Read(tagStream);

                tagContainer2 = m_TagController.Decode(id3TagInfo);
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

            Assert.AreEqual(c1.Tag.MajorVersion, c2.Tag.MajorVersion);
            Assert.AreEqual(c1.Tag.Revision, c2.Tag.Revision);
            Assert.AreEqual(c1.Tag.ExperimentalIndicator, c2.Tag.ExperimentalIndicator);
            Assert.AreEqual(c1.Tag.ExtendedHeader, c2.Tag.ExtendedHeader);
            Assert.AreEqual(c1.Tag.Unsynchronisation, c2.Tag.Unsynchronisation);

            Assert.AreEqual(c1.Tag.CrcDataPresent, c2.Tag.CrcDataPresent);
            Assert.IsTrue(ComparePayload(c1.Tag.Crc, c2.Tag.Crc));
            Assert.AreEqual(c1.Tag.PaddingSize, c2.Tag.PaddingSize);
        }

        [Test]
        public void EncodeWithExtendedHeaderTest2()
        {
            //
            // Configure the tag
            //
            var tagContainer1 = new TagContainer();
            tagContainer1.Tag.SetVersion(3, 0);
            tagContainer1.Tag.SetHeaderFlags(true, true, true);
            tagContainer1.Tag.SetExtendedHeader(10, true, new byte[] {0x10, 0x20, 0x30, 0x40});

            var titleFrame = new TextFrame
                                 {
                                     Descriptor = {ID = "TIT2"},
                                     TextEncoding = TextEncodingType.UTF16,
                                     Content = "Title1"
                                 };

            tagContainer1.Add(titleFrame);

            //
            // Write and read the tag again. 
            //
            var tagContainer2 = WriteAndRead(tagContainer1);

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
            var tagContainer1 = new TagContainer();
            tagContainer1.Tag.SetVersion(3, 0);
            tagContainer1.Tag.SetHeaderFlags(false, false, false);

            var titleFrame = new TextFrame
                                 {
                                     Descriptor = {ID = "TIT2"},
                                     TextEncoding = TextEncodingType.UTF16,
                                     Content = "Title1"
                                 };

            tagContainer1.Add(titleFrame);

            //
            // Write and read the tag again. 
            //
            var tagContainer2 = WriteAndRead(tagContainer1);

            //
            // Compare both container!
            //
            CompareContainer(tagContainer1, tagContainer2);
        }

        [Test]
        public void SynchronizedTest1()
        {
            var tagContainer1 = new TagContainer();
            tagContainer1.Tag.SetVersion(3,0);
            tagContainer1.Tag.SetHeaderFlags(true,false,false);

            const long counter = 0xFF00FF12;
            var playCounter = new PlayCounterFrame(counter);
            tagContainer1.Add(playCounter);

            var tagContainer2 = WriteAndRead(tagContainer1);
            CompareContainer(tagContainer1,tagContainer2);

            var playCounter2 = FrameUtils.ConvertToPlayCounterFrame(tagContainer2[0]);
            Assert.AreEqual(playCounter2.Counter,counter);
        }

        [Test]
        public void SynchronizedTest2()
        {
            var tagContainer1 = new TagContainer();
            tagContainer1.Tag.SetVersion(3, 0);
            tagContainer1.Tag.SetHeaderFlags(true, false, false);

            const long counter = 0xFFFFFF12;
            var playCounter = new PlayCounterFrame(counter);
            tagContainer1.Add(playCounter);

            var tagContainer2 = WriteAndRead(tagContainer1);
            CompareContainer(tagContainer1, tagContainer2);

            var playCounter2 = FrameUtils.ConvertToPlayCounterFrame(tagContainer2[0]);
            Assert.AreEqual(playCounter2.Counter, counter);
        }

        [Test]
        public void SynchronizedTest3()
        {
            var tagContainer1 = new TagContainer();
            tagContainer1.Tag.SetVersion(3, 0);
            tagContainer1.Tag.SetHeaderFlags(true, false, false);

            const long counter = 0xFFE0;
            var playCounter = new PlayCounterFrame(counter);
            tagContainer1.Add(playCounter);

            var tagContainer2 = WriteAndRead(tagContainer1);
            CompareContainer(tagContainer1, tagContainer2);

            var playCounter2 = FrameUtils.ConvertToPlayCounterFrame(tagContainer2[0]);
            Assert.AreEqual(playCounter2.Counter, counter);
        }

        [Test]
        public void SynchronizedTest4()
        {
            var tagContainer1 = new TagContainer();
            tagContainer1.Tag.SetVersion(3, 0);
            tagContainer1.Tag.SetHeaderFlags(true, false, false);

            const long counter = 0xFFE1;
            var playCounter = new PlayCounterFrame(counter);
            tagContainer1.Add(playCounter);

            var tagContainer2 = WriteAndRead(tagContainer1);
            CompareContainer(tagContainer1, tagContainer2);

            var playCounter2 = FrameUtils.ConvertToPlayCounterFrame(tagContainer2[0]);
            Assert.AreEqual(playCounter2.Counter, counter);
        }

        [Test]
        public void SynchronizedTest5()
        {
            var tagContainer1 = new TagContainer();
            tagContainer1.Tag.SetVersion(3, 0);
            tagContainer1.Tag.SetHeaderFlags(true, false, false);

            const long counter = 0x12FF;
            var playCounter = new PlayCounterFrame(counter);
            tagContainer1.Add(playCounter);

            var tagContainer2 = WriteAndRead(tagContainer1);
            CompareContainer(tagContainer1, tagContainer2);

            var playCounter2 = FrameUtils.ConvertToPlayCounterFrame(tagContainer2[0]);
            Assert.AreEqual(playCounter2.Counter, counter);
        }
    }
}