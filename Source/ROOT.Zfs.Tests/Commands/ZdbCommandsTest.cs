using Xunit;
using ROOT.Zfs.Core.Commands;

namespace ROOT.Zfs.Tests.Commands;

public class ZdbCommandsTest
{
    private readonly ZdbCommands _commands = new(new ZfsBinaries());

    [Fact]
    public void GetRawZdbOutputCommandTest()
    {
        var command = _commands.GetRawZdbOutput();
        Assert.Equal("/sbin/zdb", command.FullCommandLine);
    }
}