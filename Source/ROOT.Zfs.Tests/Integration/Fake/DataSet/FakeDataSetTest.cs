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

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void CreateDateSetTest()
    {
        var ds = GetDatasets();
        var args = new DatasetCreationArgs
        {
            DatasetName = "tank/myds",
            Type = DatasetTypes.Filesystem,
            Properties = [new PropertyValue { Property = "atime", Value = "off" }]

        };
        var dataset = ds.Create(args);
        Assert.NotNull(dataset);
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void CreateDateSetInvalidTest()
    {
        var ds = GetDatasets();
        var args = new DatasetCreationArgs
        {
            DatasetName = "tank/myds",
            Type = DatasetTypes.Bookmark,
            Properties = [new PropertyValue { Property = "atime", Value = "off" }]

        };
        Assert.Throws<ArgumentException>(() => ds.Create(args));
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void DestroyDataSetTest()
    {
        var ds = GetDatasets();
        var destroyArgs = new DatasetDestroyArgs { Dataset = "tank/myds", DestroyFlags = DatasetDestroyFlags.Recursive };
        var response = ds.Destroy(destroyArgs);
        Assert.Equal(DatasetDestroyFlags.Recursive, response.Flags);
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void PromoteDatasetTest()
    {
        var processCall = CreateProcessCall();
        var ds = GetDatasets(processCall);
        ds.Promote(new PromoteArgs { Name = "tank/myds" });
        var commands = processCall.GetCommandsInvoked();
        var command = Assert.Single(commands);
        Assert.Equal("/sbin/zfs promote tank/myds", command);
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void MountDatasetTest()
    {
        var processCall = CreateProcessCall();
        var ds = GetDatasets(processCall);
        ds.Mount(new MountArgs { Filesystem = "tank/myds" });
        var commands = processCall.GetCommandsInvoked();
        var command = Assert.Single(commands);
        Assert.Equal("/sbin/zfs mount tank/myds", command);
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void UnmountDatasetTest()
    {
        var processCall = CreateProcessCall();
        var ds = GetDatasets(processCall);
        ds.Unmount(new UnmountArgs { Filesystem = "tank/myds" });
        var commands = processCall.GetCommandsInvoked();
        var command = Assert.Single(commands);
        Assert.Equal("/sbin/zfs unmount tank/myds", command);
    }
}