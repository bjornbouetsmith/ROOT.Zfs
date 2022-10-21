using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.Datasets;

namespace ROOT.Zfs.Tests.Integration.Fake.DataSet
{
    public abstract class FakeDataSetTest
    {
        internal abstract FakeRemoteConnection CreateProcessCall();
        [TestMethod, TestCategory("FakeIntegration")]
        public void GetDataSetList()
        {
            var ds = new Datasets(CreateProcessCall());
            var dataSets = ds.GetDatasets(default);
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
            var root = ds.GetDatasets("tank", default, false).FirstOrDefault();

            Assert.IsNotNull(root);
            Assert.AreEqual("tank", root.Name);
            Assert.AreEqual(DatasetTypes.Filesystem, root.Type);
            Console.WriteLine(root.Dump(new JsonFormatter()));
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void GetNonExistingDataSetShouldThrowException()
        {
            var ds = new Datasets(CreateProcessCall());
            Assert.ThrowsException<ProcessCallException>(() => ds.GetDatasets("ungabunga", default, false).FirstOrDefault());
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void CreateDateSetTest()
        {
            var ds = new Datasets(CreateProcessCall());
            var args = new DatasetCreationArgs
            {
                DataSetName = "tank/myds",
                Type = DatasetTypes.Filesystem,
                Properties = new[] { new PropertyValue { Property = "atime", Value = "off" } }

            };
            var dataset = ds.CreateDataset(args);
            Assert.IsNotNull(dataset);
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void DestroyDataSetTest()
        {
            var ds = new Datasets(CreateProcessCall());
            var response = ds.DestroyDataset("tank/myds", DatasetDestroyFlags.Recursive);
            Assert.AreEqual(DatasetDestroyFlags.Recursive, response.Flags);
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void PromoteDatasetTest()
        {
            var processCall = CreateProcessCall();
            var ds = new Datasets(processCall);
            ds.Promote("tank/myds");
            var commands = processCall.GetCommandsInvoked();
            Assert.AreEqual(1,commands.Count);
            Assert.AreEqual("/sbin/zfs promote tank/myds", commands[0]);
        }
    }
}
