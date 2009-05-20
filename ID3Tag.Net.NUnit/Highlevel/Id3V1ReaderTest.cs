using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ID3Tag.Factory;
using NUnit.Framework;

namespace ID3Tag.Net.NUnit.Highlevel
{
    [TestFixture]
    public class Id3V1ReaderTest
    {

        [Test]
        [ExpectedException(typeof(ID3HeaderNotFoundException))]
        public void DetectInvalidID3Tag1()
        {
            var audioStream = new List<byte>();

            // add dummy audio files
            audioStream.AddRange(new byte[] { 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x20 });
            // First byte is wrong!
            audioStream.AddRange(new byte[] { 0x55, 0x41, 0x47 });
            audioStream.AddRange(CreateField(30, 0x31)); // Song title
            audioStream.AddRange(CreateField(30, 0x32)); // Artist
            audioStream.AddRange(CreateField(30, 0x33)); // Album
            audioStream.AddRange(CreateField(4, 0x34)); // Year
            audioStream.AddRange(CreateField(28, 0x35)); // Comment
            audioStream.Add(0x00);
            audioStream.Add(0x36);  // Track
            audioStream.Add(0x05); // Genre
            var bytes = audioStream.ToArray();

            using (var stream = new MemoryStream(bytes))
            {
                var id3Controller = Id3TagFactory.CreateId3V1Controller();
                var tag = id3Controller.Read(stream);
            }
        }

        [Test]
        [ExpectedException(typeof(ID3HeaderNotFoundException))]
        public void DetectInvalidID3Tag2()
        {
            var audioStream = new List<byte>();

            // add dummy audio files
            audioStream.AddRange(new byte[] { 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x20 });
            // Second byte is wrong!
            audioStream.AddRange(new byte[] { 0x54, 0x42, 0x47 });
            audioStream.AddRange(CreateField(30, 0x31)); // Song title
            audioStream.AddRange(CreateField(30, 0x32)); // Artist
            audioStream.AddRange(CreateField(30, 0x33)); // Album
            audioStream.AddRange(CreateField(4, 0x34)); // Year
            audioStream.AddRange(CreateField(28, 0x35)); // Comment
            audioStream.Add(0x00);
            audioStream.Add(0x36);  // Track
            audioStream.Add(0x05); // Genre
            var bytes = audioStream.ToArray();

            using (var stream = new MemoryStream(bytes))
            {
                var id3Controller = Id3TagFactory.CreateId3V1Controller();
                var tag = id3Controller.Read(stream);
            }
        }

        [Test]
        [ExpectedException(typeof(ID3HeaderNotFoundException))]
        public void DetectInvalidID3Tag3()
        {
            var audioStream = new List<byte>();

            // add dummy audio files
            audioStream.AddRange(new byte[] { 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x20 });
            // Third byte is wrong!
            audioStream.AddRange(new byte[] { 0x54, 0x41, 0x48 });
            audioStream.AddRange(CreateField(30, 0x31)); // Song title
            audioStream.AddRange(CreateField(30, 0x32)); // Artist
            audioStream.AddRange(CreateField(30, 0x33)); // Album
            audioStream.AddRange(CreateField(4, 0x34)); // Year
            audioStream.AddRange(CreateField(28, 0x35)); // Comment
            audioStream.Add(0x00);
            audioStream.Add(0x36);  // Track
            audioStream.Add(0x05); // Genre
            var bytes = audioStream.ToArray();

            using (var stream = new MemoryStream(bytes))
            {
                var id3Controller = Id3TagFactory.CreateId3V1Controller();
                var tag = id3Controller.Read(stream);
            }
        }

        [Test]
        public void ReadId3V1_1Test()
        {
            var audioStream = new List<byte>();

            // add dummy audio files
            audioStream.AddRange(new byte[] {0x10,0x11,0x12,0x13,0x14,0x15,0x16,0x17,0x18,0x19,0x20});
            audioStream.AddRange(new byte[] {0x54, 0x41, 0x47});
            audioStream.AddRange(CreateField(30, 0x31)); // Song title
            audioStream.AddRange(CreateField(30, 0x32)); // Artist
            audioStream.AddRange(CreateField(30, 0x33)); // Album
            audioStream.AddRange(CreateField(4, 0x34)); // Year
            audioStream.AddRange(CreateField(28, 0x35)); // Comment
            audioStream.Add(0x00);
            audioStream.Add(0x36);  // Track
            audioStream.Add(0x05); // Genre
            var bytes = audioStream.ToArray();

            using (var stream = new MemoryStream(bytes))
            {
                var id3Controller = Id3TagFactory.CreateId3V1Controller();
                var tag = id3Controller.Read(stream);

                Compare(tag.Title, '1',30);
                Compare(tag.Artist,'2',30);
                Compare(tag.Album,'3',30);
                Compare(tag.Year,'4',4);
                Compare(tag.Comment,'5',28);

                Assert.IsTrue(tag.IsID3V1_1Compliant);
                Assert.AreEqual(tag.TrackNr,"6");
                Assert.IsNotEmpty(tag.Genre);
            }
        }

        [Test]
        public void ReadId3V1Test()
        {
            var audioStream = new List<byte>();

            // add dummy audio files
            audioStream.AddRange(new byte[] { 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x20 });
            audioStream.AddRange(new byte[] { 0x54, 0x41, 0x47 });
            audioStream.AddRange(CreateField(30, 0x31)); // Song title
            audioStream.AddRange(CreateField(30, 0x32)); // Artist
            audioStream.AddRange(CreateField(30, 0x33)); // Album
            audioStream.AddRange(CreateField(4, 0x34)); // Year
            audioStream.AddRange(CreateField(30, 0x35)); // Comment
            audioStream.Add(0x05); // Genre
            var bytes = audioStream.ToArray();

            using (var stream = new MemoryStream(bytes))
            {
                var id3Controller = Id3TagFactory.CreateId3V1Controller();
                var tag = id3Controller.Read(stream);

                Compare(tag.Title, '1', 30);
                Compare(tag.Artist, '2', 30);
                Compare(tag.Album, '3', 30);
                Compare(tag.Year, '4', 4);
                Compare(tag.Comment, '5', 30);


                Assert.IsFalse(tag.IsID3V1_1Compliant);
                Assert.IsEmpty(tag.TrackNr);
                Assert.IsNotEmpty(tag.Genre);
            }
        }

        private void Compare(string value, char refChar, int count)
        {
            var refValue = new StringBuilder();
            for (int i=0; i<count; i++)
            {
                refValue.Append(refChar);
            }

            Assert.AreEqual(value,refValue.ToString(),"Compare operation failed : " + value);
        }

        private byte[] CreateField(int count, byte value)
        {
            var bytes = new byte[count];
            for (var i=0; i < count; i++)
            {
                bytes[i] = value;
            }

            return bytes;
        }
    }
}
