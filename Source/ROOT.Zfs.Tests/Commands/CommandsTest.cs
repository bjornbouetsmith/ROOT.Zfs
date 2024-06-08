using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Public.Arguments.Dataset;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Commands
{
    [TestClass]
    public class CommandsTest
    {
        [DataRow(DatasetTypes.None, "", "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t filesystem,volume")]
        [DataRow(DatasetTypes.None, null, "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t filesystem,volume")]
        [DataRow(DatasetTypes.None, "tank/myds", "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t filesystem,volume tank/myds")]
        [DataRow(DatasetTypes.None, "tank%2Fmyds", "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t filesystem,volume tank/myds")] //test url encoded

        [DataRow(DatasetTypes.Snapshot, "tank/myds", "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t snapshot tank/myds")]
        [DataRow(DatasetTypes.Snapshot, null, "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t snapshot")]

        [DataRow(DatasetTypes.Bookmark, "tank/myds", "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t bookmark tank/myds")]
        [DataRow(DatasetTypes.Bookmark, null, "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t bookmark")]

        [DataRow(DatasetTypes.Volume, "tank/myds", "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t volume tank/myds")]
        [DataRow(DatasetTypes.Volume, null, "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t volume")]

        [DataRow(DatasetTypes.Filesystem, "tank/myds", "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t filesystem tank/myds")]
        [DataRow(DatasetTypes.Filesystem, null, "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t filesystem")]
        [TestMethod]
        public void ListCommandTest(DatasetTypes types, string root, string expectedCommand)
        {
            var args = new DatasetListArgs { DatasetTypes = types, Root = root };
            var command = Core.Commands.Commands.ZfsList(args);
            Assert.AreEqual(expectedCommand, command.Arguments);
        }

        [DataRow(DatasetTypes.Snapshot | DatasetTypes.Filesystem, "tank/myds", "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t filesystem,snapshot tank/myds")]
        [DataRow(DatasetTypes.Snapshot | DatasetTypes.Filesystem, null, "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t filesystem,snapshot")]
        [DataRow(DatasetTypes.Snapshot | DatasetTypes.Filesystem | DatasetTypes.Volume, "tank/myds", "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t filesystem,snapshot,volume tank/myds")]
        [DataRow(DatasetTypes.Snapshot | DatasetTypes.Filesystem | DatasetTypes.Volume | DatasetTypes.Bookmark, "tank/myds", "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t bookmark,filesystem,snapshot,volume tank/myds")]
        [TestMethod]
        public void ListCommandCombinationsTest(DatasetTypes types, string root, string expectedCommand)
        {
            var args = new DatasetListArgs { DatasetTypes = types, Root = root };
            var command = Core.Commands.Commands.ZfsList(args);
            Assert.AreEqual(expectedCommand, command.Arguments);
        }

        [DataRow(DatasetTypes.Snapshot | DatasetTypes.Filesystem, "tank/myds", "list -Hpr -o type,creation,name,used,refer,avail,mountpoint,origin -d 99 -t filesystem,snapshot tank/myds")]
        [DataRow(DatasetTypes.Snapshot | DatasetTypes.Filesystem, null, "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t filesystem,snapshot")]
        [DataRow(DatasetTypes.Snapshot | DatasetTypes.Filesystem | DatasetTypes.Volume, "tank/myds", "list -Hpr -o type,creation,name,used,refer,avail,mountpoint,origin -d 99 -t filesystem,snapshot,volume tank/myds")]
        [DataRow(DatasetTypes.Snapshot | DatasetTypes.Filesystem | DatasetTypes.Volume | DatasetTypes.Bookmark, "tank/myds", "list -Hpr -o type,creation,name,used,refer,avail,mountpoint,origin -d 99 -t bookmark,filesystem,snapshot,volume tank/myds")]
        [TestMethod]
        public void ListWithChildrenTest(DatasetTypes types, string root, string expectedCommand)
        {
            var args = new DatasetListArgs { DatasetTypes = types, Root = root, IncludeChildren = true };
            var command = Core.Commands.Commands.ZfsList(args);
            Assert.AreEqual(expectedCommand, command.Arguments);
        }

        [TestMethod]
        public void WhichCommandTest()
        {
            var command = Core.Commands.Commands.Which("smartctl");
            Assert.AreEqual("/bin/which smartctl", command.FullCommandLine);
        }

        [TestMethod]
        public void ListBlockDevicesTest()
        {
            var command = Core.Commands.Commands.ListBlockDevices();
            Assert.AreEqual("/bin/lsblk --include 8 --include 259 -p|grep disk", command.FullCommandLine);
        }

        [TestMethod]
        public void ListDisksTest()
        {
            var command = Core.Commands.Commands.ListDisks();
            Assert.AreEqual("/bin/ls -l /dev/disk/by-id/ | awk -F ' ' '{print $9,$11}'", command.FullCommandLine);
        }


        [DataRow("tank", false)]
        [DataRow("tank && rm -rf /", true)]
        [TestMethod]
        public void DataSetListTest(string root, bool expectException)
        {
            var args = new DatasetListArgs { Root = root };
            if (expectException)
            {
                var ex = Assert.ThrowsException<ArgumentException>(() => Core.Commands.Commands.ZfsList(args));
                Console.WriteLine(ex);
            }
            else
            {
                var command = Core.Commands.Commands.ZfsList(args);
                Assert.IsNotNull(command.FullCommandLine);
            }
        }
    }
}
