using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;

namespace ROOT.Zfs.Tests.Integration
{
    [TestClass]
    public class ZfsTest
    {
        readonly SSHProcessCall _remoteProcessCall = new("bbs", "zfsdev.root.dom", true);
        [TestMethod,TestCategory("Integration")]
        public void InitializeTest()
        {
            var zfs = new Core.Zfs(_remoteProcessCall);
            zfs.Initialize();
            Assert.IsTrue(zfs.Initialized);
        }
    }
}
