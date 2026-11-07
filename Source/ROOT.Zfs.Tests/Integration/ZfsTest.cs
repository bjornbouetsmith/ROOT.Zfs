using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Integration;

[TestClass]
public class ZfsTest
{
    public TestContext TestContext { get; set; }
    private readonly IProcessCall _remoteProcessCall = TestHelpers.RequiresRemoteConnection ? new SSHProcessCall("bbs", "zfsdev.root.dom", true) : null;

    private Core.Zfs GetZfs()
    {
        var zfs = ZfsTestFactory.CreateZfs(_remoteProcessCall);
        zfs.RequiresSudo = TestHelpers.RequiresSudo;
        return zfs;
    }

    [TestMethod, TestCategory("Integration")]
    public void InitializeTest()
    {
        var zfs = GetZfs();
        zfs.Binaries.RequiresSudo = TestHelpers.RequiresSudo;
        zfs.Binaries.Initialize();
        Assert.IsTrue(zfs.Binaries.Initialized);
    }

    [TestMethod, TestCategory("Integration")]
    public async Task GetVersionInfoAsyncTest()
    {
        var zfs = GetZfs();

        var info = await zfs.GetVersionInfoAsync(TestContext.CancellationToken);

        Assert.IsNotNull(info);
        Assert.IsNotEmpty(info.Lines);
        Assert.IsTrue(info.Lines.Any(line => line.StartsWith("zfs-", StringComparison.Ordinal)),
            $"Expected a zfs version line, but got: {string.Join(", ", info.Lines)}");

        foreach (var line in info.Lines)
        {
            TestContext.WriteLine(line);
        }
    }

    [TestMethod, TestCategory("Integration")]
    public async Task ListDisksAsyncTest()
    {
        var zfs = GetZfs();

        var disks = (await zfs.ListDisksAsync(TestContext.CancellationToken)).ToList();

        Assert.IsNotNull(disks);
        Assert.IsNotEmpty(disks);

        foreach (var disk in disks)
        {
            TestContext.WriteLine($"{disk.Id}: {disk.Type}");
        }
    }

    [TestMethod, TestCategory("Integration")]
    public async Task GetSmartInfosAsyncTest()
    {
        var zfs = GetZfs();

        var infos = await zfs.GetSmartInfosAsync(TestContext.CancellationToken);

        Assert.IsNotNull(infos);
        Assert.IsNotEmpty(infos);

        foreach (var info in infos)
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(info.DeviceId));
            TestContext.WriteLine($"{info.DeviceId}: {info.DataSection?.Status}");
        }
    }

    [TestMethod, TestCategory("Integration")]
    public async Task GetSmartInfoAsyncTest()
    {
        var zfs = GetZfs();
        var disks = await zfs.ListDisksAsync(TestContext.CancellationToken);
        var disk = disks.First(d => d.Type == DeviceType.Disk);

        var info = await zfs.GetSmartInfoAsync(disk.Id, TestContext.CancellationToken);

        Assert.IsNotNull(info);
        Assert.AreEqual(disk.Id, info.DeviceId);
        Assert.IsFalse(string.IsNullOrWhiteSpace(info.DataSection?.Status));
        TestContext.WriteLine($"{info.DeviceId}: {info.DataSection?.Status}");
    }
}