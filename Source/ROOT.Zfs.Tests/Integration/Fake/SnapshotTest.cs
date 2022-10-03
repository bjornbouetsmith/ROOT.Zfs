using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;
using ROOT.Zfs.Core.Commands;

namespace ROOT.Zfs.Tests.Integration.Fake
{
    [TestClass]
    public class FakeSnapshotTest
    {
        private readonly IProcessCall _remoteProcessCall = new FakeRemoteConnection("2.1.5-2");

        [TestMethod, TestCategory("FakeIntegration")]
        public void GetSnapshotsTest()
        {
            var sn = new Snapshots(_remoteProcessCall);

            var snapshots = sn.GetSnapshots("tank/myds");
            Assert.IsNotNull(snapshots);
            foreach (var snap in snapshots)
            {
                Console.WriteLine(snap.CreationDate.AsString());
                Console.WriteLine(snap.Name);
                Console.WriteLine(snap.Size.AsString());
            }
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void RemoteCreateAndDestroySnapshot()
        {
            var snapName = "RemoteCreateSnapshot" + new DateTime(2022, 09, 22, 21, 13, 47, DateTimeKind.Local).ToString("yyyyMMddhhmmss");

            var sn = new Snapshots(_remoteProcessCall);

            sn.CreateSnapshot("tank/myds", snapName);

            var snaps = sn.GetSnapshots("tank/myds");
            Assert.IsNotNull(snaps);

            sn.DestroySnapshot("tank/myds", snapName, true);

        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void CreateSnapshotNameTEst()
        {
            var time = new DateTime(2022, 09, 22, 21, 13, 47, DateTimeKind.Local);

            var name = SnapshotCommands.CreateSnapshotName(time);
            Assert.AreEqual("20220922211347", name);
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void CreateAndDeleteByPatternTest()
        {
            var sn = new Snapshots(_remoteProcessCall);
            var prefix = new DateTime(2022, 09, 22, 21, 13, 47, DateTimeKind.Local).ToString("yyyyMMddHHmmssfff");
            sn.CreateSnapshot("tank/myds", $"{prefix}-1");
            sn.CreateSnapshot("tank/myds", $"{prefix}-2");
            sn.CreateSnapshot("tank/myds", $"{prefix}-3");

            var snaps = sn.GetSnapshots("tank/myds").Where(snap => snap.Name.StartsWith($"tank/myds@{prefix}")).ToList();

            Assert.AreEqual(3, snaps.Count);

            sn.DestroySnapshot("tank/myds", prefix, false);
        }
    }
}
