using Xunit;
using ROOT.Zfs.Core.Commands;

namespace ROOT.Zfs.Tests.Commands;

public class ZfsCommandsTest
{
    private readonly ZfsCommands _commands = new(new ZfsBinaries());

    [Fact]
    public void GetVersionCommandTest()
    {
        var command = _commands.GetVersion();
        Assert.Equal("/sbin/zfs --version", command.FullCommandLine);
    }
}