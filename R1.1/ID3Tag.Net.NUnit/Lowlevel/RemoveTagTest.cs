using System;
using System.IO;
using System.Text;
using Id3Tag.HighLevel;
using Id3Tag.HighLevel.Id3Frame;
using NUnit.Framework;

namespace Id3Tag.Net.NUnit.Lowlevel
{
    [TestFixture]
    public class RemoveTagTest : Test
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            m_TagController = Id3TagFactory.CreateTagController();
            m_Controller = Id3TagFactory.CreateIOController();

            m_AudioData = new byte[0x10];
            FillData(m_AudioData, 0xA0);
        }

        #endregion

        private byte[] GetContent()
        {
            // Creating valid tag frame first.
            TagContainer tagContainer = Id3TagFactory.CreateId3Tag(TagVersion.Id3V23);
            var textFrame = new TextFrame("TIT2", "My Title", Encoding.ASCII);
            tagContainer.Add(textFrame);

            // OK... Create a valid output stream
            Stream output = null;
            Stream audio = null;
            byte[] content = null;
            try
            {
                output = new MemoryStream(new byte[64000], true);
                audio = new MemoryStream(m_AudioData);

                m_Controller.Write(tagContainer, audio, output);

                output.Position = 0;
                using (var reader = new BinaryReader(output))
                {
                    long size = output.Length;
                    content = reader.ReadBytes(Convert.ToInt32(size));
                }
            }
            finally
            {
                if (audio != null)
                {
                    audio.Close();
                }

                if (output != null)
                {
                    output.Close();
                }
            }

            return content;
        }

        [Test]
        [ExpectedException(typeof (Id3HeaderNotFoundException))]
        public void DetectInvalidTagTest()
        {
            var content = new byte[] {0x34, 0x23, 0xff, 0x12, 0x12, 0x12};
            var result = new byte[64000];

            Stream inputStream = null;
            Stream outputStream = null;

            try
            {
                inputStream = new MemoryStream(content);
                outputStream = new MemoryStream(result);

                m_Controller.Remove(inputStream, outputStream);
            }
            finally
            {
                if (inputStream != null)
                {
                    inputStream.Close();
                }

                if (outputStream != null)
                {
                    outputStream.Close();
                }
            }
        }

        [Test]
        public void RemoteId3V2Test()
        {
            byte[] content = GetContent();
            var result = new byte[64000];

            Stream inputStream = null;
            Stream outputStream = null;

            try
            {
                inputStream = new MemoryStream(content);
                outputStream = new MemoryStream(result);

                m_Controller.Remove(inputStream, outputStream);
            }
            finally
            {
                if (inputStream != null)
                {
                    inputStream.Close();
                }

                if (outputStream != null)
                {
                    outputStream.Close();
                }
            }
        }
    }
}