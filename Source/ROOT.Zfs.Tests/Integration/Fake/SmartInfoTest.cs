using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Integration.Fake;

public class SmartInfoTest
{
    readonly FakeRemoteConnection _remoteProcessCall = new("2.1.5-2");

    private Core.Zfs GetZfs()
    {
        return ZfsTestFactory.CreateZfs(_remoteProcessCall);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task GetSmartInfosAsyncTest(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var zfs = GetZfs();
        var infos = await zfs.GetSmartInfosAsync(TestContext.Current.CancellationToken);

        Assert.NotNull(infos);
        Assert.Equal(2, infos.Count);
        Assert.Contains("/sbin/smartctl -x /dev/disk/by-id/scsi-0QEMU_QEMU_HARDDISK_drive-scsi5", _remoteProcessCall.GetCommandsInvoked());
        Assert.Contains("/sbin/smartctl -x /dev/disk/by-id/scsi-0QEMU_QEMU_HARDDISK_drive-scsi6", _remoteProcessCall.GetCommandsInvoked());
        Assert.Equal(4, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task GetSmartInfosAsyncReturnsMaterializedList(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var zfs = GetZfs();

        var infos = await zfs.GetSmartInfosAsync(TestContext.Current.CancellationToken);

        // All commands, including the smartctl calls, must have completed before the returned read-only list is returned.
        Assert.Equal(4, _remoteProcessCall.GetCommandsInvoked().Count);
        Assert.Equal(2, infos.Count);
        Assert.Equal(4, _remoteProcessCall.GetCommandsInvoked().Count);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task GetSmartInfosAsyncForwardsCancellationToken(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var zfs = GetZfs();

        await zfs.GetSmartInfosAsync(TestContext.Current.CancellationToken);

        Assert.Equal(4, _remoteProcessCall.AsyncCancellationTokens.Count);
        Assert.Equal(TestContext.Current.CancellationToken, _remoteProcessCall.AsyncCancellationTokens[0]);
        Assert.Equal(TestContext.Current.CancellationToken, _remoteProcessCall.AsyncCancellationTokens[1]);
        Assert.Equal(TestContext.Current.CancellationToken, _remoteProcessCall.AsyncCancellationTokens[2]);
        Assert.Equal(TestContext.Current.CancellationToken, _remoteProcessCall.AsyncCancellationTokens[3]);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task GetSmartInfosAsyncWithCancelledTokenDoesNotRunCommand(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var zfs = GetZfs();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => zfs.GetSmartInfosAsync(cancellationTokenSource.Token));

        Assert.Empty(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal(0, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task GetSmartInfosAsyncPropagatesCommandFailure(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        _remoteProcessCall.StdError = "command failed";
        var zfs = GetZfs();

        await Assert.ThrowsAsync<ProcessCallException>(() => zfs.GetSmartInfosAsync(TestContext.Current.CancellationToken));

        Assert.Equal(1, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task GetSmartInfoAsyncTest(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var zfs = GetZfs();
        var disks = await zfs.ListDisksAsync(TestContext.Current.CancellationToken);

        foreach (var disk in disks.Where(d => d.Type == DeviceType.Disk))
        {
            var info = await zfs.GetSmartInfoAsync(disk.Id, TestContext.Current.CancellationToken);
            Assert.Equal(disk.Id, info.DeviceId);
            Assert.Contains($"/sbin/smartctl -x {disk.Id}", _remoteProcessCall.GetCommandsInvoked());
            if (info.DeviceId.EndsWith('5'))
            {
                // FAKE returns failed status
                Assert.NotEqual("PASSED", info.DataSection.Status);
            }
            else
            {
                Assert.Equal("PASSED", info.DataSection.Status);
            }
        }

        // Two calls for ListDisksAsync and one smartctl call per disk.
        Assert.Equal(4, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task GetSmartInfoAsyncForwardsCancellationToken(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var zfs = GetZfs();
        const string deviceId = "/dev/disk/by-id/scsi-0QEMU_QEMU_HARDDISK_drive-scsi6";

        await zfs.GetSmartInfoAsync(deviceId, TestContext.Current.CancellationToken);

        var token = Assert.Single(_remoteProcessCall.AsyncCancellationTokens);
        Assert.Equal(TestContext.Current.CancellationToken, token);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task GetSmartInfoAsyncWithCancelledTokenDoesNotRunCommand(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var zfs = GetZfs();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => zfs.GetSmartInfoAsync("/dev/disk/by-id/scsi-0QEMU_QEMU_HARDDISK_drive-scsi6", cancellationTokenSource.Token));

        Assert.Empty(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal(0, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
    }
}
