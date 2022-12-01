using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Arguments.Pool;
using System;

namespace ROOT.Zfs.Tests.Public.Arguments.Pool
{
    [TestClass]
    public class PoolIOStatsArgsTest
    {
        [DataRow("tank", null, true)] // only pool
        [DataRow("tank", "", true)] //only pool 
        [DataRow("tank", "/dev/sda", true)] //single device
        [DataRow("tank", "%2fdev%2fsda", true)] //single device url encoded
        [DataRow("tank", "/dev/sda,/dev/sdb", true)] //multiple devices
        [DataRow(null,null,false)]
        [DataRow("", null, false)]
        [DataRow(" ", null, false)]
        [DataRow("rm -rf /", null, false)]
        [DataRow("tank/myds", null, false)]
        [DataRow("tank/myds", "/dev/sda,rm -rf /", false)]
        [TestMethod]
        public void ValidateTest(string pool, string devices, bool expectValid)
        {
            var args = new PoolIOStatsArgs { Name = pool, Devices = devices?.Split(',') };
            var valid = args.Validate(out var errors);
            Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
            Assert.AreEqual(expectValid, valid);
        }

        [DataRow("tank", null, "iostat -LlpPvH tank")] // only pool
        [DataRow("tank", "", "iostat -LlpPvH tank")] //only pool with empty string as device
        [DataRow("tank", "/dev/sda", "iostat -LlpPvH tank /dev/sda")] //single device
        [DataRow("tank", "%2fdev%2fsda", "iostat -LlpPvH tank /dev/sda")] //single device url encoded
        [DataRow("tank", "/dev/sda,/dev/sdb", "iostat -LlpPvH tank /dev/sda /dev/sdb")] //multiple devices
        [TestMethod]
        public void ToStringTest(string pool, string devices, string expected)
        {
            var args = new PoolIOStatsArgs { Name = pool, Devices = devices?.Split(',') };
            var stringVer = args.ToString();
            Assert.AreEqual(expected, stringVer);
        }
    }
}
