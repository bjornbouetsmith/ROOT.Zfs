using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
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
    public async Task RemoteSnapshotTest()
    {
        using var pool = TestPool.CreateSimplePool(_remoteProcessCall, _output);
        var sn = GetSnapshots();
        var cancellationToken = TestContext.Current.CancellationToken;
        await sn.CreateAsync(new SnapshotCreateArgs { Dataset = pool.Name }, cancellationToken);
        var snapshots = await sn.ListAsync(new SnapshotListArgs { Root = pool.Name }, cancellationToken);
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
    public async Task RemoteCreateAndDestroySnapshot()
    {
        var snapName = "RemoteCreateSnapshot" + DateTime.UtcNow.ToString("yyyyMMddhhmmss", CultureInfo.InvariantCulture);
        using var pool = TestPool.CreateSimplePool(_remoteProcessCall, _output);
        var sn = GetSnapshots();
        var cancellationToken = TestContext.Current.CancellationToken;

        await sn.CreateAsync(new SnapshotCreateArgs { Dataset = pool.Name, Snapshot = snapName }, cancellationToken);

        var snaps = await sn.ListAsync(new SnapshotListArgs { Root = pool.Name }, cancellationToken);

        var wasCreated = snaps.FirstOrDefault(snap => snap.SnapshotName.EndsWith(snapName, StringComparison.Ordinal));

        Assert.NotNull(wasCreated);
        var args = new SnapshotDestroyArgs { Dataset = pool.Name, Snapshot = snapName, IsExactName = true };
        await sn.DestroyAsync(args, cancellationToken);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task CreateAndDeleteByPatternTest()
    {
        var sn = GetSnapshots();
        using var pool = TestPool.CreateSimplePool(_remoteProcessCall, _output);
        var cancellationToken = TestContext.Current.CancellationToken;
        var prefix = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
        await sn.CreateAsync(new SnapshotCreateArgs { Dataset = pool.Name, Snapshot = $"{prefix}-1" }, cancellationToken);
        await sn.CreateAsync(new SnapshotCreateArgs { Dataset = pool.Name, Snapshot = $"{prefix}-2" }, cancellationToken);
        await sn.CreateAsync(new SnapshotCreateArgs { Dataset = pool.Name, Snapshot = $"{prefix}-3" }, cancellationToken);

        var snaps = (await sn.ListAsync(new SnapshotListArgs { Root = pool.Name }, cancellationToken)).Where(snap => snap.SnapshotName.StartsWith(prefix, StringComparison.Ordinal)).ToList();

        Assert.Equal(3, snaps.Count);
        var args = new SnapshotDestroyArgs { Dataset = pool.Name, Snapshot = prefix, IsExactName = false };
        await sn.DestroyAsync(args, cancellationToken);

        snaps = [.. (await sn.ListAsync(new SnapshotListArgs { Root = pool.Name }, cancellationToken)).Where(snap => snap.SnapshotName.StartsWith(prefix, StringComparison.Ordinal))];
        Assert.Empty(snaps);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task HoldAndReleaseSnapshotTest()
    {
        using var pool = TestPool.CreateSimplePool(_remoteProcessCall, _output);
        var sn = GetSnapshots();
        var cancellationToken = TestContext.Current.CancellationToken;
        const string snapName = "holdtest";
        const string tag = "mytag";

        await sn.CreateAsync(new SnapshotCreateArgs { Dataset = pool.Name, Snapshot = snapName }, cancellationToken);
        await sn.HoldAsync(new SnapshotHoldArgs { Dataset = pool.Name, Snapshot = snapName, Tag = tag }, cancellationToken);

        var holds = await sn.HoldsAsync(new SnapshotHoldsArgs { Dataset = pool.Name, Snapshot = snapName }, cancellationToken);
        var hold = Assert.Single(holds);
        Assert.Equal(tag, hold.Tag);
        Assert.EndsWith($"{pool.Name}@{snapName}", hold.Snapshot, StringComparison.Ordinal);

        await sn.ReleaseAsync(new SnapshotReleaseArgs { Dataset = pool.Name, Snapshot = snapName, Tag = tag }, cancellationToken);

        holds = await sn.HoldsAsync(new SnapshotHoldsArgs { Dataset = pool.Name, Snapshot = snapName }, cancellationToken);
        Assert.Empty(holds);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task CloneSnapshotTest()
    {
        var sn = GetSnapshots();
        var ds = GetDatasets();
        using var pool = TestPool.CreateSimplePool(_remoteProcessCall, _output);
        var cancellationToken = TestContext.Current.CancellationToken;
        await sn.CreateAsync(new SnapshotCreateArgs { Dataset = pool.Name, Snapshot = "mysnap" }, cancellationToken);

        var args = new SnapshotCloneArgs { Dataset = pool.Name, Snapshot = "mysnap", TargetDataset = $"{pool.Name}/mysnap_clone" };
        await sn.CloneAsync(args, cancellationToken);

        var getds = new DatasetListArgs { Root = pool.Name, IncludeChildren = true };
        var datasets = await ds.ListAsync(getds, cancellationToken);
        var myClone = datasets.FirstOrDefault(d => d.DatasetName == $"{pool.Name}/mysnap_clone");

        Assert.NotNull(myClone);
        Assert.True(myClone.IsClone);
    }
}
