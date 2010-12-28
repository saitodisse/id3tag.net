using NUnit.Framework;

namespace Id3Tag.Net.NUnit
{
    [TestFixture]
    public class UtilsTest
    {
        [Test]
        [ExpectedException(typeof (Id3TagException))]
        public void CalculateSizeTest()
        {
            var data = new byte[] {0xFF, 0xFF, 0xFF, 0xFF};
            Utils.CalculateSize(data);
        }

        [Test]
        public void Convert7BitEncodedToInt32Test1()
        {
            var data = new byte[] {0x00, 0x00, 0x02, 0x01};
            Assert.AreEqual(257, Utils.Convert7BitEncodedToInt32(data));
        }

        [Test]
        public void Convert7BitEncodedToInt32Test2()
        {
            var data = new byte[] {0x7F, 0x7F, 0x7F, 0x7F};
            Assert.AreEqual(268435455, Utils.Convert7BitEncodedToInt32(data));
        }

        [Test]
        public void Convert7BitEncodedToInt32Test3()
        {
            var data = new byte[] {0x70, 0x00, 0x00, 0x00};
            Assert.AreEqual(234881024, Utils.Convert7BitEncodedToInt32(data));
        }

        [Test]
        [ExpectedException(typeof (Id3TagException))]
        public void Convert7BitEncodedToInt32Test4()
        {
            var data = new byte[] {0xFF, 0xFF, 0xFF, 0xFF};
            Utils.Convert7BitEncodedToInt32(data);
        }

        [Test]
        public void Convert8BitEncodedToInt32Test1()
        {
            var data = new byte[] {0x00, 0x00, 0x01, 0x01};
            Assert.AreEqual(257, Utils.Convert8BitEncodedToInt32(data));
        }

        [Test]
        public void Convert8BitEncodedToInt32Test2()
        {
            var data = new byte[] {0x0F, 0xFF, 0xFF, 0xFF};
            Assert.AreEqual(268435455, Utils.Convert8BitEncodedToInt32(data));
        }
    }
}