using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Core.Info;

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
    }
}
