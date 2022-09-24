using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
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

            var snapshots = sn.LoadSnapshots("tank/myds");

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

            var snaps = sn.LoadSnapshots("tank/myds");

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
    }
}
