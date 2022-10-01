using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Core.Commands;

namespace ROOT.Zfs.Tests.Commands
{
    [TestClass]
    public class ZpoolCommandTest
    {
        [TestMethod]
        public void GetHistoryCommandTest()
        {
            var command = ZpoolCommands.GetHistory("tank");

            Assert.AreEqual("/sbin/zpool history -l tank", command.FullCommandLine);
        }

        [TestMethod]
        public void GetStatusCommandTest()
        {
            var command = ZpoolCommands.GetStatus("tank");

            Assert.AreEqual("/sbin/zpool status -vP tank", command.FullCommandLine);
        }

        [TestMethod]
        public void GetAllPoolInfosCommand()
        {
            var command = ZpoolCommands.GetAllPoolInfos();

            Assert.AreEqual("/sbin/zpool list -vP", command.FullCommandLine);
        }

        [TestMethod]
        public void GetPoolInfoCommand()
        {
            var command = ZpoolCommands.GetPoolInfo("tank");

            Assert.AreEqual("/sbin/zpool list -vP tank", command.FullCommandLine);
        }
    }
}
