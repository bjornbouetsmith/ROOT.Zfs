using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Public.Arguments.Dataset;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Commands
{
    [TestClass]
    public class DataSetCommandsTest
    {
        [DataRow("tank/myds", DatasetDestroyFlags.None, "/sbin/zfs destroy tank/myds", false)]
        [DataRow("tank/myds", DatasetDestroyFlags.Recursive, "/sbin/zfs destroy -r tank/myds", false)]
        [DataRow("tank/myds", DatasetDestroyFlags.RecursiveClones, "/sbin/zfs destroy -R tank/myds", false)]
        [DataRow("tank/myds", DatasetDestroyFlags.ForceUmount, "/sbin/zfs destroy -f tank/myds", false)]
        [DataRow("tank/myds", DatasetDestroyFlags.DryRun, "/sbin/zfs destroy -nvp tank/myds", false)]
        [DataRow("tank/myds", (DatasetDestroyFlags)(int)-1, "/sbin/zfs destroy -r -R -f -nvp tank/myds", false)]
        [DataRow("tank/myds@test", (DatasetDestroyFlags)(int)-1, "/sbin/zfs destroy -r -R -f -nvp tank/myds", true)]
        [TestMethod]
        public void DestroyFlagsShouldBeCorrectlyReflected(string dataset, DatasetDestroyFlags flags, string expectedCommand, bool throwException)
        {
            var args = new DatasetDestroyArgs { Dataset = dataset, DestroyFlags = flags };
            if (throwException)
            {
                var ex = Assert.ThrowsException<ArgumentException>(() => DatasetCommands.Destroy(args));
                Console.WriteLine(ex.Message);
            }
            else
            {
                var command = DatasetCommands.Destroy(args);
                Console.WriteLine(command.FullCommandLine);
                Assert.AreEqual(expectedCommand, command.FullCommandLine);
            }
        }

        [DataRow("tank/myds/clone", "/sbin/zfs promote tank/myds/clone", false)]
        [DataRow("tank/myds/clone && rm -rf /", "/sbin/zfs promote tank/myds/clone", true)]
        [TestMethod]
        public void PromoteCommandTest(string dataset, string expectedCommand, bool expectException)
        {
            var args = new PromoteArgs { Name = dataset };

            if (expectException)
            {
                var ex = Assert.ThrowsException<ArgumentException>(() => DatasetCommands.Promote(args));
                Console.WriteLine(ex);
            }
            else
            {

                var command = DatasetCommands.Promote(args);
                Console.WriteLine(command.FullCommandLine);
                Assert.AreEqual(expectedCommand, command.FullCommandLine);
            }
        }

        [TestMethod]
        [DataRow("tank/myds", false, false, "/sbin/zfs mount tank/myds")]
        [DataRow("tank/myds", true, true, "")]
        [DataRow("", true, false, "/sbin/zfs mount -a")]
        [DataRow(null, true, false, "/sbin/zfs mount -a")]
        public void DataSetMountTest(string dataset, bool all, bool throwException, string expectedCommand)
        {
            var args = new MountArgs { Filesystem = dataset, MountAllFileSystems = all };
            if (throwException)
            {
                Assert.ThrowsException<ArgumentException>(() => DatasetCommands.Mount(args));
            }
            else
            {
                var command = DatasetCommands.Mount(args);
                Assert.AreEqual(expectedCommand, command.FullCommandLine);
            }
        }

        [TestMethod]
        [DataRow("tank/myds", false, false, "/sbin/zfs unmount tank/myds")]
        [DataRow("tank/myds", true, true, "")]
        [DataRow("", true, false, "/sbin/zfs unmount -a")]
        [DataRow(null, true, false, "/sbin/zfs unmount -a")]
        public void DataSetUnmountTest(string dataset, bool all, bool throwException, string expectedCommand)
        {
            var args = new UnmountArgs { Filesystem = dataset, UnmountAllFileSystems = all };
            if (throwException)
            {
                Assert.ThrowsException<ArgumentException>(() => DatasetCommands.Unmount(args));
            }
            else
            {
                var command = DatasetCommands.Unmount(args);
                Assert.AreEqual(expectedCommand, command.FullCommandLine);
            }
        }

        [DataRow("tank/myds", DatasetTypes.Filesystem, "/sbin/zfs create tank/myds", false)]
        [DataRow("tank/myds", DatasetTypes.Bookmark, "/sbin/zfs create tank/myds", true)]
        [DataRow(null, DatasetTypes.Filesystem, "/sbin/zfs create tank/myds", true)]
        [TestMethod]
        public void CreateTest(string dataset, DatasetTypes type, string expectedCommand, bool expectException)
        {
            var args = new DatasetCreationArgs { DatasetName = dataset, Type = type };

            if (expectException)
            {
                var ex = Assert.ThrowsException<ArgumentException>(() => DatasetCommands.Create(args));
                Console.WriteLine(ex);
            }
            else
            {
                var command = DatasetCommands.Create(args);
                Assert.AreEqual(expectedCommand, command.FullCommandLine);
            }
        }

    }
}
