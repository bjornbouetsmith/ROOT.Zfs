using System;
using System.Linq;
using Xunit;
using ROOT.Zfs.Core.Commands;

namespace ROOT.Zfs.Tests.Integration.Fake;

public class FakeZfsBinariesTest
{
    private readonly FakeRemoteConnection _remoteProcessCall = new("2.1.5-2");

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void DefaultsShouldBeExpectedAndNotInitialized()
    {
        var binaries = new ZfsBinaries();

        Assert.False(binaries.Initialized);
        Assert.Equal("/sbin/zfs", binaries.Zfs);
        Assert.Equal("/sbin/zpool", binaries.Zpool);
        Assert.Equal("/sbin/zdb", binaries.Zdb);
        Assert.Equal("/bin/ls", binaries.Ls);
        Assert.Equal("/bin/lsblk", binaries.Lsblk);
        Assert.Equal("/bin/awk", binaries.Awk);
        Assert.Equal("/bin/which", binaries.Which);
        Assert.Equal("/sbin/smartctl", binaries.Smartctl);
        Assert.Equal("/bin/grep", binaries.Grep);
        Assert.Equal("/usr/bin/systemd-detect-virt", binaries.SystemdDetectVirt);
        Assert.Equal("/usr/sbin/virt-what", binaries.VirtWhat);
        Assert.Equal("/usr/bin/hostnamectl", binaries.Hostnamectl);
        Assert.Equal("/bin/cat", binaries.Cat);
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void InitializeTest()
    {
        var binaries = new ZfsBinaries(_remoteProcessCall);
        Assert.False(binaries.Initialized);

        binaries.Initialize();

        Assert.True(binaries.Initialized);
        var commandsInvoked = _remoteProcessCall.GetCommandsInvoked();

        Assert.Equal(12, commandsInvoked.Count());
        var asHashSet = commandsInvoked.ToHashSet();
        Assert.Contains("/bin/which zfs", asHashSet);
        Assert.Contains("/bin/which zpool", asHashSet);
        Assert.Contains("/bin/which zdb", asHashSet);
        Assert.Contains("/bin/which ls", asHashSet);
        Assert.Contains("/bin/which lsblk", asHashSet);
        Assert.Contains("/bin/which awk", asHashSet);
        Assert.Contains("/bin/which smartctl", asHashSet);
        Assert.Contains("/bin/which grep", asHashSet);
        Assert.Contains("/bin/which systemd-detect-virt", asHashSet);
        Assert.Contains("/bin/which virt-what", asHashSet);
        Assert.Contains("/bin/which hostnamectl", asHashSet);
        Assert.Contains("/bin/which cat", asHashSet);

        Assert.Equal("/sbin/zfs", binaries.Zfs);
        Assert.Equal("/sbin/zpool", binaries.Zpool);
        Assert.Equal("/sbin/zdb", binaries.Zdb);
        Assert.Equal("/bin/ls", binaries.Ls);
        Assert.Equal("/bin/lsblk", binaries.Lsblk);
        Assert.Equal("/bin/awk", binaries.Awk);
        Assert.Equal("/usr/bin/systemd-detect-virt", binaries.SystemdDetectVirt);
        Assert.Equal("/usr/sbin/virt-what", binaries.VirtWhat);
        Assert.Equal("/usr/bin/hostnamectl", binaries.Hostnamectl);
        Assert.Equal("/bin/cat", binaries.Cat);

        // our fake cannot find smartctl, so assert that it does not get overridden
        Assert.Equal("/sbin/smartctl", binaries.Smartctl);
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void InitializeShouldKeepDefaultsWhenWhichFails()
    {
        var binaries = new ZfsBinaries(_remoteProcessCall)
        {
            Zfs = "/custom/zfs",
            Smartctl = "/custom/smartctl"
        };
        _remoteProcessCall.StdError = "which: command not found";

        binaries.Initialize();

        Assert.True(binaries.Initialized);
        Assert.Equal("/custom/zfs", binaries.Zfs);
        Assert.Equal("/sbin/zpool", binaries.Zpool);
        Assert.Equal("/sbin/zdb", binaries.Zdb);
        Assert.Equal("/bin/ls", binaries.Ls);
        Assert.Equal("/bin/lsblk", binaries.Lsblk);
        Assert.Equal("/bin/awk", binaries.Awk);
        Assert.Equal("/bin/grep", binaries.Grep);
        Assert.Equal("/usr/bin/systemd-detect-virt", binaries.SystemdDetectVirt);
        Assert.Equal("/usr/sbin/virt-what", binaries.VirtWhat);
        Assert.Equal("/usr/bin/hostnamectl", binaries.Hostnamectl);
        Assert.Equal("/bin/cat", binaries.Cat);
        Assert.Equal("/custom/smartctl", binaries.Smartctl);
        Assert.Equal(12, _remoteProcessCall.GetCommandsInvoked().Count());
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void InitializeShouldUseConfiguredWhichAndTrimItsOutput()
    {
        var binaries = new ZfsBinaries(_remoteProcessCall)
        {
            Which = "/usr/bin/which"
        };
        _remoteProcessCall.StdOutput = "/opt/zfs/bin/zfs\n";

        binaries.Initialize();

        Assert.True(binaries.Initialized);
        Assert.Equal("/opt/zfs/bin/zfs", binaries.Zfs);
        Assert.Equal("/opt/zfs/bin/zfs", binaries.Zpool);
        Assert.Equal("/opt/zfs/bin/zfs", binaries.Zdb);
        Assert.Equal("/opt/zfs/bin/zfs", binaries.Ls);
        Assert.Equal("/opt/zfs/bin/zfs", binaries.Lsblk);
        Assert.Equal("/opt/zfs/bin/zfs", binaries.Awk);
        Assert.Equal("/opt/zfs/bin/zfs", binaries.Smartctl);
        Assert.Equal("/opt/zfs/bin/zfs", binaries.Grep);
        Assert.Equal("/opt/zfs/bin/zfs", binaries.SystemdDetectVirt);
        Assert.Equal("/opt/zfs/bin/zfs", binaries.VirtWhat);
        Assert.Equal("/opt/zfs/bin/zfs", binaries.Hostnamectl);
        Assert.Equal("/opt/zfs/bin/zfs", binaries.Cat);

        var commandsInvoked = _remoteProcessCall.GetCommandsInvoked();
        Assert.Equal(12, commandsInvoked.Count());
        Assert.True(commandsInvoked.All(command => command.StartsWith("/usr/bin/which ", StringComparison.Ordinal)));
    }
}
