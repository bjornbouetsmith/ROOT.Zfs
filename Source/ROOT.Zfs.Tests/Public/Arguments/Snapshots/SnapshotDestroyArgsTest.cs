using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Arguments.Snapshots;

namespace ROOT.Zfs.Tests.Public.Arguments.Snapshots
{
    [TestClass]
    public class SnapshotDestroyArgsTest
    {
        [DataRow("tank/myds", "snap123", true)]
        [DataRow("tank/myds", "tank/myds@snap123", true)]
        [DataRow("tank%2fmyds", "tank%2fmyds%40snap123", true)]
        [DataRow("tank%2fmyds", "snap123 %26%26+rm+-rf+%2f", false)]
        [TestMethod]
        public void ValidateTest(string dataset, string snapshot, bool expectValid)
        {
            var args = new SnapshotDestroyArgs { Dataset = dataset, Snapshot = snapshot };
            var valid = args.Validate(out var errors);

            Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
            Assert.AreEqual(expectValid, valid);
        }

        [DataRow("tank/myds", "snap123", "destroy tank/myds@snap123")]
        [DataRow("tank/myds", "tank/myds@snap123", "destroy tank/myds@snap123")]
        [DataRow("tank%2fmyds", "tank%2fmyds%40snap123", "destroy tank/myds@snap123")]
        [TestMethod]
        public void ToStringTest(string dataset, string snapshot, string expected)
        {
            var args = new SnapshotDestroyArgs { Dataset = dataset, Snapshot = snapshot };
            var stringVer = args.ToString();
            Assert.AreEqual(expected, stringVer);
        }
    }
}
