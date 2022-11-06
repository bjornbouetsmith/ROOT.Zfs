using System;
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
            var command = PropertyCommands.Get(new GetPropertyArgs { PropertyTarget = PropertyTarget.Pool });
            Assert.AreEqual("/sbin/zpool get -H", command.FullCommandLine);
        }

        [TestMethod]
        public void GetAvailableDatasetPropertiesTest()
        {
            var command = PropertyCommands.Get(new GetPropertyArgs { PropertyTarget = PropertyTarget.Dataset });
            Assert.AreEqual("/sbin/zfs get -H", command.FullCommandLine);
        }

        [TestMethod]
        [DataRow(PropertyTarget.Pool, "ashift", "tank", "/sbin/zpool get ashift tank -H")]
        [DataRow(PropertyTarget.Dataset, "atime", "tank/myds", "/sbin/zfs get atime tank/myds -H")]
        [DataRow(PropertyTarget.Dataset, "", "tank/myds", "/sbin/zfs get all tank/myds -H")]
        [DataRow(PropertyTarget.Dataset, "atime", "", "/sbin/zfs get atime -H")]
        [DataRow(PropertyTarget.Dataset, "", "", "/sbin/zfs get -H")] // These will not work runtime, since parsing will fail
        [DataRow(PropertyTarget.Pool, "", "", "/sbin/zpool get -H")] // These will not work runime, since parsing will fail
        public void GetPropertiesTest(PropertyTarget targetType, string property, string target, string expectedCommand)
        {
            var args = new GetPropertyArgs { PropertyTarget = targetType, Target = target, Property = property };
            var command = PropertyCommands.Get(args);
            Assert.AreEqual(expectedCommand, command.FullCommandLine);
        }

        [DataRow(PropertyTarget.Dataset, "atime", "tank/myds", "/sbin/zfs inherit -rS atime tank/myds", false)]
        [DataRow(PropertyTarget.Pool, "atime", "tank/myds", "/sbin/zfs inherit -rS atime tank/myds", true)]
        [TestMethod]
        public void ResetPropertyToInheritedTest(PropertyTarget targetType, string property, string target, string expectedCommand, bool expectException)
        {
            var args = new InheritPropertyArgs { PropertyTarget = targetType, Property = property, Target = target };
            if (expectException)
            {
                Assert.ThrowsException<ArgumentException>(() => PropertyCommands.Inherit(args));
            }
            else
            {
                var command = PropertyCommands.Inherit(args);
                Assert.AreEqual(expectedCommand, command.FullCommandLine);
            }
        }

        [TestMethod]
        [DataRow(PropertyTarget.Pool, "tank", "compression", "off", "/sbin/zpool set compression=off tank", false)]
        [DataRow(PropertyTarget.Dataset, "tank/myds", "atime", "off", "/sbin/zfs set atime=off tank/myds", false)]
        [DataRow(PropertyTarget.Dataset, "", "atime", "off", "/sbin/zfs set atime=off tank/myds", true)]
        public void SetPropertyTest(PropertyTarget targetType, string target, string property, string value, string expectedCommand, bool expectException)
        {
            var args = new SetPropertyArgs
            {
                PropertyTarget = targetType,
                Property = property,
                Target = target,
                Value = value
            };
            if (expectException)
            {
                Assert.ThrowsException<ArgumentException>(() => PropertyCommands.Set(args));
            }
            else
            {

                var command = PropertyCommands.Set(args);
                Console.WriteLine(command.FullCommandLine);
                Assert.AreEqual(expectedCommand, command.FullCommandLine);
            }
        }
    }
}
