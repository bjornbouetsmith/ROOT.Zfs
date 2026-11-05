using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.Serialization;

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
    public async Task GetVersionInfoAsyncHandlesAsynchronouslyCompletedCommand()
    {
        _remoteProcessCall.CompleteAsyncResponses = true;
        var zfs = GetZfs();

        var info = await zfs.GetVersionInfoAsync();

        Assert.IsNotNull(info);
        Assert.IsNotEmpty(info.Lines);
        Assert.AreEqual(1, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.AreEqual(0, _remoteProcessCall.SyncLoadResponseCalls);
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
    public void ListDisksTest()
    {
        var zfs = GetZfs();
        var disks = zfs.ListDisks().ToList();
        Assert.IsNotNull(disks);
        Assert.IsNotEmpty(disks);
        Console.WriteLine(disks.Dump(new JsonFormatter()));
    }

}