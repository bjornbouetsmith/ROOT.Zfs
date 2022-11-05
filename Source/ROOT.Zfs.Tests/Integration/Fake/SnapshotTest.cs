using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Public;

namespace ROOT.Zfs.Tests.Integration.Fake
{
    [TestClass]
    public class FakeSnapshotTest
    {
        private readonly FakeRemoteConnection _remoteProcessCall = new ("2.1.5-2");

        private ISnapshots GetSnapshots()
        {
            return new Snapshots(_remoteProcessCall);
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void ListSnapshotsTest()
        {
            var sn = GetSnapshots();

            var snapshots = sn.List("tank/myds");
            Assert.IsNotNull(snapshots);
            foreach (var snap in snapshots)
            {
                Console.WriteLine(snap.CreationDate.AsString());
                Console.WriteLine(snap.Name);
                Console.WriteLine(snap.Size.ToString());
            }
        }
        
        [TestMethod, TestCategory("FakeIntegration")]
        public void RemoteCreateAndDestroySnapshot()
        {
            var snapName = "RemoteCreateSnapshot" + new DateTime(2022, 09, 22, 21, 13, 47, DateTimeKind.Local).ToString("yyyyMMddhhmmss");

            var sn = GetSnapshots();

            sn.CreateSnapshot("tank/myds", snapName);

            var snaps = sn.List("tank/myds");
            Assert.IsNotNull(snaps);

            sn.Destroy("tank/myds", snapName, true);

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
            sn.CreateSnapshot("tank/myds", $"{prefix}-1");
            sn.CreateSnapshot("tank/myds", $"{prefix}-2");
            sn.CreateSnapshot("tank/myds", $"{prefix}-3");

            var snaps = sn.List("tank/myds").Where(snap => snap.Name.StartsWith($"tank/myds@{prefix}")).ToList();

            Assert.AreEqual(3, snaps.Count);

            sn.Destroy("tank/myds", prefix, false);
        }

        [TestMethod]
        public void SnapshotHoldTest()
        {
            var sn = GetSnapshots();
            sn.Hold("tank/myds@12345", "mytag", false);
            var commands = _remoteProcessCall.GetCommandsInvoked();
            Assert.AreEqual(1,commands.Count);
            Assert.AreEqual("/sbin/zfs hold mytag tank/myds@12345", commands[0]);
        }

        [TestMethod]
        public void SnapshotHoldsTest()
        {
            var sn = GetSnapshots();
            var holds = sn.Holds("tank/myds@12345", false);
            Assert.AreEqual(1,holds.Count);
            var commands = _remoteProcessCall.GetCommandsInvoked();
            Assert.AreEqual(1, commands.Count);
            Assert.AreEqual("/sbin/zfs holds -H tank/myds@12345", commands[0]);
        }

        [TestMethod]
        public void SnapshotReleaseTest()
        {
            var sn = GetSnapshots();
            sn.Release("tank/myds@12345","mytag", false);
            
            var commands = _remoteProcessCall.GetCommandsInvoked();
            Assert.AreEqual(1, commands.Count);
            Assert.AreEqual("/sbin/zfs release mytag tank/myds@12345", commands[0]);
        }
    }
}
