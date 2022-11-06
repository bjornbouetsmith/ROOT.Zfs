using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Arguments.Pool;

namespace ROOT.Zfs.Tests.Public.Arguments.Pool
{
    [TestClass]
    public class PoolDetachArgsTest
    {
        [DataRow("tank", "sda", true)]
        [DataRow("tank", null, false)]
        [DataRow("tank", "", false)]
        [DataRow("tank", " ", false)]
        [DataRow(null, "sda", false)]
        [DataRow("", "sda", false)]
        [DataRow(" ", "sda", false)]
        [DataRow(null, null, false)]
        [TestMethod]
        public void ValidateTest(string poolName, string device, bool expectedValid)
        {
            var args = new PoolDetachArgs { PoolName = poolName, Device = device };

            var valid = args.Validate(out var errors);
            Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
            Assert.AreEqual(expectedValid, valid);
        }

        [DataRow("tank", "sda", "detach tank sda")]
        [DataRow("tank", "/dev/sda", "detach tank /dev/sda")]
        [TestMethod]
        public void ToStringTest(string poolName, string device, string expected)
        {
            var args = new PoolDetachArgs { PoolName = poolName, Device = device };
            var stringVer = args.ToString();
            Console.WriteLine(stringVer);
            Assert.AreEqual(expected, stringVer);
        }
    }
}
