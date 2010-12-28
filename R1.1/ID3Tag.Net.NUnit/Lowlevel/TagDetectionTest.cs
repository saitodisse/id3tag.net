using System.Collections.Generic;
using System.IO;
using Id3Tag.LowLevel;
using NUnit.Framework;

namespace Id3Tag.Net.NUnit.Lowlevel
{
    [TestFixture]
    public class TagDetectionTest
    {
        private byte[] CreateDummyBytes(int count)
        {
            var bytes = new byte[count];
            for (int i = 0; i < count; i++)
            {
                bytes[i] = 0x31;
            }

            return bytes;
        }

        [Test]
        [ExpectedException(typeof (Id3IOException))]
        public void DetectNothing()
        {
            var audioStream = new List<byte>();

            // add dummy audio files
            audioStream.AddRange(new byte[] {0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x20});
            byte[] bytes = audioStream.ToArray();

            using (var stream = new MemoryStream(bytes))
            {
                IIOController ioController = Id3TagFactory.CreateIOController();
                ioController.DetermineTagStatus(stream);
            }
        }

        [Test]
        public void DetectTagV1()
        {
            var audioStream = new List<byte>();

            // add ID3 header
            audioStream.AddRange(CreateDummyBytes(100));
            audioStream.AddRange(new byte[] {0x54, 0x41, 0x47});
            audioStream.AddRange(CreateDummyBytes(125));

            byte[] bytes = audioStream.ToArray();

            using (var stream = new MemoryStream(bytes))
            {
                IIOController ioController = Id3TagFactory.CreateIOController();
                FileState states = ioController.DetermineTagStatus(stream);

                Assert.IsTrue(states.Id3V1TagFound);
                Assert.IsFalse(states.Id3V2TagFound);
            }
        }

        [Test]
        public void DetectTagV1AndV2()
        {
            var audioStream = new List<byte>();

            // add ID3 header
            audioStream.AddRange(new byte[] {0x49, 0x44, 0x33, 0x03, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00});
            audioStream.AddRange(CreateDummyBytes(200));
            audioStream.AddRange(new byte[] {0x54, 0x41, 0x47});
            audioStream.AddRange(CreateDummyBytes(125));

            byte[] bytes = audioStream.ToArray();

            using (var stream = new MemoryStream(bytes))
            {
                IIOController ioController = Id3TagFactory.CreateIOController();
                FileState states = ioController.DetermineTagStatus(stream);

                Assert.IsTrue(states.Id3V1TagFound);
                Assert.IsTrue(states.Id3V2TagFound);
            }
        }

        [Test]
        public void DetectTagV2()
        {
            var audioStream = new List<byte>();

            // add ID3 header
            audioStream.AddRange(new byte[] {0x49, 0x44, 0x33, 0x03, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00});
            audioStream.AddRange(CreateDummyBytes(200));

            byte[] bytes = audioStream.ToArray();

            using (var stream = new MemoryStream(bytes))
            {
                IIOController ioController = Id3TagFactory.CreateIOController();
                FileState states = ioController.DetermineTagStatus(stream);

                Assert.IsFalse(states.Id3V1TagFound);
                Assert.IsTrue(states.Id3V2TagFound);
            }
        }
    }
}