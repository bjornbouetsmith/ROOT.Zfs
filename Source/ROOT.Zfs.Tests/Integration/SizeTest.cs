using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Integration
{
    [TestClass]
    public class SizeTest
    {
        [TestMethod]
        [DataRow("123", 123, "123")]
        [DataRow("1024", 1024, "1K")]
        [DataRow("1048576", 1048576, "1M")]
        [DataRow("1073741824", 1073741824, "1G")]
        [DataRow("1099511627776", 1099511627776, "1T")]
        [DataRow("1209462790553", 1209462790553, "1.1T")]
        [DataRow("12094627905530", 12094627905530, "11T")]
        [DataRow("13304090696083", 13304090696083, "12.1T")]
        public void ValidStringShouldBeParsedCorrectly(string value, long expectedValue, string expectedString)
        {
            var size = new Size(value);
            Assert.AreEqual(expectedValue, size.Bytes);
            Assert.AreEqual(expectedString, size.ToString());
        }

        [TestMethod]
        public void BogusValueShouldResultInZero()
        {
            var size = new Size("seven");
            Assert.AreEqual(0, size.Bytes);
            Assert.AreEqual("0", size.ToString());
        }
    }
}
