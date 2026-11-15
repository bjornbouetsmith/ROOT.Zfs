using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;
using ROOT.Zfs.Public.Arguments.Pool;
using ROOT.Zfs.Public.Arguments.Snapshots;
using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Tests.Integration;

public class ZpoolTest
{
    private readonly ITestOutputHelper _output;

    public ZpoolTest(ITestOutputHelper output)
    {
        _output = output;
    }

    private readonly IProcessCall _remoteProcessCall = TestHelpers.RequiresRemoteConnection ? new SSHProcessCall("bbs", "zfsdev.root.dom", true) : null;

    private ZPool GetZpool()
    {
        var zp = ZfsTestFactory.CreateZpool(_remoteProcessCall);
        zp.RequiresSudo = TestHelpers.RequiresSudo;
        return zp;
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task HistoryTestWithSkip()
    {
        using var pool = await TestPool.CreateSimplePoolAsync(_remoteProcessCall, _output);
        var zp = GetZpool();
        var snap = ZfsTestFactory.CreateSnapshots(_remoteProcessCall);
        snap.RequiresSudo = TestHelpers.RequiresSudo;
        var cancellationToken = TestContext.Current.CancellationToken;
        await snap.CreateAsync(new SnapshotCreateArgs { Dataset = pool.Name, Snapshot = "test1" }, cancellationToken);
        await snap.CreateAsync(new SnapshotCreateArgs { Dataset = pool.Name, Snapshot = "test2" }, cancellationToken);
        await snap.CreateAsync(new SnapshotCreateArgs { Dataset = pool.Name, Snapshot = "test3" }, cancellationToken);
        await snap.CreateAsync(new SnapshotCreateArgs { Dataset = pool.Name }, cancellationToken);
        var args = new PoolHistoryArgs { PoolName = pool.Name };
        var lines = (await zp.HistoryAsync(args, cancellationToken)).ToList().Count;
        args = new PoolHistoryArgs { PoolName = pool.Name, SkipLines = lines - 2 };
        var history = (await zp.HistoryAsync(args, cancellationToken)).ToList();
        Assert.NotNull(history);
        _output.WriteLine(history.Dump(new JsonFormatter()));
        Assert.Equal(2, history.Count);
    }



    [Fact]
    [Trait("Category", "Integration")]
    public async Task HistoryTest()
    {
        using var pool = await TestPool.CreateSimplePoolAsync(_remoteProcessCall, _output);
        var zp = GetZpool();
        var args = new PoolHistoryArgs { PoolName = pool.Name };
        var history = await zp.HistoryAsync(args, TestContext.Current.CancellationToken);
        Assert.NotNull(history);
        _output.WriteLine(history.Dump(new JsonFormatter()));
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task HistoryAfterDateTest()
    {
        using var pool = await TestPool.CreateSimplePoolAsync(_remoteProcessCall, _output);
        var zp = GetZpool();
        var snap = ZfsTestFactory.CreateSnapshots(_remoteProcessCall);
        snap.RequiresSudo = TestHelpers.RequiresSudo;
        var cancellationToken = TestContext.Current.CancellationToken;
        await snap.CreateAsync(new SnapshotCreateArgs { Dataset = pool.Name, Snapshot = "test1" }, cancellationToken);
        await snap.CreateAsync(new SnapshotCreateArgs { Dataset = pool.Name, Snapshot = "test2" }, cancellationToken);
        await snap.CreateAsync(new SnapshotCreateArgs { Dataset = pool.Name, Snapshot = "test3" }, cancellationToken);
        await snap.CreateAsync(new SnapshotCreateArgs { Dataset = pool.Name }, cancellationToken);
        var args = new PoolHistoryArgs { PoolName = pool.Name };
        var last10AtMost = (await zp.HistoryAsync(args, cancellationToken)).TakeLast(10).ToList();

        // This is safe, since zfs always have at last pool creation as a history event
        var history = last10AtMost.First();
        args = new PoolHistoryArgs { PoolName = pool.Name, AfterDate = history.Time };
        var afterDate = (await zp.HistoryAsync(args, cancellationToken)).ToList();

        if (last10AtMost.Count > 1)
        {
            Assert.True(afterDate.Count < last10AtMost.Count);
        }

    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetStatusTest()
    {
        using var pool = await TestPool.CreateSimplePoolAsync(_remoteProcessCall, _output);
        var zp = GetZpool();
        var arg = new PoolStatusArgs { PoolName = pool.Name };
        var status = await zp.StatusAsync(arg, TestContext.Current.CancellationToken);
        Assert.NotNull(status);
        Assert.Equal(pool.Name, status.Pool.PoolName);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task NonExistingPoolStatus()
    {
        var zp = GetZpool();
        var ex = await Assert.ThrowsAsync<ProcessCallException>(() => zp.StatusAsync(new PoolStatusArgs { PoolName = "A" + Guid.NewGuid().ToString() }, TestContext.Current.CancellationToken));
        _output.WriteLine(ex.Message);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task ZpoolStatusTest()
    {
        using var testPool = await TestPool.CreateSimplePoolAsync(_remoteProcessCall, _output);

        var zp = GetZpool();
        var arg = new PoolStatusArgs { PoolName = testPool.Name };
        var status = await zp.StatusAsync(arg, TestContext.Current.CancellationToken);

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
    [Trait("Category", "Integration")]
    public async Task DestroyPoolTest()
    {
        using var pool = await TestPool.CreateSimplePoolAsync(_remoteProcessCall, _output);
        var zp = GetZpool();
        await zp.DestroyAsync(new PoolDestroyArgs { PoolName = pool.Name }, TestContext.Current.CancellationToken);
    }


    [Fact]
    [Trait("Category", "Integration")]
    public async Task CreateMirrorPool()
    {
        using var pool = new TestPool(_remoteProcessCall, _output);
        var disk1 = pool.AddDisk();
        var disk2 = pool.AddDisk();

        var name = "TestP" + Guid.NewGuid();

        var args = new PoolCreateArgs
        {
            PoolName = name,
            MountPoint = "none",
            VDevs =
            [
                new()
                {
                    Type = VDevCreationType.Mirror,
                    Devices = [disk1, disk2]
                }
            ]
        };

        var status = await pool.CreatePoolAsync(args);
        Assert.Equal(State.Online, status.State);

    }

    [Theory]
    [Trait("Category", "Integration")]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task CreateRaidZTest(int raidz)
    {
        using var pool = new TestPool(_remoteProcessCall, _output);

        var raidDisks = new List<string>();
        var disk1 = pool.AddDisk();
        var disk2 = pool.AddDisk();
        raidDisks.Add(disk1);
        raidDisks.Add(disk2);

        var disk3 = pool.AddDisk();
        for (int x = 0; x < raidz; x++)
        {
            raidDisks.Add(pool.AddDisk());
        }

        var name = "TestP" + Guid.NewGuid();
        VDevCreationType type = raidz == 1 ? VDevCreationType.Raidz1 : raidz == 2 ? VDevCreationType.Raidz2 : VDevCreationType.Raidz3;
        var args = new PoolCreateArgs
        {
            PoolName = name,
            MountPoint = "none",
            VDevs =
            [
                new()
                {
                    Type = type,
                    Devices = raidDisks
                },
                new()
                {
                    Type = VDevCreationType.Spare,
                    Devices = [disk3]
                }


            ]
        };

        var status = await pool.CreatePoolAsync(args);
        Assert.Equal(State.Online, status.State);
        _output.WriteLine(status.Dump(new JsonFormatter()));
    }

    [Theory]
    [Trait("Category", "Integration")]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task CreateDRaidZTest(int draid)
    {
        using var pool = new TestPool(_remoteProcessCall, _output);

        var raidDisks = new List<string>();
        var disk1 = pool.AddDisk();
        var disk2 = pool.AddDisk();
        var extra = pool.AddDisk();
        raidDisks.Add(disk1);
        raidDisks.Add(disk2);
        raidDisks.Add(extra);

        for (int x = 0; x < draid; x++)
        {
            raidDisks.Add(pool.AddDisk());
        }

        var name = "TestP" + Guid.NewGuid();
        VDevCreationType type = draid == 1 ? VDevCreationType.DRaid1 : draid == 2 ? VDevCreationType.DRaid2 : VDevCreationType.DRaid3;
        var args = new PoolCreateArgs
        {
            PoolName = name,
            MountPoint = "none",
            VDevs =
            [
                new DraigVDevCreationArgs()
                {
                    Type = type,
                    Devices = raidDisks,
                    DraidArgs = new DraidArgs
                    {
                        Spares=1,
                        DataDevices=2,
                        Children=3+draid
                    }
                }
            ]
        };

        var status = await pool.CreatePoolAsync(args);
        Assert.Equal(State.Online, status.State);
        _output.WriteLine(status.Dump(new JsonFormatter()));
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task ScrubTest()
    {
        using var pool = await TestPool.CreateSimplePoolAsync(_remoteProcessCall, _output);
        var zp = GetZpool();
        var args = new PoolScrubArgs { PoolName = pool.Name };
        await zp.ScrubAsync(args, TestContext.Current.CancellationToken);
        var arg = new PoolStatusArgs { PoolName = pool.Name };
        var status = await zp.StatusAsync(arg, TestContext.Current.CancellationToken);
        Assert.Equal(State.Online, status.State);

    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task TrimPoolTestSimple()
    {
        using var pool = await TestPool.CreateSimplePoolAsync(_remoteProcessCall, _output);
        var zp = GetZpool();

        var args = new PoolTrimArgs { PoolName = pool.Name };
        await zp.TrimAsync(args, TestContext.Current.CancellationToken);
        var arg = new PoolStatusArgs { PoolName = pool.Name };
        var status = await zp.StatusAsync(arg, TestContext.Current.CancellationToken);
        Assert.Equal(State.Online, status.State);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task TrimDeviceTestSimple()
    {
        using var pool = await TestPool.CreateSimplePoolAsync(_remoteProcessCall, _output);
        var zp = GetZpool();
        var firstDisk = pool.Disks[0];
        var args = new PoolTrimArgs { PoolName = pool.Name, DeviceName = firstDisk };
        await zp.TrimAsync(args, TestContext.Current.CancellationToken);
        var arg = new PoolStatusArgs { PoolName = pool.Name };
        var status = await zp.StatusAsync(arg, TestContext.Current.CancellationToken);
        Assert.Equal(State.Online, status.State);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetPoolInfoTest()
    {
        using var pool = await TestPool.CreateSimplePoolAsync(_remoteProcessCall, _output);
        var zp = GetZpool();

        var info = await zp.ListAsync(new PoolListArgs { PoolName = pool.Name }, TestContext.Current.CancellationToken);
        _output.WriteLine(info.Dump(new JsonFormatter()));
        Assert.NotNull(info);
        Assert.NotEqual(0, info.Version);
        Assert.Equal(pool.Name, info.PoolName);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task DetachTest()
    {
        using var pool = await TestPool.CreateSimplePoolAsync(_remoteProcessCall, _output);
        var zp = GetZpool();
        var args = new PoolDetachArgs { PoolName = pool.Name, Device = pool.Disks[1] };
        await zp.DetachAsync(args, TestContext.Current.CancellationToken);
        var arg = new PoolStatusArgs { PoolName = pool.Name };
        var status = await zp.StatusAsync(arg, TestContext.Current.CancellationToken);
        Assert.NotNull(status);
        _output.WriteLine(status.Dump(new JsonFormatter()));
    }


    [Fact]
    [Trait("Category", "Integration")]
    public async Task IOStatTestSpecificDevice()
    {
        using var pool = await TestPool.CreateSimplePoolAsync(_remoteProcessCall, _output);
        var zp = GetZpool();
        var args = new PoolIOStatsArgs { PoolName = pool.Name, Devices = [pool.Disks[0]] };
        var stats = await zp.IOStatsAsync(args, TestContext.Current.CancellationToken);

        _output.WriteLine(stats.Dump(new JsonFormatter()));
        Assert.NotNull(stats);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task IOStatTestEntirePool()
    {
        using var pool = await TestPool.CreateSimplePoolAsync(_remoteProcessCall, _output);
        var zp = GetZpool();
        var args = new PoolIOStatsArgs { PoolName = pool.Name };
        var stats = await zp.IOStatsAsync(args, TestContext.Current.CancellationToken);

        _output.WriteLine(stats.Dump(new JsonFormatter()));
        Assert.NotNull(stats);
        // pool
        var poolStat = Assert.Single(stats.Stats);
        //vdev
        var vdev = Assert.Single(poolStat.ChildStats);
        // actual devices
        Assert.Equal(2, vdev.ChildStats.Count);
    }

    //Turn a two way mirror into a three way mirror
    [Fact]
    [Trait("Category", "Integration")]
    public async Task AttachNewDeviceToPool()
    {
        using var pool = await TestPool.CreateSimplePoolAsync(_remoteProcessCall, _output);

        var oldDevice = pool.Disks.Last();
        var newDevice = pool.AddDisk();
        var zp = GetZpool();

        var args = new PoolAttachArgs
        {
            PoolName = pool.Name,
            OldDevice = oldDevice,
            NewDevice = newDevice
        };

        await zp.AttachAsync(args, TestContext.Current.CancellationToken);
        var arg = new PoolStatusArgs { PoolName = pool.Name };
        var status = await zp.StatusAsync(arg, TestContext.Current.CancellationToken);
        _output.WriteLine(status.Dump(new JsonFormatter()));

        Assert.Equal(3, status.Pool.VDevs[0].Devices.Count);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task ReplaceNewDeviceToPool()
    {
        using var pool = await TestPool.CreateSimplePoolAsync(_remoteProcessCall, _output);

        var oldDevice = pool.Disks.Last();
        var newDevice = pool.AddDisk();
        var zp = GetZpool();

        var args = new PoolReplaceArgs
        {
            PoolName = pool.Name,
            OldDevice = oldDevice,
            NewDevice = newDevice
        };

        await zp.ReplaceAsync(args, TestContext.Current.CancellationToken);
        var arg = new PoolStatusArgs { PoolName = pool.Name };
        var status = await zp.StatusAsync(arg, TestContext.Current.CancellationToken);
        _output.WriteLine(status.Dump(new JsonFormatter()));
        var stopwatch = Stopwatch.StartNew();
        var oldExist = status.Pool.VDevs.Any(v => v.Devices.Any(d => d.DeviceName == oldDevice));
        while (oldExist && stopwatch.Elapsed < TimeSpan.FromSeconds(5))
        {
            status = await zp.StatusAsync(arg, TestContext.Current.CancellationToken);
            oldExist = status.Pool.VDevs.Any(v => v.Devices.Any(d => d.DeviceName == oldDevice));
        }

        var newExist = status.Pool.VDevs.Any(v => v.Devices.Any(d => d.DeviceName == newDevice));

        Assert.False(oldExist);
        Assert.True(newExist);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task AddExtraMirrorToMirrorPool()
    {
        using var pool = await TestPool.CreateSimplePoolAsync(_remoteProcessCall, _output);
        var zp = GetZpool();
        var arg = new PoolStatusArgs { PoolName = pool.Name };
        var status = await zp.StatusAsync(arg, TestContext.Current.CancellationToken);
        Assert.Single(status.Pool.VDevs);
        var disk1 = pool.AddDisk();
        var disk2 = pool.AddDisk();

        var args = new PoolAddArgs
        {
            PoolName = pool.Name,
            Force = true,
            VDevs = [new VDevCreationArgs { Type = VDevCreationType.Mirror, Devices = [disk1, disk2] }],
        };


        await zp.AddAsync(args, TestContext.Current.CancellationToken);

        status = await zp.StatusAsync(arg, TestContext.Current.CancellationToken);
        _output.WriteLine(status.Dump(new JsonFormatter()));
        Assert.Equal(2, status.Pool.VDevs.Count);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task RemoveExtraMirrorFromPool()
    {
        using var pool = new TestPool(_remoteProcessCall, _output);
        var disk1 = pool.AddDisk("300M");
        var disk2 = pool.AddDisk("300M");
        var disk3 = pool.AddDisk("64M");
        var disk4 = pool.AddDisk("64M");

        var name = "TestP" + Guid.NewGuid();

        var args = new PoolCreateArgs
        {
            PoolName = name,
            MountPoint = "none",
            VDevs =
            [
                new()
                {
                    Type = VDevCreationType.Mirror,
                    Devices = [disk1, disk2],
                },
                new()
                {
                    Type = VDevCreationType.Mirror,
                    Devices = [disk3, disk4]
                }
            ]
        };

        var status = await pool.CreatePoolAsync(args);
        Assert.Equal(State.Online, status.State);

        var removeArgs = new PoolRemoveArgs
        {
            PoolName = name,
            VDev = "mirror-1"
        };

        var zp = GetZpool();
        await zp.RemoveAsync(removeArgs, TestContext.Current.CancellationToken);
        var arg = new PoolStatusArgs { PoolName = pool.Name };
        status = await zp.StatusAsync(arg, TestContext.Current.CancellationToken);

        _output.WriteLine(status.Dump(new JsonFormatter()));
        var stopwatch = Stopwatch.StartNew();
        var oldExist = status.Pool.VDevs.Count == 2;
        while (oldExist && stopwatch.Elapsed < TimeSpan.FromSeconds(5))
        {
            status = await zp.StatusAsync(arg, TestContext.Current.CancellationToken);
            oldExist = status.Pool.VDevs.Count == 2;
        }
        _output.WriteLine(status.Dump(new JsonFormatter()));
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task OfflineTest()
    {
        using var pool = await TestPool.CreateSimplePoolAsync(_remoteProcessCall, _output);
        var zp = GetZpool();
        var arg = new PoolStatusArgs { PoolName = pool.Name };
        var device = pool.Disks.Last();
        var args = new PoolOfflineArgs { PoolName = pool.Name, Device = device };
        await zp.OfflineAsync(args, TestContext.Current.CancellationToken);

        var status = await zp.StatusAsync(arg, TestContext.Current.CancellationToken);

        _output.WriteLine(status.Dump(new JsonFormatter()));
        Assert.Equal(State.Degraded, status.State);

    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task OnlineTest()
    {
        using var pool = await TestPool.CreateSimplePoolAsync(_remoteProcessCall, _output);
        var zp = GetZpool();

        var device = pool.Disks.Last();
        var args = new PoolOfflineArgs { PoolName = pool.Name, Device = device };
        await zp.OfflineAsync(args, TestContext.Current.CancellationToken);
        var arg = new PoolStatusArgs { PoolName = pool.Name };
        var status = await zp.StatusAsync(arg, TestContext.Current.CancellationToken);

        _output.WriteLine(status.Dump(new JsonFormatter()));
        Assert.Equal(State.Degraded, status.State);

        var onlineArgs = new PoolOnlineArgs { PoolName = pool.Name, Device = device };
        await zp.OnlineAsync(onlineArgs, TestContext.Current.CancellationToken);

        status = await zp.StatusAsync(arg, TestContext.Current.CancellationToken);

        _output.WriteLine(status.Dump(new JsonFormatter()));
        Assert.Equal(State.Online, status.State);

    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetPoolInfosTest()
    {
        using var pool = await TestPool.CreateSimplePoolAsync(_remoteProcessCall, _output);
        var zp = GetZpool();

        var infos = await zp.ListAsync(TestContext.Current.CancellationToken);
        _output.WriteLine(infos.Dump(new JsonFormatter()));
        Assert.NotEmpty(infos); // No pools found, this is a bug

    }
}