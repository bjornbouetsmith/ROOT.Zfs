using System;
using System.Linq;
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
            var dataSets = ds.GetDatasets(DatasetType.NotSet);
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
            var root = ds.GetDatasets("tank", DatasetType.NotSet, false).FirstOrDefault();

            Assert.IsNotNull(root);
            Assert.AreEqual("tank", root.Name);
            Assert.AreEqual(DatasetType.Filesystem, root.Type);
            Console.WriteLine(root.Dump(new JsonFormatter()));
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void GetNonExistingDataSetShouldThrowException()
        {
            var ds = new Datasets(CreateProcessCall());
            Assert.ThrowsException<ProcessCallException>(()=> ds.GetDatasets("ungabunga", DatasetType.NotSet, false).FirstOrDefault());
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
