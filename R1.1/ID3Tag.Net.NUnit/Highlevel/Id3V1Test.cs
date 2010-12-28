using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Id3Tag.HighLevel;
using NUnit.Framework;

namespace Id3Tag.Net.NUnit.Highlevel
{
    [TestFixture]
    public class Id3V1Test
    {
        private static void CheckId3Tag(string value, char refChar, int count)
        {
            var refValue = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                refValue.Append(refChar);
            }

            Assert.AreEqual(value, refValue.ToString(), "CheckId3Tag operation failed : " + value);
        }

        private static void CheckId3Tag(Id3V1Tag tag1, Id3V1Tag tag2)
        {
            Assert.AreEqual(tag1.Album, tag2.Album);
            Assert.AreEqual(tag1.Artist, tag2.Artist);
            Assert.AreEqual(tag1.Comment, tag2.Comment);
            Assert.AreEqual(tag1.Genre, tag2.Genre);
            Assert.AreEqual(tag1.GenreIdentifier, tag2.GenreIdentifier);
            Assert.AreEqual(tag1.IsId3V1Dot1Compliant, tag2.IsId3V1Dot1Compliant);
            Assert.AreEqual(tag1.Title, tag2.Title);
            Assert.AreEqual(tag1.Year, tag2.Year);

            if (tag1.IsId3V1Dot1Compliant)
            {
                Assert.AreEqual(tag1.TrackNumber, tag2.TrackNumber);
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
            for (int i = 0; i < count; i++)
            {
                bytes[i] = value;
            }

            return bytes;
        }

        private static string CreateFieldText(int count, char c)
        {
            var sb = new StringBuilder();

            for (int counter = 0; counter < count; counter++)
            {
                sb.Append(c);
            }

            return sb.ToString();
        }

        private static void WriteToStream(byte[] audioData, byte[] output, Id3V1Tag Id3Tag)
        {
            MemoryStream inputSteam = null;
            MemoryStream outputStream = null;
            try
            {
                inputSteam = new MemoryStream(audioData, false);
                outputStream = new MemoryStream(output, true);

                IId3V1Controller id3Controller = Id3TagFactory.CreateId3V1Controller();
                id3Controller.Write(Id3Tag, inputSteam, outputStream, 0);
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
                IId3V1Controller id3Controller = Id3TagFactory.CreateId3V1Controller();
                tag = id3Controller.Read(stream, 0);
            }
            return tag;
        }

        [Test]
        [ExpectedException(typeof (Id3HeaderNotFoundException))]
        public void DetectInvalidId3Tag1()
        {
            var audioStream = new List<byte>();

            // add dummy audio files
            audioStream.AddRange(new byte[] {0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x20});
            // First byte is wrong!
            audioStream.AddRange(new byte[] {0x55, 0x41, 0x47});
            audioStream.AddRange(CreateField(30, 0x31)); // Song title
            audioStream.AddRange(CreateField(30, 0x32)); // Artist
            audioStream.AddRange(CreateField(30, 0x33)); // Album
            audioStream.AddRange(CreateField(4, 0x34)); // Year
            audioStream.AddRange(CreateField(28, 0x35)); // Comment
            audioStream.Add(0x00);
            audioStream.Add(0x36); // Track
            audioStream.Add(0x05); // Genre
            byte[] bytes = audioStream.ToArray();
            ReadFromStream(bytes);
        }

        [Test]
        [ExpectedException(typeof (Id3HeaderNotFoundException))]
        public void DetectInvalidId3Tag2()
        {
            var audioStream = new List<byte>();

            // add dummy audio files
            audioStream.AddRange(new byte[] {0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x20});
            // Second byte is wrong!
            audioStream.AddRange(new byte[] {0x54, 0x42, 0x47});
            audioStream.AddRange(CreateField(30, 0x31)); // Song title
            audioStream.AddRange(CreateField(30, 0x32)); // Artist
            audioStream.AddRange(CreateField(30, 0x33)); // Album
            audioStream.AddRange(CreateField(4, 0x34)); // Year
            audioStream.AddRange(CreateField(28, 0x35)); // Comment
            audioStream.Add(0x00);
            audioStream.Add(0x36); // Track
            audioStream.Add(0x05); // Genre

            byte[] bytes = audioStream.ToArray();
            ReadFromStream(bytes);
        }

        [Test]
        [ExpectedException(typeof (Id3HeaderNotFoundException))]
        public void DetectInvalidId3Tag3()
        {
            var audioStream = new List<byte>();

            // add dummy audio files
            audioStream.AddRange(new byte[] {0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x20});
            // Third byte is wrong!
            audioStream.AddRange(new byte[] {0x54, 0x41, 0x48});
            audioStream.AddRange(CreateField(30, 0x31)); // Song title
            audioStream.AddRange(CreateField(30, 0x32)); // Artist
            audioStream.AddRange(CreateField(30, 0x33)); // Album
            audioStream.AddRange(CreateField(4, 0x34)); // Year
            audioStream.AddRange(CreateField(28, 0x35)); // Comment
            audioStream.Add(0x00);
            audioStream.Add(0x36); // Track
            audioStream.Add(0x05); // Genre
            byte[] bytes = audioStream.ToArray();
            ReadFromStream(bytes);
        }

        [Test]
        [ExpectedException(typeof (Id3IOException))]
        public void IgnoreTooShortStream()
        {
            // The Id3Tag contains of 128 Bytes. This is not
            // a valid tag. ( 127 bytes)

            var audioStream = new List<byte>();
            audioStream.AddRange(new byte[] {0x54, 0x41, 0x47});
            audioStream.AddRange(CreateField(124, 0x35));

            byte[] bytes = audioStream.ToArray();
            ReadFromStream(bytes);
        }

        [Test]
        public void ReadId3V1Test()
        {
            var audioStream = new List<byte>();

            // add dummy audio files
            audioStream.AddRange(new byte[] {0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x20});
            audioStream.AddRange(new byte[] {0x54, 0x41, 0x47});
            audioStream.AddRange(CreateField(30, 0x31)); // Song title
            audioStream.AddRange(CreateField(30, 0x32)); // Artist
            audioStream.AddRange(CreateField(30, 0x33)); // Album
            audioStream.AddRange(CreateField(4, 0x34)); // Year
            audioStream.AddRange(CreateField(30, 0x35)); // Comment
            audioStream.Add(0x05); // Genre
            byte[] bytes = audioStream.ToArray();

            Id3V1Tag tag = ReadFromStream(bytes);

            CheckId3Tag(tag.Title, '1', 30);
            CheckId3Tag(tag.Artist, '2', 30);
            CheckId3Tag(tag.Album, '3', 30);
            CheckId3Tag(tag.Year, '4', 4);
            CheckId3Tag(tag.Comment, '5', 30);

            Assert.IsFalse(tag.IsId3V1Dot1Compliant);
            Assert.IsEmpty(tag.TrackNumber);
            Assert.IsNotEmpty(tag.Genre);
        }

        [Test]
        public void ReadId3V1_1Test()
        {
            var audioStream = new List<byte>();

            // add dummy audio files
            audioStream.AddRange(new byte[] {0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x20});
            audioStream.AddRange(new byte[] {0x54, 0x41, 0x47});
            audioStream.AddRange(CreateField(30, 0x31)); // Song title
            audioStream.AddRange(CreateField(30, 0x32)); // Artist
            audioStream.AddRange(CreateField(30, 0x33)); // Album
            audioStream.AddRange(CreateField(4, 0x34)); // Year
            audioStream.AddRange(CreateField(28, 0x35)); // Comment
            audioStream.Add(0x00);
            audioStream.Add(0x06); // Track
            audioStream.Add(0x05); // Genre
            byte[] bytes = audioStream.ToArray();

            Id3V1Tag tag = ReadFromStream(bytes);
            CheckId3Tag(tag.Title, '1', 30);
            CheckId3Tag(tag.Artist, '2', 30);
            CheckId3Tag(tag.Album, '3', 30);
            CheckId3Tag(tag.Year, '4', 4);
            CheckId3Tag(tag.Comment, '5', 28);

            Assert.IsTrue(tag.IsId3V1Dot1Compliant);
            Assert.AreEqual(tag.TrackNumber, "6");
            Assert.IsNotEmpty(tag.Genre);
        }

        [Test]
        public void RemoveTest1()
        {
            var audioData = new byte[] {0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x20};
            var tag = new byte[128];
            tag[0] = 0x54;
            tag[1] = 0x41;
            tag[2] = 0x47;

            int length = audioData.Length + tag.Length;
            var content = new byte[length];
            var result = new byte[64000];
            Array.Copy(audioData, 0, content, 0, audioData.Length);
            Array.Copy(tag, 0, content, audioData.Length, tag.Length);

            var input = new MemoryStream(content, false);
            var output = new MemoryStream(result, true);

            IId3V1Controller id3Controller = Id3TagFactory.CreateId3V1Controller();
            id3Controller.Remove(input, output);
        }

        [Test]
        public void RemoveTest2()
        {
            var content = new byte[127];

            for (byte counter = 0; counter < content.Length; counter++)
            {
                content[counter] = counter;
            }

            var result = new byte[64000];

            var input = new MemoryStream(content, false);
            var output = new MemoryStream(result, true);

            IId3V1Controller id3Controller = Id3TagFactory.CreateId3V1Controller();
            id3Controller.Remove(input, output);
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
            var Id3Tag1 = new Id3V1Tag
                              {
                                  Title = "1",
                                  Artist = "2",
                                  Album = "3",
                                  Year = "4",
                                  Comment = "5",
                                  IsId3V1Dot1Compliant = false,
                                  GenreIdentifier = 12
                              };

            WriteToStream(audioData, output, Id3Tag1);

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
            for (byte i = 0; i < audioData.Length; i++)
            {
                audioData[i] = i;
            }

            // Create a new tag description.
            var Id3Tag1 = new Id3V1Tag
                              {
                                  Title = "1",
                                  Artist = "2",
                                  Album = "3",
                                  Year = "4",
                                  Comment = "5",
                                  IsId3V1Dot1Compliant = false,
                                  GenreIdentifier = 12
                              };

            WriteToStream(audioData, output, Id3Tag1);

            for (byte i = 0; i < 150; i++)
            {
                Assert.AreEqual(output[i], i);
            }
        }

        [Test]
        public void WriteID3V1Tag()
        {
            var audioData = new byte[] {0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19};
            var output = new byte[138];

            // Create a byte array...
            var Id3Tag1 = new Id3V1Tag
                              {
                                  Title = "1",
                                  Artist = "2",
                                  Album = "3",
                                  Year = "4",
                                  Comment = "5",
                                  IsId3V1Dot1Compliant = false,
                                  GenreIdentifier = 12
                              };

            WriteToStream(audioData, output, Id3Tag1);
            Id3V1Tag Id3Tag2 = ReadFromStream(output);
            CheckId3Tag(Id3Tag1, Id3Tag2);
        }

        [Test]
        public void WriteID3V1_1Tag()
        {
            var audioData = new byte[] {0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19};
            var output = new byte[138];

            var Id3Tag1 = new Id3V1Tag
                              {
                                  Title = "1",
                                  Artist = "2",
                                  Album = "3",
                                  Year = "4",
                                  Comment = "5",
                                  IsId3V1Dot1Compliant = true,
                                  TrackNumber = "6",
                                  GenreIdentifier = 12
                              };

            WriteToStream(audioData, output, Id3Tag1);

            Id3V1Tag Id3Tag2 = ReadFromStream(output);
            CheckId3Tag(Id3Tag1, Id3Tag2);
        }

        [Test]
        public void WriteWithMaxFieldSize()
        {
            var audioData = new byte[] {0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19};
            var output = new byte[138];

            var Id3Tag1 = new Id3V1Tag
                              {
                                  Title = CreateFieldText(30, '1'),
                                  Artist = CreateFieldText(30, '2'),
                                  Album = CreateFieldText(30, '3'),
                                  Year = CreateFieldText(4, '4'),
                                  Comment = CreateFieldText(28, '5'),
                                  IsId3V1Dot1Compliant = true,
                                  TrackNumber = "6",
                                  GenreIdentifier = 12
                              };

            WriteToStream(audioData, output, Id3Tag1);

            Id3V1Tag Id3Tag2 = ReadFromStream(output);
            CheckId3Tag(Id3Tag1, Id3Tag2);
        }

        [Test]
        public void WriteWithOverMaxFieldSize_ID3v1()
        {
            var audioData = new byte[] {0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19};
            var output = new byte[138];

            var Id3Tag1 = new Id3V1Tag
                              {
                                  Title = CreateFieldText(31, '1'),
                                  Artist = CreateFieldText(31, '2'),
                                  Album = CreateFieldText(31, '3'),
                                  Year = CreateFieldText(5, '4'),
                                  Comment = CreateFieldText(31, '5'),
                                  IsId3V1Dot1Compliant = false,
                                  GenreIdentifier = 12
                              };

            WriteToStream(audioData, output, Id3Tag1);

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
                                    IsId3V1Dot1Compliant = false,
                                    GenreIdentifier = 12
                                };

            Id3V1Tag Id3Tag2 = ReadFromStream(output);
            CheckId3Tag(refId3Tag, Id3Tag2);
        }

        [Test]
        public void WriteWithOverMaxFieldSize_ID3v11()
        {
            var audioData = new byte[] {0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19};
            var output = new byte[138];

            var Id3Tag1 = new Id3V1Tag
                              {
                                  Title = CreateFieldText(31, '1'),
                                  Artist = CreateFieldText(31, '2'),
                                  Album = CreateFieldText(31, '3'),
                                  Year = CreateFieldText(5, '4'),
                                  Comment = CreateFieldText(29, '5'),
                                  IsId3V1Dot1Compliant = true,
                                  TrackNumber = "6",
                                  GenreIdentifier = 12
                              };

            WriteToStream(audioData, output, Id3Tag1);

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
                                    IsId3V1Dot1Compliant = true,
                                    TrackNumber = "6",
                                    GenreIdentifier = 12
                                };

            Id3V1Tag Id3Tag2 = ReadFromStream(output);
            CheckId3Tag(refId3Tag, Id3Tag2);
        }
    }
}