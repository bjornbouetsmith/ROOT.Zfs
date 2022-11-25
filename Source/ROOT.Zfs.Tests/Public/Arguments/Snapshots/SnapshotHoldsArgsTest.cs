using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Arguments.Snapshots;

namespace ROOT.Zfs.Tests.Public.Arguments.Snapshots
{
    [TestClass]
    public class SnapshotHoldsArgsTest
    {
        [DataRow("tank/myds", "snap123", true)]
        [DataRow("tank%2fmyds", "snap123", true)]
        [DataRow("tank/myds", null, false)]
        [DataRow("tank/myds", "", false)]
        [DataRow("tank/myds", " ", false)]
        [DataRow("tank/myds", "snap123 && rm -rf /", false)]
        [TestMethod]
        public void ValidateTest(string dataset, string snapshot, bool expectedValid)
        {
            var args = new SnapshotHoldsArgs { Dataset = dataset, Snapshot = snapshot };

            var valid = args.Validate(out var errors);
            Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
            Assert.AreEqual(expectedValid, valid);
        }

        [DataRow("tank/myds", "snap123", true, "holds -r -H tank/myds@snap123")] // snapshot name is only snapshot, not including dataset
        [DataRow("tank/myds", "snap123",  false, "holds -H tank/myds@snap123")]
        [DataRow("tank/myds", "tank/myds@snap123",  true, "holds -r -H tank/myds@snap123")] // Snapshot name is complete name
        [DataRow("tank%2fmyds", "tank/myds@snap123", true, "holds -r -H tank/myds@snap123")] // Dataset name is url encoded
        [TestMethod]
        public void ToStringTest(string dataset, string snapshot, bool recursive, string expected)
        {
            var args = new SnapshotHoldsArgs { Dataset = dataset, Snapshot = snapshot, Recursive = recursive };
            var stringVer = args.ToString();
            Console.WriteLine(stringVer);
            Assert.AreEqual(expected, stringVer);
        }
    }
}
