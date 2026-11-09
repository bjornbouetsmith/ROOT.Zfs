using System;
using System.Linq;
using Xunit;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;
using ROOT.Zfs.Public.Arguments.Pool;
using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Tests.Integration.Fake;

public class FakeZpoolTest
{
    private readonly ITestOutputHelper _output;

    public FakeZpoolTest(ITestOutputHelper output)
    {
        _output = output;
    }

    readonly FakeRemoteConnection _remoteProcessCall = new("2.1.5-2");

    private ZPool GetZpool()
    {
        return ZfsTestFactory.CreateZpool(_remoteProcessCall);
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void GetHistoryTestWithSkip()
    {
        var zp = GetZpool();
        var args = new PoolHistoryArgs { PoolName = "tank" };
        var lines = zp.History(args).ToList().Count;

        args = new PoolHistoryArgs { PoolName = "tank", SkipLines = lines - 2 };
        var history = zp.History(args).ToList();
        Assert.NotNull(history);
        _output.WriteLine(history.Dump(new JsonFormatter()));
        Assert.Equal(2, history.Count);
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void GetHistory()
    {
        var zp = GetZpool();
        var args = new PoolHistoryArgs { PoolName = "tank" };
        var history = zp.History(args);
        Assert.NotNull(history);
        _output.WriteLine(history.Dump(new JsonFormatter()));
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void GetHistoryAfterDate()
    {
        var zp = GetZpool();
        var args = new PoolHistoryArgs { PoolName = "tank" };
        var last10AtMost = zp.History(args).TakeLast(10).ToList();

        // This is safe, since zfs always have at last pool creation as a history event
        var history = last10AtMost.First();
        args = new PoolHistoryArgs { PoolName = "tank", AfterDate = history.Time };
        var afterDate = zp.History(args).ToList();

        if (last10AtMost.Count > 1)
        {
            Assert.True(afterDate.Count < last10AtMost.Count);
        }
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void ZpoolStatusTest()
    {
        var zfs = ZfsTestFactory.CreateZfs(_remoteProcessCall);
        var arg = new PoolStatusArgs { PoolName = "tank" };
        var status = zfs.Pool.Status(arg);

        Assert.NotNull(status);

        if (status.State == State.Online)
        {
            return;
        }

        var pool = status.Pool;
        foreach (var vdev in pool.VDevs)
        {
            if (vdev.State == State.Online)
            {
                continue;
            }

            _output.WriteLine("vdev:{0}, State:{1}", vdev.VDevName, vdev.State);
            foreach (var device in vdev.Devices)
            {
                if (device.State == State.Online)
                {
                    continue;
                }

                _output.WriteLine("Device:{0}, State:{1}", device.DeviceName, device.State);
            }
        }
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void DestroyPoolTest()
    {
        var zp = GetZpool();
        var pool = zp.Create(new PoolCreateArgs
        {
            PoolName = "mytest",
            VDevs =
            [
                new()
                {
                    Type = VDevCreationType.Mirror,
                    Devices = ["/dev/sda", "/dev/sdb"]
                }
            ]
        });
        Assert.NotNull(pool);
        zp.Destroy(new PoolDestroyArgs { PoolName = "mytest" });
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void GetPoolInfosTest()
    {
        var zp = GetZpool();

        var infos = zp.List();
        Assert.Equal(2, infos.Count);

    }


    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void GetPoolInfoTest()
    {
        var zp = GetZpool();

        var info = zp.List(new PoolListArgs { PoolName = "tank2" });
        Assert.NotNull(info);
        Assert.Equal(5000, info.Version);
        Assert.Equal("tank2", info.PoolName);
        Assert.Equal("15.5G", info.Size.ToString());
        Assert.Equal("105K", info.Allocated.ToString());
        Assert.Equal("15.5G", info.Free.ToString());
        Assert.Equal("7.30%", info.Fragmentation.ToString());
        Assert.Equal("6.50%", info.CapacityUsed.ToString());
        Assert.Equal("1.43x", info.DedupRatio.ToString());
        Assert.Equal(State.Online, info.State);
        _output.WriteLine(info.Dump(new JsonFormatter()));

    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void OfflineTest()
    {
        var zp = GetZpool();
        var args = new PoolOfflineArgs { PoolName = "tank", Device = "/dev/sda" };
        zp.Offline(args);
        var commands = _remoteProcessCall.GetCommandsInvoked();
        Assert.Single(commands);
        Assert.Contains("/sbin/zpool offline tank /dev/sda", commands);
    }


    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void OnlineTest()
    {
        var zp = GetZpool();
        var args = new PoolOnlineArgs { PoolName = "tank", Device = "/dev/sda" };
        zp.Online(args);
        var commands = _remoteProcessCall.GetCommandsInvoked();
        Assert.Single(commands);
        Assert.Contains("/sbin/zpool online tank /dev/sda", commands);
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void ClearTest()
    {
        var zp = GetZpool();
        var args = new PoolClearArgs { PoolName = "tank", Device = "/dev/sda" };
        zp.Clear(args);
        var commands = _remoteProcessCall.GetCommandsInvoked();
        Assert.Single(commands);
        Assert.Contains("/sbin/zpool clear tank /dev/sda", commands);
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void IOStatTest()
    {
        var zp = GetZpool();
        var args = new PoolIOStatsArgs { PoolName = "tank", Devices = ["/dev/sda"] };
        var stats = zp.IOStats(args);
        _output.WriteLine(stats.Dump(new JsonFormatter()));
        var commands = _remoteProcessCall.GetCommandsInvoked();
        Assert.Single(commands);
        Assert.Contains("/sbin/zpool iostat -LlpPvH tank /dev/sda", commands);
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void ResilverTest()
    {
        var zp = GetZpool();
        var args = new PoolResilverArgs { PoolName = "tank" };
        zp.Resilver(args);
        var commands = _remoteProcessCall.GetCommandsInvoked();
        Assert.Single(commands);
        Assert.Contains("/sbin/zpool resilver tank", commands);
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void CreateDraidTest()
    {
        var zp = GetZpool();
        zp.Create(new PoolCreateArgs
        {
            PoolName = "mytest",
            VDevs =
            [
                new DraigVDevCreationArgs
                {
                    Type = VDevCreationType.DRaid1,
                    Devices = ["/dev/sda", "/dev/sdb","/dev/sdc", "/dev/sdd"],
                    DraidArgs = new DraidArgs
                    {
                        Spares=1,
                        DataDevices=2,
                        Children=4
                    }
                }
            ]
        });
        var commands = _remoteProcessCall.GetCommandsInvoked();
        Assert.Equal(2, commands.Count);
        Assert.Equal("/sbin/zpool create mytest draid1:2d:4c:1s /dev/sda /dev/sdb /dev/sdc /dev/sdd", commands[0]);
        Assert.Equal("/sbin/zpool status -vP mytest", commands[1]);

    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void ScrubTest()
    {
        var zp = GetZpool();
        var args = new PoolScrubArgs { PoolName = "tank" };
        zp.Scrub(args);
        var commands = _remoteProcessCall.GetCommandsInvoked();
        var command = Assert.Single(commands);
        Assert.Equal("/sbin/zpool scrub tank", command);
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void TrimTest()
    {
        var zp = GetZpool();
        zp.Trim(new PoolTrimArgs { PoolName = "tank" });
        var commands = _remoteProcessCall.GetCommandsInvoked();
        var command = Assert.Single(commands);
        Assert.Equal("/sbin/zpool trim tank", command);
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void UpgradeTest()
    {
        var zp = GetZpool();
        zp.Upgrade(new PoolUpgradeArgs { PoolName = "tank" });
        var commands = _remoteProcessCall.GetCommandsInvoked();
        var command = Assert.Single(commands);
        Assert.Equal("/sbin/zpool upgrade tank", command);
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void GetUpgradeablePoolsTest()
    {
        var zp = GetZpool();
        var pools = zp.GetUpgradeablePools();
        Assert.Single(pools);
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void DetachTest()
    {
        var zp = GetZpool();
        var args = new PoolDetachArgs { PoolName = "tank", Device = "/dev/sdb" };
        zp.Detach(args);

        var commands = _remoteProcessCall.GetCommandsInvoked();
        var command = Assert.Single(commands);
        Assert.Equal("/sbin/zpool detach tank /dev/sdb", command);
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void AttachNewDeviceToPool()
    {
        var zp = GetZpool();

        var args = new PoolAttachArgs
        {
            PoolName = "tank",
            OldDevice = "/dev/sdb",
            NewDevice = "/dev/sdc"
        };

        zp.Attach(args);
        var commands = _remoteProcessCall.GetCommandsInvoked();
        var command = Assert.Single(commands);
        Assert.Equal("/sbin/zpool attach tank /dev/sdb /dev/sdc", command);
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void ReplaceNewDeviceToPool()
    {
        var zp = GetZpool();

        var args = new PoolReplaceArgs
        {
            PoolName = "tank",
            OldDevice = "/dev/sdb",
            NewDevice = "/dev/sdc",
        };

        zp.Replace(args);
        var commands = _remoteProcessCall.GetCommandsInvoked();
        var command = Assert.Single(commands);
        Assert.Equal("/sbin/zpool replace tank /dev/sdb /dev/sdc", command);
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void AddExtraMirrorToMirrorPool()
    {
        var zp = GetZpool();

        var args = new PoolAddArgs
        {
            PoolName = "tank",
            Force = true,
            VDevs = [new VDevCreationArgs { Type = VDevCreationType.Mirror, Devices = ["/dev/sda", "/dev/sdb"] }],
        };


        zp.Add(args);

        var commands = _remoteProcessCall.GetCommandsInvoked();
        var command = Assert.Single(commands);
        Assert.Equal("/sbin/zpool add -f tank mirror /dev/sda /dev/sdb", command);
    }

    [Fact]
    public void ZpoolRemoveTest()
    {
        var zp = GetZpool();
        var removeArgs = new PoolRemoveArgs
        {
            PoolName = "tank",
            VDev = "mirror-1"
        };


        zp.Remove(removeArgs);
        var commands = _remoteProcessCall.GetCommandsInvoked();
        var command = Assert.Single(commands);
        Assert.Equal("/sbin/zpool remove tank mirror-1", command);
    }
}