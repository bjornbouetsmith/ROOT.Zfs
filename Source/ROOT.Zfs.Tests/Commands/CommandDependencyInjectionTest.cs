using Microsoft.Extensions.DependencyInjection;
using Xunit;
using ROOT.Zfs.Core;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Arguments.Dataset;

namespace ROOT.Zfs.Tests.Commands;

public class CommandDependencyInjectionTest
{
    [Fact]
    public void CustomBinaryPathsShouldBeUsed()
    {
        var binaries = new ZfsBinaries { Zfs = "/usr/bin/zfs" };
        var commands = new DatasetCommands(binaries);

        var command = commands.Destroy(new DatasetDestroyArgs { Dataset = "tank/myds" });

        Assert.Equal("/usr/bin/zfs destroy tank/myds", command.FullCommandLine);
    }

    [Fact]
    public void AddZfsShouldRegisterCommandsAndZfs()
    {
        var services = new ServiceCollection();
        services.AddZfs(configureBinaries: binaries => binaries.Zfs = "/usr/bin/zfs");

        using var provider = services.BuildServiceProvider();

        var zfs = provider.GetRequiredService<IZfs>();
        Assert.NotNull(zfs);

        var binaries = provider.GetRequiredService<IZfsBinaries>();
        Assert.Equal("/usr/bin/zfs", binaries.Zfs);

        var datasetCommands = provider.GetRequiredService<IDatasetCommands>();
        var command = datasetCommands.Destroy(new DatasetDestroyArgs { Dataset = "tank/myds" });
        Assert.Equal("/usr/bin/zfs destroy tank/myds", command.FullCommandLine);
    }

    [Fact]
    public void BinaryStateShouldBeIsolatedBetweenInstances()
    {
        var first = ZfsTestFactory.CreateZfs(null, binaries => binaries.Zfs = "/one/zfs");
        var second = ZfsTestFactory.CreateZfs(null, binaries => binaries.Zfs = "/two/zfs");

        Assert.Equal("/one/zfs", first.Binaries.Zfs);
        Assert.Equal("/two/zfs", second.Binaries.Zfs);
    }
}