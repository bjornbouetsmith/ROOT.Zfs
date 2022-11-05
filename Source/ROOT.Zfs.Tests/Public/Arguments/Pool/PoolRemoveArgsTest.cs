using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Arguments.Pool;

namespace ROOT.Zfs.Tests.Public.Arguments.Pool
{
    [TestClass]
    public class PoolRemoveArgsTest
    {
        [DataRow("tank", "mirror-0", false, true)]
        [DataRow("tank", "mirror-0", true, true)]
        [DataRow("tank", null, true, true)]
        [DataRow("tank", "", true, true)]
        [DataRow("tank", " ", true, true)]
        [DataRow("tank", null, false, false)]
        [DataRow("tank", "", false, false)]
        [DataRow("tank", " ", false, false)]
        [DataRow(null, "mirror-0", false, false)]
        [DataRow("", "mirror-0", false, false)]
        [DataRow(" ", "mirror-0", false, false)]
        [TestMethod]
        public void ValidateTest(string poolName, string vdevOrDevice, bool cancel, bool expectedValid)
        {
            var args = new PoolRemoveArgs
            {
                PoolName = poolName,
                VDevOrDevice = vdevOrDevice,
                Cancel = cancel,
            };

            var valid = args.Validate(out var errors);
            Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
            Assert.AreEqual(expectedValid, valid);
        }

        [DataRow("tank", "mirror-0", false, " tank mirror-0")]
        [DataRow("tank", "mirror-0", true, " -s tank")]
        [DataRow("tank", null, true, " -s tank")]
        [DataRow("tank", "", true, " -s tank")]
        [DataRow("tank", " ", true, " -s tank")]
        [TestMethod]
        public void ToStringTest(string poolName, string vdevOrDevice, bool cancel, string expected)
        {
            var args = new PoolRemoveArgs
            {
                PoolName = poolName,
                VDevOrDevice = vdevOrDevice,
                Cancel = cancel,
            };

            Assert.AreEqual(expected, args.ToString());
        }
    }
}
