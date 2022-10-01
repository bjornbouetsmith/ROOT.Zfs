using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;

namespace ROOT.Zfs.Tests
{
    [TestClass]
    public class ZfsTest
    {
        readonly SSHProcessCall _remoteProcessCall = new("bbs", "zfsdev.root.dom", true);

        [TestMethod,TestCategory("Integration")]
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

        [TestMethod, TestCategory("Integration")]
        public void ListDisksTest()
        {
            var zfs = new Core.Zfs(_remoteProcessCall);
            var disks = zfs.ListDisks();
            Console.WriteLine(disks.Dump(new JsonFormatter()));
        }
    }
}
