using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Integration.Fake;

[TestClass]
public class SmartInfoTest
{
    public TestContext TestContext { get; set; }
    readonly FakeRemoteConnection _remoteProcessCall = new("2.1.5-2");

    private Core.Zfs GetZfs()
    {
        return ZfsTestFactory.CreateZfs(_remoteProcessCall);
    }

    [TestMethod, TestCategory("FakeIntegration")]
    [DataRow(true), DataRow(false)]
    public async Task GetSmartInfosAsyncTest(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var zfs = GetZfs();
        var infos = await zfs.GetSmartInfosAsync(TestContext.CancellationToken);

        Assert.IsNotNull(infos);
        Assert.HasCount(2, infos);
        Assert.Contains("/sbin/smartctl -x /dev/disk/by-id/scsi-0QEMU_QEMU_HARDDISK_drive-scsi5", _remoteProcessCall.GetCommandsInvoked());
        Assert.Contains("/sbin/smartctl -x /dev/disk/by-id/scsi-0QEMU_QEMU_HARDDISK_drive-scsi6", _remoteProcessCall.GetCommandsInvoked());
        Assert.AreEqual(4, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.AreEqual(0, _remoteProcessCall.SyncLoadResponseCalls);
    }

    [TestMethod, TestCategory("FakeIntegration")]
    [DataRow(true), DataRow(false)]
    public async Task GetSmartInfosAsyncReturnsMaterializedList(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var zfs = GetZfs();

        var infos = await zfs.GetSmartInfosAsync(TestContext.CancellationToken);

        // All commands, including the smartctl calls, must have completed before the returned read-only list is returned.
        Assert.HasCount(4, _remoteProcessCall.GetCommandsInvoked());
        Assert.HasCount(2, infos);
        Assert.HasCount(4, _remoteProcessCall.GetCommandsInvoked());
    }

    [TestMethod, TestCategory("FakeIntegration")]
    [DataRow(true), DataRow(false)]
    public async Task GetSmartInfosAsyncForwardsCancellationToken(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var zfs = GetZfs();

        await zfs.GetSmartInfosAsync(TestContext.CancellationToken);

        Assert.HasCount(4, _remoteProcessCall.AsyncCancellationTokens);
        Assert.AreEqual(TestContext.CancellationToken, _remoteProcessCall.AsyncCancellationTokens[0]);
        Assert.AreEqual(TestContext.CancellationToken, _remoteProcessCall.AsyncCancellationTokens[1]);
        Assert.AreEqual(TestContext.CancellationToken, _remoteProcessCall.AsyncCancellationTokens[2]);
        Assert.AreEqual(TestContext.CancellationToken, _remoteProcessCall.AsyncCancellationTokens[3]);
    }

    [TestMethod, TestCategory("FakeIntegration")]
    [DataRow(true), DataRow(false)]
    public async Task GetSmartInfosAsyncWithCancelledTokenDoesNotRunCommand(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var zfs = GetZfs();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => zfs.GetSmartInfosAsync(cancellationTokenSource.Token));

        Assert.IsEmpty(_remoteProcessCall.GetCommandsInvoked());
        Assert.AreEqual(0, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.AreEqual(0, _remoteProcessCall.SyncLoadResponseCalls);
    }

    [TestMethod, TestCategory("FakeIntegration")]
    [DataRow(true), DataRow(false)]
    public async Task GetSmartInfosAsyncPropagatesCommandFailure(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        _remoteProcessCall.StdError = "command failed";
        var zfs = GetZfs();

        await Assert.ThrowsExactlyAsync<ProcessCallException>(() => zfs.GetSmartInfosAsync(TestContext.CancellationToken));

        Assert.AreEqual(1, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.AreEqual(0, _remoteProcessCall.SyncLoadResponseCalls);
    }

    [TestMethod, TestCategory("FakeIntegration")]
    [DataRow(true), DataRow(false)]
    public async Task GetSmartInfoAsyncTest(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var zfs = GetZfs();
        var disks = await zfs.ListDisksAsync(TestContext.CancellationToken);

        foreach (var disk in disks.Where(d => d.Type == DeviceType.Disk))
        {
            var info = await zfs.GetSmartInfoAsync(disk.Id, TestContext.CancellationToken);
            Assert.AreEqual(disk.Id, info.DeviceId);
            Assert.Contains($"/sbin/smartctl -x {disk.Id}", _remoteProcessCall.GetCommandsInvoked());
            if (info.DeviceId.EndsWith('5'))
            {
                // FAKE returns failed status
                Assert.AreNotEqual("PASSED", info.DataSection.Status);
            }
            else
            {
                Assert.AreEqual("PASSED", info.DataSection.Status);
            }
        }

        // Two calls for ListDisksAsync and one smartctl call per disk.
        Assert.AreEqual(4, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.AreEqual(0, _remoteProcessCall.SyncLoadResponseCalls);
    }

    [TestMethod, TestCategory("FakeIntegration")]
    [DataRow(true), DataRow(false)]
    public async Task GetSmartInfoAsyncForwardsCancellationToken(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var zfs = GetZfs();
        const string deviceId = "/dev/disk/by-id/scsi-0QEMU_QEMU_HARDDISK_drive-scsi6";

        await zfs.GetSmartInfoAsync(deviceId, TestContext.CancellationToken);

        Assert.HasCount(1, _remoteProcessCall.AsyncCancellationTokens);
        Assert.AreEqual(TestContext.CancellationToken, _remoteProcessCall.AsyncCancellationTokens[0]);
    }

    [TestMethod, TestCategory("FakeIntegration")]
    [DataRow(true), DataRow(false)]
    public async Task GetSmartInfoAsyncWithCancelledTokenDoesNotRunCommand(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var zfs = GetZfs();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => zfs.GetSmartInfoAsync("/dev/disk/by-id/scsi-0QEMU_QEMU_HARDDISK_drive-scsi6", cancellationTokenSource.Token));

        Assert.IsEmpty(_remoteProcessCall.GetCommandsInvoked());
        Assert.AreEqual(0, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.AreEqual(0, _remoteProcessCall.SyncLoadResponseCalls);
    }

    [TestMethod, TestCategory("FakeIntegration")]
    [DataRow(true), DataRow(false)]
    public async Task GetSmartInfoAsyncPropagatesCommandFailure(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        _remoteProcessCall.StdError = "smartctl failed";
        var zfs = GetZfs();

        await Assert.ThrowsExactlyAsync<ProcessCallException>(() => zfs.GetSmartInfoAsync("/dev/disk/by-id/scsi-0QEMU_QEMU_HARDDISK_drive-scsi6", TestContext.CancellationToken));

        Assert.AreEqual(1, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.AreEqual(0, _remoteProcessCall.SyncLoadResponseCalls);
    }
}
