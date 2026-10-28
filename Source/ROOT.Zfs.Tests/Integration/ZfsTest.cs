using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;

namespace ROOT.Zfs.Tests.Integration;

[TestClass]
public class ZfsTest
{
    private readonly IProcessCall _remoteProcessCall = TestHelpers.RequiresRemoteConnection ? new SSHProcessCall("bbs", "zfsdev.root.dom", true) : null;

    private Core.Zfs GetZfs()
    {
        var zfs = ZfsTestFactory.CreateZfs(_remoteProcessCall);
        zfs.RequiresSudo = TestHelpers.RequiresSudo;
        return zfs;
    }

    [TestMethod,TestCategory("Integration")]
    public void InitializeTest()
    {
        var zfs = GetZfs();
        zfs.Binaries.RequiresSudo = TestHelpers.RequiresSudo;
        zfs.Binaries.Initialize();
        Assert.IsTrue(zfs.Binaries.Initialized);
    }
}