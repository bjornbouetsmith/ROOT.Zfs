using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Public.Data
{
    [TestClass]
    public class SizeTest
    {
        [TestMethod]
        [DataRow(1024UL)] //1K
        [DataRow(1024 * 1024UL)] //1M
        [DataRow(1024 * 1024 * 1024UL)] //1G
        [DataRow(1024 * 1024 * 1024 * 1024UL)] //1T
        public void RoundTripTestSourceBytes(ulong bytes)
        {
            var size = new Size(bytes);
            var sizeAsString = size.ToString();
            Size converted = sizeAsString;
            Assert.AreEqual(size.Bytes, converted.Bytes);
        }

        [TestMethod]
        [DataRow("512", 0UL, true)] //512B
        [DataRow("ZeroB", 0UL, true)] //512B
        [DataRow("0", 0UL, false)] //512B
        [DataRow("512B", 512UL)] //512B
        [DataRow("1K", 1024UL)] //1K
        [DataRow("1M", 1024 * 1024UL)] //1M
        [DataRow("1G", 1024 * 1024 * 1024UL)] //1G
        [DataRow("1T", 1024 * 1024 * 1024 * 1024UL)] //1T
        public void RoundTripTestSourceString(string sizeString, ulong expectedBytes, bool expectException = false)
        {
            if (expectException)
            {
                Assert.ThrowsException<ArgumentException>(() =>
                {
                    Size size = sizeString;
                });
            }
            else
            {

                Size size = sizeString;
                var sizeToString = size.ToString();
                Assert.AreEqual(sizeString, sizeToString);
                Assert.AreEqual(expectedBytes, size.Bytes);
            }
        }

        [TestMethod]
        [DataRow("1.1K", 1126UL)]
        [DataRow("1.1M", 1.1d * 1024 * 1024UL)]
        [DataRow("1.1G", 1.1d * 1024 * 1024 * 1024UL)] //1G
        [DataRow("1.1T", 1.1d * 1024 * 1024 * 1024 * 1024UL)] //1T
        public void FloatingPointStrings(string sizeString, double expectedBytes)
        {
            Size size = sizeString;
            var sizeToString = size.ToString();
            Assert.AreEqual(sizeString, sizeToString);
            Assert.AreEqual((ulong)expectedBytes, size.Bytes);
        }

        [TestMethod]
        [DataRow("0", 0UL, "0")]
        [DataRow("123", 123UL, "123B")]
        [DataRow("1024", 1024UL, "1K")]
        [DataRow("1048576", 1048576UL, "1M")]
        [DataRow("1073741824", 1073741824UL, "1G")]
        [DataRow("1099511627776", 1099511627776UL, "1T")]
        [DataRow("1209462790553", 1209462790553UL, "1.1T")]
        [DataRow("12094627905530", 12094627905530UL, "11T")]
        [DataRow("13304090696083", 13304090696083UL, "12.1T")]
        public void ValidStringShouldBeParsedCorrectly(string value, ulong expectedValue, string expectedString)
        {
            var size = new Size(value);
            Assert.AreEqual(expectedValue, size.Bytes);
            Assert.AreEqual(expectedString, size.ToString());
        }

        [TestMethod]
        public void BogusValueShouldResultInZero()
        {
            var size = new Size("seven");
            Assert.AreEqual(0UL, size.Bytes);
            Assert.AreEqual("0", size.ToString());
        }
    }
}
