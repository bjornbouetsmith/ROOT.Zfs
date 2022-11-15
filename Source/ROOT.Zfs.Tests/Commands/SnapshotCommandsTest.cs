using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Public.Arguments.Snapshots;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Commands
{
    [TestClass]
    public class SnapshotCommandsTest
    {
        [TestMethod]
        public void CreateSnapshotWithNameTest()
        {
            var command = SnapshotCommands.CreateSnapshot("tank/myds", "projectX");
            Assert.AreEqual("/sbin/zfs snap tank/myds@projectX", command.FullCommandLine);
        }

        [TestMethod]
        public void CreateSnapshotWithoutNameTest()
        {
            var expectedPrefix = $"/sbin/zfs snap tank/myds@{DateTime.UtcNow.ToLocalTime():yyyyMMddHHmm}";
            var command = SnapshotCommands.CreateSnapshot("tank/myds", null);
            Console.WriteLine(command.FullCommandLine);
            Console.WriteLine(expectedPrefix);
            Assert.IsTrue(command.FullCommandLine.StartsWith(expectedPrefix));
        }

        [TestMethod]
        public void CreateSnapshotNameTest()
        {
            var time = new DateTime(2022, 09, 22, 21, 13, 47, DateTimeKind.Local);

            var name = SnapshotCommands.CreateSnapshotName(time);
            Assert.AreEqual("20220922211347", name);
        }

        [DataRow("tank/myds", "/sbin/zfs list -Hpr -o type,creation,name,used,refer,avail,mountpoint -d 99 -t snapshot tank/myds", false)]
        [DataRow("tank/myds && rm -rf /", null, true)]
        [TestMethod]
        public void ListSnapshotsTest(string dataset, string expectedCommand, bool expectException)
        {
            var args = new SnapshotListArgs { Root = dataset };
            if (expectException)
            {
                var ex = Assert.ThrowsException<ArgumentException>(() => SnapshotCommands.ListSnapshots(args));
                Console.WriteLine(ex);
            }
            else
            {
                var command = SnapshotCommands.ListSnapshots(args);
                Assert.AreEqual(expectedCommand, command.FullCommandLine);
            }
        }

        [DataRow("tank/myds", "projectX", "/sbin/zfs destroy tank/myds@projectX", false)]
        [DataRow("tank/myds", "tank/myds@projectX", "/sbin/zfs destroy tank/myds@projectX", false)]
        [DataRow("tank/myds", "tank/myds@projectX && rm -rf /", null, true)]
        [TestMethod]
        public void DestroyDatasetCommandTest(string dataset, string snapName, string expected, bool expectException)
        {
            var args = new SnapshotDestroyArgs { Dataset = dataset, Snapshot = snapName };
            if (expectException)
            {
                var ex = Assert.ThrowsException<ArgumentException>(() => SnapshotCommands.DestroySnapshot(args));
                Console.WriteLine(ex);
            }
            else
            {
                var command = SnapshotCommands.DestroySnapshot(args);

                Assert.AreEqual(expected, command.FullCommandLine);
            }
        }

        [TestMethod]
        public void CreateSnapshotWithInvalidNameShouldThrowArgumentException()
        {
            var bogusName = "2022/05/17";
            var ex = Assert.ThrowsException<ArgumentException>(() => SnapshotCommands.CreateSnapshot("tank/myds", bogusName));
            Assert.IsTrue(ex.Message.StartsWith(bogusName));
        }

        [TestMethod]
        public void CloneSnapshotTestWithoutProperties()
        {
            var command = SnapshotCommands.Clone("tank/myds", "projectX", "/tank/clones/projectX", null);
            Assert.AreEqual("/sbin/zfs clone -p tank/myds@projectX /tank/clones/projectX", command.FullCommandLine);
        }

        [TestMethod]
        public void CloneSnapshotTestWithoutProperties2()
        {
            var command = SnapshotCommands.Clone("tank/myds", "tank/myds@projectX", "/tank/clones/projectX", null);
            Assert.AreEqual("/sbin/zfs clone -p tank/myds@projectX /tank/clones/projectX", command.FullCommandLine);
        }

        [TestMethod]
        public void CloneSnapshotTestWithProperties()
        {
            var command = SnapshotCommands.Clone("tank/myds", "projectX", "/tank/clones/projectX", new[] { new PropertyValue { Property = "atime", Value = "off" }, new PropertyValue { Property = "compression", Value = "off" } });
            Assert.AreEqual("/sbin/zfs clone -p -o atime=off -o compression=off tank/myds@projectX /tank/clones/projectX", command.FullCommandLine);
        }

        [DataRow("tank/myds@12345", "myhold", true, "/sbin/zfs hold -r myhold tank/myds@12345")]
        [DataRow("tank%2fmyds@12345", "myhold", false, "/sbin/zfs hold myhold tank/myds@12345")]
        [DataRow("tank%2fmyds%4012345", "myhold", false, "/sbin/zfs hold myhold tank/myds@12345")]
        [DataRow("tank%2fmyds%4012345", "", false, null, true)]
        [DataRow("tank%2fmyds%4012345", null, false, null, true)]
        [DataRow("", "myhold", false, null, true)]
        [DataRow(null, "myhold", false, null, true)]
        [TestMethod]
        public void HoldTest(string snapshot, string tag, bool recursive, string expected, bool expectException = false)
        {
            if (expectException)
            {
                Assert.ThrowsException<ArgumentException>(() => SnapshotCommands.Hold(snapshot, tag, recursive));
            }
            else
            {
                var command = SnapshotCommands.Hold(snapshot, tag, recursive);
                Console.WriteLine(command.FullCommandLine);
                Assert.AreEqual(expected, command.FullCommandLine);
            }
        }

        [DataRow("tank/myds@12345", true, "/sbin/zfs holds -r -H tank/myds@12345")]
        [DataRow("tank%2fmyds@12345", false, "/sbin/zfs holds -H tank/myds@12345")]
        [DataRow("tank%2fmyds%4012345", false, "/sbin/zfs holds -H tank/myds@12345")]
        [DataRow("", false, null, true)]
        [DataRow(null, false, null, true)]
        [TestMethod]
        public void HoldsTest(string snapshot, bool recursive, string expected, bool expectException = false)
        {
            if (expectException)
            {
                Assert.ThrowsException<ArgumentException>(() => SnapshotCommands.Holds(snapshot, recursive));
            }
            else
            {
                var command = SnapshotCommands.Holds(snapshot, recursive);
                Console.WriteLine(command.FullCommandLine);
                Assert.AreEqual(expected, command.FullCommandLine);
            }
        }
        [DataRow("tank/myds@12345", "myhold", true, "/sbin/zfs release -r myhold tank/myds@12345")]
        [DataRow("tank%2fmyds@12345", "myhold", false, "/sbin/zfs release myhold tank/myds@12345")]
        [DataRow("tank%2fmyds%4012345", "myhold", false, "/sbin/zfs release myhold tank/myds@12345")]
        [DataRow("tank%2fmyds%4012345", "", false, null, true)]
        [DataRow("tank%2fmyds%4012345", null, false, null, true)]
        [DataRow("", "myhold", false, null, true)]
        [DataRow(null, "myhold", false, null, true)]
        [TestMethod]
        public void ReleaseTest(string snapshot, string tag, bool recursive, string expected, bool expectException = false)
        {
            if (expectException)
            {
                Assert.ThrowsException<ArgumentException>(() => SnapshotCommands.Release(snapshot, tag, recursive));
            }
            else
            {
                var command = SnapshotCommands.Release(snapshot, tag, recursive);
                Console.WriteLine(command.FullCommandLine);
                Assert.AreEqual(expected, command.FullCommandLine);
            }
        }

    }
}
