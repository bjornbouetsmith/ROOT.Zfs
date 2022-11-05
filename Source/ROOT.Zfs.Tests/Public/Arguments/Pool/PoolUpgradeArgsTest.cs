using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Arguments.Pool;

namespace ROOT.Zfs.Tests.Public.Arguments.Pool
{
    [TestClass]
    public class PoolUpgradeArgsTest
    {
        [DataRow("tank", false, true)]
        [DataRow("tank", true, true)]
        [DataRow("", true, true)]
        [DataRow(null, true, true)]
        [DataRow(" ", true, true)]
        [DataRow(" ", false, false)]
        [DataRow("", false, false)]
        [DataRow(null, false, false)]
        [TestMethod]
        public void ValidTest(string poolName, bool all, bool expectedValid)
        {
            var args = new PoolUpgradeArgs { PoolName = poolName, AllPools = all };
            var valid = args.Validate(out var errors);
            Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
            Assert.AreEqual(valid, expectedValid);
        }

        [DataRow("tank",true,"upgrade -a")]
        [DataRow("tank", false, "upgrade tank")]
        [DataRow(null, true, "upgrade -a")]
        [DataRow("", true, "upgrade -a")]
        [DataRow(" ", true, "upgrade -a")]
        [TestMethod]
        public void ToStringTest(string poolName, bool all, string expected)
        {
            var args = new PoolUpgradeArgs { PoolName = poolName, AllPools = all };
            var stringVer = args.ToString();
            Assert.AreEqual(expected, stringVer);
        }
    }
}
