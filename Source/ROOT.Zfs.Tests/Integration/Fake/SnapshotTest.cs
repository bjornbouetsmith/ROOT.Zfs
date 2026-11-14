using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Public.Arguments.Snapshots;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Integration.Fake;

public class FakeSnapshotTest
{
    private const string ListCommand = "/sbin/zfs list -Hpr -o type,creation,name,used,refer,avail,mountpoint,origin -d 99 -t snapshot tank/myds";

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

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task ListSnapshotsAsyncTest(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var sn = GetSnapshots();

        var snapshots = await sn.ListAsync(new SnapshotListArgs { Root = "tank/myds" }, TestContext.Current.CancellationToken);

        Assert.NotNull(snapshots);
        Assert.NotEmpty(snapshots);
        foreach (var snap in snapshots)
        {
            _output.WriteLine(snap.CreationDate.AsString());
            _output.WriteLine(snap.SnapshotName);
            _output.WriteLine(snap.Size.ToString());
        }

        var command = Assert.Single(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal(ListCommand, command);
        Assert.Equal(1, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public async Task ListSnapshotsAsyncReturnsMaterializedList()
    {
        var sn = GetSnapshots();

        var snapshots = await sn.ListAsync(new SnapshotListArgs { Root = "tank/myds" }, TestContext.Current.CancellationToken);

        // The command must have completed before the returned list is available.
        Assert.Single(_remoteProcessCall.GetCommandsInvoked());
        Assert.NotEmpty(snapshots);
        Assert.Single(_remoteProcessCall.GetCommandsInvoked());
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task ListAsyncForwardsCancellationToken(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var sn = GetSnapshots();

        await sn.ListAsync(new SnapshotListArgs { Root = "tank/myds" }, TestContext.Current.CancellationToken);

        var token = Assert.Single(_remoteProcessCall.AsyncCancellationTokens);
        Assert.Equal(TestContext.Current.CancellationToken, token);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task ListAsyncWithCancelledTokenDoesNotRunCommand(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var sn = GetSnapshots();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => sn.ListAsync(new SnapshotListArgs { Root = "tank/myds" }, cancellationTokenSource.Token));

        Assert.Empty(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal(0, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task ListAsyncPropagatesCommandFailure(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        _remoteProcessCall.CommandResponses[ListCommand] = (null, "cannot open 'tank/myds': dataset does not exist");
        var sn = GetSnapshots();

        await Assert.ThrowsAsync<ProcessCallException>(() => sn.ListAsync(new SnapshotListArgs { Root = "tank/myds" }, TestContext.Current.CancellationToken));

        Assert.Equal(1, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task RemoteCreateAndDestroySnapshotAsync(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var snapName = "RemoteCreateSnapshot" + new DateTime(2022, 09, 22, 21, 13, 47, DateTimeKind.Local).ToString("yyyyMMddhhmmss", CultureInfo.InvariantCulture);
        var cancellationToken = TestContext.Current.CancellationToken;

        var sn = GetSnapshots();

        await sn.CreateAsync(new SnapshotCreateArgs { Dataset = "tank/myds", Snapshot = snapName }, cancellationToken);

        var snaps = await sn.ListAsync(new SnapshotListArgs { Root = "tank/myds" }, cancellationToken);
        Assert.NotNull(snaps);
        var args = new SnapshotDestroyArgs { Dataset = "tank/myds", Snapshot = snapName, IsExactName = true };
        await sn.DestroyAsync(args, cancellationToken);

        var commands = _remoteProcessCall.GetCommandsInvoked();
        Assert.Equal(3, commands.Count);
        Assert.Equal($"/sbin/zfs snap tank/myds@{snapName}", commands[0]);
        Assert.Equal(ListCommand, commands[1]);
        Assert.Equal($"/sbin/zfs destroy tank/myds@{snapName}", commands[2]);
        Assert.Equal(3, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
        Assert.All(_remoteProcessCall.AsyncCancellationTokens, token => Assert.Equal(cancellationToken, token));
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task CreateAsyncForwardsCancellationToken(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var sn = GetSnapshots();

        await sn.CreateAsync(new SnapshotCreateArgs { Dataset = "tank/myds", Snapshot = "12345" }, TestContext.Current.CancellationToken);

        var command = Assert.Single(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal("/sbin/zfs snap tank/myds@12345", command);
        var token = Assert.Single(_remoteProcessCall.AsyncCancellationTokens);
        Assert.Equal(TestContext.Current.CancellationToken, token);
        Assert.Equal(1, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task CreateAsyncWithCancelledTokenDoesNotRunCommand(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var sn = GetSnapshots();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => sn.CreateAsync(new SnapshotCreateArgs { Dataset = "tank/myds", Snapshot = "12345" }, cancellationTokenSource.Token));

        Assert.Empty(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal(0, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
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

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task CreateAndDeleteByPatternTest(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var cancellationToken = TestContext.Current.CancellationToken;
        var sn = GetSnapshots();
        var prefix = "20220922211347000";
        await sn.CreateAsync(new SnapshotCreateArgs { Dataset = "tank/myds", Snapshot = $"{prefix}-1" }, cancellationToken);
        await sn.CreateAsync(new SnapshotCreateArgs { Dataset = "tank/myds", Snapshot = $"{prefix}-2" }, cancellationToken);
        await sn.CreateAsync(new SnapshotCreateArgs { Dataset = "tank/myds", Snapshot = $"{prefix}-3" }, cancellationToken);

        var snaps = (await sn.ListAsync(new SnapshotListArgs { Root = "tank/myds" }, cancellationToken)).Where(snap => snap.SnapshotName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)).ToList();

        Assert.Equal(3, snaps.Count);
        var args = new SnapshotDestroyArgs { Dataset = "tank/myds", Snapshot = prefix, IsExactName = false };
        await sn.DestroyAsync(args, cancellationToken);

        var commands = _remoteProcessCall.GetCommandsInvoked();
        Assert.Equal(8, commands.Count);
        Assert.Equal($"/sbin/zfs snap tank/myds@{prefix}-1", commands[0]);
        Assert.Equal($"/sbin/zfs snap tank/myds@{prefix}-2", commands[1]);
        Assert.Equal($"/sbin/zfs snap tank/myds@{prefix}-3", commands[2]);
        Assert.Equal(ListCommand, commands[3]);
        Assert.Equal(ListCommand, commands[4]);
        Assert.Equal($"/sbin/zfs destroy tank/myds@{prefix}-1", commands[5]);
        Assert.Equal($"/sbin/zfs destroy tank/myds@{prefix}-2", commands[6]);
        Assert.Equal($"/sbin/zfs destroy tank/myds@{prefix}-3", commands[7]);
        Assert.Equal(8, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
        Assert.All(_remoteProcessCall.AsyncCancellationTokens, token => Assert.Equal(cancellationToken, token));
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task DeleteByNonMatchingPatternOnlyListsSnapshots(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var sn = GetSnapshots();

        var args = new SnapshotDestroyArgs { Dataset = "tank/myds", Snapshot = "doesnotexist", IsExactName = false };
        await sn.DestroyAsync(args, TestContext.Current.CancellationToken);

        var command = Assert.Single(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal(ListCommand, command);
        Assert.Equal(1, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task DestroyExactAsyncForwardsCancellationToken(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        _remoteProcessCall.CommandResponses["/sbin/zfs destroy tank/myds@12345"] = (null, null);
        var sn = GetSnapshots();

        await sn.DestroyAsync(new SnapshotDestroyArgs { Dataset = "tank/myds", Snapshot = "12345", IsExactName = true }, TestContext.Current.CancellationToken);

        var command = Assert.Single(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal("/sbin/zfs destroy tank/myds@12345", command);
        var token = Assert.Single(_remoteProcessCall.AsyncCancellationTokens);
        Assert.Equal(TestContext.Current.CancellationToken, token);
        Assert.Equal(1, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task DestroyPatternAsyncForwardsCancellationToken(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var cancellationToken = TestContext.Current.CancellationToken;
        var sn = GetSnapshots();

        await sn.DestroyAsync(new SnapshotDestroyArgs { Dataset = "tank/myds", Snapshot = "20220922211347000", IsExactName = false }, cancellationToken);

        Assert.Equal(4, _remoteProcessCall.AsyncCancellationTokens.Count);
        Assert.All(_remoteProcessCall.AsyncCancellationTokens, token => Assert.Equal(cancellationToken, token));
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task DestroyAsyncWithCancelledTokenDoesNotRunCommand(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var sn = GetSnapshots();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => sn.DestroyAsync(new SnapshotDestroyArgs { Dataset = "tank/myds", Snapshot = "12345", IsExactName = true }, cancellationTokenSource.Token));
        await Assert.ThrowsAsync<OperationCanceledException>(() => sn.DestroyAsync(new SnapshotDestroyArgs { Dataset = "tank/myds", Snapshot = "12345", IsExactName = false }, cancellationTokenSource.Token));

        Assert.Empty(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal(0, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task SnapshotHoldAsyncTest(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var sn = GetSnapshots();

        await sn.HoldAsync(new SnapshotHoldArgs { Dataset = "tank/myds", Snapshot = "12345", Tag = "mytag" }, TestContext.Current.CancellationToken);

        var command = Assert.Single(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal("/sbin/zfs hold mytag tank/myds@12345", command);
        var token = Assert.Single(_remoteProcessCall.AsyncCancellationTokens);
        Assert.Equal(TestContext.Current.CancellationToken, token);
        Assert.Equal(1, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task HoldAsyncWithCancelledTokenDoesNotRunCommand(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var sn = GetSnapshots();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => sn.HoldAsync(new SnapshotHoldArgs { Dataset = "tank/myds", Snapshot = "12345", Tag = "mytag" }, cancellationTokenSource.Token));

        Assert.Empty(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal(0, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task SnapshotHoldsAsyncTest(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var sn = GetSnapshots();
        var args = new SnapshotHoldsArgs { Dataset = "tank/myds", Snapshot = "tank/myds@12345" };

        var holds = await sn.HoldsAsync(args, TestContext.Current.CancellationToken);

        var hold = Assert.Single(holds);
        Assert.Equal("tank/myds@12345", hold.Snapshot);
        Assert.Equal("mytag", hold.Tag);
        var command = Assert.Single(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal("/sbin/zfs holds -H tank/myds@12345", command);
        var token = Assert.Single(_remoteProcessCall.AsyncCancellationTokens);
        Assert.Equal(TestContext.Current.CancellationToken, token);
        Assert.Equal(1, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task HoldsAsyncWithCancelledTokenDoesNotRunCommand(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var sn = GetSnapshots();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => sn.HoldsAsync(new SnapshotHoldsArgs { Dataset = "tank/myds", Snapshot = "tank/myds@12345" }, cancellationTokenSource.Token));

        Assert.Empty(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal(0, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task SnapshotReleaseAsyncTest(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var sn = GetSnapshots();

        await sn.ReleaseAsync(new SnapshotReleaseArgs { Dataset = "tank/myds", Snapshot = "12345", Tag = "mytag" }, TestContext.Current.CancellationToken);

        var command = Assert.Single(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal("/sbin/zfs release mytag tank/myds@12345", command);
        var token = Assert.Single(_remoteProcessCall.AsyncCancellationTokens);
        Assert.Equal(TestContext.Current.CancellationToken, token);
        Assert.Equal(1, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task ReleaseAsyncWithCancelledTokenDoesNotRunCommand(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var sn = GetSnapshots();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => sn.ReleaseAsync(new SnapshotReleaseArgs { Dataset = "tank/myds", Snapshot = "12345", Tag = "mytag" }, cancellationTokenSource.Token));

        Assert.Empty(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal(0, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task CloneSnapshotAsyncTest(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var cancellationToken = TestContext.Current.CancellationToken;
        var sn = GetSnapshots();
        var snapName = "tank/myds@12345";
        await sn.CreateAsync(new SnapshotCreateArgs { Dataset = "tank/myds", Snapshot = snapName }, cancellationToken);

        var args = new SnapshotCloneArgs { Dataset = "tank/myds", Snapshot = snapName, TargetDataset = "tank/myds/mysnap_clone" };
        await sn.CloneAsync(args, cancellationToken);

        args = new SnapshotCloneArgs { Dataset = "tank/myds", Snapshot = snapName, TargetDataset = "tank/myds/mysnap_clone2", Properties = [new PropertyValue { Property = "atime", Value = "off" }] };
        await sn.CloneAsync(args, cancellationToken);
        var commands = _remoteProcessCall.GetCommandsInvoked();
        Assert.Equal(3, commands.Count);
        Assert.Equal("/sbin/zfs snap tank/myds@12345", commands[0]);
        Assert.Equal("/sbin/zfs clone -p tank/myds@12345 tank/myds/mysnap_clone", commands[1]);
        Assert.Equal("/sbin/zfs clone -p -o atime=off tank/myds@12345 tank/myds/mysnap_clone2", commands[2]);
        Assert.Equal(3, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
        Assert.All(_remoteProcessCall.AsyncCancellationTokens, token => Assert.Equal(cancellationToken, token));
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task CloneAsyncForwardsCancellationToken(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var sn = GetSnapshots();
        var args = new SnapshotCloneArgs { Dataset = "tank/myds", Snapshot = "12345", TargetDataset = "tank/myds/mysnap_clone" };

        await sn.CloneAsync(args, TestContext.Current.CancellationToken);

        var command = Assert.Single(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal("/sbin/zfs clone -p tank/myds@12345 tank/myds/mysnap_clone", command);
        var token = Assert.Single(_remoteProcessCall.AsyncCancellationTokens);
        Assert.Equal(TestContext.Current.CancellationToken, token);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task CloneAsyncWithCancelledTokenDoesNotRunCommand(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var sn = GetSnapshots();
        var args = new SnapshotCloneArgs { Dataset = "tank/myds", Snapshot = "12345", TargetDataset = "tank/myds/mysnap_clone" };
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => sn.CloneAsync(args, cancellationTokenSource.Token));

        Assert.Empty(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal(0, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
    }
}
