using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests
{
    [TestClass]
    public class DataSetTest
    {
        readonly RemoteProcessCall _remoteProcessCall = new("bbs", "zfsdev.root.dom", true);

        [TestMethod, TestCategory("Integration")]
        public void GetDataSetList()
        {
            var ds = new DataSets(_remoteProcessCall);
            var dataSets = ds.GetDataSets();

            foreach (var set in dataSets)
            {
                Console.WriteLine(set.Dump(new JsonFormatter()));
            }
        }

        [TestMethod, TestCategory("Integration")]
        [DataRow(true)]
        [DataRow(false)]
        public void CreateDataSetTest(bool addProperties)
        {
            var ds = new DataSets(_remoteProcessCall);
            var dataSetName = Guid.NewGuid().ToString();
            string fullName = null;
            try
            {
                var quota = DataSetProperties.Lookup("quota");
                var compression = DataSetProperties.Lookup("compression");
                var props = new[]
                    {
                        new PropertyValue(quota.Name, PropertySources.Local.Name, "1G"),
                        new PropertyValue(compression.Name, PropertySources.Local.Name, "gzip")
                    };
                var dataSets = ds.GetDataSets();
                var parent = dataSets.FirstOrDefault(ds => ds.Name == "tank");
                Assert.IsNotNull(parent);
                Console.WriteLine(parent.Dump(new JsonFormatter()));
                fullName = DataSetHelper.CreateDataSetName(parent.Name, dataSetName);
                var dataSet = ds.CreateDataSet(fullName, addProperties ? props : null);
                Assert.IsNotNull(dataSet);
                Console.WriteLine(dataSet.Dump(new JsonFormatter()));
            }
            finally
            {
                // Check to prevent issues in case dataset creation failed
                if (fullName != null)
                {
                    ds.DestroyDataSet(fullName);
                }
            }
        }

        [TestMethod, TestCategory("Integration")]
        public void GetDataSetShouldReturnDataSet()
        {
            var ds = new DataSets(_remoteProcessCall);
            var root = ds.GetDataSet("tank");

            Assert.IsNotNull(root);
            Console.WriteLine(root.Dump(new JsonFormatter()));
        }

        [TestMethod, TestCategory("Integration")]
        public void GetNonExistingDataSetShouldReturnNull()
        {
            var ds = new DataSets(_remoteProcessCall);
            var dataset = ds.GetDataSet("ungabunga" + Guid.NewGuid());
            Assert.IsNull(dataset);
        }
    }
}
