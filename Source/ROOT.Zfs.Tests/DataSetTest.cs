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
        [DataRow(true)]
        [DataRow(false)]
        public void CreateDataSetTest(bool addProperties)
        {

            var dataSetName = Guid.NewGuid().ToString();
            string fullName = null;
            try
            {
                var quota = Core.Info.DataSetProperties.Lookup("quota");
                var compression = Core.Info.DataSetProperties.Lookup("compression");
                var props = new[]
                    {
                        new PropertyValue(quota.Name, PropertySources.Local.Name, "1G"),
                        new PropertyValue(compression.Name, PropertySources.Local.Name, "gzip")
                    };
                var dataSets = Core.Zfs.DataSets.GetDataSets(pc);
                var parent = dataSets.FirstOrDefault(ds => ds.Name == "tank");
                Assert.IsNotNull(parent);
                Console.WriteLine(parent.Dump(new JsonFormatter()));
                fullName = DataSetHelper.CreateDataSetName(parent.Name, dataSetName);
                var dataSet = Core.Zfs.DataSets.CreateDataSet(fullName, addProperties ? props : null, pc);
                Assert.IsNotNull(dataSet);
                Console.WriteLine(dataSet.Dump(new JsonFormatter()));
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

        [TestMethod]
        public void GetDataSetShouldReturnDataSet()
        {
            var root = Core.Zfs.DataSets.GetDataSet("tank", pc);

            Assert.IsNotNull(root);
            Console.WriteLine(root.Dump(new JsonFormatter()));
        }

        [TestMethod]
        public void GetNonExistingDataSetShouldReturnNull()
        {
            var ds = Core.Zfs.DataSets.GetDataSet("ungabunga" + Guid.NewGuid(), pc);
            Assert.IsNull(ds);
        }
    }
}
