using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests
{
    [TestClass]
    public class DataSetTest
    {
        readonly SSHProcessCall _remoteProcessCall = new("bbs", "zfsdev.root.dom", true);

        [TestMethod, TestCategory("Integration")]
        public void GetDataSetList()
        {
            var ds = new Datasets(_remoteProcessCall);
            var dataSets = ds.GetDatasets();
            Assert.IsNotNull(dataSets);
            foreach (var set in dataSets)
            {
                Console.WriteLine(set.Dump(new JsonFormatter()));
            }
        }

        [TestMethod, TestCategory("Integration")]
        public void GetDataSetTest()
        {
            var ds = new Datasets(_remoteProcessCall);
            var dataset = ds.GetDataset("tank");
            Assert.IsNotNull(dataset);
            Assert.AreEqual("tank", dataset.Name);
        }

        [TestMethod, TestCategory("Integration")]
        [DataRow(true)]
        [DataRow(false)]
        public void CreateDataSetTest(bool addProperties)
        {
            var ds = new Datasets(_remoteProcessCall);
            var dataSetName = Guid.NewGuid().ToString();
            string fullName = null;
            try
            {
                var props = new[]
                    {
                        new PropertyValue { Property = "quota", Value = "1G" },
                        new PropertyValue { Property = "compression", Value = "gzip" }
                    };
                var dataSets = ds.GetDatasets().ToList();
                var parent = dataSets.FirstOrDefault(ds => ds.Name == "tank");
                Assert.IsNotNull(parent);
                Console.WriteLine(parent.Dump(new JsonFormatter()));
                fullName = DatasetHelper.CreateDatasetName(parent.Name, dataSetName);
                var dataSet = ds.CreateDataset(fullName, addProperties ? props : null);
                Assert.IsNotNull(dataSet);
                Console.WriteLine(dataSet.Dump(new JsonFormatter()));
            }
            finally
            {
                // Check to prevent issues in case dataset creation failed
                if (fullName != null)
                {
                    ds.DestroyDataset(fullName, default);
                }
            }
        }

        [TestMethod, TestCategory("Integration")]
        public void GetDataSetShouldReturnDataSet()
        {
            var ds = new Datasets(_remoteProcessCall);
            var root = ds.GetDataset("tank");

            Assert.IsNotNull(root);
            Console.WriteLine(root.Dump(new JsonFormatter()));
        }

        [TestMethod, TestCategory("Integration")]
        public void GetNonExistingDataSetShouldReturnNull()
        {
            var ds = new Datasets(_remoteProcessCall);
            var dataset = ds.GetDataset("ungabunga" + Guid.NewGuid());
            Assert.IsNull(dataset);
        }

        [TestMethod, TestCategory("Integration")]
        public void DestroyRecursiveDryRunTest()
        {
            var dsHelper = new Datasets(_remoteProcessCall);
            var rootId = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var root = $"tank/myds/{rootId}";
            var child1 = $"tank/myds/{rootId}/child1";
            var child2 = $"tank/myds/{rootId}/child2";
            var granchild1 = $"tank/myds/{rootId}/child1/granchild";
            var datasets = new[] { root, child1, child2, granchild1 };

            try
            {

                foreach (var ds in datasets)
                {
                    dsHelper.CreateDataset(ds, null);
                }

                var flags = DatasetDestroyFlags.Recursive | DatasetDestroyFlags.DryRun;
                var response = dsHelper.DestroyDataset(root, flags);
                Assert.AreEqual(flags, response.Flags);
                var lines = response.DryRun.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(l => l.Replace("\t", " ")).ToList();
                Assert.AreEqual(4, lines.Count);
                Assert.IsTrue(lines.Contains($"destroy tank/myds/{rootId}/child1/granchild"));
                Assert.IsTrue(lines.Contains($"destroy tank/myds/{rootId}/child1"));
                Assert.IsTrue(lines.Contains($"destroy tank/myds/{rootId}/child2"));
                Assert.IsTrue(lines.Contains($"destroy tank/myds/{rootId}"));

                Console.WriteLine(response.DryRun);
            }
            finally
            {
                try
                {
                    dsHelper.DestroyDataset(root, DatasetDestroyFlags.Recursive);
                }
                catch
                {
                    //We dont care if cleaning up fails
                }
            }

        }
    }
}
