using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Core.Commands;

namespace ROOT.Zfs.Tests.Commands;

[TestClass]
public class ZdbCommandsTest
{
    private readonly ZdbCommands _commands = new(new ZfsBinaries());

    [TestMethod]
    public void GetRawZdbOutputCommandTest()
    {
        var command = _commands.GetRawZdbOutput();
        Assert.AreEqual("/sbin/zdb", command.FullCommandLine);
    }
}