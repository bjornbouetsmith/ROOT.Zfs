using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Core.Commands;

namespace ROOT.Zfs.Tests.Commands
{
    [TestClass]
    public class ZdbCommandsTest
    {
        [TestMethod]
        public void GetRawZdbOutputCommandTest()
        {
            var command = ZdbCommands.GetRawZdbOutput();
            Assert.AreEqual("/sbin/zdb", command.FullCommandLine);
        }
    }
}
