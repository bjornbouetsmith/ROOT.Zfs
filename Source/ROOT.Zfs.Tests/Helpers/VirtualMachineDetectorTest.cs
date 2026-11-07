using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Core.Helpers;

namespace ROOT.Zfs.Tests.Helpers;

public class VirtualMachineDetectorTest
{
    private readonly FakeRemoteConnection _remoteProcessCall = new("2.1.5-2");

    private VirtualMachineDetector GetDetector()
    {
        var binaries = new ZfsBinaries(_remoteProcessCall);
        return new VirtualMachineDetector(_remoteProcessCall, new VirtualizationCommands(binaries));
    }

    [Fact]
    public async Task IsVirtualMachineAsyncReturnsTrueWhenSystemdDetectVirtReportsVirtualization()
    {
        _remoteProcessCall.StdOutput = "kvm\n";
        var detector = GetDetector();

        var isVirtualMachine = await detector.IsVirtualMachineAsync(TestContext.Current.CancellationToken);

        Assert.True(isVirtualMachine);
        Assert.Contains("/usr/bin/systemd-detect-virt --vm", _remoteProcessCall.GetCommandsInvoked());
        Assert.Equal(1, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
    }

    [Fact]
    public async Task IsVirtualMachineAsyncFallsBackToVirtWhat()
    {
        _remoteProcessCall.CommandResponses["/usr/sbin/virt-what"] = ("kvm\n", null);
        var detector = GetDetector();

        var isVirtualMachine = await detector.IsVirtualMachineAsync(TestContext.Current.CancellationToken);

        Assert.True(isVirtualMachine);
        Assert.Equal(2, _remoteProcessCall.GetCommandsInvoked().Count());
        Assert.Contains("/usr/sbin/virt-what", _remoteProcessCall.GetCommandsInvoked());
    }

    [Fact]
    public async Task IsVirtualMachineAsyncFallsBackToHostnamectl()
    {
        _remoteProcessCall.CommandResponses["/usr/bin/hostnamectl status"] = (" Static hostname: zfsdev\n Virtualization: kvm", null);
        var detector = GetDetector();

        var isVirtualMachine = await detector.IsVirtualMachineAsync(TestContext.Current.CancellationToken);

        Assert.True(isVirtualMachine);
        Assert.Equal(3, _remoteProcessCall.GetCommandsInvoked().Count());
        Assert.Contains("/usr/bin/hostnamectl status", _remoteProcessCall.GetCommandsInvoked());
    }

    [Fact]
    public async Task IsVirtualMachineAsyncFallsBackToCpuHypervisorFlag()
    {
        _remoteProcessCall.CommandResponses["/bin/cat /proc/cpuinfo"] = ("processor\t: 0\nflags\t\t: fpu vme de pse tsc msr hypervisor", null);
        var detector = GetDetector();

        var isVirtualMachine = await detector.IsVirtualMachineAsync(TestContext.Current.CancellationToken);

        Assert.True(isVirtualMachine);
        Assert.Equal(5, _remoteProcessCall.GetCommandsInvoked().Count());
        Assert.Contains("/bin/cat /proc/cpuinfo", _remoteProcessCall.GetCommandsInvoked());
    }

    [Fact]
    public async Task IsVirtualMachineAsyncPropagatesUnexpectedProbeExceptions()
    {
        _remoteProcessCall.CommandExceptions["/usr/bin/systemd-detect-virt --vm"] = new InvalidOperationException("boom");
        var detector = GetDetector();

        await Assert.ThrowsAsync<InvalidOperationException>(() => detector.IsVirtualMachineAsync(TestContext.Current.CancellationToken));

        Assert.Single(_remoteProcessCall.GetCommandsInvoked());
    }

    [Fact]
    public async Task IsVirtualMachineAsyncFallsBackWhenDetectionBinariesAreNotInstalled()
    {
        _remoteProcessCall.CommandExceptions["/usr/bin/systemd-detect-virt --vm"] = new Win32Exception(2);
        _remoteProcessCall.CommandExceptions["/usr/sbin/virt-what"] = new TimeoutException("virt-what timed out");
        _remoteProcessCall.CommandExceptions["/usr/bin/hostnamectl status"] = new Win32Exception(2);
        _remoteProcessCall.CommandResponses["/bin/cat /sys/class/dmi/id/sys_vendor /sys/class/dmi/id/product_name"] = ("QEMU\nStandard PC (Q35 + ICH9, 2009)", null);
        var detector = GetDetector();

        var isVirtualMachine = await detector.IsVirtualMachineAsync(TestContext.Current.CancellationToken);

        Assert.True(isVirtualMachine);
        Assert.Equal(4, _remoteProcessCall.GetCommandsInvoked().Count());
        Assert.Contains("/bin/cat /sys/class/dmi/id/sys_vendor /sys/class/dmi/id/product_name", _remoteProcessCall.GetCommandsInvoked());
    }

    [Fact]
    public async Task IsVirtualMachineAsyncFallsBackToDmiWhenSystemdCommandsAreUnavailable()
    {
        _remoteProcessCall.CommandResponses["/usr/bin/systemd-detect-virt --vm"] = (null, "systemd-detect-virt: command not found");
        _remoteProcessCall.CommandResponses["/usr/sbin/virt-what"] = ("", null);
        _remoteProcessCall.CommandResponses["/usr/bin/hostnamectl status"] = (" Static hostname: zfsdev\n         Chassis: desktop", null);
        _remoteProcessCall.CommandResponses["/bin/cat /sys/class/dmi/id/sys_vendor /sys/class/dmi/id/product_name"] = ("QEMU\nStandard PC (Q35 + ICH9, 2009)", null);
        var detector = GetDetector();

        var isVirtualMachine = await detector.IsVirtualMachineAsync(TestContext.Current.CancellationToken);

        Assert.True(isVirtualMachine);
        Assert.Equal(4, _remoteProcessCall.GetCommandsInvoked().Count());
        Assert.Contains("/bin/cat /sys/class/dmi/id/sys_vendor /sys/class/dmi/id/product_name", _remoteProcessCall.GetCommandsInvoked());
        Assert.DoesNotContain("/bin/cat /proc/cpuinfo", _remoteProcessCall.GetCommandsInvoked());
        Assert.Equal(4, _remoteProcessCall.AsyncLoadResponseCalls);
    }

    [Fact]
    public async Task IsVirtualMachineAsyncReturnsFalseWhenNoProbeDetectsVirtualization()
    {
        _remoteProcessCall.StdOutput = "";
        _remoteProcessCall.CompleteAsyncResponses = true;
        var detector = GetDetector();

        var isVirtualMachine = await detector.IsVirtualMachineAsync(TestContext.Current.CancellationToken);

        Assert.False(isVirtualMachine);
        Assert.Equal(5, _remoteProcessCall.GetCommandsInvoked().Count());
        Assert.Contains("/usr/bin/systemd-detect-virt --vm", _remoteProcessCall.GetCommandsInvoked());
        Assert.Contains("/usr/sbin/virt-what", _remoteProcessCall.GetCommandsInvoked());
        Assert.Contains("/usr/bin/hostnamectl status", _remoteProcessCall.GetCommandsInvoked());
        Assert.Contains("/bin/cat /sys/class/dmi/id/sys_vendor /sys/class/dmi/id/product_name", _remoteProcessCall.GetCommandsInvoked());
        Assert.Contains("/bin/cat /proc/cpuinfo", _remoteProcessCall.GetCommandsInvoked());
        Assert.Equal(5, _remoteProcessCall.AsyncLoadResponseCalls);
    }

    [Fact]
    public async Task IsVirtualMachineAsyncForwardsCancellationToken()
    {
        var detector = GetDetector();

        await detector.IsVirtualMachineAsync(TestContext.Current.CancellationToken);

        Assert.Equal(5, _remoteProcessCall.AsyncCancellationTokens.Count());
        Assert.True(_remoteProcessCall.AsyncCancellationTokens.All(token => token == TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task IsVirtualMachineAsyncWithCancelledTokenDoesNotRunCommand()
    {
        var detector = GetDetector();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => detector.IsVirtualMachineAsync(cancellationTokenSource.Token));

        Assert.Empty(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal(0, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
    }
}
