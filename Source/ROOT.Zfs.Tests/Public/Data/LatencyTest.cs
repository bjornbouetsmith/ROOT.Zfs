using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Data.Statistics;

namespace ROOT.Zfs.Tests.Public.Data
{
    [TestClass]
    public class LatencyTest
    {
        [TestMethod]
        public void ValidLatencyValuesShouldParse()
        {
            var latency = new Latency("1000", "1100");

            Assert.AreEqual(10L, latency.Read.Ticks);
            Assert.AreEqual(11L, latency.Write.Ticks);
        }

        [TestMethod]
        public void InvalidLatencyValuesShouldNotThrowResultInZero()
        {
            var latency = new Latency("50us", "60us");

            Assert.AreEqual(0L, latency.Read.Ticks);
            Assert.AreEqual(0L, latency.Write.Ticks);
        }

    }
}
