using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;
using ROOT.Zfs.Public;

namespace ROOT.Zfs.Tests.Integration.Fake
{
    [TestClass]
    public class FakePropertiesTest
    {
        private readonly IProcessCall _remoteProcessCall = new FakeRemoteConnection("2.1.5-2");

        [TestMethod, TestCategory("FakeIntegration")]
        public void ListAllProperties()
        {
            var pr = new Properties(_remoteProcessCall);

            var props = pr.GetProperties(PropertyTarget.Dataset,"tank/myds");
            Assert.IsNotNull(props);
            foreach (var prop in props)
            {
                Console.WriteLine(prop.Dump(new JsonFormatter()));
            }
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void SetDataSetProperty()
        {
            var pr = new Properties(_remoteProcessCall);
            var newVal = pr.SetProperty(PropertyTarget.Dataset,"tank/myds", "atime", "off");

            Assert.AreEqual("off", newVal.Value);
            Console.WriteLine(newVal.Dump(new JsonFormatter()));
        }

        /// <summary>
        /// We cannot test that setting works with the current fake implementation.
        /// </summary>
        [TestMethod, TestCategory("FakeIntegration")]
        public void GetDataSetProperty()
        {
            var pr = new Properties(_remoteProcessCall);
            var newVal = pr.SetProperty(PropertyTarget.Dataset,"tank/myds", "atime", "off");

            Assert.AreEqual("off", newVal.Value);
            
            newVal = pr.SetProperty(PropertyTarget.Dataset,"tank/myds", "atime", "on");
            
            Console.WriteLine(newVal.Dump(new JsonFormatter()));
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void GetAvailableDatasetProperties()
        {
            var pr = new Properties(_remoteProcessCall);
            var props = pr.GetAvailableProperties(PropertyTarget.Dataset).ToList();
            Assert.IsTrue(props.Count > 0);
            Console.WriteLine(props.Dump(new JsonFormatter()));
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void GetAvailablePoolProperties()
        {
            var pr = new Properties(_remoteProcessCall);
            var props = pr.GetAvailableProperties(PropertyTarget.Pool).ToList();
            Assert.IsTrue(props.Count > 0);
            Console.WriteLine(props.Dump(new JsonFormatter()));
        }

        /// <summary>
        /// We cannot test that reset works with the current fake implementation.
        /// </summary>
        [TestMethod, TestCategory("FakeIntegration")]
        public void ResetPropertyTest()
        {
            var pr = new Properties(_remoteProcessCall);

            var before = pr.GetProperty(PropertyTarget.Dataset,"tank/myds", "atime");
            Assert.AreEqual("off", before.Value);

            pr.SetProperty(PropertyTarget.Dataset,"tank/myds", "atime", "on");
            pr.ResetPropertyToInherited("tank/myds", "atime");
        }
    }
}
