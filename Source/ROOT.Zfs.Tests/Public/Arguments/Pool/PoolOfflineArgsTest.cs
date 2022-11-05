using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Arguments.Pool;

namespace ROOT.Zfs.Tests.Public.Arguments.Pool
{
    [TestClass]
    public class PoolOfflineArgsTest
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
            var args = new PoolOfflineArgs
            {
                PoolName = pool,
                Device = device,
            };

            var valid = args.Validate(out var errors);
            Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
            Assert.AreEqual(expectedValid, valid);
        }

        [DataRow("tank", "sda", false, false, "offline tank sda")]
        [DataRow("tank", "sda", false, true, "offline -t tank sda")]
        [DataRow("tank", "sda", true, true, "offline -f -t tank sda")]
        [DataRow("tank", "sda", true, false, "offline -f tank sda")]
        [TestMethod]
        public void BuildArgsTest(string pool, string device, bool forceFault, bool temporary, string expected)
        {
            var args = new PoolOfflineArgs
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
