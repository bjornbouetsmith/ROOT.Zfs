using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Arguments.Snapshots;

namespace ROOT.Zfs.Tests.Public.Arguments.Snapshots
{
    [TestClass]
    public class SnapshotCreateArgsTest
    {
        [DataRow("tank/myds",null,true)]
        [DataRow("tank/myds", "", true)]
        [DataRow("tank/myds", " ", true)]
        [DataRow("tank/myds", "tank/myds@test123", true)]
        [DataRow("tank/myds", "test123", true)]
        [DataRow("tank/myds", "test123/mytest", false)]
        [DataRow("tank/myds", "test123 && rm -rf /", false)]
        [DataRow(null, "test123", false)]
        [DataRow("", "test123", false)]
        [DataRow(" ", "test123", false)]
        [TestMethod]
        public void ValidateTest(string dataset, string snapshot, bool expectedValid)
        {
            var args = new SnapshotCreateArgs { Dataset = dataset, Snapshot = snapshot };

            var valid = args.Validate(out var errors);
            Console.WriteLine(string.Join(Environment.NewLine,errors ?? Array.Empty<string>()));
            Assert.AreEqual(expectedValid, valid);
        }

        [DataRow("tank/myds", "tank/myds@test123", "snap tank/myds@test123")]
        [DataRow("tank/myds", "test123", "snap tank/myds@test123")]
        [DataRow("tank/myds", null, "snap tank/myds@")]
        [DataRow("tank/myds", "", "snap tank/myds@")]
        [DataRow("tank/myds", " ", "snap tank/myds@")]
        [TestMethod]
        public void ToStringTest(string dataset, string snapshot,string expected)
        {
            var args = new SnapshotCreateArgs { Dataset = dataset, Snapshot = snapshot };

            var stringVer = args.ToString();
            if (!string.IsNullOrWhiteSpace(snapshot))
            {
                Assert.AreEqual(expected, stringVer);
            }
            else
            {
                var suffix = DateTime.UtcNow.ToLocalTime().ToString("yyyyMMddHH");
                Assert.IsTrue(stringVer.StartsWith(expected + suffix));
            }

            Console.WriteLine(stringVer);
        }
    }
}
