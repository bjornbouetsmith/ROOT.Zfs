using System;
using System.Linq;
using Xunit;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Public.Arguments.Snapshots;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Integration.Fake;

public class FakeSnapshotTest
{
    private readonly ITestOutputHelper _output;

    public FakeSnapshotTest(ITestOutputHelper output)
    {
        _output = output;
    }

    private readonly FakeRemoteConnection _remoteProcessCall = new("2.1.5-2");

    private Snapshots GetSnapshots()
    {
        return ZfsTestFactory.CreateSnapshots(_remoteProcessCall);
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void ListSnapshotsTest()
    {
        var sn = GetSnapshots();

        var snapshots = sn.List(new SnapshotListArgs { Root = "tank/myds" });
        Assert.NotNull(snapshots);
        foreach (var snap in snapshots)
        {
            _output.WriteLine(snap.CreationDate.AsString());
            _output.WriteLine(snap.SnapshotName);
            _output.WriteLine(snap.Size.ToString());
        }
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void RemoteCreateAndDestroySnapshot()
    {
        var snapName = "RemoteCreateSnapshot" + new DateTime(2022, 09, 22, 21, 13, 47, DateTimeKind.Local).ToString("yyyyMMddhhmmss");

        var sn = GetSnapshots();

        sn.Create(new SnapshotCreateArgs { Dataset = "tank/myds", Snapshot = snapName });

        var snaps = sn.List(new SnapshotListArgs { Root = "tank/myds" });
        Assert.NotNull(snaps);
        var args = new SnapshotDestroyArgs { Dataset = "tank/myds", Snapshot = snapName, IsExactName = true };
        sn.Destroy(args);

    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void CreateSnapshotNameTEst()
    {
        var time = new DateTime(2022, 09, 22, 21, 13, 47, DateTimeKind.Local);

        var binaries = new ZfsBinaries();
        var commands = new SnapshotCommands(binaries, new DatasetCommands(binaries));
        var name = commands.CreateSnapshotName(time);
        Assert.Equal("20220922211347", name);
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void CreateAndDeleteByPatternTest()
    {
        var sn = GetSnapshots();
        var prefix = new DateTime(2022, 09, 22, 21, 13, 47, DateTimeKind.Local).ToString("yyyyMMddHHmmssfff");
        sn.Create(new SnapshotCreateArgs { Dataset = "tank/myds", Snapshot = $"{prefix}-1" });
        sn.Create(new SnapshotCreateArgs { Dataset = "tank/myds", Snapshot = $"{prefix}-2" });
        sn.Create(new SnapshotCreateArgs { Dataset = "tank/myds", Snapshot = $"{prefix}-3" });

        var snaps = sn.List(new SnapshotListArgs { Root = "tank/myds" }).Where(snap => snap.SnapshotName.StartsWith($"{prefix}")).ToList();

        Assert.Equal(3, snaps.Count);
        var args = new SnapshotDestroyArgs { Dataset = "tank/myds", Snapshot = prefix, IsExactName = false };
        sn.Destroy(args);
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void SnapshotHoldTest()
    {
        var sn = GetSnapshots();
        sn.Hold(new SnapshotHoldArgs { Dataset = "tank/myds", Snapshot = "12345", Tag = "mytag" });
        var commands = _remoteProcessCall.GetCommandsInvoked();
        var command = Assert.Single(commands);
        Assert.Equal("/sbin/zfs hold mytag tank/myds@12345", command);
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void SnapshotHoldsTest()
    {
        var sn = GetSnapshots();
        var args = new SnapshotHoldsArgs { Dataset = "tank/myds", Snapshot = "tank/myds@12345"};
        var holds = sn.Holds(args);
        Assert.Single(holds);
        var commands = _remoteProcessCall.GetCommandsInvoked();
        var command = Assert.Single(commands);
        Assert.Equal("/sbin/zfs holds -H tank/myds@12345", command);
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void SnapshotReleaseTest()
    {
        var sn = GetSnapshots();
        sn.Release(new SnapshotReleaseArgs{Dataset="tank/myds",Snapshot= "12345", Tag="mytag"});

        var commands = _remoteProcessCall.GetCommandsInvoked();
        var command = Assert.Single(commands);
        Assert.Equal("/sbin/zfs release mytag tank/myds@12345", command);
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void CloneSnapshotTest()
    {
        var sn = GetSnapshots();
        var snapName = "tank/myds@12345";
        sn.Create(new SnapshotCreateArgs { Dataset = "tank/myds", Snapshot = snapName });

        var args = new SnapshotCloneArgs { Dataset = "tank/myds", Snapshot = snapName, TargetDataset = "tank/myds/mysnap_clone" };
        sn.Clone(args);

        args = new SnapshotCloneArgs { Dataset = "tank/myds", Snapshot = snapName, TargetDataset = "tank/myds/mysnap_clone2", Properties = [new PropertyValue { Property = "atime", Value = "off" }] };
        sn.Clone(args);
        var commands = _remoteProcessCall.GetCommandsInvoked();
        Assert.Equal(3, commands.Count);
        Assert.Equal("/sbin/zfs snap tank/myds@12345", commands[0]);
        Assert.Equal("/sbin/zfs clone -p tank/myds@12345 tank/myds/mysnap_clone", commands[1]);
        Assert.Equal("/sbin/zfs clone -p -o atime=off tank/myds@12345 tank/myds/mysnap_clone2", commands[2]);

    }
}