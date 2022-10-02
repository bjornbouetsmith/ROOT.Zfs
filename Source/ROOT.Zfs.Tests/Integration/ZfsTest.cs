using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.Serialization;

namespace ROOT.Zfs.Tests.Integration
{
    [TestClass]
    public class ZfsTest
    {
        readonly FakeRemoteConnection _remoteProcessCall = new ("2.1.5-2");// "bbs", "zfsdev.root.dom", true);

        [TestMethod,TestCategory("FakeIntegration")]
        public void GetVersionInfo()
        {
            var zfs = new Core.Zfs(_remoteProcessCall);
            var info = zfs.GetVersionInfo();
            Assert.IsNotNull(info);
            Assert.IsTrue(info.Lines.Any());
            foreach (var line in info.Lines)
            {
                Console.WriteLine(line);
            }
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void ListDisksTest()
        {
            var zfs = new Core.Zfs(_remoteProcessCall);
            var disks = zfs.ListDisks();
            Console.WriteLine(disks.Dump(new JsonFormatter()));
        }
    }
}
