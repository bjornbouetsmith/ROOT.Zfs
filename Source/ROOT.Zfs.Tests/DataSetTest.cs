using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core.Commands;

namespace ROOT.Zfs.Tests
{
    [TestClass]
    public class DataSetTest
    {
        RemoteProcessCall pc = new RemoteProcessCall("bbs", "zfsdev.root.dom", true);

        [TestMethod]
        public void GetDataSetList()
        {
            var dataSets = Core.Zfs.GetDataSets(pc);

            foreach (var set in dataSets)
            {
                Console.WriteLine(set.Dump(new JsonFormatter()));
            }
        }
    }
}
