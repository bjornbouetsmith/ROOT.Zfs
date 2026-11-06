using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Core.Helpers;

namespace ROOT.Zfs.Tests.Helpers;

[TestClass]
public class VirtualMachineDetectorTest
{
    public TestContext TestContext { get; set; }
    private readonly FakeRemoteConnection _remoteProcessCall = new("2.1.5-2");

    private VirtualMachineDetector GetDetector()
    {
        var binaries = new ZfsBinaries(_remoteProcessCall);
        return new VirtualMachineDetector(_remoteProcessCall, new VirtualizationCommands(binaries));
    }

    [TestMethod]
    public async Task IsVirtualMachineAsyncReturnsTrueWhenSystemdDetectVirtReportsVirtualization()
    {
        _remoteProcessCall.StdOutput = "kvm\n";
        var detector = GetDetector();

        var isVirtualMachine = await detector.IsVirtualMachineAsync(TestContext.CancellationToken);

        Assert.IsTrue(isVirtualMachine);
        Assert.Contains("/usr/bin/systemd-detect-virt --vm", _remoteProcessCall.GetCommandsInvoked());
        Assert.AreEqual(1, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.AreEqual(0, _remoteProcessCall.SyncLoadResponseCalls);
    }

    [TestMethod]
    public async Task IsVirtualMachineAsyncFallsBackToVirtWhat()
    {
        _remoteProcessCall.CommandResponses["/usr/sbin/virt-what"] = ("kvm\n", null);
        var detector = GetDetector();

        var isVirtualMachine = await detector.IsVirtualMachineAsync(TestContext.CancellationToken);

        Assert.IsTrue(isVirtualMachine);
        Assert.HasCount(2, _remoteProcessCall.GetCommandsInvoked());
        Assert.Contains("/usr/sbin/virt-what", _remoteProcessCall.GetCommandsInvoked());
    }

    [TestMethod]
    public async Task IsVirtualMachineAsyncFallsBackToHostnamectl()
    {
        _remoteProcessCall.CommandResponses["/usr/bin/hostnamectl status"] = (" Static hostname: zfsdev\n Virtualization: kvm", null);
        var detector = GetDetector();

        var isVirtualMachine = await detector.IsVirtualMachineAsync(TestContext.CancellationToken);

        Assert.IsTrue(isVirtualMachine);
        Assert.HasCount(3, _remoteProcessCall.GetCommandsInvoked());
        Assert.Contains("/usr/bin/hostnamectl status", _remoteProcessCall.GetCommandsInvoked());
    }

    [TestMethod]
    public async Task IsVirtualMachineAsyncFallsBackToCpuHypervisorFlag()
    {
        _remoteProcessCall.CommandResponses["/bin/cat /proc/cpuinfo"] = ("processor\t: 0\nflags\t\t: fpu vme de pse tsc msr hypervisor", null);
        var detector = GetDetector();

        var isVirtualMachine = await detector.IsVirtualMachineAsync(TestContext.CancellationToken);

        Assert.IsTrue(isVirtualMachine);
        Assert.HasCount(5, _remoteProcessCall.GetCommandsInvoked());
        Assert.Contains("/bin/cat /proc/cpuinfo", _remoteProcessCall.GetCommandsInvoked());
    }

    [TestMethod]
    public async Task IsVirtualMachineAsyncPropagatesUnexpectedProbeExceptions()
    {
        _remoteProcessCall.CommandExceptions["/usr/bin/systemd-detect-virt --vm"] = new InvalidOperationException("boom");
        var detector = GetDetector();

        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => detector.IsVirtualMachineAsync(TestContext.CancellationToken));

        Assert.HasCount(1, _remoteProcessCall.GetCommandsInvoked());
    }

    [TestMethod]
    public async Task IsVirtualMachineAsyncFallsBackWhenDetectionBinariesAreNotInstalled()
    {
        _remoteProcessCall.CommandExceptions["/usr/bin/systemd-detect-virt --vm"] = new Win32Exception(2);
        _remoteProcessCall.CommandExceptions["/usr/sbin/virt-what"] = new TimeoutException("virt-what timed out");
        _remoteProcessCall.CommandExceptions["/usr/bin/hostnamectl status"] = new Win32Exception(2);
        _remoteProcessCall.CommandResponses["/bin/cat /sys/class/dmi/id/sys_vendor /sys/class/dmi/id/product_name"] = ("QEMU\nStandard PC (Q35 + ICH9, 2009)", null);
        var detector = GetDetector();

        var isVirtualMachine = await detector.IsVirtualMachineAsync(TestContext.CancellationToken);

        Assert.IsTrue(isVirtualMachine);
        Assert.HasCount(4, _remoteProcessCall.GetCommandsInvoked());
        Assert.Contains("/bin/cat /sys/class/dmi/id/sys_vendor /sys/class/dmi/id/product_name", _remoteProcessCall.GetCommandsInvoked());
    }

    [TestMethod]
    public async Task IsVirtualMachineAsyncFallsBackToDmiWhenSystemdCommandsAreUnavailable()
    {
        _remoteProcessCall.CommandResponses["/usr/bin/systemd-detect-virt --vm"] = (null, "systemd-detect-virt: command not found");
        _remoteProcessCall.CommandResponses["/usr/sbin/virt-what"] = ("", null);
        _remoteProcessCall.CommandResponses["/usr/bin/hostnamectl status"] = (" Static hostname: zfsdev\n         Chassis: desktop", null);
        _remoteProcessCall.CommandResponses["/bin/cat /sys/class/dmi/id/sys_vendor /sys/class/dmi/id/product_name"] = ("QEMU\nStandard PC (Q35 + ICH9, 2009)", null);
        var detector = GetDetector();

        var isVirtualMachine = await detector.IsVirtualMachineAsync(TestContext.CancellationToken);

        Assert.IsTrue(isVirtualMachine);
        Assert.HasCount(4, _remoteProcessCall.GetCommandsInvoked());
        Assert.Contains("/bin/cat /sys/class/dmi/id/sys_vendor /sys/class/dmi/id/product_name", _remoteProcessCall.GetCommandsInvoked());
        Assert.DoesNotContain("/bin/cat /proc/cpuinfo", _remoteProcessCall.GetCommandsInvoked());
        Assert.AreEqual(4, _remoteProcessCall.AsyncLoadResponseCalls);
    }

    [TestMethod]
    public async Task IsVirtualMachineAsyncReturnsFalseWhenNoProbeDetectsVirtualization()
    {
        _remoteProcessCall.StdOutput = "";
        _remoteProcessCall.CompleteAsyncResponses = true;
        var detector = GetDetector();

        var isVirtualMachine = await detector.IsVirtualMachineAsync(TestContext.CancellationToken);

        Assert.IsFalse(isVirtualMachine);
        Assert.HasCount(5, _remoteProcessCall.GetCommandsInvoked());
        Assert.Contains("/usr/bin/systemd-detect-virt --vm", _remoteProcessCall.GetCommandsInvoked());
        Assert.Contains("/usr/sbin/virt-what", _remoteProcessCall.GetCommandsInvoked());
        Assert.Contains("/usr/bin/hostnamectl status", _remoteProcessCall.GetCommandsInvoked());
        Assert.Contains("/bin/cat /sys/class/dmi/id/sys_vendor /sys/class/dmi/id/product_name", _remoteProcessCall.GetCommandsInvoked());
        Assert.Contains("/bin/cat /proc/cpuinfo", _remoteProcessCall.GetCommandsInvoked());
        Assert.AreEqual(5, _remoteProcessCall.AsyncLoadResponseCalls);
    }

    [TestMethod]
    public async Task IsVirtualMachineAsyncForwardsCancellationToken()
    {
        var detector = GetDetector();

        await detector.IsVirtualMachineAsync(TestContext.CancellationToken);

        Assert.HasCount(5, _remoteProcessCall.AsyncCancellationTokens);
        Assert.IsTrue(_remoteProcessCall.AsyncCancellationTokens.All(token => token == TestContext.CancellationToken));
    }

    [TestMethod]
    public async Task IsVirtualMachineAsyncWithCancelledTokenDoesNotRunCommand()
    {
        var detector = GetDetector();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => detector.IsVirtualMachineAsync(cancellationTokenSource.Token));

        Assert.IsEmpty(_remoteProcessCall.GetCommandsInvoked());
        Assert.AreEqual(0, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.AreEqual(0, _remoteProcessCall.SyncLoadResponseCalls);
    }
}
