﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Arguments;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.Datasets;

namespace ROOT.Zfs.Tests.Commands
{
    [TestClass]
    public class DataSetCommandsTest
    {

        [DataRow(DatasetDestroyFlags.None, "/sbin/zfs destroy tank/myds")]
        [DataRow(DatasetDestroyFlags.Recursive, "/sbin/zfs destroy -r tank/myds")]
        [DataRow(DatasetDestroyFlags.RecursiveClones, "/sbin/zfs destroy -R tank/myds")]
        [DataRow(DatasetDestroyFlags.ForceUmount, "/sbin/zfs destroy -f tank/myds")]
        [DataRow(DatasetDestroyFlags.DryRun, "/sbin/zfs destroy -nvp tank/myds")]
        [DataRow((DatasetDestroyFlags)(int)-1, "/sbin/zfs destroy -r -R -f -nvp tank/myds")]
        [TestMethod]
        public void DestroyFlagsShouldBeCorrectlyReflected(DatasetDestroyFlags flags, string expectedCommand)
        {
            var command = DatasetCommands.DestroyDataset("tank/myds", flags);
            Console.WriteLine(command.FullCommandLine);
            Assert.AreEqual(expectedCommand, command.FullCommandLine);
        }

        [TestMethod]
        public void PromoteCommandTest()
        {
            var command = DatasetCommands.Promote("tank/myds/clone");
            Console.WriteLine(command.FullCommandLine);
            Assert.AreEqual("/sbin/zfs promote tank/myds/clone", command.FullCommandLine);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void CreateDatasetWithoutProperties(bool nullProperties)
        {
            var command = DatasetCommands.CreateDataset("tank/myds", nullProperties ? null : Array.Empty<PropertyValue>());
            Console.WriteLine(command.FullCommandLine);
            Assert.AreEqual("/sbin/zfs create tank/myds", command.FullCommandLine);
        }

        [TestMethod]
        public void CreateDataSetWithPropertiesTest()
        {
            var command = DatasetCommands.CreateDataset("tank/myds", new[] { new PropertyValue { Property = "atime", Value = "off" }, new PropertyValue { Property = "compression", Value = "off" } });
            Assert.AreEqual("/sbin/zfs create -o atime=off -o compression=off tank/myds", command.FullCommandLine);
        }

        [DataRow(DatasetTypes.Filesystem, true, true, null, false, null, null, "/sbin/zfs create -p -u tank/child")]
        [DataRow(DatasetTypes.Filesystem, true, false, null, false, null, null, "/sbin/zfs create -p tank/child")]
        [DataRow(DatasetTypes.Filesystem, false, false, null, false, null, null, "/sbin/zfs create tank/child")]

        [DataRow(DatasetTypes.Volume, true, true, null, true, "8K", "18G", "/sbin/zfs create -b 8K -V 18G -s -p tank/child")]
        [DataRow(DatasetTypes.Volume, false, false, null, true, "8K", "18G", "/sbin/zfs create -b 8K -V 18G -s tank/child")]
        [DataRow(DatasetTypes.Volume, true, true, null, false, "8K", "18G", "/sbin/zfs create -b 8K -V 18G -p tank/child")]
        [DataRow(DatasetTypes.Volume, false, false, null, false, "8K", "18G", "/sbin/zfs create -b 8K -V 18G tank/child")]

        [TestMethod]
        public void CreateDatasetWithArgumentsTest(DatasetTypes type, bool createParents, bool doNotMount, string properties, bool sparseVolume, string blockSize, string volumeSize, string expectedCommand)
        {
            var props = properties?.Split(',').Select(p => p.Split('=')).Select(a => new PropertyValue { Property = a[0], Source = a[1] }).ToArray();
            var args = new DatasetCreationArgs
            {
                Type = type,
                CreateParents = createParents,
                DataSetName = "tank/child",
                DoNotMount = doNotMount,
                Properties = props,
                VolumeArguments = type == DatasetTypes.Filesystem ? null : new VolumeCreationArgs
                {
                    BlockSize = blockSize,
                    VolumeSize = volumeSize,
                    Sparse = sparseVolume
                }
            };
            var command = DatasetCommands.CreateDataset(args);
            Assert.AreEqual(expectedCommand, command.FullCommandLine);
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
    }
}
