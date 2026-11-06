using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Core.Commands;

namespace ROOT.Zfs.Tests.Integration.Fake;

[TestClass]
public class FakeZfsBinariesTest
{
    private readonly FakeRemoteConnection _remoteProcessCall = new("2.1.5-2");

    [TestMethod, TestCategory("FakeIntegration")]
    public void DefaultsShouldBeExpectedAndNotInitialized()
    {
        var binaries = new ZfsBinaries();

        Assert.IsFalse(binaries.Initialized);
        Assert.AreEqual("/sbin/zfs", binaries.Zfs);
        Assert.AreEqual("/sbin/zpool", binaries.Zpool);
        Assert.AreEqual("/sbin/zdb", binaries.Zdb);
        Assert.AreEqual("/bin/ls", binaries.Ls);
        Assert.AreEqual("/bin/lsblk", binaries.Lsblk);
        Assert.AreEqual("/bin/awk", binaries.Awk);
        Assert.AreEqual("/bin/which", binaries.Which);
        Assert.AreEqual("/sbin/smartctl", binaries.Smartctl);
        Assert.AreEqual("/bin/grep", binaries.Grep);
    }

    [TestMethod, TestCategory("FakeIntegration")]
    public void InitializeTest()
    {
        var binaries = new ZfsBinaries(_remoteProcessCall);
        Assert.IsFalse(binaries.Initialized);

        binaries.Initialize();

        Assert.IsTrue(binaries.Initialized);
        var commandsInvoked = _remoteProcessCall.GetCommandsInvoked();

        Assert.HasCount(8, commandsInvoked);
        var asHashSet = commandsInvoked.ToHashSet();
        Assert.Contains("/bin/which zfs", asHashSet);
        Assert.Contains("/bin/which zpool", asHashSet);
        Assert.Contains("/bin/which zdb", asHashSet);
        Assert.Contains("/bin/which ls", asHashSet);
        Assert.Contains("/bin/which lsblk", asHashSet);
        Assert.Contains("/bin/which awk", asHashSet);
        Assert.Contains("/bin/which smartctl", asHashSet);
        Assert.Contains("/bin/which grep", asHashSet);

        Assert.AreEqual("/sbin/zfs", binaries.Zfs);
        Assert.AreEqual("/sbin/zpool", binaries.Zpool);
        Assert.AreEqual("/sbin/zdb", binaries.Zdb);
        Assert.AreEqual("/bin/ls", binaries.Ls);
        Assert.AreEqual("/bin/lsblk", binaries.Lsblk);
        Assert.AreEqual("/bin/awk", binaries.Awk);

        // our fake cannot find smartctl, so assert that it does not get overridden
        Assert.AreEqual("/sbin/smartctl", binaries.Smartctl);
    }

    [TestMethod, TestCategory("FakeIntegration")]
    public void InitializeShouldKeepDefaultsWhenWhichFails()
    {
        var binaries = new ZfsBinaries(_remoteProcessCall)
        {
            Zfs = "/custom/zfs",
            Smartctl = "/custom/smartctl"
        };
        _remoteProcessCall.StdError = "which: command not found";

        binaries.Initialize();

        Assert.IsTrue(binaries.Initialized);
        Assert.AreEqual("/custom/zfs", binaries.Zfs);
        Assert.AreEqual("/sbin/zpool", binaries.Zpool);
        Assert.AreEqual("/sbin/zdb", binaries.Zdb);
        Assert.AreEqual("/bin/ls", binaries.Ls);
        Assert.AreEqual("/bin/lsblk", binaries.Lsblk);
        Assert.AreEqual("/bin/awk", binaries.Awk);
        Assert.AreEqual("/bin/grep", binaries.Grep);
        Assert.AreEqual("/custom/smartctl", binaries.Smartctl);
        Assert.HasCount(8, _remoteProcessCall.GetCommandsInvoked());
    }

    [TestMethod, TestCategory("FakeIntegration")]
    public void InitializeShouldUseConfiguredWhichAndTrimItsOutput()
    {
        var binaries = new ZfsBinaries(_remoteProcessCall)
        {
            Which = "/usr/bin/which"
        };
        _remoteProcessCall.StdOutput = "/opt/zfs/bin/zfs\n";

        binaries.Initialize();

        Assert.IsTrue(binaries.Initialized);
        Assert.AreEqual("/opt/zfs/bin/zfs", binaries.Zfs);
        Assert.AreEqual("/opt/zfs/bin/zfs", binaries.Zpool);
        Assert.AreEqual("/opt/zfs/bin/zfs", binaries.Zdb);
        Assert.AreEqual("/opt/zfs/bin/zfs", binaries.Ls);
        Assert.AreEqual("/opt/zfs/bin/zfs", binaries.Lsblk);
        Assert.AreEqual("/opt/zfs/bin/zfs", binaries.Awk);
        Assert.AreEqual("/opt/zfs/bin/zfs", binaries.Smartctl);
        Assert.AreEqual("/opt/zfs/bin/zfs", binaries.Grep);

        var commandsInvoked = _remoteProcessCall.GetCommandsInvoked();
        Assert.HasCount(8, commandsInvoked);
        Assert.IsTrue(commandsInvoked.All(command => command.StartsWith("/usr/bin/which ", StringComparison.Ordinal)));
    }
}
