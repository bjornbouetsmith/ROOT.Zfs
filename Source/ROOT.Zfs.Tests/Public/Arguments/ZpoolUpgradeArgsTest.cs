using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Arguments;

namespace ROOT.Zfs.Tests.Public.Arguments
{
    [TestClass]
    public class ZpoolUpgradeArgsTest
    {
        [DataRow("tank",false,true)]
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
            var args = new ZpoolUpgradeArgs { PoolName = poolName, AllPools = all };
            var valid = args.Validate(out var errors);
            Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
            Assert.AreEqual(valid, expectedValid);
        }
    }
}
