using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Core.Commands;
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
            var expectedPrefix = $"/sbin/zfs snap tank/myds@{DateTime.UtcNow:yyyyMMddHHmm}";
            var command = SnapshotCommands.CreateSnapshot("tank/myds");
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

        [DataRow("tank/myds", "projectX", "/sbin/zfs destroy tank/myds@projectX")]
        [DataRow("tank/myds", "tank/myds@projectX", "/sbin/zfs destroy tank/myds@projectX")]
        [TestMethod]
        public void DestroyDatasetCommandTest(string dataset, string snapName, string expected)
        {
            var command = SnapshotCommands.DestroySnapshot(dataset, snapName);
            Assert.AreEqual(expected, command.FullCommandLine);
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

    }
}
