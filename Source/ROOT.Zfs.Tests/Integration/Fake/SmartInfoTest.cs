using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Integration.Fake;

[TestClass]
public class SmartInfoTest
{
    readonly IProcessCall _remoteProcessCall = new FakeRemoteConnection("2.1.5-2");

    private Core.Zfs GetZfs()
    {
        return ZfsTestFactory.CreateZfs(_remoteProcessCall);
    }

    [TestMethod, TestCategory("FakeIntegration")]
    public void GetSmartInfosTest()
    {
        var zfs = GetZfs();
        var infos = zfs.GetSmartInfos().ToList();

        Assert.IsNotNull(infos);
        Assert.HasCount(2, infos);
    }

    [TestMethod]
    public void GetSmartInfoTest()
    {
        var zfs = GetZfs();
        var disks = zfs.ListDisks();

        foreach (var disk in disks.Where(d => d.Type == DeviceType.Disk))
        {
            var info = zfs.GetSmartInfo(disk.Id);
            if (info.DeviceId.EndsWith('5'))
            {
                // FAKE returns failed status
                Assert.AreNotEqual("PASSED", info.DataSection.Status);
            }
            else
            {
                Assert.AreEqual("PASSED", info.DataSection.Status);
            }
        }
    }
}