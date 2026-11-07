using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Integration;

public class ZfsTest
{
    private readonly IProcessCall _remoteProcessCall = TestHelpers.RequiresRemoteConnection ? new SSHProcessCall("bbs", "zfsdev.root.dom", true) : null;

    private Core.Zfs GetZfs()
    {
        var zfs = ZfsTestFactory.CreateZfs(_remoteProcessCall);
        zfs.RequiresSudo = TestHelpers.RequiresSudo;
        return zfs;
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void InitializeTest()
    {
        var zfs = GetZfs();
        zfs.Binaries.RequiresSudo = TestHelpers.RequiresSudo;
        zfs.Binaries.Initialize();
        Assert.True(zfs.Binaries.Initialized);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetVersionInfoAsyncTest()
    {
        var zfs = GetZfs();

        var info = await zfs.GetVersionInfoAsync(TestContext.Current.CancellationToken);

        Assert.NotNull(info);
        Assert.NotEmpty(info.Lines);
        Assert.True(info.Lines.Any(line => line.StartsWith("zfs-", StringComparison.Ordinal)),
            $"Expected a zfs version line, but got: {string.Join(", ", info.Lines)}");

        foreach (var line in info.Lines)
        {
            TestContext.Current.TestOutputHelper.WriteLine(line);
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task ListDisksAsyncTest()
    {
        var zfs = GetZfs();

        var disks = (await zfs.ListDisksAsync(TestContext.Current.CancellationToken)).ToList();

        Assert.NotNull(disks);
        Assert.NotEmpty(disks);

        foreach (var disk in disks)
        {
            TestContext.Current.TestOutputHelper.WriteLine($"{disk.Id}: {disk.Type}");
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetSmartInfosAsyncTest()
    {
        var zfs = GetZfs();

        var infos = await zfs.GetSmartInfosAsync(TestContext.Current.CancellationToken);

        Assert.NotNull(infos);
        Assert.NotEmpty(infos);

        foreach (var info in infos)
        {
            Assert.False(string.IsNullOrWhiteSpace(info.DeviceId));
            TestContext.Current.TestOutputHelper.WriteLine($"{info.DeviceId}: {info.DataSection?.Status}");
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetSmartInfoAsyncTest()
    {
        var zfs = GetZfs();
        var disks = await zfs.ListDisksAsync(TestContext.Current.CancellationToken);
        var disk = disks.First(d => d.Type == DeviceType.Disk);

        var info = await zfs.GetSmartInfoAsync(disk.Id, TestContext.Current.CancellationToken);

        Assert.NotNull(info);
        Assert.Equal(disk.Id, info.DeviceId);
        
        TestContext.Current.TestOutputHelper.WriteLine($"{info.DeviceId}: {info.DataSection?.Status}");
    }
}