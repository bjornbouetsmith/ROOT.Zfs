using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Integration.Fake.DataSet
{
    public abstract class FakeDataSetTest
    {
        protected abstract IProcessCall CreateProcessCall();
        [TestMethod, TestCategory("FakeIntegration")]
        public void GetDataSetList()
        {
            var ds = new Datasets(CreateProcessCall());
            var dataSets = ds.GetDatasets();
            Assert.IsNotNull(dataSets);
            foreach (var set in dataSets)
            {
                Console.WriteLine(set.Dump(new JsonFormatter()));
            }
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void GetDataSetShouldReturnDataSet()
        {
            var ds = new Datasets(CreateProcessCall());
            var root = ds.GetDataset("tank");

            Assert.IsNotNull(root);
            Assert.AreEqual("tank", root.Name);
            Console.WriteLine(root.Dump(new JsonFormatter()));
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void GetNonExistingDataSetShouldReturnNull()
        {
            var ds = new Datasets(CreateProcessCall());
            var dataset = ds.GetDataset("ungabunga");
            Assert.IsNull(dataset);
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void CreateDateSetTest()
        {
            var ds = new Datasets(CreateProcessCall());
            var dataset = ds.CreateDataset("tank/myds", new[] { new PropertyValue { Property = "atime", Value = "off" } });
            Assert.IsNotNull(dataset);
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void DestroyDataSetTest()
        {
            var ds = new Datasets(CreateProcessCall());
            var response = ds.DestroyDataset("tank/myds", Public.DatasetDestroyFlags.Recursive);
            Assert.AreEqual(Public.DatasetDestroyFlags.Recursive, response.Flags);
        }
    }
}
