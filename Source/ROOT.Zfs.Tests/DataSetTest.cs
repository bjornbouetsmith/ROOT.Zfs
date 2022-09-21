using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core.Info;

namespace ROOT.Zfs.Tests
{
    [TestClass]
    public class DataSetTest
    {
        RemoteProcessCall pc = new RemoteProcessCall("bbs", "zfsdev.root.dom", true);

        [TestMethod]
        public void GetDataSetList()
        {
            var dataSets = Core.Zfs.DataSets.GetDataSets(pc);

            foreach (var set in dataSets)
            {
                Console.WriteLine(set.Dump(new JsonFormatter()));
            }
        }

        [TestMethod]
        public void CreateDataSetTest()
        {

            var dataSetName = Guid.NewGuid().ToString();
            string fullName = null;
            try
            {
                var dataSets = Core.Zfs.DataSets.GetDataSets(pc);
                var parent = dataSets.FirstOrDefault(ds => ds.Name == "tank");
                Assert.IsNotNull(parent);
                Console.WriteLine(parent.Dump(new JsonFormatter()));
                var dataSet = Core.Zfs.DataSets.CreateDataSet(parent, dataSetName, pc);
                Assert.IsNotNull(dataSet);
                Console.WriteLine(dataSet.Dump(new JsonFormatter()));
                fullName = DataSetHelper.CreateDataSetName(parent.Name, dataSetName);

            }
            finally
            {
                // Check to prevent issues in case dataset creation failed
                if (fullName != null)
                {
                    Core.Zfs.DataSets.DeleteDataSet(fullName, pc);
                }
            }
        }
    }
}
