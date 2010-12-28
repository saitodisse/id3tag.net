using NUnit.Framework;

namespace Id3Tag.Net.NUnit
{
    [TestFixture]
    public class VersionTest
    {
        [Test]
        public void GetReadableVersionTest()
        {
            string ver = Version.ReadableVersion;
            Assert.IsNotEmpty(ver);
            Assert.IsTrue(ver.Contains("."));
        }
    }
}