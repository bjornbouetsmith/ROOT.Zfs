using System;
using Xunit;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Public.Arguments.Dataset;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Commands;

public class CommandsTest
{
    private readonly ITestOutputHelper _output;

    private readonly SystemCommands _systemCommands;
    private readonly VirtualizationCommands _virtualizationCommands;
    private readonly DatasetCommands _datasetCommands;

    public CommandsTest(ITestOutputHelper output)
    {
        _output = output;
        var binaries = new ZfsBinaries();
        _systemCommands = new SystemCommands(binaries);
        _virtualizationCommands = new VirtualizationCommands(binaries);
        _datasetCommands = new DatasetCommands(binaries);
    }

    [InlineData(DatasetTypes.None, "", "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t filesystem,volume")]
    [InlineData(DatasetTypes.None, null, "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t filesystem,volume")]
    [InlineData(DatasetTypes.None, "tank/myds", "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t filesystem,volume tank/myds")]
    [InlineData(DatasetTypes.None, "tank%2Fmyds", "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t filesystem,volume tank/myds")] //test url encoded

    [InlineData(DatasetTypes.Snapshot, "tank/myds", "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t snapshot tank/myds")]
    [InlineData(DatasetTypes.Snapshot, null, "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t snapshot")]

    [InlineData(DatasetTypes.Bookmark, "tank/myds", "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t bookmark tank/myds")]
    [InlineData(DatasetTypes.Bookmark, null, "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t bookmark")]

    [InlineData(DatasetTypes.Volume, "tank/myds", "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t volume tank/myds")]
    [InlineData(DatasetTypes.Volume, null, "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t volume")]

    [InlineData(DatasetTypes.Filesystem, "tank/myds", "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t filesystem tank/myds")]
    [InlineData(DatasetTypes.Filesystem, null, "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t filesystem")]
    [Theory]
    public void ListCommandTest(DatasetTypes types, string root, string expectedCommand)
    {
        var args = new DatasetListArgs { DatasetTypes = types, Root = root };
        var command = _datasetCommands.List(args);
        Assert.Equal(expectedCommand, command.Arguments);
    }

    [InlineData(DatasetTypes.Snapshot | DatasetTypes.Filesystem, "tank/myds", "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t filesystem,snapshot tank/myds")]
    [InlineData(DatasetTypes.Snapshot | DatasetTypes.Filesystem, null, "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t filesystem,snapshot")]
    [InlineData(DatasetTypes.Snapshot | DatasetTypes.Filesystem | DatasetTypes.Volume, "tank/myds", "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t filesystem,snapshot,volume tank/myds")]
    [InlineData(DatasetTypes.Snapshot | DatasetTypes.Filesystem | DatasetTypes.Volume | DatasetTypes.Bookmark, "tank/myds", "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t bookmark,filesystem,snapshot,volume tank/myds")]
    [Theory]
    public void ListCommandCombinationsTest(DatasetTypes types, string root, string expectedCommand)
    {
        var args = new DatasetListArgs { DatasetTypes = types, Root = root };
        var command = _datasetCommands.List(args);
        Assert.Equal(expectedCommand, command.Arguments);
    }

    [InlineData(DatasetTypes.Snapshot | DatasetTypes.Filesystem, "tank/myds", "list -Hpr -o type,creation,name,used,refer,avail,mountpoint,origin -d 99 -t filesystem,snapshot tank/myds")]
    [InlineData(DatasetTypes.Snapshot | DatasetTypes.Filesystem, null, "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t filesystem,snapshot")]
    [InlineData(DatasetTypes.Snapshot | DatasetTypes.Filesystem | DatasetTypes.Volume, "tank/myds", "list -Hpr -o type,creation,name,used,refer,avail,mountpoint,origin -d 99 -t filesystem,snapshot,volume tank/myds")]
    [InlineData(DatasetTypes.Snapshot | DatasetTypes.Filesystem | DatasetTypes.Volume | DatasetTypes.Bookmark, "tank/myds", "list -Hpr -o type,creation,name,used,refer,avail,mountpoint,origin -d 99 -t bookmark,filesystem,snapshot,volume tank/myds")]
    [Theory]
    public void ListWithChildrenTest(DatasetTypes types, string root, string expectedCommand)
    {
        var args = new DatasetListArgs { DatasetTypes = types, Root = root, IncludeChildren = true };
        var command = _datasetCommands.List(args);
        Assert.Equal(expectedCommand, command.Arguments);
    }

    [Fact]
    public void WhichCommandTest()
    {
        var command = _systemCommands.Which("smartctl");
        Assert.Equal("/bin/which smartctl", command.FullCommandLine);
    }

    [Fact]
    public void ListBlockDevicesTest()
    {
        var command = _systemCommands.ListBlockDevices();
        Assert.Equal("/bin/lsblk --include 8 --include 259 -p | /bin/grep disk", command.FullCommandLine);
    }

    [Fact]
    public void ListDisksTest()
    {
        var command = _systemCommands.ListDisks();
        Assert.Equal("/bin/ls -l /dev/disk/by-id/ | /bin/awk -F ' ' '{print $9,$11}'", command.FullCommandLine);
    }

    [Fact]
    public void DetectVirtualizationTest()
    {
        var command = _virtualizationCommands.DetectVirtualization();
        Assert.Equal("/usr/bin/systemd-detect-virt --vm", command.FullCommandLine);
    }

    [Fact]
    public void DetectVirtualizationWithVirtWhatTest()
    {
        var command = _virtualizationCommands.DetectVirtualizationWithVirtWhat();
        Assert.Equal("/usr/sbin/virt-what", command.FullCommandLine);
    }

    [Fact]
    public void GetHostnamectlStatusTest()
    {
        var command = _virtualizationCommands.GetHostnamectlStatus();
        Assert.Equal("/usr/bin/hostnamectl status", command.FullCommandLine);
    }

    [Fact]
    public void GetDmiInfoTest()
    {
        var command = _virtualizationCommands.GetDmiInfo();
        Assert.Equal("/bin/cat /sys/class/dmi/id/sys_vendor /sys/class/dmi/id/product_name", command.FullCommandLine);
    }

    [Fact]
    public void GetCpuInfoTest()
    {
        var command = _virtualizationCommands.GetCpuInfo();
        Assert.Equal("/bin/cat /proc/cpuinfo", command.FullCommandLine);
    }

    [InlineData("tank", false)]
    [InlineData("tank && rm -rf /", true)]
    [Theory]
    public void DataSetListTest(string root, bool expectException)
    {
        var args = new DatasetListArgs { Root = root };
        if (expectException)
        {
            var ex = Assert.Throws<ArgumentException>(() => _datasetCommands.List(args));
            _output.WriteLine(ex.ToString());
        }
        else
        {
            var command = _datasetCommands.List(args);
            Assert.NotNull(command.FullCommandLine);
        }
    }
}