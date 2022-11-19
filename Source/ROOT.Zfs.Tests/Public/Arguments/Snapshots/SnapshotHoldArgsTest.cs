using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Arguments.Snapshots;

namespace ROOT.Zfs.Tests.Public.Arguments.Snapshots
{
    [TestClass]
    public class SnapshotHoldArgsTest
    {
        [DataRow("tank/myds","snap123","mytag",true)]
        [DataRow("tank/myds", "snap123", null, false)]
        [DataRow("tank/myds", "snap123", "", false)]
        [DataRow("tank/myds", "snap123", " ", false)]
        [DataRow("tank/myds", "snap123", "mytag/test", true)]
        [DataRow("tank/myds", "snap123", "mytag/test@1234", true)]
        [DataRow("tank/myds", "snap123", "mytag && rm -rf /", false)]
        [TestMethod]
        public void ValidateTest(string dataset, string snapshot, string tag, bool expectedValid)
        {
            var args = new SnapshotHoldArgs { Dataset = dataset, Snapshot = snapshot, Tag = tag };

            var valid = args.Validate(out var errors);
            Console.WriteLine(string.Join(Environment.NewLine,errors ?? Array.Empty<string>()));
            Assert.AreEqual(expectedValid, valid);
        }

        [DataRow("tank/myds","snap123","mytag",true,"hold -r mytag tank/myds@snap123")] // snapshot name is only snapshot, not including dataset
        [DataRow("tank/myds", "snap123", "mytag", false, "hold mytag tank/myds@snap123")]
        [DataRow("tank/myds", "tank/myds@snap123", "mytag", true, "hold -r mytag tank/myds@snap123")] // Snapshot name is complete name
        [TestMethod]
        public void ToStringTest(string dataset, string snapshot, string tag, bool recursive, string expected)
        {
            var args = new SnapshotHoldArgs { Dataset = dataset, Snapshot = snapshot, Tag = tag,Recursive=recursive };
            var stringVer = args.ToString();
            Console.WriteLine(stringVer);
            Assert.AreEqual(expected, stringVer);
        }
    }
}
