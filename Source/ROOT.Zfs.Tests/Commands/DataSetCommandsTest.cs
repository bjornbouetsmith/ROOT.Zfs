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

        [DataRow(DataSetDestroyFlags.None, "/sbin/zfs destroy tank/myds")]
        [DataRow(DataSetDestroyFlags.Recursive, "/sbin/zfs destroy -r tank/myds")]
        [DataRow(DataSetDestroyFlags.RecursiveClones, "/sbin/zfs destroy -R tank/myds")]
        [DataRow(DataSetDestroyFlags.ForceUmount, "/sbin/zfs destroy -f tank/myds")]
        [DataRow(DataSetDestroyFlags.DryRun, "/sbin/zfs destroy -nvp tank/myds")]
        [DataRow((DataSetDestroyFlags)(int)-1, "/sbin/zfs destroy -r -R -f -nvp tank/myds")]
        [TestMethod]
        public void DestroyFlagsShouldBeCorrectlyReflected(DataSetDestroyFlags flags, string expectedCommand)
        {
            var command = DataSetCommands.DestroyDataSet("tank/myds", flags);
            Console.WriteLine(command.FullCommandLine);
            Assert.AreEqual(expectedCommand, command.FullCommandLine);
        }

        [TestMethod]
        public void PromoteCommandTest()
        {
            var command = DataSetCommands.Promote("tank/myds/clone");
            Console.WriteLine(command.FullCommandLine);
            Assert.AreEqual("/sbin/zfs promote tank/myds/clone", command.FullCommandLine);
        }

        [TestMethod]
        public void GetDataSetCommandTest()
        {
            var command = DataSetCommands.GetDataSet("tank/myds");
            Console.WriteLine(command.FullCommandLine);
            Assert.AreEqual("/sbin/zfs list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t filesystem tank/myds", command.FullCommandLine);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void CreateDatasetWithoutProperties(bool nullProperties)
        {
            var command = DataSetCommands.CreateDataSet("tank/myds", nullProperties ? null : new PropertyValue[] { });
            Console.WriteLine(command.FullCommandLine);
            Assert.AreEqual("/sbin/zfs create tank/myds", command.FullCommandLine);
        }

        [TestMethod]
        public void CreateDataSetWithPropertiesTest()
        {
            var command = DataSetCommands.CreateDataSet("tank/myds", new [] {new PropertyValue{Property="atime",Value="off" }, new PropertyValue { Property = "compression", Value = "off" }});
            Assert.AreEqual("/sbin/zfs create -o atime=off -o compression=off tank/myds", command.FullCommandLine);
        }
    }
}
