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
            RemoteProcessCall pc = new RemoteProcessCall("bbs", "zfsdev.root.dom", true);

            var remote = pc | Properties.ProcessCalls.SetProperty("tank/myds", DataSetProperties.Lookup("atime"), "off");
            var response = remote.LoadResponse();
            Assert.IsTrue(response.Success);

        }

        [TestMethod]
        public void GetDataSetProperty()
        {
            RemoteProcessCall pc = new RemoteProcessCall("bbs", "zfsdev.root.dom", true);

            var property = DataSetProperties.Lookup("atime");
            var remote = pc | Properties.ProcessCalls.SetProperty("tank/myds", property, "off");
            Console.WriteLine(remote.FullCommandLine);

            var response = remote.LoadResponse();
            Assert.IsTrue(response.Success);

            remote = pc | Properties.ProcessCalls.GetProperty("tank/myds", property);
            response = remote.LoadResponse();
            Assert.IsTrue(response.Success);

            remote = pc | Properties.ProcessCalls.SetProperty("tank/myds", property, "on");
            response = remote.LoadResponse();
            Assert.IsTrue(response.Success);

            remote = pc | Properties.ProcessCalls.GetProperty("tank/myds", property);
            response = remote.LoadResponse();
            Assert.IsTrue(response.Success);

            var props = DataSetProperties.FromStdOutput(response.StdOut);
            var std = props.Dump(new JsonFormatter());
            var prop = props.FirstOrDefault(p => p.Property == property);

            Console.WriteLine(std);
            std = prop.Dump(new JsonFormatter());
            Console.WriteLine(std);
            Assert.IsNotNull(prop);
            Assert.AreEqual("on", prop.Value);

        }
    }
}
