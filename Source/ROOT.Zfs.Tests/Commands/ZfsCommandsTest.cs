using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Core.Commands;

namespace ROOT.Zfs.Tests.Commands
{
    [TestClass]
    public class ZfsCommandsTest
    {
        [TestMethod]
        public void GetVersionCommandTest()
        {
            var command = ZfsCommands.GetVersion();
            Assert.AreEqual("/sbin/zfs --version", command.FullCommandLine);
        }
    }
}
