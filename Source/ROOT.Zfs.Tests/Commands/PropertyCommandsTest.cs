using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Public.Arguments;

namespace ROOT.Zfs.Tests.Commands
{
    [TestClass]
    public class PropertyCommandsTest
    {
        [TestMethod]
        public void GetAvailablePoolPropertiesTest()
        {
            var command = PropertyCommands.GetPoolProperties();
            Assert.AreEqual("/sbin/zpool get -H", command.FullCommandLine);
        }

        [TestMethod]
        public void GetAvailableDatasetPropertiesTest()
        {
            var command = PropertyCommands.GetDatasetProperties();
            Assert.AreEqual("/sbin/zfs get -H", command.FullCommandLine);
        }

        [TestMethod]
        [DataRow(PropertyTarget.Pool, "tank", "/sbin/zpool get all tank -H")]
        [DataRow(PropertyTarget.Dataset, "tank/myds", "/sbin/zfs get all tank/myds -H")]
        public void GetPropertiesTest(PropertyTarget targetType, string target, string expectedCommand)
        {
            var command = PropertyCommands.GetProperties(targetType, target);
            Assert.AreEqual(expectedCommand, command.FullCommandLine);
        }

        [TestMethod]
        public void ResetPropertyToInheritedTest()
        {
            var command = PropertyCommands.ResetPropertyToInherited("tank/myds", "atime");
            Assert.AreEqual("/sbin/zfs inherit -rS atime tank/myds", command.FullCommandLine);
        }

        [TestMethod]
        [DataRow(PropertyTarget.Pool, "tank", "compression", "off", "/sbin/zpool set compression=off tank")]
        [DataRow(PropertyTarget.Dataset, "tank/myds", "atime", "off", "/sbin/zfs set atime=off tank/myds")]
        public void SetPropertyTest(PropertyTarget targetType, string target, string property, string value, string expectedCommand)
        {
            var command = PropertyCommands.SetProperty(targetType, target, property, value);
            Assert.AreEqual(expectedCommand,command.FullCommandLine);
        }

        [TestMethod]
        [DataRow(PropertyTarget.Pool, "tank", "compression",  "/sbin/zpool get compression tank -H")]
        [DataRow(PropertyTarget.Dataset, "tank/myds", "atime",  "/sbin/zfs get atime tank/myds -H")]
        public void PropertyTest(PropertyTarget targetType, string target, string property, string expectedCommand)
        {
            var command = PropertyCommands.GetProperty(targetType, target, property);
            Assert.AreEqual(expectedCommand, command.FullCommandLine);
        }
    }
}
