using Id3Tag.HighLevel;
using Id3Tag.HighLevel.Id3Frame;
using Id3Tag.LowLevel;
using NUnit.Framework;

namespace Id3Tag.Net.NUnit.Highlevel.FrameTest
{
    [TestFixture]
    public class UnknownFrameTest
    {
        [Test]
        public void ConvertTest1()
        {
            var unknownFrame = new UnknownFrame
                                   {
                                       Descriptor =
                                           {
                                               Id = "ABCD",
                                               TagAlterPreservation = true
                                           },
                                   };

            unknownFrame.SetContent(new byte[] {0x30, 0x31, 0x32, 0x33});

            RawFrame rawFrame = unknownFrame.Convert(TagVersion.Id3V23);
            Assert.AreEqual(rawFrame.Id, "ABCD");
            Assert.AreEqual(rawFrame.Payload.Count, 4);
            Assert.AreEqual(rawFrame.Payload[0], 0x30);
            Assert.AreEqual(rawFrame.Payload[1], 0x31);
            Assert.AreEqual(rawFrame.Payload[2], 0x32);
            Assert.AreEqual(rawFrame.Payload[3], 0x33);

            Assert.AreEqual(rawFrame.Options.TagAlterPreservation, true, "Tag Alter Preservation failed.");
            Assert.AreEqual(rawFrame.Options.FileAlterPreservation, false, "File Alter Preservation failed.");
            Assert.AreEqual(rawFrame.Options.ReadOnly, false, "Read Only Failed.");
            Assert.AreEqual(rawFrame.Options.Compression, false, "Compression failed.");
            Assert.AreEqual(rawFrame.Options.Encryption, false, "Encryption failed.");
            Assert.AreEqual(rawFrame.Options.GroupingIdentify, false, "Grouping Identify failed.");
        }

        [Test]
        public void ConvertTest2()
        {
            var unknownFrame = new UnknownFrame
                                   {
                                       Descriptor =
                                           {
                                               Id = "ABCD",
                                               FileAlterPreservation = true
                                           },
                                   };

            unknownFrame.SetContent(new byte[] {0x30, 0x31, 0x32, 0x33});

            RawFrame rawFrame = unknownFrame.Convert(TagVersion.Id3V23);
            Assert.AreEqual(rawFrame.Id, "ABCD");
            Assert.AreEqual(rawFrame.Payload.Count, 4);
            Assert.AreEqual(rawFrame.Payload[0], 0x30);
            Assert.AreEqual(rawFrame.Payload[1], 0x31);
            Assert.AreEqual(rawFrame.Payload[2], 0x32);
            Assert.AreEqual(rawFrame.Payload[3], 0x33);

            Assert.AreEqual(rawFrame.Options.TagAlterPreservation, false, "Tag Alter Preservation failed.");
            Assert.AreEqual(rawFrame.Options.FileAlterPreservation, true, "File Alter Preservation failed.");
            Assert.AreEqual(rawFrame.Options.ReadOnly, false, "Read Only Failed.");
            Assert.AreEqual(rawFrame.Options.Compression, false, "Compression failed.");
            Assert.AreEqual(rawFrame.Options.Encryption, false, "Encryption failed.");
            Assert.AreEqual(rawFrame.Options.GroupingIdentify, false, "Grouping Identify failed.");
        }

        [Test]
        public void ConvertTest3()
        {
            var unknownFrame = new UnknownFrame
                                   {
                                       Descriptor =
                                           {
                                               Id = "ABCD",
                                               ReadOnly = true
                                           },
                                   };

            unknownFrame.SetContent(new byte[] {0x30, 0x31, 0x32, 0x33});

            RawFrame rawFrame = unknownFrame.Convert(TagVersion.Id3V23);
            Assert.AreEqual(rawFrame.Id, "ABCD");
            Assert.AreEqual(rawFrame.Payload.Count, 4);
            Assert.AreEqual(rawFrame.Payload[0], 0x30);
            Assert.AreEqual(rawFrame.Payload[1], 0x31);
            Assert.AreEqual(rawFrame.Payload[2], 0x32);
            Assert.AreEqual(rawFrame.Payload[3], 0x33);

            Assert.AreEqual(rawFrame.Options.TagAlterPreservation, false, "Tag Alter Preservation failed.");
            Assert.AreEqual(rawFrame.Options.FileAlterPreservation, false, "File Alter Preservation failed.");
            Assert.AreEqual(rawFrame.Options.ReadOnly, true, "Read Only Failed.");
            Assert.AreEqual(rawFrame.Options.Compression, false, "Compression failed.");
            Assert.AreEqual(rawFrame.Options.Encryption, false, "Encryption failed.");
            Assert.AreEqual(rawFrame.Options.GroupingIdentify, false, "Grouping Identify failed.");
        }

        [Test]
        public void ConvertTest4()
        {
            var unknownFrame = new UnknownFrame
                                   {
                                       Descriptor =
                                           {
                                               Id = "ABCD",
                                               Compression = true
                                           },
                                   };

            unknownFrame.SetContent(new byte[] {0x30, 0x31, 0x32, 0x33});

            RawFrame rawFrame = unknownFrame.Convert(TagVersion.Id3V23);
            Assert.AreEqual(rawFrame.Id, "ABCD");
            Assert.AreEqual(rawFrame.Payload.Count, 4);
            Assert.AreEqual(rawFrame.Payload[0], 0x30);
            Assert.AreEqual(rawFrame.Payload[1], 0x31);
            Assert.AreEqual(rawFrame.Payload[2], 0x32);
            Assert.AreEqual(rawFrame.Payload[3], 0x33);

            Assert.AreEqual(rawFrame.Options.TagAlterPreservation, false, "Tag Alter Preservation failed.");
            Assert.AreEqual(rawFrame.Options.FileAlterPreservation, false, "File Alter Preservation failed.");
            Assert.AreEqual(rawFrame.Options.ReadOnly, false, "Read Only Failed.");
            Assert.AreEqual(rawFrame.Options.Compression, true, "Compression failed.");
            Assert.AreEqual(rawFrame.Options.Encryption, false, "Encryption failed.");
            Assert.AreEqual(rawFrame.Options.GroupingIdentify, false, "Grouping Identify failed.");
        }

        [Test]
        public void ConvertTest5()
        {
            var unknownFrame = new UnknownFrame
                                   {
                                       Descriptor =
                                           {
                                               Id = "ABCD",
                                               Encryption = true
                                           },
                                   };

            unknownFrame.SetContent(new byte[] {0x30, 0x31, 0x32, 0x33});

            RawFrame rawFrame = unknownFrame.Convert(TagVersion.Id3V23);
            Assert.AreEqual(rawFrame.Id, "ABCD");
            Assert.AreEqual(rawFrame.Payload.Count, 4);
            Assert.AreEqual(rawFrame.Payload[0], 0x30);
            Assert.AreEqual(rawFrame.Payload[1], 0x31);
            Assert.AreEqual(rawFrame.Payload[2], 0x32);
            Assert.AreEqual(rawFrame.Payload[3], 0x33);

            Assert.AreEqual(rawFrame.Options.TagAlterPreservation, false, "Tag Alter Preservation failed.");
            Assert.AreEqual(rawFrame.Options.FileAlterPreservation, false, "File Alter Preservation failed.");
            Assert.AreEqual(rawFrame.Options.ReadOnly, false, "Read Only Failed.");
            Assert.AreEqual(rawFrame.Options.Compression, false, "Compression failed.");
            Assert.AreEqual(rawFrame.Options.Encryption, true, "Encryption failed.");
            Assert.AreEqual(rawFrame.Options.GroupingIdentify, false, "Grouping Identify failed.");
        }

        [Test]
        public void ConvertTest6()
        {
            var unknownFrame = new UnknownFrame
                                   {
                                       Descriptor =
                                           {
                                               Id = "ABCD",
                                               GroupingIdentify = true
                                           },
                                   };
            unknownFrame.SetContent(new byte[] {0x30, 0x31, 0x32, 0x33});

            RawFrame rawFrame = unknownFrame.Convert(TagVersion.Id3V23);
            Assert.AreEqual(rawFrame.Id, "ABCD");
            Assert.AreEqual(rawFrame.Payload.Count, 4);
            Assert.AreEqual(rawFrame.Payload[0], 0x30);
            Assert.AreEqual(rawFrame.Payload[1], 0x31);
            Assert.AreEqual(rawFrame.Payload[2], 0x32);
            Assert.AreEqual(rawFrame.Payload[3], 0x33);

            Assert.AreEqual(rawFrame.Options.TagAlterPreservation, false, "Tag Alter Preservation failed.");
            Assert.AreEqual(rawFrame.Options.FileAlterPreservation, false, "File Alter Preservation failed.");
            Assert.AreEqual(rawFrame.Options.ReadOnly, false, "Read Only Failed.");
            Assert.AreEqual(rawFrame.Options.Compression, false, "Compression failed.");
            Assert.AreEqual(rawFrame.Options.Encryption, false, "Encryption failed.");
            Assert.AreEqual(rawFrame.Options.GroupingIdentify, true, "Grouping Identify failed.");
        }

        [Test]
        public void ImportTest()
        {
            var payload = new byte[] {0x30, 0x31, 0x32, 0x33};
            var flags = new FrameOptions();
            var rawFrame = new RawFrameV3("ABCD", flags, payload);

            var frame = new UnknownFrame();
            frame.Import(rawFrame, 0);

            Assert.AreEqual("ABCD", frame.Descriptor.Id);
            Assert.That(frame.Content, Is.EquivalentTo(payload));
        }
    }
}