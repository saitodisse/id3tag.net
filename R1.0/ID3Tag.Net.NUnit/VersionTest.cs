using NUnit.Framework;

namespace ID3Tag.Net.NUnit
{
	[TestFixture]
	public class VersionTest
	{
		[Test]
		public void GetReadableVersionTest()
		{
			var ver = Version.GetReadableVersion();
			Assert.IsNotEmpty(ver);
			Assert.IsTrue(ver.Contains("."));
		}
	}
}
