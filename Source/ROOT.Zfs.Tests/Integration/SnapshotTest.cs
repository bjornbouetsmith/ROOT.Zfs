using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Arguments.Dataset;
using ROOT.Zfs.Public.Arguments.Snapshots;
using ROOT.Zfs.Public.Data;

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

        private IDatasets GetDatasets()
        {
            var sn = new Datasets(_remoteProcessCall);
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
                Console.WriteLine(snap.SnapshotName);
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

            var wasCreated = snaps.FirstOrDefault(snap => snap.SnapshotName.EndsWith(snapName));

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

            var snaps = sn.List(new SnapshotListArgs { Root = pool.Name }).Where(snap => snap.SnapshotName.StartsWith(prefix)).ToList();

            Assert.AreEqual(3, snaps.Count);
            var args = new SnapshotDestroyArgs { Dataset = pool.Name, Snapshot = prefix, IsExactName = false };
            sn.Destroy(args);

            snaps = sn.List(new SnapshotListArgs { Root = pool.Name }).Where(snap => snap.SnapshotName.StartsWith(prefix)).ToList();
            Assert.AreEqual(0, snaps.Count);
        }

        [TestMethod, TestCategory("Integration")]
        public void CloneSnapshotTest()
        {
            var sn = GetSnapshots();
            var ds = GetDatasets();
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            sn.Create(new SnapshotCreateArgs { Dataset = pool.Name, Snapshot = "mysnap" });

            var args = new SnapshotCloneArgs { Dataset = pool.Name, Snapshot = "mysnap", TargetDataset = $"{pool.Name}/mysnap_clone" };
            sn.Clone(args);

            var getds = new DatasetListArgs { Root = pool.Name, IncludeChildren = true };
            var datasets = ds.List(getds);
            var myClone = datasets.FirstOrDefault(d => d.DatasetName == $"{pool.Name}/mysnap_clone");
            Assert.IsNotNull(myClone);
            Assert.IsTrue(myClone.IsClone);
        }
    }
}
