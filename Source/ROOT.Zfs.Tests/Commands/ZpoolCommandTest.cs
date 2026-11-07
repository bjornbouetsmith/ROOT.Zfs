using System;
using Xunit;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Public.Arguments.Pool;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Tests.Commands;

public class ZpoolCommandTest
{
    private readonly ZpoolCommands _commands = new(new ZfsBinaries());

    [Fact]
    public void GetHistoryCommandTest()
    {
        var args = new PoolHistoryArgs { PoolName = "tank" };
        var command = _commands.History(args);

        Assert.Equal("/sbin/zpool history -l tank", command.FullCommandLine);
    }

    [Fact]
    public void GetHistoryCommandTestNotValid()
    {
        var args = new PoolHistoryArgs();
        Assert.Throws<ArgumentException>(() => _commands.History(args));

    }

    [InlineData("", true)]
    [InlineData("tank", false)]
    [InlineData("tank && rm -rf /", true)]
    [Theory]
    public void GetStatusCommandTest(string pool, bool expectException)
    {
        var arg = new PoolStatusArgs { PoolName = pool };
        if (expectException)
        {
            var ex = Assert.Throws<ArgumentException>(() => _commands.GetStatus(arg));
            Console.WriteLine(ex);
        }
        else
        {

            var command = _commands.GetStatus(arg);

            Assert.Equal("/sbin/zpool status -vP tank", command.FullCommandLine);
        }
    }

    [Fact]
    public void GetAllPoolInfosCommand()
    {
        var command = _commands.GetAllPoolInfos();

        Assert.Equal("/sbin/zpool list -PHp", command.FullCommandLine);
    }

    [InlineData("tank", false)]
    [InlineData("tank/myds", true)]
    [Theory]
    public void GetPoolInfoCommand(string name, bool expectException)
    {
        if (expectException)
        {
            Assert.Throws<ArgumentException>(() => _commands.GetPoolInfo(new PoolListArgs { PoolName = name }));
        }
        else
        {
            var command = _commands.GetPoolInfo(new PoolListArgs { PoolName = name });

            Assert.Equal("/sbin/zpool list -PHp tank", command.FullCommandLine);
        }
    }

    [Fact]
    public void CreateMirrorPoolTest()
    {
        var args = new PoolCreateArgs
        {
            PoolName = "tank3",
            VDevs =
            [
                new VDevCreationArgs { Type = VDevCreationType.Mirror, Devices = ["/dev/sdc", "/dev/sdd"] },
                new VDevCreationArgs { Type = VDevCreationType.Cache, Devices = ["/dev/sde"] },
                new VDevCreationArgs { Type = VDevCreationType.Log, Devices = ["/dev/sdf"] }
            ],
            MountPoint = "/mnt/tank3",
            PoolProperties = [new PropertyValue { Property = "ashift", Value = "12" }],
            FileSystemProperties = [new PropertyValue { Property = "atime", Value = "off" }]
        };
        var command = _commands.Create(args);
        Assert.Equal("/sbin/zpool create tank3 -m /mnt/tank3 -o ashift=12 -O atime=off mirror /dev/sdc /dev/sdd cache /dev/sde log /dev/sdf", command.FullCommandLine);
        Console.WriteLine(command.FullCommandLine);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CreateWithEmptyNameShouldThrowArgumentException(bool nameNull)
    {
        var args = new PoolCreateArgs
        {
            PoolName = nameNull ? null : "",
            VDevs =
            [
                new VDevCreationArgs { Type = VDevCreationType.Mirror, Devices = ["/dev/sdc", "/dev/sdd"] }
            ]
        };

        var ex = Assert.Throws<ArgumentException>(() => _commands.Create(args));
        Assert.Equal("PoolName cannot be empty (Parameter 'PoolCreateArgs args')", ex.Message);
    }

    [Fact]
    public void CreateMirrorNeedsAtLeastTwoDevices()
    {
        var args = new PoolCreateArgs
        {
            PoolName = "tank2",
            VDevs =
            [
                new VDevCreationArgs { Type = VDevCreationType.Mirror, Devices = ["/dev/sdc"] }
            ]
        };

        var ex = Assert.Throws<ArgumentException>(() => _commands.Create(args));
        Assert.Equal("Please provide at least two devices when creating a mirror (Parameter 'PoolCreateArgs args')", ex.Message);
    }

    [Theory]
    [InlineData("tank", "mirror-0", "/sbin/zpool clear tank mirror-0", false)]
    [InlineData("tank", "sda", "/sbin/zpool clear tank sda", false)]
    [InlineData("tank", "", "/sbin/zpool clear tank", false)]
    [InlineData("tank", null, "/sbin/zpool clear tank", false)]
    [InlineData("tank", "rm -rf /tank", null, true)]
    [InlineData("tank", "rm /tank", null, true)]
    [InlineData("tank", "rm%20-rf%20/tank", null, true)]
    public void ClearPoolTest(string pool, string device, string expectedCommand, bool expectException)
    {
        var args = new PoolClearArgs { PoolName = pool, Device = device };

        if (expectException)
        {
            var ex = Assert.Throws<ArgumentException>(() => _commands.Clear(args));
            Console.WriteLine(ex);
        }
        else
        {
            var command = _commands.Clear(args);
            Console.WriteLine(command.FullCommandLine);
            Assert.Equal(expectedCommand, command.FullCommandLine);
        }
    }

    [Theory]
    [InlineData("tank", "", null, true)]
    [InlineData("tank", "sda", "/sbin/zpool online tank sda", false)]
    public void OnlinePoolDeviceTest(string pool, string device, string expectedCommand, bool expectException)
    {
        var args = new PoolOnlineArgs { PoolName = pool, Device = device };
        if (expectException)
        {
            Assert.Throws<ArgumentException>(() => _commands.Online(args));
        }
        else
        {
            var command = _commands.Online(args);
            Assert.Equal(expectedCommand, command.FullCommandLine);
        }
    }

    [InlineData("tank", null, null, true)]
    [InlineData(null, "sda", null, true)]
    [InlineData("tank", "sda", "/sbin/zpool offline tank sda", false)]
    [Theory]
    public void OfflinePoolDeviceTest(string pool, string device, string expectedCommand, bool expectException)
    {
        var args = new PoolOfflineArgs
        {
            PoolName = pool,
            Device = device,
        };
        if (expectException)
        {
            Assert.Throws<ArgumentException>(() => _commands.Offline(args));
        }
        else
        {

            var command = _commands.Offline(args);

            Assert.Equal(expectedCommand, command.FullCommandLine);
        }
    }

    [Theory]
    [InlineData("tank", null, "/sbin/zpool iostat -LlpPvH tank", false)] // only pool
    [InlineData("tank", "", "/sbin/zpool iostat -LlpPvH tank", false)] //only pool
    [InlineData("tank", "/dev/sda", "/sbin/zpool iostat -LlpPvH tank /dev/sda", false)] //single device
    [InlineData("tank", "/dev/sda,/dev/sdb", "/sbin/zpool iostat -LlpPvH tank /dev/sda /dev/sdb", false)] //multiple devices
    [InlineData("tank", "rm -rf /tank", null, true)] //multiple devices
    public void IoStatTest(string pool, string deviceList, string expectedCommand, bool expectException)
    {
        var args = new PoolIOStatsArgs { PoolName = pool, Devices = deviceList?.Split(',') };
        if (expectException)
        {
            var ex = Assert.Throws<ArgumentException>(() => _commands.IoStats(args));
            Console.WriteLine(ex);
        }
        else
        {

            var command = _commands.IoStats(args);
            Assert.Equal(expectedCommand, command.FullCommandLine);
        }
    }

    [InlineData("tank", "/sbin/zpool resilver tank", false)]
    [InlineData("", null, true)]
    [Theory]
    public void ResilverCommandTest(string pool, string expected, bool expectException)
    {
        var args = new PoolResilverArgs { PoolName = pool };
        if (expectException)
        {
            Assert.Throws<ArgumentException>(() => _commands.Resilver(args));
        }
        else
        {
            var command = _commands.Resilver(args);
            Console.WriteLine(command.FullCommandLine);
            Assert.Equal(expected, command.FullCommandLine);
        }
    }

    [InlineData("tank", ScrubOption.None, "/sbin/zpool scrub tank")]
    [InlineData("tank", ScrubOption.Stop, "/sbin/zpool scrub -s tank")]
    [InlineData("tank", ScrubOption.Pause, "/sbin/zpool scrub -p tank")]
    [InlineData(null, ScrubOption.None, null, true)]
    [InlineData("", ScrubOption.None, null, true)]
    [InlineData("  ", ScrubOption.None, null, true)]
    [Theory]
    public void ScrubTest(string pool, ScrubOption option, string expected, bool expectException = false)
    {
        var args = new PoolScrubArgs { PoolName = pool, Options = option };
        if (expectException)
        {
            Assert.Throws<ArgumentException>(() => _commands.Scrub(args));
        }
        else
        {
            var command = _commands.Scrub(args);
            Assert.Equal(expected, command.FullCommandLine);
        }
    }

    [InlineData("tank", null, "/sbin/zpool trim tank")]
    [InlineData("tank", "/dev/sda", "/sbin/zpool trim tank /dev/sda")]
    [InlineData("", "/dev/sda", null, true)]
    [InlineData(null, "/dev/sda", null, true)]
    [InlineData(" ", "/dev/sda", null, true)]
    [Theory]
    public void TrimTest(string pool, string device, string expected, bool expectException = false)
    {
        var args = new PoolTrimArgs
        {
            PoolName = pool,
            DeviceName = device,
        };

        if (expectException)
        {
            Assert.Throws<ArgumentException>(() => _commands.Trim(args));
        }
        else
        {
            var command = _commands.Trim(args);
            Console.WriteLine(command.FullCommandLine);
            Assert.Equal(expected, command.FullCommandLine);
        }
    }

    [Fact]
    public void GetUpgradeablePoolsTest()
    {
        var command = _commands.GetUpgradeablePools();
        Assert.Equal("/sbin/zpool upgrade", command.FullCommandLine);
    }

    [InlineData("tank", false, "/sbin/zpool upgrade tank")]
    [InlineData("tank", true, "/sbin/zpool upgrade -a")]
    [InlineData("", true, "/sbin/zpool upgrade -a")]
    [InlineData(null, true, "/sbin/zpool upgrade -a")]
    [InlineData(" ", true, "/sbin/zpool upgrade -a")]
    [InlineData(null, false, null, true)]
    [InlineData("", false, null, true)]
    [InlineData(" ", false, null, true)]
    [Theory]
    public void UpgradePoolTest(string poolName, bool all, string expected, bool expectException = false)
    {
        var args = new PoolUpgradeArgs { PoolName = poolName, AllPools = all };
        if (expectException)
        {
            Assert.Throws<ArgumentException>(() => _commands.Upgrade(args));
        }
        else
        {
            var command = _commands.Upgrade(args);
            Console.WriteLine(command.FullCommandLine);
            Assert.Equal(expected, command.FullCommandLine);
        }
    }

    [InlineData("tank", "/dev/sdc", "/sbin/zpool detach tank /dev/sdc", false)]
    [InlineData("tank", "", null, true)]
    [InlineData("", "/dev/sdc", null, true)]
    [Theory]
    public void DetachTest(string pool, string device, string expected, bool expectException)
    {
        var args = new PoolDetachArgs { PoolName = pool, Device = device };

        if (expectException)
        {
            Assert.Throws<ArgumentException>(() => _commands.Detach(args));
        }
        else
        {
            var command = _commands.Detach(args);
            Assert.Equal(expected, command.FullCommandLine);
        }
    }

    [InlineData(true)]
    [InlineData(false)]
    [Theory]
    public void AttachTest(bool valid)
    {
        var args = new PoolAttachArgs
        {
            PoolName = valid ? "tank" : null,
            OldDevice = "/dev/sdb",
            NewDevice = "/dev/sdc",
            Force = true,
            RestoreSequentially = true,
            PropertyValues = [new PropertyValue { Property = "ashift", Value = "12" }]
        };
        var stringVer = args.ToString();
        if (!valid)
        {
            Assert.Throws<ArgumentException>(() => _commands.Attach(args));
        }
        else
        {

            var command = _commands.Attach(args);
            Console.WriteLine(command.FullCommandLine);

            Assert.Equal(stringVer, command.Arguments);
            Assert.Equal($"/sbin/zpool {stringVer}", command.FullCommandLine);
        }
    }

    [InlineData(true)]
    [InlineData(false)]
    [Theory]
    public void ReplaceTest(bool valid)
    {
        var args = new PoolReplaceArgs
        {
            PoolName = valid ? "tank" : null,
            OldDevice = "/dev/sdb",
            NewDevice = "/dev/sdc",
            Force = true,
            RestoreSequentially = true,
            PropertyValues = [new PropertyValue { Property = "ashift", Value = "12" }]
        };
        var stringVer = args.ToString();

        if (!valid)
        {
            Assert.Throws<ArgumentException>(() => _commands.Replace(args));
        }
        else
        {
            var command = _commands.Replace(args);
            Console.WriteLine(command.FullCommandLine);

            Assert.Equal(stringVer, command.Arguments);
            Assert.Equal($"/sbin/zpool {stringVer}", command.FullCommandLine);
        }
    }

    [InlineData(true)]
    [InlineData(false)]
    [Theory]
    public void AddTest(bool valid)
    {
        var args = new PoolAddArgs
        {
            PoolName = valid ? "tank" : null,
            VDevs = [new VDevCreationArgs { Type = VDevCreationType.Mirror, Devices = ["disk1", "disk2"] }],
        };
        if (!valid)
        {
            Assert.Throws<ArgumentException>(() => _commands.Add(args));
        }
        else
        {
            var command = _commands.Add(args);
            Console.WriteLine(command.FullCommandLine);
            Assert.Equal("/sbin/zpool add tank mirror disk1 disk2", command.FullCommandLine);
        }
    }

    [InlineData(true)]
    [InlineData(false)]
    [Theory]
    public void RemoveTest(bool valid)
    {
        var args = new PoolRemoveArgs
        {
            PoolName = valid ? "tank" : null,
            VDev = "mirror-0",
        };
        if (!valid)
        {
            Assert.Throws<ArgumentException>(() => _commands.Remove(args));
        }
        else
        {
            var command = _commands.Remove(args);
            Console.WriteLine(command.FullCommandLine);
            Assert.Equal("/sbin/zpool remove tank mirror-0", command.FullCommandLine);
        }
    }
    [InlineData("tank", "/sbin/zpool destroy -f tank", false)]
    [InlineData("tank%5ftest", "/sbin/zpool destroy -f tank_test", false)]
    [InlineData("tank/myds", null, true)]
    [InlineData("tank%2fmyds", null, true)]
    [InlineData("", null, true)]
    [Theory]
    public void DestroyTest(string name, string expected, bool expectException)
    {
        var args = new PoolDestroyArgs { PoolName = name };

        if (expectException)
        {
            var ex = Assert.Throws<ArgumentException>(() => _commands.Destroy(args));
            Console.WriteLine(ex);
        }
        else
        {
            var command = _commands.Destroy(args);
            Assert.Equal(expected, command.FullCommandLine);
        }
    }
}