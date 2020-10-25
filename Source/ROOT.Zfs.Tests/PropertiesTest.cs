using System;
using System.Collections.Generic;
using System.Text;
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
    }
}
