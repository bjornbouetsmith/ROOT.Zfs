using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;
using ROOT.Zfs.Public.Arguments;
using ROOT.Zfs.Public.Arguments.Dataset;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Integration
{
    [TestClass]
    public class PropertiesTest
    {
        private readonly IProcessCall _remoteProcessCall = TestHelpers.RequiresRemoteConnection ? new SSHProcessCall("bbs", "zfsdev.root.dom", true) : null;

        private Properties GetProperties()
        {
            var props =  new Properties(_remoteProcessCall);
            props.RequiresSudo = TestHelpers.RequiresSudo;
            return props;
        }

        [TestMethod, TestCategory("Integration")]
        public void ListAllProperties()
        {
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var pr = GetProperties();

            var props = pr.GetProperties(PropertyTarget.Dataset, pool.Name);
            Assert.IsNotNull(props);
            foreach (var prop in props)
            {
                Console.WriteLine(prop.Dump(new JsonFormatter()));
            }
        }

        [TestMethod, TestCategory("Integration")]
        public void SetDataSetProperty()
        {
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var pr = GetProperties();
            var newVal = pr.SetProperty(PropertyTarget.Dataset, pool.Name, "atime", "off");

            Assert.AreEqual("off", newVal.Value);
            Console.WriteLine(newVal.Dump(new JsonFormatter()));
        }

        [TestMethod, TestCategory("Integration")]
        public void GetDataSetProperty()
        {
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var pr = GetProperties();
            var newVal = pr.SetProperty(PropertyTarget.Dataset, pool.Name, "atime", "off");

            Assert.AreEqual("off", newVal.Value);


            newVal = pr.SetProperty(PropertyTarget.Dataset, pool.Name, "atime", "on");
            Assert.AreEqual("on", newVal.Value);

            Console.WriteLine(newVal.Dump(new JsonFormatter()));
        }

        [TestMethod, TestCategory("Integration")]
        public void GetAvailableDatasetProperties()
        {
            var pr = GetProperties();
            var props = pr.GetAvailableProperties(PropertyTarget.Dataset).ToList();
            Assert.IsTrue(props.Count > 0);
            Console.WriteLine(props.Dump(new JsonFormatter()));
        }

        [TestMethod, TestCategory("Integration")]
        public void ResetPropertyTest()
        {
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var pr = GetProperties();
            var dsHelper = new Datasets(_remoteProcessCall);
            dsHelper.RequiresSudo = pr.RequiresSudo;

            var dataset = $"{pool.Name}/{Guid.NewGuid()}";
            var args = new DatasetCreationArgs
            {
                DataSetName = dataset,
                Type = DatasetTypes.Filesystem
            };
            dsHelper.Create(args);
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
                dsHelper.Destroy(dataset, default);
            }
        }
    }
}
