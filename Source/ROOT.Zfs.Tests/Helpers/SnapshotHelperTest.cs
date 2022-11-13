using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Core;
using ROOT.Zfs.Core.Helpers;

namespace ROOT.Zfs.Tests.Helpers
{
    [TestClass]
    public class SnapshotHelperTest
    {
        private const string SnapshotList = @"snapshot        1664116031      tank/myds@20220925162707        14336   25600   -       -";

        private const string JunkLine = @"snapshot        1664116031      tank/myds@20220925162707        14336   25600";
        private const string JunkLine2 = @"snapshot        2022-05-05      tank/myds@20220925162707        40G   80G   -       -";

        [TestMethod]
        public void BadInputTest1()
        {
            var ex = Assert.ThrowsException<FormatException>(() => SnapshotHelper.FromString(JunkLine));
            Assert.IsTrue(ex.Message.Contains("expected 7 parts"));

        }

        [TestMethod]
        public void BadInputTest2()
        {
            var snapshot = SnapshotHelper.FromString(JunkLine2);
            Assert.AreEqual(default, snapshot.CreationDate);
        }

        [TestMethod]
        public void GoodInput()
        {
            var snapshot = SnapshotHelper.FromString(SnapshotList);
            Assert.AreEqual("tank/myds@20220925162707", snapshot.Name);
            Assert.AreEqual(14336UL, snapshot.Size.Bytes);
            Assert.AreEqual(new DateTime(2022, 09, 25, 14, 27, 11, DateTimeKind.Utc), snapshot.CreationDate);
        }

        [TestMethod]
        [DataRow("tank/myds", "tank/myds@testing123", "testing123", true)] // Exact match except for dataset prefix
        [DataRow("tank/myds", "tank/myds@testing123", "testing", true)] // partial match
        [DataRow("tank/myds", "tank/myds@testing123", "tank/myds@testing123", true)] // ExactMatch
        [DataRow("tank/myds", "tank/myds@testing123", "tank/myds@tes", true)] // partial match
        [DataRow("tank/myds", "tank/myds@testing123", "esting123", false)] // we only match on beginning
        [DataRow("tank2/myds", "tank/myds@testing123", "testing123", false)] //wrong dataset
        [DataRow("tank%2Fmyds", "tank/myds@testing123", "testing123", true)] //right dataset, url encoded
        public void SnapshotMatchingTest(string dataset, string snapshotName, string pattern, bool expectMatch)
        {
            var isMatch = Snapshots.SnapshotMatches(dataset, snapshotName, pattern);
            Assert.AreEqual(expectMatch, isMatch);
        }
    }
}
