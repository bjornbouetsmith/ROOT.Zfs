using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Integration.Fake
{
    [TestClass]
    public class SmartInfoTest
    {
        readonly IProcessCall _remoteProcessCall = new FakeRemoteConnection("2.1.5-2");

        [TestMethod, TestCategory("FakeIntegration")]
        public void GetSmartInfosTest()
        {
            var zfs = new Core.Zfs(_remoteProcessCall);
            var infos = zfs.GetSmartInfos().ToList();

            Assert.IsNotNull(infos);
            Assert.AreEqual(2, infos.Count);
        }

        [TestMethod]
        public void GetSmartInfoTest()
        {
            var zfs = new Core.Zfs(_remoteProcessCall);
            var disks = zfs.ListDisks();

            foreach (var disk in disks.Where(d => d.Type == DeviceType.Disk))
            {
                var info = zfs.GetSmartInfo(disk.Id);
                if (info.DeviceId.EndsWith("5"))
                {
                    // FAKE returns failed status
                    Assert.IsFalse(info.DataSection.Status == "PASSED");
                }
                else
                {
                    Assert.IsTrue(info.DataSection.Status == "PASSED");
                }
            }
        }
    }
}
