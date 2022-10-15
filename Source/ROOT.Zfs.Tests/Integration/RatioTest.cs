using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Integration
{
    [TestClass]
    public class RatioTest
    {
        [TestMethod]
        [DataRow("1.23",1.23d,"1.23x")]
        [DataRow("1.1", 1.1d, "1.10x")]
        [DataRow("0.123", 0.123d, "0.12x")]
        [DataRow("0.126", 0.126d, "0.13x")]
        public void ValidStringShouldBeParsedCorrectly(string value, double expectedValue, string expectedString)
        {
            var part = new Ratio(value);
            Assert.AreEqual(expectedValue, part.Value);
            Assert.AreEqual(expectedString, part.ToString());
        }

        [TestMethod]
        public void BogusValueShouldResultInZero()
        {
            var part = new Ratio("seven.3");
            Assert.AreEqual(0d, part.Value);
            Assert.AreEqual("0.00x", part.ToString());
        }
    }
}
