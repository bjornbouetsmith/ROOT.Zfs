using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Arguments.Snapshots;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Integration.Fake
{
    [TestClass]
    public class FakeSnapshotTest
    {
        private readonly FakeRemoteConnection _remoteProcessCall = new("2.1.5-2");

        private ISnapshots GetSnapshots()
        {
            return new Snapshots(_remoteProcessCall);
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void ListSnapshotsTest()
        {
            var sn = GetSnapshots();

            var snapshots = sn.List(new SnapshotListArgs { Root = "tank/myds" });
            Assert.IsNotNull(snapshots);
            foreach (var snap in snapshots)
            {
                Console.WriteLine(snap.CreationDate.AsString());
                Console.WriteLine(snap.SnapshotName);
                Console.WriteLine(snap.Size.ToString());
            }
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void RemoteCreateAndDestroySnapshot()
        {
            var snapName = "RemoteCreateSnapshot" + new DateTime(2022, 09, 22, 21, 13, 47, DateTimeKind.Local).ToString("yyyyMMddhhmmss");

            var sn = GetSnapshots();

            sn.Create(new SnapshotCreateArgs { Dataset = "tank/myds", Snapshot = snapName });

            var snaps = sn.List(new SnapshotListArgs { Root = "tank/myds" });
            Assert.IsNotNull(snaps);
            var args = new SnapshotDestroyArgs { Dataset = "tank/myds", Snapshot = snapName, IsExactName = true };
            sn.Destroy(args);

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
            var sn = GetSnapshots();
            var prefix = new DateTime(2022, 09, 22, 21, 13, 47, DateTimeKind.Local).ToString("yyyyMMddHHmmssfff");
            sn.Create(new SnapshotCreateArgs { Dataset = "tank/myds", Snapshot = $"{prefix}-1" });
            sn.Create(new SnapshotCreateArgs { Dataset = "tank/myds", Snapshot = $"{prefix}-2" });
            sn.Create(new SnapshotCreateArgs { Dataset = "tank/myds", Snapshot = $"{prefix}-3" });

            var snaps = sn.List(new SnapshotListArgs { Root = "tank/myds" }).Where(snap => snap.SnapshotName.StartsWith($"{prefix}")).ToList();

            Assert.AreEqual(3, snaps.Count);
            var args = new SnapshotDestroyArgs { Dataset = "tank/myds", Snapshot = prefix, IsExactName = false };
            sn.Destroy(args);
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void SnapshotHoldTest()
        {
            var sn = GetSnapshots();
            sn.Hold(new SnapshotHoldArgs { Dataset = "tank/myds", Snapshot = "12345", Tag = "mytag" });
            var commands = _remoteProcessCall.GetCommandsInvoked();
            Assert.AreEqual(1, commands.Count);
            Assert.AreEqual("/sbin/zfs hold mytag tank/myds@12345", commands[0]);
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void SnapshotHoldsTest()
        {
            var sn = GetSnapshots();
            var args = new SnapshotHoldsArgs { Dataset = "tank/myds", Snapshot = "tank/myds@12345"};
            var holds = sn.Holds(args);
            Assert.AreEqual(1, holds.Count);
            var commands = _remoteProcessCall.GetCommandsInvoked();
            Assert.AreEqual(1, commands.Count);
            Assert.AreEqual("/sbin/zfs holds -H tank/myds@12345", commands[0]);
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void SnapshotReleaseTest()
        {
            var sn = GetSnapshots();
            sn.Release(new SnapshotReleaseArgs{Dataset="tank/myds",Snapshot= "12345", Tag="mytag"});

            var commands = _remoteProcessCall.GetCommandsInvoked();
            Assert.AreEqual(1, commands.Count);
            Assert.AreEqual("/sbin/zfs release mytag tank/myds@12345", commands[0]);
        }
    }
}
