using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Arguments.Pool;

namespace ROOT.Zfs.Tests.Public.Arguments.Pool
{
    [TestClass]
    public class PoolClearArgsTest
    {
        [DataRow("tank", "sda", true)]
        [DataRow("tank", "/dev/sda", true)]
        [DataRow("tank", "%2fdev%2fsda", true)]
        [DataRow("tank", null, true)]
        [DataRow("tank", "", true)]
        [DataRow("tank", " ", true)]
        [DataRow("tank", "/dev/sda@123", false)]
        [DataRow(null, "/dev/sda", false)]
        [DataRow("", "/dev/sda", false)]
        [DataRow(" ", "/dev/sda", false)]
        [DataRow("tank/myds@snap", "/dev/sda", false)]
        [DataRow("tank", "/dev/sda && rm -rf /", false)]
        [TestMethod]
        public void ValidateTest(string pool, string device, bool expectValid)
        {
            var args = new PoolClearArgs { PoolName = pool, Device = device };
            var valid = args.Validate(out var errors);
            Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
            Assert.AreEqual(expectValid, valid);
        }

        [DataRow("tank", "sda", "clear tank sda")]
        [DataRow("tank", "/dev/sda", "clear tank /dev/sda")]
        [DataRow("tank", "%2fdev%2fsda", "clear tank /dev/sda")]
        [DataRow("tank", "%2fdev%2fsda", "clear tank /dev/sda")]
        [DataRow("tank.2", "%2fdev%2fsda", "clear tank.2 /dev/sda")]
        [DataRow("tank", null, "clear tank")]
        [DataRow("tank", "", "clear tank")]
        [DataRow("tank", " ", "clear tank")]
        [TestMethod]
        public void ToStringTest(string pool, string device, string expected)
        {
            var args = new PoolClearArgs { PoolName = pool, Device = device };

            var stringVer = args.ToString();
            Assert.AreEqual(expected, stringVer);
        }
    }
}
