using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;

namespace ROOT.Zfs.Tests.Integration.Fake
{
    [TestClass]
    public class FakeDataSetTest
    {
        readonly IProcessCall _remoteProcessCall = new FakeRemoteConnection("2.1.5-2");

        [TestMethod, TestCategory("FakeIntegration")]
        public void GetDataSetList()
        {
            var ds = new DataSets(_remoteProcessCall);
            var dataSets = ds.GetDataSets();
            Assert.IsNotNull(dataSets);
            foreach (var set in dataSets)
            {
                Console.WriteLine(set.Dump(new JsonFormatter()));
            }
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void GetDataSetShouldReturnDataSet()
        {
            var ds = new DataSets(_remoteProcessCall);
            var root = ds.GetDataSet("tank");

            Assert.IsNotNull(root);
            Assert.AreEqual("tank", root.Name);
            Console.WriteLine(root.Dump(new JsonFormatter()));
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void GetNonExistingDataSetShouldReturnNull()
        {
            var ds = new DataSets(new FakeRemoteConnection("2.1.5-2") );
            var dataset = ds.GetDataSet("ungabunga");
            Assert.IsNull(dataset);
        }
    }
}
