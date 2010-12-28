using System.Text;
using Id3Tag.HighLevel;
using NUnit.Framework;

namespace Id3Tag.Net.NUnit.Highlevel
{
    [TestFixture]
    public class FrameDataWriterTest
    {
        [Test]
        [ExpectedException("System.ObjectDisposedException")]
        public void DisposeTest()
        {
            var writer = new FrameDataWriter();
            writer.Dispose();
            writer.WriteByte(0x00);
        }

        [Test]
        public void WriteAsciiStringTest()
        {
            var expected = new byte[] {0x41, 0x42, 0x43};
            byte[] result;
            using (var writer = new FrameDataWriter())
            {
                writer.WriteString("ABC", Encoding.ASCII);
                result = writer.ToArray();
            }

            Assert.IsNotNull(result);
            Assert.That(result, Is.EquivalentTo(expected));
        }

        [Test]
        public void WriteBigUInt64Test()
        {
            var expected = new byte[] {0x01, 0x00, 0x00, 0x00, 0x02};
            byte[] result;
            using (var writer = new FrameDataWriter())
            {
                writer.WriteUInt64(4294967298);
                result = writer.ToArray();
            }

            Assert.IsNotNull(result);
            Assert.That(result, Is.EquivalentTo(expected));
        }

        [Test]
        public void WriteByteTest()
        {
            byte[] result;
            using (var writer = new FrameDataWriter())
            {
                writer.WriteByte(0x01);
                writer.WriteByte(0x02);
                writer.WriteByte(0x03);
                result = writer.ToArray();
            }

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual(0x01, result[0]);
            Assert.AreEqual(0x02, result[1]);
            Assert.AreEqual(0x03, result[2]);
        }

        [Test]
        public void WriteBytesTest()
        {
            var bytes = new byte[] {0x01, 0x02, 0x03};
            byte[] result;
            using (var writer = new FrameDataWriter())
            {
                writer.WriteBytes(bytes);
                result = writer.ToArray();
            }

            Assert.IsNotNull(result);
            Assert.That(result, Is.EquivalentTo(bytes));
        }

        [Test]
        public void WriteEncodingByteTest()
        {
            byte[] result;
            using (var writer = new FrameDataWriter())
            {
                writer.WriteEncodingByte(Encoding.Unicode);
                result = writer.ToArray();
            }

            Assert.IsNotNull(result);
            Assert.That(result[0], Is.EqualTo((byte) TextEncodingType.Unicode));
        }

        [Test]
        public void WritePreambleTest()
        {
            byte[] result;
            using (var writer = new FrameDataWriter())
            {
                writer.WriteEncodingByte(Encoding.Unicode);
                writer.WritePreamble(Encoding.Unicode);
                result = writer.ToArray();
            }

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual((byte) TextEncodingType.Unicode, result[0]);
            Assert.AreEqual(0xFF, result[1]);
            Assert.AreEqual(0xFE, result[2]);
        }

        [Test]
        public void WriteSmallUInt64Test()
        {
            var expected = new byte[] {0x00, 0x00, 0x00, 0x01};
            byte[] result;
            using (var writer = new FrameDataWriter())
            {
                writer.WriteUInt64(1);
                result = writer.ToArray();
            }

            Assert.IsNotNull(result);
            Assert.That(result, Is.EquivalentTo(expected));
        }

        [Test]
        public void WriteUInt16Test()
        {
            var expected = new byte[] {0x00, 0x01};
            byte[] result;
            using (var writer = new FrameDataWriter())
            {
                writer.WriteUInt16(1);
                result = writer.ToArray();
            }

            Assert.IsNotNull(result);
            Assert.That(result, Is.EquivalentTo(expected));
        }

        [Test]
        public void WriteUInt32Test()
        {
            var expected = new byte[] {0x00, 0x00, 0x00, 0x01};
            byte[] result;
            using (var writer = new FrameDataWriter())
            {
                writer.WriteUInt32(1);
                result = writer.ToArray();
            }

            Assert.IsNotNull(result);
            Assert.That(result, Is.EquivalentTo(expected));
        }

        [Test]
        public void WriteUnicodeStringTest()
        {
            var expected = new byte[] {0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x00, 0x00};
            byte[] result;
            using (var writer = new FrameDataWriter())
            {
                writer.WriteString("ABC", Encoding.Unicode, true);
                result = writer.ToArray();
            }

            Assert.IsNotNull(result);
            Assert.That(result, Is.EquivalentTo(expected));
        }
    }
}