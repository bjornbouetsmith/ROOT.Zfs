using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;
using ROOT.Zfs.Public.Arguments.Pool;
using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Tests.Integration.Fake;

public class FakeZpoolTest
{
    private const string HistoryCommand = "/sbin/zpool history -l tank";
    private const string StatusCommand = "/sbin/zpool status -vP tank";
    private const string ListCommand = "/sbin/zpool list -PHp";
    private const string ListSingleCommand = "/sbin/zpool list -PHp tank2";
    private const string ZdbCommand = "/sbin/zdb";

    private readonly ITestOutputHelper _output;

    public FakeZpoolTest(ITestOutputHelper output)
    {
        _output = output;
    }

    private readonly FakeRemoteConnection _remoteProcessCall = new("2.1.5-2");

    private ZPool GetZpool()
    {
        return ZfsTestFactory.CreateZpool(_remoteProcessCall);
    }

    private void AssertAsyncCalls(int expectedCalls, CancellationToken cancellationToken)
    {
        Assert.Equal(expectedCalls, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
        Assert.All(_remoteProcessCall.AsyncCancellationTokens, token => Assert.Equal(cancellationToken, token));
    }

    private void AssertNoCalls()
    {
        Assert.Empty(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal(0, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task HistoryAsyncTest(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var cancellationToken = TestContext.Current.CancellationToken;
        var zp = GetZpool();
        var args = new PoolHistoryArgs { PoolName = "tank" };

        var history = (await zp.HistoryAsync(args, cancellationToken)).ToList();

        Assert.NotEmpty(history);
        _output.WriteLine(history.Dump(new JsonFormatter()));
        var command = Assert.Single(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal(HistoryCommand, command);
        AssertAsyncCalls(1, cancellationToken);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task HistoryAsyncWithSkipTest(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var cancellationToken = TestContext.Current.CancellationToken;
        var zp = GetZpool();
        var args = new PoolHistoryArgs { PoolName = "tank" };
        var lines = (await zp.HistoryAsync(args, cancellationToken)).Count();

        args = new PoolHistoryArgs { PoolName = "tank", SkipLines = lines - 2 };
        var history = (await zp.HistoryAsync(args, cancellationToken)).ToList();

        Assert.Equal(2, history.Count);
        Assert.All(_remoteProcessCall.GetCommandsInvoked(), command => Assert.Equal(HistoryCommand, command));
        AssertAsyncCalls(2, cancellationToken);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task HistoryAsyncAfterDateTest(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var cancellationToken = TestContext.Current.CancellationToken;
        var zp = GetZpool();
        var args = new PoolHistoryArgs { PoolName = "tank" };
        var last10AtMost = (await zp.HistoryAsync(args, cancellationToken)).TakeLast(10).ToList();

        // This is safe, since zfs always have at last pool creation as a history event
        var history = last10AtMost.First();
        args = new PoolHistoryArgs { PoolName = "tank", AfterDate = history.Time };
        var afterDate = (await zp.HistoryAsync(args, cancellationToken)).ToList();

        if (last10AtMost.Count > 1)
        {
            Assert.True(afterDate.Count < last10AtMost.Count);
        }

        AssertAsyncCalls(2, cancellationToken);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task HistoryAsyncWithCancelledTokenDoesNotRunCommand(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var zp = GetZpool();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => zp.HistoryAsync(new PoolHistoryArgs { PoolName = "tank" }, cancellationTokenSource.Token));

        AssertNoCalls();
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task HistoryAsyncPropagatesCommandFailure(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        _remoteProcessCall.CommandResponses[HistoryCommand] = (null, "cannot open 'tank': no such pool");
        var zp = GetZpool();

        await Assert.ThrowsAsync<ProcessCallException>(() => zp.HistoryAsync(new PoolHistoryArgs { PoolName = "tank" }, TestContext.Current.CancellationToken));

        Assert.Equal(1, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task StatusAsyncTest(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var cancellationToken = TestContext.Current.CancellationToken;
        var zp = GetZpool();

        var status = await zp.StatusAsync(new PoolStatusArgs { PoolName = "tank" }, cancellationToken);

        Assert.NotNull(status);
        Assert.Equal(State.Online, status.State);
        Assert.Equal("tank", status.Pool.PoolName);
        var command = Assert.Single(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal(StatusCommand, command);
        AssertAsyncCalls(1, cancellationToken);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task StatusAsyncWithCancelledTokenDoesNotRunCommand(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var zp = GetZpool();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => zp.StatusAsync(new PoolStatusArgs { PoolName = "tank" }, cancellationTokenSource.Token));

        AssertNoCalls();
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task StatusAsyncPropagatesCommandFailure(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        _remoteProcessCall.CommandResponses[StatusCommand] = (null, "cannot open 'tank': no such pool");
        var zp = GetZpool();

        await Assert.ThrowsAsync<ProcessCallException>(() => zp.StatusAsync(new PoolStatusArgs { PoolName = "tank" }, TestContext.Current.CancellationToken));

        Assert.Equal(1, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task ListAsyncTest(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var cancellationToken = TestContext.Current.CancellationToken;
        var zp = GetZpool();

        var infos = await zp.ListAsync(cancellationToken);

        Assert.Equal(2, infos.Count);
        Assert.All(infos, info => Assert.Equal(5000, info.Version));
        var commands = _remoteProcessCall.GetCommandsInvoked();
        Assert.Equal(2, commands.Count);
        Assert.Equal(ListCommand, commands[0]);
        Assert.Equal(ZdbCommand, commands[1]);
        AssertAsyncCalls(2, cancellationToken);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task ListAsyncWithCancelledTokenDoesNotRunCommand(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var zp = GetZpool();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => zp.ListAsync(cancellationTokenSource.Token));

        AssertNoCalls();
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task ListAsyncPropagatesCommandFailure(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        _remoteProcessCall.CommandResponses[ListCommand] = (null, "cannot list pools");
        var zp = GetZpool();

        await Assert.ThrowsAsync<ProcessCallException>(() => zp.ListAsync(TestContext.Current.CancellationToken));

        Assert.Equal(1, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task ListSinglePoolAsyncTest(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var cancellationToken = TestContext.Current.CancellationToken;
        var zp = GetZpool();

        var info = await zp.ListAsync(new PoolListArgs { PoolName = "tank2" }, cancellationToken);

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

        var commands = _remoteProcessCall.GetCommandsInvoked();
        Assert.Equal(2, commands.Count);
        Assert.Equal(ListSingleCommand, commands[0]);
        Assert.Equal(ZdbCommand, commands[1]);
        AssertAsyncCalls(2, cancellationToken);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task ListSinglePoolAsyncWithCancelledTokenDoesNotRunCommand(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var zp = GetZpool();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => zp.ListAsync(new PoolListArgs { PoolName = "tank2" }, cancellationTokenSource.Token));

        AssertNoCalls();
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task CreateAsyncTest(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var cancellationToken = TestContext.Current.CancellationToken;
        var zp = GetZpool();

        var pool = await zp.CreateAsync(new PoolCreateArgs
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
        }, cancellationToken);

        Assert.NotNull(pool);
        Assert.Equal("mytest", pool.Pool.PoolName);
        var commands = _remoteProcessCall.GetCommandsInvoked();
        Assert.Equal(2, commands.Count);
        Assert.Equal("/sbin/zpool create mytest mirror /dev/sda /dev/sdb", commands[0]);
        Assert.Equal("/sbin/zpool status -vP mytest", commands[1]);
        AssertAsyncCalls(2, cancellationToken);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task CreateDraidAsyncTest(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var cancellationToken = TestContext.Current.CancellationToken;
        var zp = GetZpool();

        await zp.CreateAsync(new PoolCreateArgs
        {
            PoolName = "mytest",
            VDevs =
            [
                new DraigVDevCreationArgs
                {
                    Type = VDevCreationType.DRaid1,
                    Devices = ["/dev/sda", "/dev/sdb", "/dev/sdc", "/dev/sdd"],
                    DraidArgs = new DraidArgs
                    {
                        Spares = 1,
                        DataDevices = 2,
                        Children = 4
                    }
                }
            ]
        }, cancellationToken);

        var commands = _remoteProcessCall.GetCommandsInvoked();
        Assert.Equal(2, commands.Count);
        Assert.Equal("/sbin/zpool create mytest draid1:2d:4c:1s /dev/sda /dev/sdb /dev/sdc /dev/sdd", commands[0]);
        Assert.Equal("/sbin/zpool status -vP mytest", commands[1]);
        AssertAsyncCalls(2, cancellationToken);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task CreateAsyncWithCancelledTokenDoesNotRunCommand(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var zp = GetZpool();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => zp.CreateAsync(new PoolCreateArgs { PoolName = "mytest" }, cancellationTokenSource.Token));

        AssertNoCalls();
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task DestroyAsyncTest(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var cancellationToken = TestContext.Current.CancellationToken;
        var zp = GetZpool();

        await zp.DestroyAsync(new PoolDestroyArgs { PoolName = "mytest" }, cancellationToken);

        var command = Assert.Single(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal("/sbin/zpool destroy -f mytest", command);
        AssertAsyncCalls(1, cancellationToken);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task DestroyAsyncWithCancelledTokenDoesNotRunCommand(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var zp = GetZpool();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => zp.DestroyAsync(new PoolDestroyArgs { PoolName = "mytest" }, cancellationTokenSource.Token));

        AssertNoCalls();
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task OfflineAsyncTest(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var cancellationToken = TestContext.Current.CancellationToken;
        var zp = GetZpool();

        await zp.OfflineAsync(new PoolOfflineArgs { PoolName = "tank", Device = "/dev/sda" }, cancellationToken);

        var command = Assert.Single(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal("/sbin/zpool offline tank /dev/sda", command);
        AssertAsyncCalls(1, cancellationToken);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task OfflineAsyncWithCancelledTokenDoesNotRunCommand(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var zp = GetZpool();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => zp.OfflineAsync(new PoolOfflineArgs { PoolName = "tank", Device = "/dev/sda" }, cancellationTokenSource.Token));

        AssertNoCalls();
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task OnlineAsyncTest(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var cancellationToken = TestContext.Current.CancellationToken;
        var zp = GetZpool();

        await zp.OnlineAsync(new PoolOnlineArgs { PoolName = "tank", Device = "/dev/sda" }, cancellationToken);

        var command = Assert.Single(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal("/sbin/zpool online tank /dev/sda", command);
        AssertAsyncCalls(1, cancellationToken);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task OnlineAsyncWithCancelledTokenDoesNotRunCommand(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var zp = GetZpool();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => zp.OnlineAsync(new PoolOnlineArgs { PoolName = "tank", Device = "/dev/sda" }, cancellationTokenSource.Token));

        AssertNoCalls();
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task ClearAsyncTest(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var cancellationToken = TestContext.Current.CancellationToken;
        var zp = GetZpool();

        await zp.ClearAsync(new PoolClearArgs { PoolName = "tank", Device = "/dev/sda" }, cancellationToken);

        var command = Assert.Single(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal("/sbin/zpool clear tank /dev/sda", command);
        AssertAsyncCalls(1, cancellationToken);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task ClearAsyncWithCancelledTokenDoesNotRunCommand(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var zp = GetZpool();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => zp.ClearAsync(new PoolClearArgs { PoolName = "tank", Device = "/dev/sda" }, cancellationTokenSource.Token));

        AssertNoCalls();
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task IOStatsAsyncTest(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var cancellationToken = TestContext.Current.CancellationToken;
        var zp = GetZpool();

        var stats = await zp.IOStatsAsync(new PoolIOStatsArgs { PoolName = "tank", Devices = ["/dev/sda"] }, cancellationToken);

        Assert.NotNull(stats);
        _output.WriteLine(stats.Dump(new JsonFormatter()));
        var command = Assert.Single(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal("/sbin/zpool iostat -LlpPvH tank /dev/sda", command);
        AssertAsyncCalls(1, cancellationToken);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task IOStatsAsyncWithCancelledTokenDoesNotRunCommand(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var zp = GetZpool();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => zp.IOStatsAsync(new PoolIOStatsArgs { PoolName = "tank", Devices = ["/dev/sda"] }, cancellationTokenSource.Token));

        AssertNoCalls();
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task ResilverAsyncTest(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var cancellationToken = TestContext.Current.CancellationToken;
        var zp = GetZpool();

        await zp.ResilverAsync(new PoolResilverArgs { PoolName = "tank" }, cancellationToken);

        var command = Assert.Single(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal("/sbin/zpool resilver tank", command);
        AssertAsyncCalls(1, cancellationToken);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task ResilverAsyncWithCancelledTokenDoesNotRunCommand(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var zp = GetZpool();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => zp.ResilverAsync(new PoolResilverArgs { PoolName = "tank" }, cancellationTokenSource.Token));

        AssertNoCalls();
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task ScrubAsyncTest(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var cancellationToken = TestContext.Current.CancellationToken;
        var zp = GetZpool();

        await zp.ScrubAsync(new PoolScrubArgs { PoolName = "tank" }, cancellationToken);

        var command = Assert.Single(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal("/sbin/zpool scrub tank", command);
        AssertAsyncCalls(1, cancellationToken);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task ScrubAsyncWithCancelledTokenDoesNotRunCommand(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var zp = GetZpool();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => zp.ScrubAsync(new PoolScrubArgs { PoolName = "tank" }, cancellationTokenSource.Token));

        AssertNoCalls();
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task TrimAsyncTest(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var cancellationToken = TestContext.Current.CancellationToken;
        var zp = GetZpool();

        await zp.TrimAsync(new PoolTrimArgs { PoolName = "tank" }, cancellationToken);

        var command = Assert.Single(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal("/sbin/zpool trim tank", command);
        AssertAsyncCalls(1, cancellationToken);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task TrimAsyncWithCancelledTokenDoesNotRunCommand(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var zp = GetZpool();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => zp.TrimAsync(new PoolTrimArgs { PoolName = "tank" }, cancellationTokenSource.Token));

        AssertNoCalls();
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task GetUpgradeablePoolsAsyncTest(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var cancellationToken = TestContext.Current.CancellationToken;
        var zp = GetZpool();

        var pools = await zp.GetUpgradeablePoolsAsync(cancellationToken);

        Assert.Single(pools);
        var command = Assert.Single(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal("/sbin/zpool upgrade", command);
        AssertAsyncCalls(1, cancellationToken);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task GetUpgradeablePoolsAsyncWithCancelledTokenDoesNotRunCommand(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var zp = GetZpool();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => zp.GetUpgradeablePoolsAsync(cancellationTokenSource.Token));

        AssertNoCalls();
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task UpgradeAsyncTest(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var cancellationToken = TestContext.Current.CancellationToken;
        var zp = GetZpool();

        await zp.UpgradeAsync(new PoolUpgradeArgs { PoolName = "tank" }, cancellationToken);

        var command = Assert.Single(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal("/sbin/zpool upgrade tank", command);
        AssertAsyncCalls(1, cancellationToken);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task UpgradeAsyncWithCancelledTokenDoesNotRunCommand(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var zp = GetZpool();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => zp.UpgradeAsync(new PoolUpgradeArgs { PoolName = "tank" }, cancellationTokenSource.Token));

        AssertNoCalls();
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task DetachAsyncTest(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var cancellationToken = TestContext.Current.CancellationToken;
        var zp = GetZpool();

        await zp.DetachAsync(new PoolDetachArgs { PoolName = "tank", Device = "/dev/sdb" }, cancellationToken);

        var command = Assert.Single(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal("/sbin/zpool detach tank /dev/sdb", command);
        AssertAsyncCalls(1, cancellationToken);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task DetachAsyncWithCancelledTokenDoesNotRunCommand(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var zp = GetZpool();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => zp.DetachAsync(new PoolDetachArgs { PoolName = "tank", Device = "/dev/sdb" }, cancellationTokenSource.Token));

        AssertNoCalls();
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task AttachAsyncTest(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var cancellationToken = TestContext.Current.CancellationToken;
        var zp = GetZpool();

        await zp.AttachAsync(new PoolAttachArgs
        {
            PoolName = "tank",
            OldDevice = "/dev/sdb",
            NewDevice = "/dev/sdc"
        }, cancellationToken);

        var command = Assert.Single(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal("/sbin/zpool attach tank /dev/sdb /dev/sdc", command);
        AssertAsyncCalls(1, cancellationToken);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task AttachAsyncWithCancelledTokenDoesNotRunCommand(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var zp = GetZpool();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => zp.AttachAsync(new PoolAttachArgs { PoolName = "tank", OldDevice = "/dev/sdb", NewDevice = "/dev/sdc" }, cancellationTokenSource.Token));

        AssertNoCalls();
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task ReplaceAsyncTest(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var cancellationToken = TestContext.Current.CancellationToken;
        var zp = GetZpool();

        await zp.ReplaceAsync(new PoolReplaceArgs
        {
            PoolName = "tank",
            OldDevice = "/dev/sdb",
            NewDevice = "/dev/sdc",
        }, cancellationToken);

        var command = Assert.Single(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal("/sbin/zpool replace tank /dev/sdb /dev/sdc", command);
        AssertAsyncCalls(1, cancellationToken);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task ReplaceAsyncWithCancelledTokenDoesNotRunCommand(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var zp = GetZpool();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => zp.ReplaceAsync(new PoolReplaceArgs { PoolName = "tank", OldDevice = "/dev/sdb", NewDevice = "/dev/sdc" }, cancellationTokenSource.Token));

        AssertNoCalls();
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task AddAsyncTest(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var cancellationToken = TestContext.Current.CancellationToken;
        var zp = GetZpool();

        await zp.AddAsync(new PoolAddArgs
        {
            PoolName = "tank",
            Force = true,
            VDevs = [new VDevCreationArgs { Type = VDevCreationType.Mirror, Devices = ["/dev/sda", "/dev/sdb"] }],
        }, cancellationToken);

        var command = Assert.Single(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal("/sbin/zpool add -f tank mirror /dev/sda /dev/sdb", command);
        AssertAsyncCalls(1, cancellationToken);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task AddAsyncWithCancelledTokenDoesNotRunCommand(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var zp = GetZpool();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => zp.AddAsync(new PoolAddArgs { PoolName = "tank", VDevs = [new VDevCreationArgs { Type = VDevCreationType.Mirror, Devices = ["/dev/sda", "/dev/sdb"] }] }, cancellationTokenSource.Token));

        AssertNoCalls();
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task RemoveAsyncTest(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var cancellationToken = TestContext.Current.CancellationToken;
        var zp = GetZpool();
        var removeArgs = new PoolRemoveArgs
        {
            PoolName = "tank",
            VDev = "mirror-1"
        };

        await zp.RemoveAsync(removeArgs, cancellationToken);

        var command = Assert.Single(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal("/sbin/zpool remove tank mirror-1", command);
        AssertAsyncCalls(1, cancellationToken);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task RemoveAsyncWithCancelledTokenDoesNotRunCommand(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var zp = GetZpool();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => zp.RemoveAsync(new PoolRemoveArgs { PoolName = "tank", VDev = "mirror-1" }, cancellationTokenSource.Token));

        AssertNoCalls();
    }
}
