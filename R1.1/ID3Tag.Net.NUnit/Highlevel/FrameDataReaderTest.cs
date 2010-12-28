using System.Text;
using Id3Tag.HighLevel;
using NUnit.Framework;

namespace Id3Tag.Net.NUnit.Highlevel
{
    [TestFixture]
    public class FrameDataReaderTest
    {
        [Test]
        [ExpectedException("System.ObjectDisposedException")]
        public void DisposeTest()
        {
            var reader = new FrameDataReader(new byte[] {});
            reader.Dispose();
            reader.ReadByte();
        }

        [Test]
        public void ReadBigEndianUnicodeEncodingTest()
        {
            var payload = new byte[] {0xFE, 0xFF, 0x41};
            Encoding result;
            using (var reader = new FrameDataReader(payload))
            {
                result = reader.ReadEncoding((byte) TextEncodingType.Unicode, 0);
                Assert.AreEqual(2, reader.Position);
            }

            Assert.IsNotNull(result);
            Assert.AreEqual(Encoding.BigEndianUnicode.CodePage, result.CodePage);
        }

        [Test]
        public void ReadByteTest()
        {
            var payload = new byte[] {0x02, 0x00};
            byte result;
            using (var reader = new FrameDataReader(payload))
            {
                result = reader.ReadByte();
                Assert.AreEqual(1, reader.Position);
            }

            Assert.IsNotNull(result);
            Assert.AreEqual(payload[0], result);
        }

        [Test]
        public void ReadBytesTest()
        {
            var payload = new byte[] {0x02, 0x00, 0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x44};
            byte[] result;
            using (var reader = new FrameDataReader(payload))
            {
                result = reader.ReadBytes();
            }

            Assert.IsNotNull(result);
            Assert.AreEqual(payload.Length, result.Length);
            Assert.That(result, Is.EquivalentTo(payload));
        }

        [Test]
        public void ReadFixedStringTest()
        {
            var payload = new byte[] {0x41, 0x42, 0x43};
            string result;
            using (var reader = new FrameDataReader(payload))
            {
                result = reader.ReadFixedString(Encoding.Default, 2);
                Assert.AreEqual(2, reader.Position);
            }

            Assert.AreEqual("AB", result);
        }

        [Test]
        public void ReadUInt16Test()
        {
            var payload = new byte[] {0x00, 0x01, 0x02};
            ushort result;
            using (var reader = new FrameDataReader(payload))
            {
                result = reader.ReadUInt16();
                Assert.AreEqual(2, reader.Position);
            }

            Assert.AreEqual(1, result);
        }

        [Test]
        public void ReadUInt32Test()
        {
            var payload = new byte[] {0x00, 0x00, 0x00, 0x01, 0x02};
            uint result;
            using (var reader = new FrameDataReader(payload))
            {
                result = reader.ReadUInt32();
                Assert.AreEqual(4, reader.Position);
            }

            Assert.AreEqual(1, result);
        }

        [Test]
        public void ReadUInt64Test()
        {
            var payload = new byte[] {0x00, 0x00, 0x00, 0x01};
            ulong result;
            using (var reader = new FrameDataReader(payload))
            {
                result = reader.ReadUInt64();
            }

            Assert.AreEqual(1, result);
        }

        [Test]
        public void ReadUInt64Test2()
        {
            var payload = new byte[] {0x01, 0x00, 0x00, 0x00, 0x02};
            ulong result;
            using (var reader = new FrameDataReader(payload))
            {
                result = reader.ReadUInt64();
            }

            Assert.AreEqual(4294967298, result);
        }

        [Test]
        public void ReadUnicodeEncodingTest()
        {
            var payload = new byte[] {0xFF, 0xFE, 0x41};
            Encoding result;
            using (var reader = new FrameDataReader(payload))
            {
                result = reader.ReadEncoding((byte) TextEncodingType.Unicode, 0);
                Assert.AreEqual(2, reader.Position);
            }

            Assert.IsNotNull(result);
            Assert.AreEqual(Encoding.Unicode.CodePage, result.CodePage);
        }

        [Test]
        public void ReadUtf8EncodingTest()
        {
            var payload = new byte[] {0x41};
            Encoding result;
            using (var reader = new FrameDataReader(payload))
            {
                result = reader.ReadEncoding((byte) TextEncodingType.Utf8, 0);
                Assert.AreEqual(0, reader.Position);
            }

            Assert.IsNotNull(result);
            Assert.AreEqual(Encoding.UTF8.CodePage, result.CodePage);
        }

        [Test]
        public void ReadVariableStringTest()
        {
            var payload = new byte[] {0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x00, 0x00, 0x41, 0x00};
            string result;
            using (var reader = new FrameDataReader(payload))
            {
                result = reader.ReadVariableString(Encoding.Unicode);
                Assert.AreEqual(8, reader.Position);
            }

            Assert.AreEqual("ABC", result);
        }
    }
}