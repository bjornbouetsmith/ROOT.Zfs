using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Arguments.Pool;

namespace ROOT.Zfs.Tests.Public.Arguments
{
    [TestClass]
    public class ZpoolOfflineArgsTest
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
            var args = new ZpoolOfflineArgs
            {
                PoolName = pool,
                Device = device,
            };

            var valid = args.Validate(out var errors);
            Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
            Assert.AreEqual(expectedValid, valid);
        }

        [DataRow("tank", "sda", false, false, " tank sda")]
        [DataRow("tank", "sda", false, true, " -t tank sda")]
        [DataRow("tank", "sda", true, true, " -f -t tank sda")]
        [DataRow("tank", "sda", true, false, " -f tank sda")]
        [TestMethod]
        public void ToStringTest(string pool, string device, bool forceFault, bool temporary, string expected)
        {
            var args = new ZpoolOfflineArgs
            {
                PoolName = pool,
                Device = device,
                ForceFault = forceFault,
                Temporary = temporary
            };
            var stringVer = args.ToString();

            Assert.AreEqual(expected, stringVer);
        }
    }
}
