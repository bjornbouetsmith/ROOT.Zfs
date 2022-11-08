using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Arguments.Dataset;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Integration
{
    [TestClass]
    public class DataSetTest
    {
        private readonly IProcessCall _remoteProcessCall = TestHelpers.RequiresRemoteConnection ? new SSHProcessCall("bbs", "zfsdev.root.dom", true) : null;

        private IDatasets GetDataSets()
        {
            var ds = new Datasets(_remoteProcessCall);
            ds.RequiresSudo = TestHelpers.RequiresSudo;
            return ds;
        }

        [TestMethod, TestCategory("Integration")]
        public void GetDataSetList()
        {
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);

            var ds = GetDataSets();
            var dataSets = ds.List(new DatasetListArgs());
            Assert.IsNotNull(dataSets);
            foreach (var set in dataSets)
            {
                Console.WriteLine(set.Dump(new JsonFormatter()));
            }
        }

        [TestMethod, TestCategory("Integration")]
        public void ListDataSetTest()
        {
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var ds = GetDataSets();
            var args = new DatasetListArgs { Root = pool.Name };
            var dataset = ds.List(args).FirstOrDefault();
            Assert.IsNotNull(dataset);
            Assert.AreEqual(pool.Name, dataset.Name);
        }

        [TestMethod, TestCategory("Integration")]
        [DataRow(true)]
        [DataRow(false)]
        public void CreateDataSetTest(bool addProperties)
        {
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var ds = GetDataSets();
            var dataSetName = Guid.NewGuid().ToString();
            string fullName = null;
            try
            {
                var props = new[]
                    {
                        new PropertyValue { Property = "quota", Value = "1G" },
                        new PropertyValue { Property = "compression", Value = "gzip" }
                    };

                var dataSets = ds.List(new DatasetListArgs()).ToList();
                var parent = dataSets.FirstOrDefault(d => d.Name == pool.Name);
                Assert.IsNotNull(parent);
                Console.WriteLine(parent.Dump(new JsonFormatter()));
                fullName = DatasetHelper.CreateDatasetName(parent.Name, dataSetName);
                var args = new DatasetCreationArgs
                {
                    DataSetName = fullName,
                    Type = DatasetTypes.Filesystem,
                    Properties = addProperties ? props : null
                };
                var dataSet = ds.Create(args);
                Assert.IsNotNull(dataSet);
                Console.WriteLine(dataSet.Dump(new JsonFormatter()));
            }
            finally
            {
                // Check to prevent issues in case dataset creation failed
                if (fullName != null)
                {
                    ds.Destroy(fullName, default);
                }
            }
        }

        [TestMethod, TestCategory("Integration")]
        public void ListDataSetShouldReturnDataSet()
        {
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var ds = GetDataSets();
            var args = new DatasetListArgs { Root = pool.Name };
            var root = ds.List(args).FirstOrDefault();

            Assert.IsNotNull(root);
            Console.WriteLine(root.Dump(new JsonFormatter()));
        }

        [TestMethod, TestCategory("Integration")]
        public void GetNonExistingDataSetShouldThrowException()
        {
            var ds = GetDataSets();
            Assert.ThrowsException<ProcessCallException>(() => ds.List(new DatasetListArgs { Root = "ungabunga" }).FirstOrDefault());
        }

        [TestMethod, TestCategory("Integration")]
        public void DestroyRecursiveDryRunTest()
        {
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var dsHelper = GetDataSets();
            var rootId = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var root = $"{pool.Name}/myds/{rootId}";
            var child1 = $"{pool.Name}/myds/{rootId}/child1";
            var child2 = $"{pool.Name}/myds/{rootId}/child2";
            var granchild1 = $"{pool.Name}/myds/{rootId}/child1/granchild";
            var datasets = new[] { root, child1, child2, granchild1 };

            try
            {

                foreach (var ds in datasets)
                {
                    var args = new DatasetCreationArgs
                    {
                        DataSetName = ds,
                        Type = DatasetTypes.Filesystem,
                        CreateParents = true,

                    };
                    dsHelper.Create(args);
                }

                var flags = DatasetDestroyFlags.Recursive | DatasetDestroyFlags.DryRun;
                var response = dsHelper.Destroy(root, flags);
                Assert.AreEqual(flags, response.Flags);
                var lines = response.DryRun.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(l => l.Replace("\t", " ")).ToList();
                Assert.AreEqual(4, lines.Count);
                Assert.IsTrue(lines.Contains($"destroy {pool.Name}/myds/{rootId}/child1/granchild"));
                Assert.IsTrue(lines.Contains($"destroy {pool.Name}/myds/{rootId}/child1"));
                Assert.IsTrue(lines.Contains($"destroy {pool.Name}/myds/{rootId}/child2"));
                Assert.IsTrue(lines.Contains($"destroy {pool.Name}/myds/{rootId}"));

                Console.WriteLine(response.DryRun);
            }
            finally
            {
                try
                {
                    dsHelper.Destroy(root, DatasetDestroyFlags.Recursive);
                }
                catch
                {
                    //We dont care if cleaning up fails
                }
            }

        }
    }
}
