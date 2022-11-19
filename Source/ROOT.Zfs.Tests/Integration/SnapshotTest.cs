using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Arguments.Snapshots;

namespace ROOT.Zfs.Tests.Integration
{
    [TestClass]
    public class SnapshotTest
    {
        private readonly IProcessCall _remoteProcessCall = TestHelpers.RequiresRemoteConnection ? new SSHProcessCall("bbs", "zfsdev.root.dom", true) : null;

        private ISnapshots GetSnapshots()
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
            sn.Create(new SnapshotCreateArgs { Dataset = pool.Name });
            var snapshots = sn.List(new SnapshotListArgs { Root = pool.Name });
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

            sn.Create(new SnapshotCreateArgs { Dataset = pool.Name, Snapshot = snapName });

            var snaps = sn.List(new SnapshotListArgs { Root = pool.Name });

            var wasCreated = snaps.FirstOrDefault(snap => snap.Name.EndsWith(snapName));

            Assert.IsNotNull(wasCreated);
            var args = new SnapshotDestroyArgs { Dataset = pool.Name, Snapshot = snapName, IsExactName = true };
            sn.Destroy(args);

        }


        [TestMethod, TestCategory("Integration")]
        public void CreateAndDeleteByPatternTest()
        {
            var sn = GetSnapshots();
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var prefix = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            sn.Create(new SnapshotCreateArgs { Dataset = pool.Name, Snapshot = $"{prefix}-1" });
            sn.Create(new SnapshotCreateArgs { Dataset = pool.Name, Snapshot = $"{prefix}-2" });
            sn.Create(new SnapshotCreateArgs { Dataset = pool.Name, Snapshot = $"{prefix}-3" });

            var snaps = sn.List(new SnapshotListArgs { Root = pool.Name }).Where(snap => snap.Name.StartsWith(prefix)).ToList();

            Assert.AreEqual(3, snaps.Count);
            var args = new SnapshotDestroyArgs { Dataset = pool.Name, Snapshot = prefix, IsExactName = false };
            sn.Destroy(args);

            snaps = sn.List(new SnapshotListArgs { Root = pool.Name }).Where(snap => snap.Name.StartsWith(prefix)).ToList();
            Assert.AreEqual(0, snaps.Count);
        }
    }
}
