using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.Serialization;

namespace ROOT.Zfs.Tests.Integration.Fake;

[TestClass]
public class FakeZfsTest
{
    readonly FakeRemoteConnection _remoteProcessCall = new ("2.1.5-2");

    private Core.Zfs GetZfs()
    {
        return ZfsTestFactory.CreateZfs(_remoteProcessCall);
    }

    [TestMethod,TestCategory("FakeIntegration")]
    public void GetVersionInfo()
    {
        var zfs = GetZfs();
        var info = zfs.GetVersionInfo();
        Assert.IsNotNull(info);
        Assert.IsNotEmpty(info.Lines);
        foreach (var line in info.Lines)
        {
            Console.WriteLine(line);
        }
    }
        
    [TestMethod, TestCategory("FakeIntegration")]
    public void ListDisksTest()
    {
        var zfs = GetZfs();
        var disks = zfs.ListDisks().ToList();
        Assert.IsNotNull(disks);
        Assert.IsNotEmpty(disks);
        Console.WriteLine(disks.Dump(new JsonFormatter()));
    }

}