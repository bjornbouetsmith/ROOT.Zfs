using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Core.Commands;

namespace ROOT.Zfs.Tests.Commands;

[TestClass]
public class ZfsCommandsTest
{
    private readonly ZfsCommands _commands = new(new ZfsBinaries());

    [TestMethod]
    public void GetVersionCommandTest()
    {
        var command = _commands.GetVersion();
        Assert.AreEqual("/sbin/zfs --version", command.FullCommandLine);
    }
}