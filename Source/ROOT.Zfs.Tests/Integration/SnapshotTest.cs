using System;
using System.Linq;
using Xunit;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;
using ROOT.Zfs.Public.Arguments.Dataset;
using ROOT.Zfs.Public.Arguments.Snapshots;

namespace ROOT.Zfs.Tests.Integration;

public class SnapshotTest
{
    private readonly ITestOutputHelper _output;

    public SnapshotTest(ITestOutputHelper output)
    {
        _output = output;
    }

    private readonly IProcessCall _remoteProcessCall = TestHelpers.RequiresRemoteConnection ? new SSHProcessCall("bbs", "zfsdev.root.dom", true) : null;

    private Snapshots GetSnapshots()
    {
        var sn = ZfsTestFactory.CreateSnapshots(_remoteProcessCall);
        sn.RequiresSudo = TestHelpers.RequiresSudo;
        return sn;
    }

    private Datasets GetDatasets()
    {
        var sn = ZfsTestFactory.CreateDatasets(_remoteProcessCall);
        sn.RequiresSudo = TestHelpers.RequiresSudo;
        return sn;
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void RemoteSnapshotTest()
    {
        using var pool = TestPool.CreateSimplePool(_remoteProcessCall, _output);
        var sn = GetSnapshots();
        sn.Create(new SnapshotCreateArgs { Dataset = pool.Name });
        var snapshots = sn.List(new SnapshotListArgs { Root = pool.Name });
        Assert.NotNull(snapshots);
        foreach (var snap in snapshots)
        {
            _output.WriteLine(snap.CreationDate.AsString());
            _output.WriteLine(snap.SnapshotName);
            _output.WriteLine(snap.Size.ToString());
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void RemoteCreateAndDestroySnapshot()
    {
        var snapName = "RemoteCreateSnapshot" + DateTime.UtcNow.ToString("yyyyMMddhhmmss");
        using var pool = TestPool.CreateSimplePool(_remoteProcessCall, _output);
        var sn = GetSnapshots();

        sn.Create(new SnapshotCreateArgs { Dataset = pool.Name, Snapshot = snapName });

        var snaps = sn.List(new SnapshotListArgs { Root = pool.Name });

        var wasCreated = snaps.FirstOrDefault(snap => snap.SnapshotName.EndsWith(snapName));

        Assert.NotNull(wasCreated);
        var args = new SnapshotDestroyArgs { Dataset = pool.Name, Snapshot = snapName, IsExactName = true };
        sn.Destroy(args);

    }

    [Fact]
    [Trait("Category", "Integration")]
    public void CreateAndDeleteByPatternTest()
    {
        var sn = GetSnapshots();
        using var pool = TestPool.CreateSimplePool(_remoteProcessCall, _output);
        var prefix = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        sn.Create(new SnapshotCreateArgs { Dataset = pool.Name, Snapshot = $"{prefix}-1" });
        sn.Create(new SnapshotCreateArgs { Dataset = pool.Name, Snapshot = $"{prefix}-2" });
        sn.Create(new SnapshotCreateArgs { Dataset = pool.Name, Snapshot = $"{prefix}-3" });

        var snaps = sn.List(new SnapshotListArgs { Root = pool.Name }).Where(snap => snap.SnapshotName.StartsWith(prefix)).ToList();

        Assert.Equal(3, snaps.Count());
        var args = new SnapshotDestroyArgs { Dataset = pool.Name, Snapshot = prefix, IsExactName = false };
        sn.Destroy(args);

        snaps = [.. sn.List(new SnapshotListArgs { Root = pool.Name }).Where(snap => snap.SnapshotName.StartsWith(prefix))];
        Assert.Empty(snaps);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void CloneSnapshotTest()
    {
        var sn = GetSnapshots();
        var ds = GetDatasets();
        using var pool = TestPool.CreateSimplePool(_remoteProcessCall, _output);
        sn.Create(new SnapshotCreateArgs { Dataset = pool.Name, Snapshot = "mysnap" });

        var args = new SnapshotCloneArgs { Dataset = pool.Name, Snapshot = "mysnap", TargetDataset = $"{pool.Name}/mysnap_clone" };
        sn.Clone(args);

        var getds = new DatasetListArgs { Root = pool.Name, IncludeChildren = true };
        var datasets = ds.List(getds);
        var myClone = datasets.FirstOrDefault(d => d.DatasetName == $"{pool.Name}/mysnap_clone");
        Assert.NotNull(myClone);
        Assert.True(myClone.IsClone);
    }
}