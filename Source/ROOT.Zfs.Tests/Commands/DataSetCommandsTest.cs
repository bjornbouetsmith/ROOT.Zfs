﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Public;

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

            Assert.AreEqual(expectedCommand, command.FullCommandLine);
        }
    }
}
