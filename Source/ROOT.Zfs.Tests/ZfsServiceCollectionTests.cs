using Microsoft.Extensions.DependencyInjection;
using Xunit;
using ROOT.Zfs.Core;
using ROOT.Zfs.Public;

namespace ROOT.Zfs.Tests;

public class ZfsServiceCollectionTests
{
    [Fact]
    public void ShouldBeAbleToResolveZfs()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddZfs();

        var provider = serviceCollection.BuildServiceProvider();

        var zfs = provider.GetRequiredService<IZfs>();

        Assert.NotNull(zfs);
    }
}