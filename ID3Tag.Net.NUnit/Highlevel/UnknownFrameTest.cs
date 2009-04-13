using ID3Tag.HighLevel.ID3Frame;
using NUnit.Framework;

namespace ID3Tag.Net.NUnit.Highlevel
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
                                               ID = "ABCD",
                                               TagAlterPreservation = true
                                           },
                                       Content = new byte[] {0x30, 0x31, 0x32, 0x33}
                                   };

            var rawFrame = unknownFrame.Convert();
            Assert.AreEqual(rawFrame.ID, "ABCD");
            Assert.AreEqual(rawFrame.Payload.Length, 4);
            Assert.AreEqual(rawFrame.Payload[0], 0x30);
            Assert.AreEqual(rawFrame.Payload[1], 0x31);
            Assert.AreEqual(rawFrame.Payload[2], 0x32);
            Assert.AreEqual(rawFrame.Payload[3], 0x33);

            Assert.AreEqual(rawFrame.TagAlterPreservation, true, "Tag Alter Preservation failed.");
            Assert.AreEqual(rawFrame.FileAlterPreservation, false, "File Alter Preservation failed.");
            Assert.AreEqual(rawFrame.ReadOnly, false, "Read Only Failed.");
            Assert.AreEqual(rawFrame.Compression, false, "Compression failed.");
            Assert.AreEqual(rawFrame.Encryption, false, "Encryption failed.");
            Assert.AreEqual(rawFrame.GroupingIdentify, false, "Grouping Identify failed.");
        }

        [Test]
        public void ConvertTest2()
        {
            var unknownFrame = new UnknownFrame
                                   {
                                       Descriptor =
                                           {
                                               ID = "ABCD",
                                               FileAlterPreservation = true
                                           },
                                       Content = new byte[] {0x30, 0x31, 0x32, 0x33}
                                   };

            var rawFrame = unknownFrame.Convert();
            Assert.AreEqual(rawFrame.ID, "ABCD");
            Assert.AreEqual(rawFrame.Payload.Length, 4);
            Assert.AreEqual(rawFrame.Payload[0], 0x30);
            Assert.AreEqual(rawFrame.Payload[1], 0x31);
            Assert.AreEqual(rawFrame.Payload[2], 0x32);
            Assert.AreEqual(rawFrame.Payload[3], 0x33);

            Assert.AreEqual(rawFrame.TagAlterPreservation, false, "Tag Alter Preservation failed.");
            Assert.AreEqual(rawFrame.FileAlterPreservation, true, "File Alter Preservation failed.");
            Assert.AreEqual(rawFrame.ReadOnly, false, "Read Only Failed.");
            Assert.AreEqual(rawFrame.Compression, false, "Compression failed.");
            Assert.AreEqual(rawFrame.Encryption, false, "Encryption failed.");
            Assert.AreEqual(rawFrame.GroupingIdentify, false, "Grouping Identify failed.");
        }

        [Test]
        public void ConvertTest3()
        {
            var unknownFrame = new UnknownFrame
                                   {
                                       Descriptor =
                                           {
                                               ID = "ABCD",
                                               ReadOnly = true
                                           },
                                       Content = new byte[] {0x30, 0x31, 0x32, 0x33}
                                   };

            var rawFrame = unknownFrame.Convert();
            Assert.AreEqual(rawFrame.ID, "ABCD");
            Assert.AreEqual(rawFrame.Payload.Length, 4);
            Assert.AreEqual(rawFrame.Payload[0], 0x30);
            Assert.AreEqual(rawFrame.Payload[1], 0x31);
            Assert.AreEqual(rawFrame.Payload[2], 0x32);
            Assert.AreEqual(rawFrame.Payload[3], 0x33);

            Assert.AreEqual(rawFrame.TagAlterPreservation, false, "Tag Alter Preservation failed.");
            Assert.AreEqual(rawFrame.FileAlterPreservation, false, "File Alter Preservation failed.");
            Assert.AreEqual(rawFrame.ReadOnly, true, "Read Only Failed.");
            Assert.AreEqual(rawFrame.Compression, false, "Compression failed.");
            Assert.AreEqual(rawFrame.Encryption, false, "Encryption failed.");
            Assert.AreEqual(rawFrame.GroupingIdentify, false, "Grouping Identify failed.");
        }

        [Test]
        public void ConvertTest4()
        {
            var unknownFrame = new UnknownFrame
                                   {
                                       Descriptor =
                                           {
                                               ID = "ABCD",
                                               Compression = true
                                           },
                                       Content = new byte[] {0x30, 0x31, 0x32, 0x33}
                                   };

            var rawFrame = unknownFrame.Convert();
            Assert.AreEqual(rawFrame.ID, "ABCD");
            Assert.AreEqual(rawFrame.Payload.Length, 4);
            Assert.AreEqual(rawFrame.Payload[0], 0x30);
            Assert.AreEqual(rawFrame.Payload[1], 0x31);
            Assert.AreEqual(rawFrame.Payload[2], 0x32);
            Assert.AreEqual(rawFrame.Payload[3], 0x33);

            Assert.AreEqual(rawFrame.TagAlterPreservation, false, "Tag Alter Preservation failed.");
            Assert.AreEqual(rawFrame.FileAlterPreservation, false, "File Alter Preservation failed.");
            Assert.AreEqual(rawFrame.ReadOnly, false, "Read Only Failed.");
            Assert.AreEqual(rawFrame.Compression, true, "Compression failed.");
            Assert.AreEqual(rawFrame.Encryption, false, "Encryption failed.");
            Assert.AreEqual(rawFrame.GroupingIdentify, false, "Grouping Identify failed.");
        }

        [Test]
        public void ConvertTest5()
        {
            var unknownFrame = new UnknownFrame
                                   {
                                       Descriptor =
                                           {
                                               ID = "ABCD",
                                               Encryption = true
                                           },
                                       Content = new byte[] {0x30, 0x31, 0x32, 0x33}
                                   };

            var rawFrame = unknownFrame.Convert();
            Assert.AreEqual(rawFrame.ID, "ABCD");
            Assert.AreEqual(rawFrame.Payload.Length, 4);
            Assert.AreEqual(rawFrame.Payload[0], 0x30);
            Assert.AreEqual(rawFrame.Payload[1], 0x31);
            Assert.AreEqual(rawFrame.Payload[2], 0x32);
            Assert.AreEqual(rawFrame.Payload[3], 0x33);

            Assert.AreEqual(rawFrame.TagAlterPreservation, false, "Tag Alter Preservation failed.");
            Assert.AreEqual(rawFrame.FileAlterPreservation, false, "File Alter Preservation failed.");
            Assert.AreEqual(rawFrame.ReadOnly, false, "Read Only Failed.");
            Assert.AreEqual(rawFrame.Compression, false, "Compression failed.");
            Assert.AreEqual(rawFrame.Encryption, true, "Encryption failed.");
            Assert.AreEqual(rawFrame.GroupingIdentify, false, "Grouping Identify failed.");
        }

        [Test]
        public void ConvertTest6()
        {
            var unknownFrame = new UnknownFrame
                                   {
                                       Descriptor =
                                           {
                                               ID = "ABCD",
                                               GroupingIdentify = true
                                           },
                                       Content = new byte[] {0x30, 0x31, 0x32, 0x33}
                                   };

            var rawFrame = unknownFrame.Convert();
            Assert.AreEqual(rawFrame.ID, "ABCD");
            Assert.AreEqual(rawFrame.Payload.Length, 4);
            Assert.AreEqual(rawFrame.Payload[0], 0x30);
            Assert.AreEqual(rawFrame.Payload[1], 0x31);
            Assert.AreEqual(rawFrame.Payload[2], 0x32);
            Assert.AreEqual(rawFrame.Payload[3], 0x33);

            Assert.AreEqual(rawFrame.TagAlterPreservation, false, "Tag Alter Preservation failed.");
            Assert.AreEqual(rawFrame.FileAlterPreservation, false, "File Alter Preservation failed.");
            Assert.AreEqual(rawFrame.ReadOnly, false, "Read Only Failed.");
            Assert.AreEqual(rawFrame.Compression, false, "Compression failed.");
            Assert.AreEqual(rawFrame.Encryption, false, "Encryption failed.");
            Assert.AreEqual(rawFrame.GroupingIdentify, true, "Grouping Identify failed.");
        }
    }
}