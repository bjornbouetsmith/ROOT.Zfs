using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Integration.Fake;

[TestClass]
public class FakeZfsTest
{
    readonly FakeRemoteConnection _remoteProcessCall = new ("2.1.5-2");

    private Core.Zfs GetZfs()
    {
        return ZfsTestFactory.CreateZfs(_remoteProcessCall);
    }

    [TestMethod,TestCategory("FakeIntegration")]
    public async Task GetVersionInfoAsyncTest()
    {
        var zfs = GetZfs();
        var info = await zfs.GetVersionInfoAsync();

        Assert.IsNotNull(info);
        Assert.IsNotEmpty(info.Lines);
        Assert.IsTrue(info.Lines.Any(line => line.Contains("zfs-2.1.5-2", StringComparison.Ordinal)));
        CollectionAssert.Contains(_remoteProcessCall.GetCommandsInvoked(), "/sbin/zfs --version");
        Assert.AreEqual(1, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.AreEqual(0, _remoteProcessCall.SyncLoadResponseCalls);

        foreach (var line in info.Lines)
        {
            Console.WriteLine(line);
        }
    }

    [TestMethod, TestCategory("FakeIntegration")]
    public async Task GetVersionInfoAsyncForwardsCancellationToken()
    {
        var zfs = GetZfs();
        using var cancellationTokenSource = new CancellationTokenSource();

        await zfs.GetVersionInfoAsync(cancellationTokenSource.Token);

        Assert.AreEqual(1, _remoteProcessCall.AsyncCancellationTokens.Count);
        Assert.AreEqual(cancellationTokenSource.Token, _remoteProcessCall.AsyncCancellationTokens[0]);
    }

    [TestMethod, TestCategory("FakeIntegration")]
    public async Task GetVersionInfoAsyncWithCancelledTokenDoesNotRunCommand()
    {
        var zfs = GetZfs();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => zfs.GetVersionInfoAsync(cancellationTokenSource.Token));

        Assert.IsEmpty(_remoteProcessCall.GetCommandsInvoked());
        Assert.AreEqual(0, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.AreEqual(0, _remoteProcessCall.SyncLoadResponseCalls);
    }
        
    [TestMethod, TestCategory("FakeIntegration")]
    public async Task ListDisksAsyncTest()
    {
        var zfs = GetZfs();

        var disks = (await zfs.ListDisksAsync()).ToList();

        Assert.IsNotNull(disks);
        Assert.HasCount(6, disks);
        Assert.AreEqual(2, disks.Count(d => d.Type == DeviceType.Disk));
        Assert.AreEqual(4, disks.Count(d => d.Type == DeviceType.Partition));
        CollectionAssert.Contains(_remoteProcessCall.GetCommandsInvoked(), "/bin/lsblk --include 8 --include 259 -p | /bin/grep disk");
        CollectionAssert.Contains(_remoteProcessCall.GetCommandsInvoked(), "/bin/ls -l /dev/disk/by-id/ | /bin/awk -F ' ' '{print $9,$11}'");
        Assert.AreEqual(2, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.AreEqual(0, _remoteProcessCall.SyncLoadResponseCalls);

        Console.WriteLine(disks.Dump(new JsonFormatter()));
    }

    [TestMethod, TestCategory("FakeIntegration")]
    public async Task ListDisksAsyncReturnsMaterializedList()
    {
        var zfs = GetZfs();

        var disks = await zfs.ListDisksAsync();

        // Both commands must have completed before the returned enumerable is enumerated.
        Assert.HasCount(2, _remoteProcessCall.GetCommandsInvoked());
        Assert.HasCount(6, disks);
        Assert.HasCount(2, _remoteProcessCall.GetCommandsInvoked());
    }

    [TestMethod, TestCategory("FakeIntegration")]
    public async Task ListDisksAsyncForwardsCancellationToken()
    {
        var zfs = GetZfs();
        using var cancellationTokenSource = new CancellationTokenSource();

        await zfs.ListDisksAsync(cancellationTokenSource.Token);

        Assert.HasCount(2, _remoteProcessCall.AsyncCancellationTokens);
        Assert.AreEqual(cancellationTokenSource.Token, _remoteProcessCall.AsyncCancellationTokens[0]);
        Assert.AreEqual(cancellationTokenSource.Token, _remoteProcessCall.AsyncCancellationTokens[1]);
    }

    [TestMethod, TestCategory("FakeIntegration")]
    public async Task ListDisksAsyncWithCancelledTokenDoesNotRunCommand()
    {
        var zfs = GetZfs();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => zfs.ListDisksAsync(cancellationTokenSource.Token));

        Assert.IsEmpty(_remoteProcessCall.GetCommandsInvoked());
        Assert.AreEqual(0, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.AreEqual(0, _remoteProcessCall.SyncLoadResponseCalls);
    }

    [TestMethod, TestCategory("FakeIntegration")]
    public async Task ListDisksAsyncPropagatesCommandFailure()
    {
        _remoteProcessCall.StdError = "lsblk failed";
        var zfs = GetZfs();

        await Assert.ThrowsExactlyAsync<ProcessCallException>(() => zfs.ListDisksAsync());

        Assert.AreEqual(1, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.AreEqual(0, _remoteProcessCall.SyncLoadResponseCalls);
    }

}