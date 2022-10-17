using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Core.Commands;

namespace ROOT.Zfs.Tests.Commands
{
    [TestClass]
    public class BaseCommandsTest
    {
        [DataRow(ListTypes.None, "", "list -Hpr -o type,creation,name,used,refer,avail,mountpoint")]
        [DataRow(ListTypes.None, null, "list -Hpr -o type,creation,name,used,refer,avail,mountpoint")]
        [DataRow(ListTypes.None, "tank/myds", "list -Hpr -o type,creation,name,used,refer,avail,mountpoint tank/myds")]
        [DataRow(ListTypes.None, "tank%2Fmyds", "list -Hpr -o type,creation,name,used,refer,avail,mountpoint tank/myds")] //test url encoded

        [DataRow(ListTypes.Snapshot, "tank/myds", "list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t snapshot tank/myds")]
        [DataRow(ListTypes.Snapshot, null, "list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t snapshot")]

        [DataRow(ListTypes.Bookmark, "tank/myds", "list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t bookmark tank/myds")]
        [DataRow(ListTypes.Bookmark, null, "list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t bookmark")]

        [DataRow(ListTypes.Volume, "tank/myds", "list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t volume tank/myds")]
        [DataRow(ListTypes.Volume, null, "list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t volume")]

        [DataRow(ListTypes.FileSystem, "tank/myds", "list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t filesystem tank/myds")]
        [DataRow(ListTypes.FileSystem, null, "list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t filesystem")]
        [TestMethod]
        public void ListCommandTest(ListTypes types, string root, string expectedCommand)
        {
            var command = DatasetCommands.ZfsList(types, root);
            Assert.AreEqual(expectedCommand, command.Arguments);
        }

        [DataRow(ListTypes.Snapshot | ListTypes.FileSystem, "tank/myds", "list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t filesystem,snapshot tank/myds")]
        [DataRow(ListTypes.Snapshot | ListTypes.FileSystem, null, "list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t filesystem,snapshot")]
        [DataRow(ListTypes.Snapshot | ListTypes.FileSystem | ListTypes.Volume, "tank/myds", "list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t filesystem,snapshot,volume tank/myds")]
        [DataRow(ListTypes.Snapshot | ListTypes.FileSystem | ListTypes.Volume | ListTypes.Bookmark, "tank/myds", "list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t bookmark,filesystem,snapshot,volume tank/myds")]
        [TestMethod]
        public void ListCommandCombinationsTest(ListTypes types, string root, string expectedCommand)
        {
            var command = DatasetCommands.ZfsList(types, root);
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
