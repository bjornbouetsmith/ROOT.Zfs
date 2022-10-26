using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;
using ROOT.Zfs.Core.Commands;

namespace ROOT.Zfs.Tests.Integration
{
    [TestClass]
    public class SnapshotTest
    {
        private readonly IProcessCall _remoteProcessCall = Environment.MachineName == "BBS-DESKTOP" ? new SSHProcessCall("bbs", "zfsdev.root.dom", true) : new ProcessCall("/usr/bin/sudo");

        [TestMethod, TestCategory("Integration")]
        public void RemoteSnapshotTest()
        {
            var sn = new Snapshots(_remoteProcessCall);

            var snapshots = sn.GetSnapshots("tank/myds");
            Assert.IsNotNull(snapshots);
            foreach (var snap in snapshots)
            {
                Console.WriteLine(snap.CreationDate.AsString());
                Console.WriteLine(snap.Name);
                Console.WriteLine(snap.Size.ToString());
            }
        }

        [TestMethod, TestCategory("Integration")]
        public void RemoteCreateAndDestroySnapshot()
        {
            var snapName = "RemoteCreateSnapshot" + DateTime.UtcNow.ToString("yyyyMMddhhmmss");

            var sn = new Snapshots(_remoteProcessCall);

            sn.CreateSnapshot("tank/myds", snapName);

            var snaps = sn.GetSnapshots("tank/myds");

            var wasCreated = snaps.FirstOrDefault(snap => snap.Name.EndsWith(snapName));

            Assert.IsNotNull(wasCreated);

            sn.DestroySnapshot("tank/myds", snapName, true);

        }

        [TestMethod, TestCategory("Integration")]
        public void SnapshotsWithoutGivenNameShouldUseCurrentDate()
        {
            var time = new DateTime(2022, 09, 22, 21, 13, 47, DateTimeKind.Local);

            var name = SnapshotCommands.CreateSnapshotName(time);
            Assert.AreEqual("20220922211347", name);

            var sn = new Snapshots(_remoteProcessCall);

            sn.CreateSnapshot("tank/myds");

        }

        [TestMethod, TestCategory("Integration")]
        public void CreateAndDeleteByPatternTest()
        {
            var sn = new Snapshots(_remoteProcessCall);
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
    }
}
