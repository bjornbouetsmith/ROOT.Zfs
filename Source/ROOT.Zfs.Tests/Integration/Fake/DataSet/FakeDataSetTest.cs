using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Arguments.Dataset;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Integration.Fake.DataSet;

public abstract class FakeDataSetTest
{
    private readonly ITestOutputHelper _output;

    protected FakeDataSetTest(ITestOutputHelper output)
    {
        _output = output;
    }

    internal abstract FakeRemoteConnection CreateProcessCall();

    internal IDatasets GetDatasets(FakeRemoteConnection remoteConnection = null)
    {
        return ZfsTestFactory.CreateDatasets(remoteConnection ?? CreateProcessCall());
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task ListAsyncTest(bool completeAsyncResponses)
    {
        var remoteConnection = CreateProcessCall();
        remoteConnection.CompleteAsyncResponses = completeAsyncResponses;
        var ds = GetDatasets(remoteConnection);

        var dataSets = await ds.ListAsync(new DatasetListArgs(), TestContext.Current.CancellationToken);

        Assert.NotNull(dataSets);
        Assert.NotEmpty(dataSets);
        foreach (var set in dataSets)
        {
            _output.WriteLine(set.Dump(new JsonFormatter()));
        }

        var command = Assert.Single(remoteConnection.GetCommandsInvoked());
        Assert.Equal("/sbin/zfs list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t filesystem,volume", command);
        Assert.Equal(1, remoteConnection.AsyncLoadResponseCalls);
        Assert.Equal(0, remoteConnection.SyncLoadResponseCalls);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task GetDataSetShouldReturnDataSetAsync(bool completeAsyncResponses)
    {
        var remoteConnection = CreateProcessCall();
        remoteConnection.CompleteAsyncResponses = completeAsyncResponses;
        var ds = GetDatasets(remoteConnection);
        var args = new DatasetListArgs { Root = "tank" };

        var dataSets = await ds.ListAsync(args, TestContext.Current.CancellationToken);

        Assert.NotEmpty(dataSets);
        var root = dataSets[0];
        Assert.Equal("tank", root.DatasetName);
        Assert.Equal(DatasetTypes.Filesystem, root.Type);
        _output.WriteLine(root.Dump(new JsonFormatter()));
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task GetNonExistingDataSetShouldThrowExceptionAsync(bool completeAsyncResponses)
    {
        var remoteConnection = CreateProcessCall();
        remoteConnection.CompleteAsyncResponses = completeAsyncResponses;
        var ds = GetDatasets(remoteConnection);

        await Assert.ThrowsAsync<ProcessCallException>(() => ds.ListAsync(new DatasetListArgs { Root = "ungabunga" }, TestContext.Current.CancellationToken));

        Assert.Equal(1, remoteConnection.AsyncLoadResponseCalls);
        Assert.Equal(0, remoteConnection.SyncLoadResponseCalls);
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public async Task ListAsyncReturnsMaterializedList()
    {
        var remoteConnection = CreateProcessCall();
        var ds = GetDatasets(remoteConnection);

        var dataSets = await ds.ListAsync(new DatasetListArgs(), TestContext.Current.CancellationToken);

        // The command must have completed before the returned list is used.
        Assert.Single(remoteConnection.GetCommandsInvoked());
        Assert.NotEmpty(dataSets);
        Assert.Single(remoteConnection.GetCommandsInvoked());
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task ListAsyncForwardsCancellationToken(bool completeAsyncResponses)
    {
        var remoteConnection = CreateProcessCall();
        remoteConnection.CompleteAsyncResponses = completeAsyncResponses;
        var ds = GetDatasets(remoteConnection);

        await ds.ListAsync(new DatasetListArgs(), TestContext.Current.CancellationToken);

        var token = Assert.Single(remoteConnection.AsyncCancellationTokens);
        Assert.Equal(TestContext.Current.CancellationToken, token);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task ListAsyncWithCancelledTokenDoesNotRunCommand(bool completeAsyncResponses)
    {
        var remoteConnection = CreateProcessCall();
        remoteConnection.CompleteAsyncResponses = completeAsyncResponses;
        var ds = GetDatasets(remoteConnection);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => ds.ListAsync(new DatasetListArgs(), cancellationTokenSource.Token));

        Assert.Empty(remoteConnection.GetCommandsInvoked());
        Assert.Equal(0, remoteConnection.AsyncLoadResponseCalls);
        Assert.Equal(0, remoteConnection.SyncLoadResponseCalls);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task CreateDataSetTest(bool completeAsyncResponses)
    {
        var remoteConnection = CreateProcessCall();
        remoteConnection.CompleteAsyncResponses = completeAsyncResponses;
        var ds = GetDatasets(remoteConnection);
        var cancellationToken = TestContext.Current.CancellationToken;
        var args = new DatasetCreationArgs
        {
            DatasetName = "tank/myds",
            Type = DatasetTypes.Filesystem,
            Properties = [new PropertyValue { Property = "atime", Value = "off" }]

        };

        var dataset = await ds.CreateAsync(args, cancellationToken);

        Assert.NotNull(dataset);
        Assert.Equal("tank/myds", dataset.DatasetName);
        Assert.Equal(2, remoteConnection.AsyncLoadResponseCalls);
        Assert.Equal(0, remoteConnection.SyncLoadResponseCalls);
        Assert.Equal(2, remoteConnection.AsyncCancellationTokens.Count);
        Assert.All(remoteConnection.AsyncCancellationTokens, token => Assert.Equal(cancellationToken, token));
        var commands = remoteConnection.GetCommandsInvoked();
        Assert.Equal(2, commands.Count);
        Assert.Equal("/sbin/zfs create -o atime=off tank/myds", commands[0]);
        Assert.Equal("/sbin/zfs list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t filesystem tank/myds", commands[1]);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task CreateDataSetInvalidTest(bool completeAsyncResponses)
    {
        var remoteConnection = CreateProcessCall();
        remoteConnection.CompleteAsyncResponses = completeAsyncResponses;
        var ds = GetDatasets(remoteConnection);
        var args = new DatasetCreationArgs
        {
            DatasetName = "tank/myds",
            Type = DatasetTypes.Bookmark,
            Properties = [new PropertyValue { Property = "atime", Value = "off" }]

        };

        await Assert.ThrowsAsync<ArgumentException>(() => ds.CreateAsync(args, TestContext.Current.CancellationToken));

        Assert.Empty(remoteConnection.GetCommandsInvoked());
        Assert.Equal(0, remoteConnection.AsyncLoadResponseCalls);
        Assert.Equal(0, remoteConnection.SyncLoadResponseCalls);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task CreateDataSetReturnsNullWhenItCannotBeListedTest(bool completeAsyncResponses)
    {
        var remoteConnection = CreateProcessCall();
        remoteConnection.CompleteAsyncResponses = completeAsyncResponses;
        remoteConnection.CommandResponses["/sbin/zfs list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t filesystem tank/myds"] = (string.Empty, null);
        var ds = GetDatasets(remoteConnection);
        var args = new DatasetCreationArgs
        {
            DatasetName = "tank/myds",
            Type = DatasetTypes.Filesystem,
            Properties = [new PropertyValue { Property = "atime", Value = "off" }]

        };

        var dataset = await ds.CreateAsync(args, TestContext.Current.CancellationToken);

        Assert.Null(dataset);
        Assert.Equal(2, remoteConnection.AsyncLoadResponseCalls);
        Assert.Equal(0, remoteConnection.SyncLoadResponseCalls);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task CreateDataSetWithCancelledTokenDoesNotRunCommandTest(bool completeAsyncResponses)
    {
        var remoteConnection = CreateProcessCall();
        remoteConnection.CompleteAsyncResponses = completeAsyncResponses;
        var ds = GetDatasets(remoteConnection);
        var args = new DatasetCreationArgs
        {
            DatasetName = "tank/myds",
            Type = DatasetTypes.Filesystem
        };
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => ds.CreateAsync(args, cancellationTokenSource.Token));

        Assert.Empty(remoteConnection.GetCommandsInvoked());
        Assert.Equal(0, remoteConnection.AsyncLoadResponseCalls);
        Assert.Equal(0, remoteConnection.SyncLoadResponseCalls);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task DestroyDataSetTest(bool completeAsyncResponses)
    {
        var remoteConnection = CreateProcessCall();
        remoteConnection.CompleteAsyncResponses = completeAsyncResponses;
        var ds = GetDatasets(remoteConnection);
        var cancellationToken = TestContext.Current.CancellationToken;
        var destroyArgs = new DatasetDestroyArgs { Dataset = "tank/myds", DestroyFlags = DatasetDestroyFlags.Recursive };

        var response = await ds.DestroyAsync(destroyArgs, cancellationToken);

        Assert.Equal(DatasetDestroyFlags.Recursive, response.Flags);
        Assert.Null(response.DryRun);
        var command = Assert.Single(remoteConnection.GetCommandsInvoked());
        Assert.Equal("/sbin/zfs destroy -r tank/myds", command);
        Assert.Equal(1, remoteConnection.AsyncLoadResponseCalls);
        Assert.Equal(0, remoteConnection.SyncLoadResponseCalls);
        var token = Assert.Single(remoteConnection.AsyncCancellationTokens);
        Assert.Equal(cancellationToken, token);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task DestroyDataSetDryRunTest(bool completeAsyncResponses)
    {
        var remoteConnection = CreateProcessCall();
        remoteConnection.CompleteAsyncResponses = completeAsyncResponses;
        const string dryRunOutput = "would destroy tank/myds/child1";
        remoteConnection.CommandResponses["/sbin/zfs destroy -r -nvp tank/myds"] = (dryRunOutput, null);
        var ds = GetDatasets(remoteConnection);
        var cancellationToken = TestContext.Current.CancellationToken;
        var flags = DatasetDestroyFlags.Recursive | DatasetDestroyFlags.DryRun;
        var destroyArgs = new DatasetDestroyArgs { Dataset = "tank/myds", DestroyFlags = flags };

        var response = await ds.DestroyAsync(destroyArgs, cancellationToken);

        Assert.Equal(flags, response.Flags);
        Assert.Equal(dryRunOutput, response.DryRun);
        var command = Assert.Single(remoteConnection.GetCommandsInvoked());
        Assert.Equal("/sbin/zfs destroy -r -nvp tank/myds", command);
        Assert.Equal(1, remoteConnection.AsyncLoadResponseCalls);
        Assert.Equal(0, remoteConnection.SyncLoadResponseCalls);
        var token = Assert.Single(remoteConnection.AsyncCancellationTokens);
        Assert.Equal(cancellationToken, token);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task DestroyDataSetWithCancelledTokenDoesNotRunCommandTest(bool completeAsyncResponses)
    {
        var remoteConnection = CreateProcessCall();
        remoteConnection.CompleteAsyncResponses = completeAsyncResponses;
        var ds = GetDatasets(remoteConnection);
        var destroyArgs = new DatasetDestroyArgs { Dataset = "tank/myds", DestroyFlags = DatasetDestroyFlags.Recursive };
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => ds.DestroyAsync(destroyArgs, cancellationTokenSource.Token));

        Assert.Empty(remoteConnection.GetCommandsInvoked());
        Assert.Equal(0, remoteConnection.AsyncLoadResponseCalls);
        Assert.Equal(0, remoteConnection.SyncLoadResponseCalls);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task PromoteDatasetTest(bool completeAsyncResponses)
    {
        var remoteConnection = CreateProcessCall();
        remoteConnection.CompleteAsyncResponses = completeAsyncResponses;
        var ds = GetDatasets(remoteConnection);
        var cancellationToken = TestContext.Current.CancellationToken;

        await ds.PromoteAsync(new PromoteArgs { Name = "tank/myds" }, cancellationToken);

        var command = Assert.Single(remoteConnection.GetCommandsInvoked());
        Assert.Equal("/sbin/zfs promote tank/myds", command);
        Assert.Equal(1, remoteConnection.AsyncLoadResponseCalls);
        Assert.Equal(0, remoteConnection.SyncLoadResponseCalls);
        var token = Assert.Single(remoteConnection.AsyncCancellationTokens);
        Assert.Equal(cancellationToken, token);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task PromoteDatasetWithCancelledTokenDoesNotRunCommandTest(bool completeAsyncResponses)
    {
        var remoteConnection = CreateProcessCall();
        remoteConnection.CompleteAsyncResponses = completeAsyncResponses;
        var ds = GetDatasets(remoteConnection);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => ds.PromoteAsync(new PromoteArgs { Name = "tank/myds" }, cancellationTokenSource.Token));

        Assert.Empty(remoteConnection.GetCommandsInvoked());
        Assert.Equal(0, remoteConnection.AsyncLoadResponseCalls);
        Assert.Equal(0, remoteConnection.SyncLoadResponseCalls);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task MountDatasetTest(bool completeAsyncResponses)
    {
        var remoteConnection = CreateProcessCall();
        remoteConnection.CompleteAsyncResponses = completeAsyncResponses;
        var ds = GetDatasets(remoteConnection);
        var cancellationToken = TestContext.Current.CancellationToken;

        await ds.MountAsync(new MountArgs { Filesystem = "tank/myds" }, cancellationToken);

        var command = Assert.Single(remoteConnection.GetCommandsInvoked());
        Assert.Equal("/sbin/zfs mount tank/myds", command);
        Assert.Equal(1, remoteConnection.AsyncLoadResponseCalls);
        Assert.Equal(0, remoteConnection.SyncLoadResponseCalls);
        var token = Assert.Single(remoteConnection.AsyncCancellationTokens);
        Assert.Equal(cancellationToken, token);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task MountDatasetWithCancelledTokenDoesNotRunCommandTest(bool completeAsyncResponses)
    {
        var remoteConnection = CreateProcessCall();
        remoteConnection.CompleteAsyncResponses = completeAsyncResponses;
        var ds = GetDatasets(remoteConnection);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => ds.MountAsync(new MountArgs { Filesystem = "tank/myds" }, cancellationTokenSource.Token));

        Assert.Empty(remoteConnection.GetCommandsInvoked());
        Assert.Equal(0, remoteConnection.AsyncLoadResponseCalls);
        Assert.Equal(0, remoteConnection.SyncLoadResponseCalls);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task UnmountDatasetTest(bool completeAsyncResponses)
    {
        var remoteConnection = CreateProcessCall();
        remoteConnection.CompleteAsyncResponses = completeAsyncResponses;
        var ds = GetDatasets(remoteConnection);
        var cancellationToken = TestContext.Current.CancellationToken;

        await ds.UnmountAsync(new UnmountArgs { Filesystem = "tank/myds" }, cancellationToken);

        var command = Assert.Single(remoteConnection.GetCommandsInvoked());
        Assert.Equal("/sbin/zfs unmount tank/myds", command);
        Assert.Equal(1, remoteConnection.AsyncLoadResponseCalls);
        Assert.Equal(0, remoteConnection.SyncLoadResponseCalls);
        var token = Assert.Single(remoteConnection.AsyncCancellationTokens);
        Assert.Equal(cancellationToken, token);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task UnmountDatasetWithCancelledTokenDoesNotRunCommandTest(bool completeAsyncResponses)
    {
        var remoteConnection = CreateProcessCall();
        remoteConnection.CompleteAsyncResponses = completeAsyncResponses;
        var ds = GetDatasets(remoteConnection);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => ds.UnmountAsync(new UnmountArgs { Filesystem = "tank/myds" }, cancellationTokenSource.Token));

        Assert.Empty(remoteConnection.GetCommandsInvoked());
        Assert.Equal(0, remoteConnection.AsyncLoadResponseCalls);
        Assert.Equal(0, remoteConnection.SyncLoadResponseCalls);
    }
}