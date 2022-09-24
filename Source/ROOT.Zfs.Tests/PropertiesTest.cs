using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;

namespace ROOT.Zfs.Tests
{
    [TestClass]
    public class PropertiesTest
    {
        RemoteProcessCall pc = new RemoteProcessCall("bbs", "zfsdev.root.dom", true);
        [TestMethod]
        public void ListAllProperties()
        {

            var props = Core.Zfs.Properties.GetProperties("tank/myds", pc);
            foreach (var prop in props)
            {
                Console.WriteLine(prop.Dump(new JsonFormatter()));
            }
        }

        [TestMethod]
        public void SetDataSetProperty()
        {
            
            var newVal = Core.Zfs.Properties.SetProperty("tank/myds", "atime", "off", pc);

            Assert.AreEqual("off", newVal.Value);
            Console.WriteLine(newVal.Dump(new JsonFormatter()));
        }

        [TestMethod]
        public void GetDataSetProperty()
        {
            
            var newVal = Core.Zfs.Properties.SetProperty("tank/myds", "atime", "off", pc);

            Assert.AreEqual("off", newVal.Value);


            newVal = Core.Zfs.Properties.SetProperty("tank/myds", "atime", "on", pc);
            Assert.AreEqual("on", newVal.Value);

            Console.WriteLine(newVal.Dump(new JsonFormatter()));
        }

        [TestMethod]
        public void GetAvailableDatasetProperties()
        {

            var props = Core.Zfs.Properties.GetAvailableDataSetProperties( pc);
            Console.WriteLine(props.Dump(new JsonFormatter()));
        }

        [TestMethod]
        public void ResetPropertyTest()
        {
            var dataset = $"tank/{Guid.NewGuid()}";
            var ds = Core.Zfs.DataSets.CreateDataSet(dataset, null, pc);
            try
            {
                var before = Core.Zfs.Properties.GetProperty(dataset, "atime", pc);
                Assert.AreEqual("on", before.Value);

                Core.Zfs.Properties.SetProperty(dataset, "atime", "off", pc);
                Core.Zfs.Properties.ResetPropertyToInherited(dataset, "atime", pc);
                var reset = Core.Zfs.Properties.GetProperty(dataset, "atime", pc);
                Assert.AreEqual("on",reset.Value);
            }
            finally
            {
                Core.Zfs.DataSets.DeleteDataSet(dataset, pc);
            }
        }
    }
}
