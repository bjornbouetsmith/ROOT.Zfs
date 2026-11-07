using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Integration.Fake;

public class FakeZfsTest
{
    readonly FakeRemoteConnection _remoteProcessCall = new ("2.1.5-2");

    private Core.Zfs GetZfs()
    {
        return ZfsTestFactory.CreateZfs(_remoteProcessCall);
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public async Task GetVersionInfoAsyncTest()
    {
        var zfs = GetZfs();
        var info = await zfs.GetVersionInfoAsync(TestContext.Current.CancellationToken);

        Assert.NotNull(info);
        Assert.NotEmpty(info.Lines);
        Assert.Contains(info.Lines, line => line.Contains("zfs-2.1.5-2", StringComparison.Ordinal));
        var commandsInvoked = _remoteProcessCall.GetCommandsInvoked();
        Assert.Contains("/sbin/zfs --version", commandsInvoked);
        Assert.Equal(1, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);

        foreach (var line in info.Lines)
        {
            Console.WriteLine(line);
        }
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public async Task GetVersionInfoAsyncForwardsCancellationToken()
    {
        var zfs = GetZfs();

        await zfs.GetVersionInfoAsync(TestContext.Current.CancellationToken);

        Assert.Single(_remoteProcessCall.AsyncCancellationTokens);
        Assert.Equal(TestContext.Current.CancellationToken, _remoteProcessCall.AsyncCancellationTokens[0]);
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public async Task GetVersionInfoAsyncWithCancelledTokenDoesNotRunCommand()
    {
        var zfs = GetZfs();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => zfs.GetVersionInfoAsync(cancellationTokenSource.Token));

        Assert.Empty(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal(0, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
    }
        
    [Fact]
    [Trait("Category", "FakeIntegration")]
    public async Task ListDisksAsyncTest()
    {
        var zfs = GetZfs();

        var disks = (await zfs.ListDisksAsync(TestContext.Current.CancellationToken)).ToList();

        Assert.NotNull(disks);
        Assert.Equal(6, disks.Count());
        Assert.Equal(2, disks.Count(d => d.Type == DeviceType.Disk));
        Assert.Equal(4, disks.Count(d => d.Type == DeviceType.Partition));
        Assert.Contains("/bin/lsblk --include 8 --include 259 -p | /bin/grep disk", _remoteProcessCall.GetCommandsInvoked());
        Assert.Contains("/bin/ls -l /dev/disk/by-id/ | /bin/awk -F ' ' '{print $9,$11}'", _remoteProcessCall.GetCommandsInvoked());
        Assert.Equal(2, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);

        Console.WriteLine(disks.Dump(new JsonFormatter()));
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public async Task ListDisksAsyncReturnsMaterializedList()
    {
        var zfs = GetZfs();

        var disks = await zfs.ListDisksAsync(TestContext.Current.CancellationToken);

        // Both commands must have completed before the returned enumerable is enumerated.
        Assert.Equal(2, _remoteProcessCall.GetCommandsInvoked().Count());
        Assert.Equal(6, disks.Count());
        Assert.Equal(2, _remoteProcessCall.GetCommandsInvoked().Count());
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public async Task ListDisksAsyncForwardsCancellationToken()
    {
        var zfs = GetZfs();

        await zfs.ListDisksAsync(TestContext.Current.CancellationToken);

        Assert.Equal(2, _remoteProcessCall.AsyncCancellationTokens.Count());
        Assert.Equal(TestContext.Current.CancellationToken, _remoteProcessCall.AsyncCancellationTokens[0]);
        Assert.Equal(TestContext.Current.CancellationToken, _remoteProcessCall.AsyncCancellationTokens[1]);
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public async Task ListDisksAsyncWithCancelledTokenDoesNotRunCommand()
    {
        var zfs = GetZfs();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => zfs.ListDisksAsync(cancellationTokenSource.Token));

        Assert.Empty(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal(0, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public async Task ListDisksAsyncPropagatesCommandFailure()
    {
        _remoteProcessCall.StdError = "lsblk failed";
        var zfs = GetZfs();

        await Assert.ThrowsAsync<ProcessCallException>(() => zfs.ListDisksAsync(TestContext.Current.CancellationToken));

        Assert.Equal(1, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
    }

}