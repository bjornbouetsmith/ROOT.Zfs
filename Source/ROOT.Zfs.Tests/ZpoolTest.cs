using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;
using ROOT.Zfs.Core.Helpers;

namespace ROOT.Zfs.Tests
{
    [TestClass]
    public class ZpoolTest
    {
        private const string ZPoolHistory = @"History for 'tank':
2022-09-20.21:30:14 zpool create tank mirror /dev/disk/by-id/ata-QEMU_HARDDISK_QM00015 /dev/disk/by-id/ata-QEMU_HARDDISK_QM00017 [user 0 (root) on zfsdev.root.dom:linux]
2022-09-20.21:30:45 zpool import -c /etc/zfs/zpool.cache -aN [user 0 (root) on zfsdev.root.dom:linux]
2022-09-21.17:17:39 zfs create tank/test [user 0 (root) on zfsdev.root.dom:linux]
2022-09-21.17:25:16 zfs create tank/c0818844-0f03-4fd9-b2ea-b24926ef7191 [user 0 (root) on zfsdev.root.dom:linux]
2022-09-21.17:37:03 zfs destroy tank/c0818844-0f03-4fd9-b2ea-b24926ef7191 [user 0 (root) on zfsdev.root.dom:linux]";

        readonly SSHProcessCall _remoteProcessCall = new("bbs", "zfsdev.root.dom", true);

        [TestMethod, TestCategory("Integration")]
        public void GetHistoryTestWithSkip()
        {
            var zp = new ZPool(_remoteProcessCall);
            var lines = zp.GetHistory("tank").ToList().Count;

            var history = zp.GetHistory("tank", lines - 2).ToList();
            Assert.IsNotNull(history);
            Console.WriteLine(history.Dump(new JsonFormatter()));
            Assert.AreEqual(2, history.Count);
        }

        [TestMethod, TestCategory("Integration")]
        public void GetHistory()
        {
            var zp = new ZPool(_remoteProcessCall);
            var history = zp.GetHistory("tank");
            Assert.IsNotNull(history);
            Console.WriteLine(history.Dump(new JsonFormatter()));
        }

        [TestMethod, TestCategory("Integration")]
        public void GetHistoryAfterDate()
        {
            var zp = new ZPool(_remoteProcessCall);
            var last10AtMost = zp.GetHistory("tank").TakeLast(10).ToList();

            // This is safe, since zfs always have at last pool creation as a history event
            var history = last10AtMost.First();

            var afterDate = zp.GetHistory("tank", 0, history.Time).ToList();

            if (last10AtMost.Count > 1)
            {
                Assert.IsTrue(afterDate.Count < last10AtMost.Count);
            }

        }

        [TestMethod, TestCategory("Integration")]
        public void GetStatusTest()
        {
            var zp = new ZPool(_remoteProcessCall);
            var status = zp.GetStatus("tank");
            Assert.IsNotNull(status);
            Assert.AreEqual("tank", status.Pool.Name);
        }

        [TestMethod]
        public void ParsingTest()
        {
            var history = CommandHistoryHelper.FromStdOut(ZPoolHistory).ToList();
            Assert.AreEqual(5, history.Count);
            var expectedDate = new DateTime(2022, 09, 20, 21, 30, 14, DateTimeKind.Local);
            Assert.AreEqual("zpool create tank mirror /dev/disk/by-id/ata-QEMU_HARDDISK_QM00015 /dev/disk/by-id/ata-QEMU_HARDDISK_QM00017", history[0].Command);
            Assert.AreEqual("user 0 (root) on zfsdev.root.dom:linux", history[0].Caller);
            Assert.AreEqual(expectedDate, history[0].Time);

            var lastEntry = history[^1];
            var lastDate = new DateTime(2022, 09, 21, 17, 37, 3, DateTimeKind.Local);
            Assert.AreEqual("zfs destroy tank/c0818844-0f03-4fd9-b2ea-b24926ef7191", lastEntry.Command);
            Assert.AreEqual("user 0 (root) on zfsdev.root.dom:linux", lastEntry.Caller);
            Assert.AreEqual(lastDate, lastEntry.Time);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        public void SkipCountTest(int skip)
        {
            var history = CommandHistoryHelper.FromStdOut(ZPoolHistory, skip).ToList();
            Assert.AreEqual(5 - skip, history.Count);
            if (skip < 5)
            {
                var lastEntry = history[^1];
                var lastDate = new DateTime(2022, 09, 21, 17, 37, 3, DateTimeKind.Local);
                Assert.AreEqual("zfs destroy tank/c0818844-0f03-4fd9-b2ea-b24926ef7191", lastEntry.Command);
                Assert.AreEqual("user 0 (root) on zfsdev.root.dom:linux", lastEntry.Caller);
                Assert.AreEqual(lastDate, lastEntry.Time);
            }
            else
            {
                Assert.AreEqual(0, history.Count);
            }
        }

        [TestMethod]
        public void GreaterThanTest()
        {
            var cutOff = new DateTime(2022, 09, 21, 17, 25, 16);
            var history = CommandHistoryHelper.FromStdOut(ZPoolHistory, 0, cutOff).ToList();
            Assert.AreEqual(1, history.Count);
            var lastEntry = history[^1];
            var lastDate = new DateTime(2022, 09, 21, 17, 37, 3, DateTimeKind.Local);
            Assert.AreEqual("zfs destroy tank/c0818844-0f03-4fd9-b2ea-b24926ef7191", lastEntry.Command);
            Assert.AreEqual("user 0 (root) on zfsdev.root.dom:linux", lastEntry.Caller);
            Assert.AreEqual(lastDate, lastEntry.Time);
        }
    }
}
