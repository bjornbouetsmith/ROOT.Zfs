using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.Datasets;

namespace ROOT.Zfs.Tests.Integration
{
    [TestClass]
    public class PropertiesTest
    {
        private readonly SSHProcessCall _remoteProcessCall = new("bbs", "zfsdev.root.dom", true);

        [TestMethod, TestCategory("Integration")]
        public void ListAllProperties()
        {
            var pr = new Properties(_remoteProcessCall);

            var props = pr.GetProperties(PropertyTarget.Dataset, "tank/myds");
            Assert.IsNotNull(props);
            foreach (var prop in props)
            {
                Console.WriteLine(prop.Dump(new JsonFormatter()));
            }
        }

        [TestMethod, TestCategory("Integration")]
        public void SetDataSetProperty()
        {
            var pr = new Properties(_remoteProcessCall);
            var newVal = pr.SetProperty(PropertyTarget.Dataset, "tank/myds", "atime", "off");

            Assert.AreEqual("off", newVal.Value);
            Console.WriteLine(newVal.Dump(new JsonFormatter()));
        }

        [TestMethod, TestCategory("Integration")]
        public void GetDataSetProperty()
        {
            var pr = new Properties(_remoteProcessCall);
            var newVal = pr.SetProperty(PropertyTarget.Dataset, "tank/myds", "atime", "off");

            Assert.AreEqual("off", newVal.Value);


            newVal = pr.SetProperty(PropertyTarget.Dataset, "tank/myds", "atime", "on");
            Assert.AreEqual("on", newVal.Value);

            Console.WriteLine(newVal.Dump(new JsonFormatter()));
        }

        [TestMethod, TestCategory("Integration")]
        public void GetAvailableDatasetProperties()
        {
            var pr = new Properties(_remoteProcessCall);
            var props = pr.GetAvailableProperties(PropertyTarget.Dataset).ToList();
            Assert.IsTrue(props.Count > 0);
            Console.WriteLine(props.Dump(new JsonFormatter()));
        }

        [TestMethod, TestCategory("Integration")]
        public void ResetPropertyTest()
        {
            var pr = new Properties(_remoteProcessCall);
            var dsHelper = new Datasets(_remoteProcessCall);
            var dataset = $"tank/{Guid.NewGuid()}";
            var args = new DatasetCreationArgs
            {
                DataSetName = dataset,
                Type = DatasetType.Filesystem
            };
            dsHelper.CreateDataset(args);
            try
            {
                var before = pr.GetProperty(PropertyTarget.Dataset, dataset, "atime");
                Assert.AreEqual("on", before.Value);

                pr.SetProperty(PropertyTarget.Dataset, dataset, "atime", "off");
                pr.ResetPropertyToInherited(dataset, "atime");
                var reset = pr.GetProperty(PropertyTarget.Dataset, dataset, "atime");
                Assert.AreEqual("on", reset.Value);
            }
            finally
            {
                dsHelper.DestroyDataset(dataset, default);
            }
        }
    }
}
