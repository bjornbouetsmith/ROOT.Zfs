using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Commands
{
    [TestClass]
    public class BaseCommandsTest
    {
        [DataRow(DatasetType.NotSet, "", "list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t filesystem,volume")]
        [DataRow(DatasetType.NotSet, null, "list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t filesystem,volume")]
        [DataRow(DatasetType.NotSet, "tank/myds", "list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t filesystem,volume tank/myds")]
        [DataRow(DatasetType.NotSet, "tank%2Fmyds", "list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t filesystem,volume tank/myds")] //test url encoded

        [DataRow(DatasetType.Snapshot, "tank/myds", "list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t snapshot tank/myds")]
        [DataRow(DatasetType.Snapshot, null, "list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t snapshot")]

        [DataRow(DatasetType.Bookmark, "tank/myds", "list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t bookmark tank/myds")]
        [DataRow(DatasetType.Bookmark, null, "list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t bookmark")]

        [DataRow(DatasetType.Volume, "tank/myds", "list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t volume tank/myds")]
        [DataRow(DatasetType.Volume, null, "list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t volume")]

        [DataRow(DatasetType.Filesystem, "tank/myds", "list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t filesystem tank/myds")]
        [DataRow(DatasetType.Filesystem, null, "list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t filesystem")]
        [TestMethod]
        public void ListCommandTest(DatasetType types, string root, string expectedCommand)
        {
            var command = DatasetCommands.ZfsList(types, root, false);
            Assert.AreEqual(expectedCommand, command.Arguments);
        }

        [DataRow(DatasetType.Snapshot | DatasetType.Filesystem, "tank/myds", "list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t filesystem,snapshot tank/myds")]
        [DataRow(DatasetType.Snapshot | DatasetType.Filesystem, null, "list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t filesystem,snapshot")]
        [DataRow(DatasetType.Snapshot | DatasetType.Filesystem | DatasetType.Volume, "tank/myds", "list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t filesystem,snapshot,volume tank/myds")]
        [DataRow(DatasetType.Snapshot | DatasetType.Filesystem | DatasetType.Volume | DatasetType.Bookmark, "tank/myds", "list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t bookmark,filesystem,snapshot,volume tank/myds")]
        [TestMethod]
        public void ListCommandCombinationsTest(DatasetType types, string root, string expectedCommand)
        {
            var command = DatasetCommands.ZfsList(types, root, false);
            Assert.AreEqual(expectedCommand, command.Arguments);
        }

        [DataRow(DatasetType.Snapshot | DatasetType.Filesystem, "tank/myds", "list -Hpr -o type,creation,name,used,refer,avail,mountpoint -d 99 -t filesystem,snapshot tank/myds")]
        [DataRow(DatasetType.Snapshot | DatasetType.Filesystem, null, "list -Hpr -o type,creation,name,used,refer,avail,mountpoint -d 99 -t filesystem,snapshot")]
        [DataRow(DatasetType.Snapshot | DatasetType.Filesystem | DatasetType.Volume, "tank/myds", "list -Hpr -o type,creation,name,used,refer,avail,mountpoint -d 99 -t filesystem,snapshot,volume tank/myds")]
        [DataRow(DatasetType.Snapshot | DatasetType.Filesystem | DatasetType.Volume | DatasetType.Bookmark, "tank/myds", "list -Hpr -o type,creation,name,used,refer,avail,mountpoint -d 99 -t bookmark,filesystem,snapshot,volume tank/myds")]
        [TestMethod]
        public void ListWithChildrenTest(DatasetType types, string root, string expectedCommand)
        {
            var command = DatasetCommands.ZfsList(types, root, true);
            Assert.AreEqual(expectedCommand, command.Arguments);
        }

        [TestMethod]
        public void WhichCommandTest()
        {
            var command = BaseCommands.Which("smartctl");
            Assert.AreEqual("/bin/which smartctl", command.FullCommandLine);
        }

        [TestMethod]
        public void ListBlockDevicesTest()
        {
            var command = BaseCommands.ListBlockDevices();
            Assert.AreEqual("/bin/lsblk --include 8 --include 259 -p|grep disk", command.FullCommandLine);
        }

        [TestMethod]
        public void ListDisksTest()
        {
            var command = BaseCommands.ListDisks();
            Assert.AreEqual("/bin/ls -l /dev/disk/by-id/ | awk -F ' ' '{print $9,$11}'", command.FullCommandLine);
        }
    }
}
