using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;

namespace ROOT.Zfs.Tests
{
    [TestClass]
    public class ZpoolTest
    {
        RemoteProcessCall pc = new RemoteProcessCall("bbs", "zfsdev.root.dom", true);

        [TestMethod]
        public void GetHistoryTest()
        {
            var history = Zfs.Core.Zfs.ZPool.GetHistory("tank", 0, pc);
            Assert.IsNotNull(history);
            Console.WriteLine(history.Dump(new JsonFormatter()));
        }

        [TestMethod]
        public void GetHistoryTestWithSkip()
        {
            var lines = Core.Zfs.ZPool.GetHistory("tank", 0, pc).ToList().Count;
            var history = Core.Zfs.ZPool.GetHistory("tank", lines - 2, pc).ToList();
            Assert.IsNotNull(history);
            Console.WriteLine(history.Dump(new JsonFormatter()));
            Assert.AreEqual(2, history.Count);
        }
    }
}
