using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Tests.Commands
{
    [TestClass]
    public class ZpoolCommandTest
    {
        [TestMethod]
        public void GetHistoryCommandTest()
        {
            var command = ZpoolCommands.GetHistory("tank");

            Assert.AreEqual("/sbin/zpool history -l tank", command.FullCommandLine);
        }

        [TestMethod]
        public void GetStatusCommandTest()
        {
            var command = ZpoolCommands.GetStatus("tank");

            Assert.AreEqual("/sbin/zpool status -vP tank", command.FullCommandLine);
        }

        [TestMethod]
        public void GetAllPoolInfosCommand()
        {
            var command = ZpoolCommands.GetAllPoolInfos();

            Assert.AreEqual("/sbin/zpool list -PH", command.FullCommandLine);
        }

        [TestMethod]
        public void GetPoolInfoCommand()
        {
            var command = ZpoolCommands.GetPoolInfo("tank");

            Assert.AreEqual("/sbin/zpool list -PH tank", command.FullCommandLine);
        }

        [TestMethod]
        public void CreateMirrorPoolTest()
        {
            var args = new PoolCreationArgs
            {
                Name = "tank3",
                VDevs = new[]
                    {
                        new VDevCreationArgs { Type = VDevCreationType.Mirror, Devices = new[] { "/dev/sdc", "/dev/sdd" } },
                        new VDevCreationArgs { Type = VDevCreationType.Cache, Devices = new[] { "/dev/sde" } },
                        new VDevCreationArgs { Type = VDevCreationType.Log, Devices = new[] { "/dev/sdf" } },
                    },
                MountPoint = "/mnt/tank3",
                PoolProperties = new[] { new PropertyValue("ashift","local","12") },
                FileSystemProperties = new[] { new PropertyValue("atime", "local", "off") }
            };
            var command = ZpoolCommands.CreatePool(args);
            Assert.AreEqual("/sbin/zpool create tank3 -m /mnt/tank3 -o ashift=12 -O atime=off mirror /dev/sdc /dev/sdd cache /dev/sde log /dev/sdf", command.FullCommandLine);
            Console.WriteLine(command.FullCommandLine);
        }
    }
}
