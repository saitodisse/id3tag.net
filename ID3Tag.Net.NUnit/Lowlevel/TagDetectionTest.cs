using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ID3Tag.Factory;
using NUnit.Framework;

namespace ID3Tag.Net.NUnit.Lowlevel
{
    [TestFixture]
    public class TagDetectionTest
    {

        [Test]
        public void DetectNothing()
        {
            //var audioStream = new List<byte>();

            //// add dummy audio files
            //audioStream.AddRange(new byte[] { 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x20 });
            //var bytes = audioStream.ToArray();

            //using (var stream = new MemoryStream(bytes))
            //{
            //    var id3Controller = Id3TagFactory.CreateId3V1Controller();
            //    var tag = id3Controller.Read(stream);
            //}
        }

        [Test]
        public void DetectTagV1()
        {

        }

        [Test]
        public void DetectTagV2()
        {

        }

        [Test]
        public void DetectTagV1AndV2()
        {

        }
    }
}
