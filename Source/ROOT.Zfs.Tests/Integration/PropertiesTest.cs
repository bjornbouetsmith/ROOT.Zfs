using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Arguments;
using ROOT.Zfs.Public.Arguments.Dataset;
using ROOT.Zfs.Public.Arguments.Properties;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Integration
{
    [TestClass]
    public class PropertiesTest
    {
        private readonly IProcessCall _remoteProcessCall = TestHelpers.RequiresRemoteConnection ? new SSHProcessCall("bbs", "zfsdev.root.dom", true) : null;

        private IProperties GetProperties()
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
            var args = new GetPropertyArgs { PropertyTarget = PropertyTarget.Dataset, Target = pool.Name };
            var props = pr.Get(args);
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
            var args = new SetPropertyArgs { PropertyTarget = PropertyTarget.Dataset, Target = pool.Name, Property = "atime", Value = "off" };
            var newVal = pr.Set(args);
            
            Assert.AreEqual("off", newVal.Value);
            Console.WriteLine(newVal.Dump(new JsonFormatter()));
        }

        [TestMethod, TestCategory("Integration")]
        public void GetDataSetProperty()
        {
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var pr = GetProperties();
            var getArgs = new GetPropertyArgs { PropertyTarget = PropertyTarget.Dataset, Target = pool.Name, Property = "atime"};
            var val = pr.Get(getArgs).First();
            var newPropVal = val.Value == "off" ? "on" : "off";

            var setArgs = new SetPropertyArgs { PropertyTarget = PropertyTarget.Dataset, Target = pool.Name, Property = "atime", Value = newPropVal };
            var newVal = pr.Set(setArgs);
            Assert.AreEqual(newPropVal, newVal.Value);

            Console.WriteLine(newVal.Dump(new JsonFormatter()));
        }

        [TestMethod, TestCategory("Integration")]
        public void GetAvailableDatasetProperties()
        {
            var pr = GetProperties();
            var props = pr.GetAvailable(PropertyTarget.Dataset).ToList();
            Assert.IsTrue(props.Count > 0);
            Console.WriteLine(props.Dump(new JsonFormatter()));
        }

        [TestMethod, TestCategory("Integration")]
        public void ResetPropertyTest()
        {
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var pr = GetProperties();
            IDatasets dsHelper = new Datasets(_remoteProcessCall);
            dsHelper.RequiresSudo = pr.RequiresSudo;

            var dataset = $"{pool.Name}/{Guid.NewGuid()}";
            var args = new DatasetCreationArgs
            {
                DatasetName = dataset,
                Type = DatasetTypes.Filesystem
            };
            dsHelper.Create(args);
            try
            {

                var target = dataset;
                var property = "atime";

                var getArgs = new GetPropertyArgs { PropertyTarget = PropertyTarget.Dataset, Target = target, Property = property };
                var before = pr.Get(getArgs).First();
                Assert.AreEqual("on", before.Value);
                var setArg = new SetPropertyArgs { PropertyTarget = PropertyTarget.Dataset, Target = target, Property = property, Value = "off" };
                var prop = pr.Set(setArg);
                Assert.AreEqual("off", prop.Value);
                var inheritArgs = new InheritPropertyArgs { PropertyTarget = PropertyTarget.Dataset, Target = target, Property = property };
                pr.Reset(inheritArgs);
                getArgs = new GetPropertyArgs { PropertyTarget = PropertyTarget.Dataset, Target = target, Property = property };
                var reset = pr.Get(getArgs).First();
                Assert.AreEqual("on", reset.Value);

            }
            finally
            {
                dsHelper.Destroy(dataset, default);
            }
        }
    }
}
