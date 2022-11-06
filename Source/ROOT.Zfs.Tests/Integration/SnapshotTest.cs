using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;
using ROOT.Zfs.Public;

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
            sn.Create(pool.Name, null);
            var snapshots = sn.List(pool.Name);
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

            sn.Create(pool.Name, snapName);

            var snaps = sn.List(pool.Name);

            var wasCreated = snaps.FirstOrDefault(snap => snap.Name.EndsWith(snapName));

            Assert.IsNotNull(wasCreated);

            sn.Destroy(pool.Name, snapName, true);

        }


        [TestMethod, TestCategory("Integration")]
        public void CreateAndDeleteByPatternTest()
        {
            var sn = GetSnapshots();
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var prefix = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            sn.Create(pool.Name, $"{prefix}-1");
            sn.Create(pool.Name, $"{prefix}-2");
            sn.Create(pool.Name, $"{prefix}-3");

            var snaps = sn.List(pool.Name).Where(snap => snap.Name.StartsWith($"{pool.Name}@{prefix}")).ToList();

            Assert.AreEqual(3, snaps.Count);

            sn.Destroy(pool.Name, prefix, false);

            snaps = sn.List(pool.Name).Where(snap => snap.Name.StartsWith($"{pool.Name}@{prefix}")).ToList();
            Assert.AreEqual(0, snaps.Count);
        }
    }
}
