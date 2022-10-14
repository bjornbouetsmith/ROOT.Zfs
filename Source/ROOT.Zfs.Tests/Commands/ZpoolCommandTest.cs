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
                PoolProperties = new[] { new PropertyValue { Property = "ashift", Value = "12" } },
                FileSystemProperties = new[] { new PropertyValue { Property = "atime", Value = "off" } }
            };
            var command = ZpoolCommands.CreatePool(args);
            Assert.AreEqual("/sbin/zpool create tank3 -m /mnt/tank3 -o ashift=12 -O atime=off mirror /dev/sdc /dev/sdd cache /dev/sde log /dev/sdf", command.FullCommandLine);
            Console.WriteLine(command.FullCommandLine);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void CreateWithEmptyNameShouldThrowArgumentException(bool nameNull)
        {
            var args = new PoolCreationArgs
            {
                Name = nameNull ? null : "",
                VDevs = new[]
                {
                    new VDevCreationArgs { Type = VDevCreationType.Mirror, Devices = new[] { "/dev/sdc", "/dev/sdd" } },
                }
            };

            var ex = Assert.ThrowsException<ArgumentException>(() => ZpoolCommands.CreatePool(args));
            Assert.AreEqual("Please provide a name for the pool (Parameter 'args')", ex.Message);
        }

        [TestMethod]
        public void CreateMirrorNeedsAtLeastTwoDevices()
        {
            var args = new PoolCreationArgs
            {
                Name = "tank2",
                VDevs = new[]
                {
                    new VDevCreationArgs { Type = VDevCreationType.Mirror, Devices = new[] { "/dev/sdc" } },
                }
            };

            var ex = Assert.ThrowsException<ArgumentException>(() => ZpoolCommands.CreatePool(args));
            Assert.AreEqual("Please provide at least two devices when creating a mirror (Parameter 'args')", ex.Message);
        }

        [TestMethod]
        [DataRow("tank", "mirror-0", "/sbin/zpool clear tank mirror-0")]
        [DataRow("tank", "sda", "/sbin/zpool clear tank sda")]
        [DataRow("tank", "", "/sbin/zpool clear tank")]
        [DataRow("tank", null, "/sbin/zpool clear tank")]
        public void ClearPoolTest(string pool, string device, string expectedCommand)
        {
            var command = ZpoolCommands.Clear(pool, device);
            Console.WriteLine(command.FullCommandLine);
            Assert.AreEqual(expectedCommand, command.FullCommandLine);
        }

        [TestMethod]
        [DataRow("tank", "sda", false, "/sbin/zpool online tank sda")]
        [DataRow("tank", "sda", true, "/sbin/zpool online tank sda -e")]
        public void OnlinePoolDeviceTest(string pool, string device, bool expandSize, string expectedCommand)
        {
            var command = ZpoolCommands.Online(pool, device, expandSize);
            Assert.AreEqual(expectedCommand, command.FullCommandLine);
        }
        [TestMethod]
        [DataRow("tank", "sda", false, false,"/sbin/zpool offline tank sda")]
        [DataRow("tank", "sda", true, false,"/sbin/zpool offline tank sda -f")]
        [DataRow("tank", "sda", false, true, "/sbin/zpool offline tank sda -t")]
        [DataRow("tank", "sda", true, true, "/sbin/zpool offline tank sda -f -t")]
        public void OfflinePoolDeviceTest(string pool, string device, bool forceFault, bool temporary, string expectedCommand)
        {
            var command = ZpoolCommands.Offline(pool, device, forceFault, temporary);
            Assert.AreEqual(expectedCommand, command.FullCommandLine);
        }

        [TestMethod]
        [DataRow("tank",null,false,"/sbin/zpool iostat -LlPvH tank")] // only pool, exclude latency stats
        [DataRow("tank", null, true, "/sbin/zpool iostat -LlPvHl tank")] //only pool include latency stats
        [DataRow("tank","",true, "/sbin/zpool iostat -LlPvHl tank")] //only pool include latency stats
        [DataRow("tank", "/dev/sda", false, "/sbin/zpool iostat -LlPvH tank /dev/sda")] //single device, exclude stats
        [DataRow("tank", "/dev/sda,/dev/sdb", false, "/sbin/zpool iostat -LlPvH tank /dev/sda /dev/sdb")] //multiple devices, exclude stats
        [DataRow("tank", "/dev/sda", true, "/sbin/zpool iostat -LlPvHl tank /dev/sda")] //single device, include stats
        [DataRow("tank", "/dev/sda,/dev/sdb", true, "/sbin/zpool iostat -LlPvHl tank /dev/sda /dev/sdb")] //multiple devices, include stats
        public void IoStatTest(string pool, string deviceList, bool includeAverageLatency,string expectedCommand)
        {
            var command = ZpoolCommands.IoStat(pool, deviceList?.Split(','), includeAverageLatency);
            Assert.AreEqual(expectedCommand, command.FullCommandLine);
        }
    }
}
