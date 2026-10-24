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
        Assert.IsTrue(info.Lines.Length != 0);
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
        Assert.IsTrue(disks.Any());
        Console.WriteLine(disks.Dump(new JsonFormatter()));
    }

}