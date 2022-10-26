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
        private readonly IProcessCall _remoteProcessCall = TestHelpers.RequiresRemoteConnection ? new SSHProcessCall("bbs", "zfsdev.root.dom", true) : null;

        private Snapshots GetSnapshots()
        {
            var sn = new Snapshots(_remoteProcessCall);
            sn.RequiresSudo = TestHelpers.RequiresSudo;
            return sn;
        }

        [TestMethod, TestCategory("Integration")]
        public void RemoteSnapshotTest()
        {
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var sn = GetSnapshots();
            sn.CreateSnapshot(pool.Name);
            var snapshots = sn.GetSnapshots(pool.Name);
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
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var sn = GetSnapshots();

            sn.CreateSnapshot(pool.Name, snapName);

            var snaps = sn.GetSnapshots(pool.Name);

            var wasCreated = snaps.FirstOrDefault(snap => snap.Name.EndsWith(snapName));

            Assert.IsNotNull(wasCreated);

            sn.DestroySnapshot(pool.Name, snapName, true);

        }


        [TestMethod, TestCategory("Integration")]
        public void CreateAndDeleteByPatternTest()
        {
            var sn = GetSnapshots();
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var prefix = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            sn.CreateSnapshot(pool.Name, $"{prefix}-1");
            sn.CreateSnapshot(pool.Name, $"{prefix}-2");
            sn.CreateSnapshot(pool.Name, $"{prefix}-3");

            var snaps = sn.GetSnapshots(pool.Name).Where(snap => snap.Name.StartsWith($"{pool.Name}@{prefix}")).ToList();

            Assert.AreEqual(3, snaps.Count);

            sn.DestroySnapshot(pool.Name, prefix, false);

            snaps = sn.GetSnapshots(pool.Name).Where(snap => snap.Name.StartsWith($"{pool.Name}@{prefix}")).ToList();
            Assert.AreEqual(0, snaps.Count);
        }
    }
}
