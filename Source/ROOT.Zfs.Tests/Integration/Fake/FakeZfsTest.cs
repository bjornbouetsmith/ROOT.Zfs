using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Public;

namespace ROOT.Zfs.Tests.Integration.Fake
{
    [TestClass]
    public class FakeZfsTest
    {
        readonly FakeRemoteConnection _remoteProcessCall = new ("2.1.5-2");

        private IZfs GetZfs()
        {
            return new Core.Zfs(_remoteProcessCall);
        }

        [TestMethod,TestCategory("FakeIntegration")]
        public void GetVersionInfo()
        {
            var zfs = GetZfs();
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
            var zfs = GetZfs();
            var disks = zfs.ListDisks();
            Assert.IsNotNull(disks);
            Assert.IsTrue(disks.Any());
            Console.WriteLine(disks.Dump(new JsonFormatter()));
        }

        // This test should be re-written to so we can assert properly that Smartctl does not get overriden with nothing
        // But it requires a non-static BaseCommands - otherwise one test can ruin the binaries of other tests
        [TestMethod, TestCategory("FakeIntegration")]
        public void InitializeTest()
        {
            var zfs = GetZfs();
            zfs.Initialize();

            var commandsInvoked = _remoteProcessCall.GetCommandsInvoked();

            Assert.AreEqual(6, commandsInvoked.Count);
            var asHashSet = commandsInvoked.ToHashSet();
            Assert.IsTrue(asHashSet.Contains("/bin/which zfs"));
            Assert.IsTrue(asHashSet.Contains("/bin/which zpool"));
            Assert.IsTrue(asHashSet.Contains("/bin/which zdb"));
            Assert.IsTrue(asHashSet.Contains("/bin/which ls"));
            Assert.IsTrue(asHashSet.Contains("/bin/which lsblk"));
            Assert.IsTrue(asHashSet.Contains("/bin/which smartctl"));
            
            // our fake cannot find smartctl, so assert that it does not get overriden
            Assert.AreEqual("/sbin/smartctl", Core.Commands.Commands.WhichSmartctl);
        }
    }
}
