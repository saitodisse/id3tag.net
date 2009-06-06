using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ID3Tag.Factory;
using ID3Tag.HighLevel;
using NUnit.Framework;

namespace ID3Tag.Net.NUnit.Highlevel
{
    [TestFixture]
    public class Id3V1Test
    {
        [Test]
        [ExpectedException(typeof(ID3IOException))]
        public void IgnoreTooShortStream()
        {
            // The ID3Tag contains of 128 Bytes. This is not
            // a valid tag. ( 127 bytes)

            var audioStream = new List<byte>();
            audioStream.AddRange(new byte[] { 0x54, 0x41, 0x47 });
            audioStream.AddRange(CreateField(124,0x35));

            var bytes = audioStream.ToArray();
            ReadFromStream(bytes);
        }

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
            ReadFromStream(bytes);
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
            ReadFromStream(bytes);
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
            ReadFromStream(bytes);
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
            audioStream.Add(0x06);  // Track
            audioStream.Add(0x05); // Genre
            var bytes = audioStream.ToArray();

            var tag = ReadFromStream(bytes);
            CheckID3Tag(tag.Title, '1', 30);
            CheckID3Tag(tag.Artist, '2', 30);
            CheckID3Tag(tag.Album, '3', 30);
            CheckID3Tag(tag.Year, '4', 4);
            CheckID3Tag(tag.Comment, '5', 28);

            Assert.IsTrue(tag.IsID3V1_1Compliant);
            Assert.AreEqual(tag.TrackNr, "6");
            Assert.IsNotEmpty(tag.Genre);
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

            var tag = ReadFromStream(bytes);

            CheckID3Tag(tag.Title, '1', 30);
            CheckID3Tag(tag.Artist, '2', 30);
            CheckID3Tag(tag.Album, '3', 30);
            CheckID3Tag(tag.Year, '4', 4);
            CheckID3Tag(tag.Comment, '5', 30);

            Assert.IsFalse(tag.IsID3V1_1Compliant);
            Assert.IsEmpty(tag.TrackNr);
            Assert.IsNotEmpty(tag.Genre);
        }

        [Test]
        public void WriteID3V1Tag()
        {
            var audioData = new byte[] { 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19};
            var output = new byte[138];

            // Create a byte array...
            var id3Tag1 = new Id3V1Tag
                             {
                                 Title = "1",
                                 Artist = "2",
                                 Album = "3",
                                 Year = "4",
                                 Comment = "5",
                                 IsID3V1_1Compliant = false,
                                 GenreIdentifier = 12
                             };

            WriteToStream(audioData, output, id3Tag1);
            var id3Tag2 = ReadFromStream(output);
            CheckID3Tag(id3Tag1, id3Tag2);
        }

        [Test]
        public void WriteID3V1_1Tag()
        {
            var audioData = new byte[] { 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19};
            var output = new byte[138];

            var id3Tag1 = new Id3V1Tag
                             {
                                 Title = "1",
                                 Artist = "2",
                                 Album = "3",
                                 Year = "4",
                                 Comment = "5",
                                 IsID3V1_1Compliant = true,
                                 TrackNr = "6",
                                 GenreIdentifier = 12
            };

            WriteToStream(audioData, output, id3Tag1);

            var id3Tag2 = ReadFromStream(output);
            CheckID3Tag(id3Tag1,id3Tag2);
        }

        [Test]
        public void WriteWithMaxFieldSize()
        {
            var audioData = new byte[] { 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19 };
            var output = new byte[138];

            var id3Tag1 = new Id3V1Tag
                              {
                                    Title = CreateFieldText(30, '1'),
                                    Artist = CreateFieldText(30,'2'),
                                    Album = CreateFieldText(30,'3'),
                                    Year = CreateFieldText(4,'4'),
                                    Comment = CreateFieldText(28,'5'),
                                    IsID3V1_1Compliant = true,
                                    TrackNr = "6",
                                    GenreIdentifier = 12
            };

            WriteToStream(audioData, output, id3Tag1);

            var id3Tag2 = ReadFromStream(output);
            CheckID3Tag(id3Tag1, id3Tag2);
        }

        [Test]
        public void WriteWithOverMaxFieldSize_ID3v11()
        {
            var audioData = new byte[] { 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19 };
            var output = new byte[138];

            var id3Tag1 = new Id3V1Tag
            {
                Title = CreateFieldText(31, '1'),
                Artist = CreateFieldText(31, '2'),
                Album = CreateFieldText(31, '3'),
                Year = CreateFieldText(5, '4'),
                Comment = CreateFieldText(29, '5'),
                IsID3V1_1Compliant = true,
                TrackNr = "6",
                GenreIdentifier = 12
            };

            WriteToStream(audioData, output, id3Tag1);

            //
            //  The coding if the fiels is over the limit. The I/O Controller cuts the string.
            //
            var refId3Tag = new Id3V1Tag
            {
                Title = CreateFieldText(30, '1'),
                Artist = CreateFieldText(30, '2'),
                Album = CreateFieldText(30, '3'),
                Year = CreateFieldText(4, '4'),
                Comment = CreateFieldText(28, '5'),
                IsID3V1_1Compliant = true,
                TrackNr = "6",
                GenreIdentifier = 12
            };

            var id3Tag2 = ReadFromStream(output);
            CheckID3Tag(refId3Tag, id3Tag2);
        }

        [Test]
        public void WriteWithOverMaxFieldSize_ID3v1()
        {
            var audioData = new byte[] { 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19 };
            var output = new byte[138];

            var id3Tag1 = new Id3V1Tag
            {
                Title = CreateFieldText(31, '1'),
                Artist = CreateFieldText(31, '2'),
                Album = CreateFieldText(31, '3'),
                Year = CreateFieldText(5, '4'),
                Comment = CreateFieldText(31, '5'),
                IsID3V1_1Compliant = false,
                GenreIdentifier = 12
            };

            WriteToStream(audioData, output, id3Tag1);

            //
            //  The coding if the fiels is over the limit. The I/O Controller cuts the string.
            //
            var refId3Tag = new Id3V1Tag
            {
                Title = CreateFieldText(30, '1'),
                Artist = CreateFieldText(30, '2'),
                Album = CreateFieldText(30, '3'),
                Year = CreateFieldText(4, '4'),
                Comment = CreateFieldText(30, '5'),
                IsID3V1_1Compliant = false,
                GenreIdentifier = 12
            };

            var id3Tag2 = ReadFromStream(output);
            CheckID3Tag(refId3Tag, id3Tag2);
        }

        [Test]
        public void WriteAudioContent1()
        {
            var audioData = new byte[10];
            var output = new byte[audioData.Length + 128];

            //
            // Create dummy audio content..
            //
            for (byte i = 0; i < audioData.Length; i++)
            {
                audioData[i] = i;
            }

            // Create a new tag description.
            var id3Tag1 = new Id3V1Tag
            {
                Title = "1",
                Artist = "2",
                Album = "3",
                Year = "4",
                Comment = "5",
                IsID3V1_1Compliant = false,
                GenreIdentifier = 12
            };

            WriteToStream(audioData, output, id3Tag1);

            for (byte i = 0; i < 10; i++)
            {
                Assert.AreEqual(output[i], i);
            }
        }

        [Test]
        public void WriteAudioContent2()
        {
            var audioData = new byte[150];
            var output = new byte[audioData.Length + 128];

            //
            // Create dummy audio content..
            //
            for (byte i = 0; i<audioData.Length;i++)
            {
                audioData[i] = i;
            }

            // Create a new tag description.
            var id3Tag1 = new Id3V1Tag
            {
                Title = "1",
                Artist = "2",
                Album = "3",
                Year = "4",
                Comment = "5",
                IsID3V1_1Compliant = false,
                GenreIdentifier = 12
            };

            WriteToStream(audioData, output, id3Tag1);

            for (byte i = 0; i < 150; i++)
            {
                Assert.AreEqual(output[i], i);
            }
        }

        #region Helper...


        private static void CheckID3Tag(string value, char refChar, int count)
        {
            var refValue = new StringBuilder();
            for (int i=0; i<count; i++)
            {
                refValue.Append(refChar);
            }

            Assert.AreEqual(value,refValue.ToString(),"CheckID3Tag operation failed : " + value);
        }

        private static void CheckID3Tag(Id3V1Tag tag1, Id3V1Tag tag2)
        {
            Assert.AreEqual(tag1.Album,tag2.Album);
            Assert.AreEqual(tag1.Artist,tag2.Artist);
            Assert.AreEqual(tag1.Comment,tag2.Comment);
            Assert.AreEqual(tag1.Genre, tag2.Genre);
            Assert.AreEqual(tag1.GenreIdentifier, tag2.GenreIdentifier);
            Assert.AreEqual(tag1.IsID3V1_1Compliant,tag2.IsID3V1_1Compliant);
            Assert.AreEqual(tag1.Title,tag2.Title);
            Assert.AreEqual(tag1.Year,tag2.Year);

            if (tag1.IsID3V1_1Compliant)
            {
                Assert.AreEqual(tag1.TrackNr,tag2.TrackNr);
            }
            else
            {
                Assert.IsNotNull(tag1);
                Assert.IsNotNull(tag2);
            }
        }

        private static byte[] CreateField(int count, byte value)
        {
            var bytes = new byte[count];
            for (var i=0; i < count; i++)
            {
                bytes[i] = value;
            }

            return bytes;
        }

        private static string CreateFieldText(int count, char c)
        {
            var sb = new StringBuilder();

            for (var counter = 0; counter < count; counter++)
            {
                sb.Append(c);
            }

            return sb.ToString();
        }

        private static void WriteToStream(byte[] audioData, byte[] output, Id3V1Tag id3Tag)
        {
            MemoryStream inputSteam = null;
            MemoryStream outputStream = null;
            try
            {
                inputSteam = new MemoryStream(audioData, false);
                outputStream = new MemoryStream(output, true);

                var id3Controller = Id3TagFactory.CreateId3V1Controller();
                id3Controller.Write(id3Tag, inputSteam, outputStream);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (inputSteam != null)
                {
                    inputSteam.Dispose();
                }

                if (outputStream != null)
                {
                    inputSteam.Dispose();
                }
            }
        }

        private static Id3V1Tag ReadFromStream(byte[] bytes)
        {
            Id3V1Tag tag;
            using (var stream = new MemoryStream(bytes))
            {
                var id3Controller = Id3TagFactory.CreateId3V1Controller();
                tag = id3Controller.Read(stream);
            }
            return tag;
        }

        #endregion
    }
}
