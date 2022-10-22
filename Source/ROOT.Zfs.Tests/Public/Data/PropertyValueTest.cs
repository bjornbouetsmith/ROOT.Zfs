using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Public.Data
{
    [TestClass]
    public class PropertyValueTest
    {

        [TestMethod]
        [DataRow("atime", "off", "noatime")]
        [DataRow("atime", "on", "atime")]
        [DataRow("canmount", "off", "noauto")]
        [DataRow("canmount", "on", "auto")]
        [DataRow("devices", "off", "nodev")]
        [DataRow("devices", "on", "dev")]
        [DataRow("exec", "off", "noexec")]
        [DataRow("exec", "on", "exec")]
        [DataRow("readonly", "off", "rw")]
        [DataRow("readonly", "on", "ro")]
        [DataRow("realatime", "off", "norealatime")]
        [DataRow("realatime", "on", "realatime")]
        [DataRow("setuid", "off", "nosuid")]
        [DataRow("setuid", "on", "suid")]
        [DataRow("xattr", "off", "noxattr")]
        [DataRow("xattr", "on", "xattr")]
        [DataRow("nbmand", "off", "nomand")]
        [DataRow("nbmand", "on", "mand")]
        [DataRow("context", "bbs", "context=bbs")]
        [DataRow("fscontext", "bbs", "fscontext=bbs")]
        [DataRow("defcontext", "bbs", "defcontext=bbs")]
        [DataRow("rootcontext", "bbs", "rootcontext=bbs")]
        [DataRow("canmount", "", "")]
        [DataRow("canmount", null, "")]
        [DataRow("", "off", "")]
        [DataRow(null, "off", "")]
        [DataRow(null, null, "")]
        [DataRow(null, "", "")]
        [DataRow("", null, "")]
        [DataRow("", "", "")]
        [DataRow("vscan", "off", "")]
        public void ToMountArgumentTest(string property, string value, string expectedMountArg)
        {
            var prop = new PropertyValue { Property = property, Value = value };
            var mountArg = prop.ToMountArgument();
            Assert.AreEqual(expectedMountArg, mountArg);
        }
    }
}
