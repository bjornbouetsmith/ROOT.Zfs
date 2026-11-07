using System;
using System.Collections.Generic;
using Xunit;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Helpers;

public class DiskHelperTest
{
    private readonly ITestOutputHelper _output;

    public DiskHelperTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void BlockDeviceTest()
    {
        string lines = @"/dev/sdc      8:32   0   16G  0 disk
/dev/hda      8:32   0   16G  0 disk
/dev/nvme0n1             259:0    0 232.9G  0 disk
";
        var disks = DiskHelper.BlockDevicesFromStdOutput(lines);
        Assert.Contains("/dev/sdc", disks);
        Assert.Contains("/dev/hda", disks);
        Assert.Contains("/dev/nvme0n1", disks);
    }

    [Theory]
    [InlineData("scsi-0QEMU_QEMU_HARDDISK_drive-scsi6 ../../sda", "/dev/sda", DeviceType.Disk)]
    [InlineData("scsi-0QEMU_QEMU_HARDDISK_drive-scsi6 ../../sda1", "/dev/sda", DeviceType.Partition)]
    [InlineData("scsi-0QEMU_QEMU_HARDDISK_drive-scsi6 ../../sda", null, DeviceType.Partition)] // If no disks are loaded, then Partition is default
    public void DiskInfoDiskTypeTest(string line, string disk, DeviceType expectedType)
    {
        var disks = new HashSet<string> { disk };
        var info = DiskHelper.FromString(line, disks);
        _output.WriteLine(info.Type.ToString());
        Assert.Equal(expectedType, info.Type);
    }

    [Theory]
    [InlineData("scsi-0QEMU_QEMU_HARDDISK_drive-scsi6 ../../sda", "/dev/disk/by-id/scsi-0QEMU_QEMU_HARDDISK_drive-scsi6")]
    [InlineData("scsi-0QEMU_QEMU_HARDDISK_drive-scsi6-part1 ../../sda1", "/dev/disk/by-id/scsi-0QEMU_QEMU_HARDDISK_drive-scsi6-part1")]
    [InlineData("wwn-0x524693f2ca217979 ../../sdj", "/dev/disk/by-id/wwn-0x524693f2ca217979")]
    public void DiskInfoIdTest(string line, string expectedId)
    {
        var disks = new HashSet<string>();
        var info = DiskHelper.FromString(line, disks);
        _output.WriteLine(info.Id);
        Assert.Equal(expectedId, info.Id);
    }
    [Theory]
    [InlineData("scsi-0QEMU_QEMU_HARDDISK_drive-scsi6 ../../sda", "/dev/sda")]
    [InlineData("scsi-0QEMU_QEMU_HARDDISK_drive-scsi6-part1 ../../sda1", "/dev/sda1")]
    [InlineData("wwn-0x524693f2ca217979 ../../sdj","/dev/sdj")]
    public void DiskInfoDeviceNameTest(string line, string expectedDeviceName)
    {
        var disks = new HashSet<string>();
        var info = DiskHelper.FromString(line, disks);
        _output.WriteLine(info.DeviceName);
        Assert.Equal(expectedDeviceName, info.DeviceName);
    }

    [Theory]
    [InlineData("nvme-eui.0025385c71b0595a ../../nvme0n1","/dev/nvme0n1", "/dev/disk/by-id/nvme-eui.0025385c71b0595a")]
    [InlineData("nvme-Samsung_SSD_960_EVO_250GB_S3ESNX0JC22874D ../../nvme0n1", "/dev/nvme0n1", "/dev/disk/by-id/nvme-Samsung_SSD_960_EVO_250GB_S3ESNX0JC22874D")]
    public void NvmeDriveDiskHelperTest(string line, string expectedDeviceName, string expectedId)
    {
        var disks = new HashSet<string>();
        var info = DiskHelper.FromString(line, disks);
        _output.WriteLine(info.DeviceName);
        Assert.Equal(expectedDeviceName, info.DeviceName);
        _output.WriteLine(info.Id);
        Assert.Equal(expectedId, info.Id);
    }

}