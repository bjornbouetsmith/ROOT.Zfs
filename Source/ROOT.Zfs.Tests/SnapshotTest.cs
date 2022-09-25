using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;
using ROOT.Zfs.Core.Commands;

namespace ROOT.Zfs.Tests
{
    [TestClass]
    public class SnapshotTest
    {
        private RemoteProcessCall _pc = new RemoteProcessCall("bbs", "zfsdev.root.dom", true);

        private const string SnapshotList = @"1663944453      tank/myds@RemoteCreateSnapshot20220923144730    10
1663944856      tank/myds@RemoteCreateSnapshot20220923145450    20
1663945816      tank/myds@RemoteCreateSnapshot20220923101050    30
1663947484      tank/myds@RemoteCreateSnapshot20220923153801    40
1663947547      tank/myds@RemoteCreateSnapshot20220923153941    50";


        [TestMethod]
        public void SnapshotParserTest()
        {
            var list = SnapshotParser.Parse(SnapshotList).ToList();

            Assert.AreEqual(5, list.Count);

            foreach (var snap in list)
            {
                Console.WriteLine(snap.CreationDate.AsString());
                Console.WriteLine(snap.Name);
                Console.WriteLine(snap.Size.AsString());
            }
        }

        [TestMethod, TestCategory("Integration")]
        public void RemoteSnapshotTest()
        {
            var sn = new Core.Snapshot(_pc);

            var snapshots = sn.GetSnapshots("tank/myds");

            foreach (var snap in snapshots)
            {
                Console.WriteLine(snap.CreationDate.AsString());
                Console.WriteLine(snap.Name);
                Console.WriteLine(snap.Size.AsString());
            }
        }

        [TestMethod, TestCategory("Integration")]
        public void RemoteCreateAndDestroySnapshot()
        {
            var snapName = "RemoteCreateSnapshot" + DateTime.UtcNow.ToString("yyyyMMddhhmmss");

            var sn = new Core.Snapshot(_pc);

            sn.CreateSnapshot("tank/myds", snapName);

            var snaps = sn.GetSnapshots("tank/myds");

            var wasCreated = snaps.FirstOrDefault(snap => snap.Name.EndsWith(snapName));

            Assert.IsNotNull(wasCreated);

            sn.DestroySnapshot("tank/myds", snapName);

        }

        [TestMethod, TestCategory("Integration")]
        public void SnapshotsWithoutGivenNameShouldUseCurrentDate()
        {
            var time = new DateTime(2022, 09, 22, 21, 13, 47, DateTimeKind.Local);

            var name = Snapshots.ProcessCalls.CreateSnapshotName(time);
            Assert.AreEqual("20220922211347", name);

            var sn = new Core.Snapshot(_pc);

            sn.CreateSnapshot("tank/myds");

        }

        //[TestMethod]
        //public void MyTestMethod()
        //{
        //    var sn = new Core.Snapshot(_pc);
        //    sn.DestroySnapshot("tank/myds", "RemoteCreateSnapshot", false);
        //}

        [TestMethod, TestCategory("Integration")]
        public void CreateAndDeleteByPatternTest()
        {
            var sn = new Core.Snapshot(_pc);
            var prefix = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            sn.CreateSnapshot("tank/myds", $"{prefix}-1");
            sn.CreateSnapshot("tank/myds", $"{prefix}-2");
            sn.CreateSnapshot("tank/myds", $"{prefix}-3");

            var snaps = sn.GetSnapshots("tank/myds").Where(snap => snap.Name.StartsWith($"tank/myds@{prefix}")).ToList();

            Assert.AreEqual(3, snaps.Count);

            sn.DestroySnapshot("tank/myds", prefix, false);

            snaps = sn.GetSnapshots("tank/myds").Where(snap => snap.Name.StartsWith($"tank/myds@{prefix}")).ToList();
            Assert.AreEqual(0, snaps.Count);

        }


        [TestMethod]
        [DataRow("tank/myds", "tank/myds@testing123", "testing123", true)] // Exact match except for dataset prefix
        [DataRow("tank/myds", "tank/myds@testing123", "testing", true)] // partial match
        [DataRow("tank/myds", "tank/myds@testing123", "tank/myds@testing123", true)] // ExactMatch
        [DataRow("tank/myds", "tank/myds@testing123", "tank/myds@tes", true)] // partial match
        [DataRow("tank/myds", "tank/myds@testing123", "esting123", false)] // we only match on beginning
        [DataRow("tank2/myds", "tank/myds@testing123", "testing123", false)] //wrong dataset
        public void SnapshotMatchingTest(string dataset, string snapshotName, string pattern, bool expectMatch)
        {
            var isMatch = Snapshot.SnapshotMatches(dataset, snapshotName, pattern);
            Assert.AreEqual(expectMatch, isMatch);
        }
    }
}
