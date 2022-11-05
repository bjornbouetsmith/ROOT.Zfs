using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Arguments.Pool;

namespace ROOT.Zfs.Tests.Public.Arguments.Pool
{
    [TestClass]
    public class PoolOnlineArgsTest
    {
        [DataRow(null, "disk", false)]
        [DataRow("", "disk", false)]
        [DataRow(" ", "disk", false)]
        [DataRow("tank", "disk", true)]
        [DataRow("tank", null, false)]
        [DataRow("tank", "", false)]
        [DataRow("tank", " ", false)]
        [TestMethod]
        public void ValidateTest(string pool, string device, bool expectedValid)
        {
            var args = new PoolOnlineArgs
            {
                PoolName = pool,
                Device = device,
            };

            var valid = args.Validate(out var errors);
            Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
            Assert.AreEqual(expectedValid, valid);
        }


        [DataRow("tank", "sda", false, "online tank sda")]
        [DataRow("tank", "sda", true, "online -e tank sda")]
        [TestMethod]
        public void BuildArgsTest(string pool, string device, bool expandSpace, string expected)
        {
            var args = new PoolOnlineArgs
            {
                PoolName = pool,
                Device = device,
                ExpandSpace = expandSpace
            };
            var stringVer = args.BuildArgs("online");

            Assert.AreEqual(expected, stringVer);
        }
    }
}
