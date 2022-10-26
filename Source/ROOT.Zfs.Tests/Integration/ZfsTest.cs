using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;

namespace ROOT.Zfs.Tests.Integration
{
    [TestClass]
    public class ZfsTest
    {
        private readonly IProcessCall _remoteProcessCall = Environment.MachineName == "BBS-DESKTOP" ? new SSHProcessCall("bbs", "zfsdev.root.dom", true) : new ProcessCall("/usr/bin/sudo");
        [TestMethod,TestCategory("Integration")]
        public void InitializeTest()
        {
            var zfs = new Core.Zfs(_remoteProcessCall);
            zfs.Initialize();
            Assert.IsTrue(zfs.Initialized);
        }
    }
}
