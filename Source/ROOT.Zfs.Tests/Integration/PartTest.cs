using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.Statistics;

namespace ROOT.Zfs.Tests.Integration
{
    [TestClass]
    public class PartTest
    {
        [TestMethod]
        [DataRow("1.23",1.23d,"1.23%")]
        [DataRow("1.1", 1.1d, "1.10%")]
        [DataRow("0.123", 0.123d, "0.12%")]
        [DataRow("0.126", 0.126d, "0.13%")]
        public void ValidStringShouldBeParsedCorrectly(string value, double expectedValue, string expectedString)
        {
            var part = new Part(value);
            Assert.AreEqual(expectedValue, part.Value);
            Assert.AreEqual(expectedString, part.ToString());
        }

        [TestMethod]
        public void BogusValueShouldResultInZero()
        {
            var part = new Part("seven.3");
            Assert.AreEqual(0d, part.Value);
            Assert.AreEqual("0.00%", part.ToString());
        }
    }
}
