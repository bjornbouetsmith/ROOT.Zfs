using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Helpers
{
    [TestClass]
    public class DiskHelperTest
    {
        [TestMethod]
        public void BlockDeviceTest()
        {
            string lines = @"/dev/sdc      8:32   0   16G  0 disk
/dev/hda      8:32   0   16G  0 disk";
            var disks = DiskHelper.BlockDevicesFromStdOutput(lines);
            Assert.IsTrue(disks.Contains("/dev/sdc"));
            Assert.IsTrue(disks.Contains("/dev/hda"));
        }

        [TestMethod]
        [DataRow("scsi-0QEMU_QEMU_HARDDISK_drive-scsi6 ../../sda", "/dev/sda", DiskType.Disk)]
        [DataRow("scsi-0QEMU_QEMU_HARDDISK_drive-scsi6 ../../sda1", "/dev/sda", DiskType.Partition)]
        [DataRow("scsi-0QEMU_QEMU_HARDDISK_drive-scsi6 ../../sda", null, DiskType.Partition)] // If no disks are loaded, then Partition is default
        public void DiskInfoDiskTypeTest(string line, string disk, DiskType expectedType)
        {
            var disks = new HashSet<string> { disk };
            var info = DiskHelper.FromString(line, disks);
            Console.WriteLine(info.Type);
            Assert.AreEqual(expectedType, info.Type);
        }

        [TestMethod]
        [DataRow("scsi-0QEMU_QEMU_HARDDISK_drive-scsi6 ../../sda", "/dev/disk/by-id/scsi-0QEMU_QEMU_HARDDISK_drive-scsi6")]
        [DataRow("scsi-0QEMU_QEMU_HARDDISK_drive-scsi6-part1 ../../sda1", "/dev/disk/by-id/scsi-0QEMU_QEMU_HARDDISK_drive-scsi6-part1")]
        public void DiskInfoIdTest(string line, string expectedId)
        {
            var disks = new HashSet<string>();
            var info = DiskHelper.FromString(line, disks);
            Console.WriteLine(info.Id);
            Assert.AreEqual(expectedId, info.Id);
        }
        [TestMethod]
        [DataRow("scsi-0QEMU_QEMU_HARDDISK_drive-scsi6 ../../sda", "/dev/sda")]
        [DataRow("scsi-0QEMU_QEMU_HARDDISK_drive-scsi6-part1 ../../sda1", "/dev/sda1")]
        public void DiskInfoDeviceNameTest(string line, string expectedDeviceName)
        {
            var disks = new HashSet<string>();
            var info = DiskHelper.FromString(line, disks);
            Console.WriteLine(info.DeviceName);
            Assert.AreEqual(expectedDeviceName, info.DeviceName);
        }

    }
}
