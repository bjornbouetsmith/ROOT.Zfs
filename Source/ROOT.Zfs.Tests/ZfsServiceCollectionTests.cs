using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Core;
using ROOT.Zfs.Public;

namespace ROOT.Zfs.Tests;

[TestClass]
public class ZfsServiceCollectionTests
{
    [TestMethod]
    public void ShouldBeAbleToResolveZfs()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddZfs();

        var provider = serviceCollection.BuildServiceProvider();

        var zfs = provider.GetRequiredService<IZfs>();

        Assert.IsNotNull(zfs);
    }
}