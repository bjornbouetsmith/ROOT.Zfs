using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Commands
{
    [TestClass]
    public class DataSetCommandsTest
    {

        [DataRow(DatasetDestroyFlags.None, "/sbin/zfs destroy tank/myds")]
        [DataRow(DatasetDestroyFlags.Recursive, "/sbin/zfs destroy -r tank/myds")]
        [DataRow(DatasetDestroyFlags.RecursiveClones, "/sbin/zfs destroy -R tank/myds")]
        [DataRow(DatasetDestroyFlags.ForceUmount, "/sbin/zfs destroy -f tank/myds")]
        [DataRow(DatasetDestroyFlags.DryRun, "/sbin/zfs destroy -nvp tank/myds")]
        [DataRow((DatasetDestroyFlags)(int)-1, "/sbin/zfs destroy -r -R -f -nvp tank/myds")]
        [TestMethod]
        public void DestroyFlagsShouldBeCorrectlyReflected(DatasetDestroyFlags flags, string expectedCommand)
        {
            var command = DatasetCommands.DestroyDataset("tank/myds", flags);
            Console.WriteLine(command.FullCommandLine);
            Assert.AreEqual(expectedCommand, command.FullCommandLine);
        }

        [TestMethod]
        public void PromoteCommandTest()
        {
            var command = DatasetCommands.Promote("tank/myds/clone");
            Console.WriteLine(command.FullCommandLine);
            Assert.AreEqual("/sbin/zfs promote tank/myds/clone", command.FullCommandLine);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void CreateDatasetWithoutProperties(bool nullProperties)
        {
            var command = DatasetCommands.CreateDataset("tank/myds", nullProperties ? null : Array.Empty<PropertyValue>());
            Console.WriteLine(command.FullCommandLine);
            Assert.AreEqual("/sbin/zfs create tank/myds", command.FullCommandLine);
        }

        [TestMethod]
        public void CreateDataSetWithPropertiesTest()
        {
            var command = DatasetCommands.CreateDataset("tank/myds", new [] {new PropertyValue{Property="atime",Value="off" }, new PropertyValue { Property = "compression", Value = "off" }});
            Assert.AreEqual("/sbin/zfs create -o atime=off -o compression=off tank/myds", command.FullCommandLine);
        }
    }
}
