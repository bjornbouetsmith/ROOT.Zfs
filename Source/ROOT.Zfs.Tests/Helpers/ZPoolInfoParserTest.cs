using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Tests.Helpers
{
    [TestClass]
    public class ZPoolInfoParserTest
    {
        [TestMethod]
        public void ParseLineTest()
        {
            var line = "tank\t3813930958848\t128800165888\t3685130792960\t-\t-\t9\t3\t1.00\tONLINE\t-";
            var info = ZPoolInfoParser.ParseLine(line);

            Assert.AreEqual("tank", info.PoolName);
            Assert.AreEqual("3.5T", info.Size.ToString());
            Assert.AreEqual("120G", info.Allocated.ToString());
            Assert.AreEqual("3.4T", info.Free.ToString());
            Assert.AreEqual("9.00%", info.Fragmentation.ToString());
            Assert.AreEqual("3.00%", info.CapacityUsed.ToString());
            Assert.AreEqual("1.00x", info.DedupRatio.ToString());
            Assert.AreEqual(State.Online, info.State);
            Assert.AreEqual("-", info.AltRoot);
        }

        [TestMethod]
        public void ParseFromStdOutTest()
        {
            var stdOut = @"rpool   31138512896     3078807552      28059705344     -       -       10      9       1.00    ONLINE  -
tank    3813930958848   128800165888    3685130792960   -       -       9       3       1.00    ONLINE  -";

            var list = ZPoolInfoParser.ParseFromStdOut(stdOut);

            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("rpool", list[0].PoolName);
            Assert.AreEqual("tank", list[1].PoolName);
        }
    }
}
