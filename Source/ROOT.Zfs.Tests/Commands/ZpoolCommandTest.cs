using System;
using Microsoft.VisualBasic.FileIO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Public.Arguments.Pool;
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
            var args = new PoolHistoryArgs { PoolName = "tank" };
            var command = ZpoolCommands.History(args);

            Assert.AreEqual("/sbin/zpool history -l tank", command.FullCommandLine);
        }

        [TestMethod]
        public void GetHistoryCommandTestNotValid()
        {
            var args = new PoolHistoryArgs();
            Assert.ThrowsException<ArgumentException>(() => ZpoolCommands.History(args));

        }

        [DataRow("", true)]
        [DataRow("tank", false)]
        [DataRow("tank && rm -rf /", true)]
        [TestMethod]
        public void GetStatusCommandTest(string pool, bool expectException)
        {
            var arg = new PoolStatusArgs { PoolName = pool };
            if (expectException)
            {
                var ex = Assert.ThrowsException<ArgumentException>(() => ZpoolCommands.GetStatus(arg));
                Console.WriteLine(ex);
            }
            else
            {

                var command = ZpoolCommands.GetStatus(arg);

                Assert.AreEqual("/sbin/zpool status -vP tank", command.FullCommandLine);
            }
        }

        [TestMethod]
        public void GetAllPoolInfosCommand()
        {
            var command = ZpoolCommands.GetAllPoolInfos();

            Assert.AreEqual("/sbin/zpool list -PHp", command.FullCommandLine);
        }

        [DataRow("tank", false)]
        [DataRow("tank/myds", true)]
        [TestMethod]
        public void GetPoolInfoCommand(string name, bool expectException)
        {
            if (expectException)
            {
                Assert.ThrowsException<ArgumentException>(() => ZpoolCommands.GetPoolInfo(new PoolListArgs { PoolName = name }));
            }
            else
            {
                var command = ZpoolCommands.GetPoolInfo(new PoolListArgs { PoolName = name });

                Assert.AreEqual("/sbin/zpool list -PHp tank", command.FullCommandLine);
            }
        }

        [TestMethod]
        public void CreateMirrorPoolTest()
        {
            var args = new PoolCreateArgs
            {
                PoolName = "tank3",
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
            var command = ZpoolCommands.Create(args);
            Assert.AreEqual("/sbin/zpool create tank3 -m /mnt/tank3 -o ashift=12 -O atime=off mirror /dev/sdc /dev/sdd cache /dev/sde log /dev/sdf", command.FullCommandLine);
            Console.WriteLine(command.FullCommandLine);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void CreateWithEmptyNameShouldThrowArgumentException(bool nameNull)
        {
            var args = new PoolCreateArgs
            {
                PoolName = nameNull ? null : "",
                VDevs = new[]
                {
                    new VDevCreationArgs { Type = VDevCreationType.Mirror, Devices = new[] { "/dev/sdc", "/dev/sdd" } },
                }
            };

            var ex = Assert.ThrowsException<ArgumentException>(() => ZpoolCommands.Create(args));
            Assert.AreEqual("PoolName cannot be empty (Parameter 'PoolCreateArgs args')", ex.Message);
        }

        [TestMethod]
        public void CreateMirrorNeedsAtLeastTwoDevices()
        {
            var args = new PoolCreateArgs
            {
                PoolName = "tank2",
                VDevs = new[]
                {
                    new VDevCreationArgs { Type = VDevCreationType.Mirror, Devices = new[] { "/dev/sdc" } },
                }
            };

            var ex = Assert.ThrowsException<ArgumentException>(() => ZpoolCommands.Create(args));
            Assert.AreEqual("Please provide at least two devices when creating a mirror (Parameter 'PoolCreateArgs args')", ex.Message);
        }

        [TestMethod]
        [DataRow("tank", "mirror-0", "/sbin/zpool clear tank mirror-0", false)]
        [DataRow("tank", "sda", "/sbin/zpool clear tank sda", false)]
        [DataRow("tank", "", "/sbin/zpool clear tank", false)]
        [DataRow("tank", null, "/sbin/zpool clear tank", false)]
        [DataRow("tank", "rm -rf /tank", null, true)]
        [DataRow("tank", "rm /tank", null, true)]
        [DataRow("tank", "rm%20-rf%20/tank", null, true)]
        public void ClearPoolTest(string pool, string device, string expectedCommand, bool expectException)
        {
            var args = new PoolClearArgs { PoolName = pool, Device = device };

            if (expectException)
            {
                var ex = Assert.ThrowsException<ArgumentException>(() => ZpoolCommands.Clear(args));
                Console.WriteLine(ex);
            }
            else
            {
                var command = ZpoolCommands.Clear(args);
                Console.WriteLine(command.FullCommandLine);
                Assert.AreEqual(expectedCommand, command.FullCommandLine);
            }
        }

        [TestMethod]
        [DataRow("tank", "", null, true)]
        [DataRow("tank", "sda", "/sbin/zpool online tank sda", false)]
        public void OnlinePoolDeviceTest(string pool, string device, string expectedCommand, bool expectException)
        {
            var args = new PoolOnlineArgs { PoolName = pool, Device = device };
            if (expectException)
            {
                Assert.ThrowsException<ArgumentException>(() => ZpoolCommands.Online(args));
            }
            else
            {
                var command = ZpoolCommands.Online(args);
                Assert.AreEqual(expectedCommand, command.FullCommandLine);
            }
        }

        [DataRow("tank", null, null, true)]
        [DataRow(null, "sda", null, true)]
        [DataRow("tank", "sda", "/sbin/zpool offline tank sda", false)]
        [TestMethod]
        public void OfflinePoolDeviceTest(string pool, string device, string expectedCommand, bool expectException)
        {
            var args = new PoolOfflineArgs
            {
                PoolName = pool,
                Device = device,
            };
            if (expectException)
            {
                Assert.ThrowsException<ArgumentException>(() => ZpoolCommands.Offline(args));
            }
            else
            {

                var command = ZpoolCommands.Offline(args);

                Assert.AreEqual(expectedCommand, command.FullCommandLine);
            }
        }

        [TestMethod]
        [DataRow("tank", null, "/sbin/zpool iostat -LlpPvH tank", false)] // only pool
        [DataRow("tank", "", "/sbin/zpool iostat -LlpPvH tank", false)] //only pool 
        [DataRow("tank", "/dev/sda", "/sbin/zpool iostat -LlpPvH tank /dev/sda", false)] //single device
        [DataRow("tank", "/dev/sda,/dev/sdb", "/sbin/zpool iostat -LlpPvH tank /dev/sda /dev/sdb", false)] //multiple devices
        [DataRow("tank", "rm -rf /tank", null, true)] //multiple devices
        public void IoStatTest(string pool, string deviceList, string expectedCommand, bool expectException)
        {
            var args = new PoolIOStatsArgs { PoolName = pool, Devices = deviceList?.Split(',') };
            if (expectException)
            {
                var ex = Assert.ThrowsException<ArgumentException>(() => ZpoolCommands.IoStats(args));
                Console.WriteLine(ex);
            }
            else
            {

                var command = ZpoolCommands.IoStats(args);
                Assert.AreEqual(expectedCommand, command.FullCommandLine);
            }
        }

        [DataRow("tank", "/sbin/zpool resilver tank", false)]
        [DataRow("", null, true)]
        [TestMethod]
        public void ResilverCommandTest(string pool, string expected, bool expectException)
        {
            var args = new PoolResilverArgs { PoolName = pool };
            if (expectException)
            {
                Assert.ThrowsException<ArgumentException>(() => ZpoolCommands.Resilver(args));
            }
            else
            {
                var command = ZpoolCommands.Resilver(args);
                Console.WriteLine(command.FullCommandLine);
                Assert.AreEqual(expected, command.FullCommandLine);
            }
        }

        [DataRow("tank", ScrubOption.None, "/sbin/zpool scrub tank")]
        [DataRow("tank", ScrubOption.Stop, "/sbin/zpool scrub -s tank")]
        [DataRow("tank", ScrubOption.Pause, "/sbin/zpool scrub -p tank")]
        [DataRow(null, ScrubOption.None, null, true)]
        [DataRow("", ScrubOption.None, null, true)]
        [DataRow("  ", ScrubOption.None, null, true)]
        [TestMethod]
        public void ScrubTest(string pool, ScrubOption option, string expected, bool expectException = false)
        {
            var args = new PoolScrubArgs { PoolName = pool, Options = option };
            if (expectException)
            {
                Assert.ThrowsException<ArgumentException>(() => ZpoolCommands.Scrub(args));
            }
            else
            {
                var command = ZpoolCommands.Scrub(args);
                Assert.AreEqual(expected, command.FullCommandLine);
            }
        }

        [DataRow("tank", null, "/sbin/zpool trim tank")]
        [DataRow("tank", "/dev/sda", "/sbin/zpool trim tank /dev/sda")]
        [DataRow("", "/dev/sda", null, true)]
        [DataRow(null, "/dev/sda", null, true)]
        [DataRow(" ", "/dev/sda", null, true)]
        [TestMethod]
        public void TrimTest(string pool, string device, string expected, bool expectException = false)
        {
            var args = new PoolTrimArgs
            {
                PoolName = pool,
                DeviceName = device,
            };

            if (expectException)
            {
                Assert.ThrowsException<ArgumentException>(() => ZpoolCommands.Trim(args));
            }
            else
            {
                var command = ZpoolCommands.Trim(args);
                Console.WriteLine(command.FullCommandLine);
                Assert.AreEqual(expected, command.FullCommandLine);
            }
        }

        [TestMethod]
        public void GetUpgradeablePoolsTest()
        {
            var command = ZpoolCommands.GetUpgradeablePools();
            Assert.AreEqual("/sbin/zpool upgrade", command.FullCommandLine);
        }

        [DataRow("tank", false, "/sbin/zpool upgrade tank")]
        [DataRow("tank", true, "/sbin/zpool upgrade -a")]
        [DataRow("", true, "/sbin/zpool upgrade -a")]
        [DataRow(null, true, "/sbin/zpool upgrade -a")]
        [DataRow(" ", true, "/sbin/zpool upgrade -a")]
        [DataRow(null, false, null, true)]
        [DataRow("", false, null, true)]
        [DataRow(" ", false, null, true)]
        [TestMethod]
        public void UpgradePoolTest(string poolName, bool all, string expected, bool expectException = false)
        {
            var args = new PoolUpgradeArgs { PoolName = poolName, AllPools = all };
            if (expectException)
            {
                Assert.ThrowsException<ArgumentException>(() => ZpoolCommands.Upgrade(args));
            }
            else
            {
                var command = ZpoolCommands.Upgrade(args);
                Console.WriteLine(command.FullCommandLine);
                Assert.AreEqual(expected, command.FullCommandLine);
            }
        }

        [DataRow("tank", "/dev/sdc", "/sbin/zpool detach tank /dev/sdc", false)]
        [DataRow("tank", "", null, true)]
        [DataRow("", "/dev/sdc", null, true)]
        [TestMethod]
        public void DetachTest(string pool, string device, string expected, bool expectException)
        {
            var args = new PoolDetachArgs { PoolName = pool, Device = device };

            if (expectException)
            {
                Assert.ThrowsException<ArgumentException>(() => ZpoolCommands.Detach(args));
            }
            else
            {
                var command = ZpoolCommands.Detach(args);
                Assert.AreEqual(expected, command.FullCommandLine);
            }
        }

        [DataRow(true)]
        [DataRow(false)]
        [TestMethod]
        public void AttachTest(bool valid)
        {
            var args = new PoolAttachArgs
            {
                PoolName = valid ? "tank" : null,
                OldDevice = "/dev/sdb",
                NewDevice = "/dev/sdc",
                Force = true,
                RestoreSequentially = true,
                PropertyValues = new[] { new PropertyValue { Property = "ashift", Value = "12" } }
            };
            var stringVer = args.ToString();
            if (!valid)
            {
                Assert.ThrowsException<ArgumentException>(() => ZpoolCommands.Attach(args));
            }
            else
            {

                var command = ZpoolCommands.Attach(args);
                Console.WriteLine(command.FullCommandLine);

                Assert.AreEqual(stringVer, command.Arguments);
                Assert.AreEqual($"/sbin/zpool {stringVer}", command.FullCommandLine);
            }
        }

        [DataRow(true)]
        [DataRow(false)]
        [TestMethod]
        public void ReplaceTest(bool valid)
        {
            var args = new PoolReplaceArgs
            {
                PoolName = valid ? "tank" : null,
                OldDevice = "/dev/sdb",
                NewDevice = "/dev/sdc",
                Force = true,
                RestoreSequentially = true,
                PropertyValues = new[] { new PropertyValue { Property = "ashift", Value = "12" } }
            };
            var stringVer = args.ToString();

            if (!valid)
            {
                Assert.ThrowsException<ArgumentException>(() => ZpoolCommands.Replace(args));
            }
            else
            {
                var command = ZpoolCommands.Replace(args);
                Console.WriteLine(command.FullCommandLine);

                Assert.AreEqual(stringVer, command.Arguments);
                Assert.AreEqual($"/sbin/zpool {stringVer}", command.FullCommandLine);
            }
        }

        [DataRow(true)]
        [DataRow(false)]
        [TestMethod]
        public void AddTest(bool valid)
        {
            var args = new PoolAddArgs
            {
                PoolName = valid ? "tank" : null,
                VDevs = new[] { new VDevCreationArgs { Type = VDevCreationType.Mirror, Devices = new[] { "disk1", "disk2" } } },
            };
            if (!valid)
            {
                Assert.ThrowsException<ArgumentException>(() => ZpoolCommands.Add(args));
            }
            else
            {
                var command = ZpoolCommands.Add(args);
                Console.WriteLine(command.FullCommandLine);
                Assert.AreEqual("/sbin/zpool add tank mirror disk1 disk2", command.FullCommandLine);
            }
        }

        [DataRow(true)]
        [DataRow(false)]
        [TestMethod]
        public void RemoveTest(bool valid)
        {
            var args = new PoolRemoveArgs
            {
                PoolName = valid ? "tank" : null,
                VDevOrDevice = "mirror-0",
            };
            if (!valid)
            {
                Assert.ThrowsException<ArgumentException>(() => ZpoolCommands.Remove(args));
            }
            else
            {
                var command = ZpoolCommands.Remove(args);
                Console.WriteLine(command.FullCommandLine);
                Assert.AreEqual("/sbin/zpool remove tank mirror-0", command.FullCommandLine);
            }
        }
        [DataRow("tank", "/sbin/zpool destroy -f tank", false)]
        [DataRow("tank%5ftest", "/sbin/zpool destroy -f tank_test", false)]
        [DataRow("tank/myds", null, true)]
        [DataRow("tank%2fmyds", null, true)]
        [DataRow("", null, true)]
        [TestMethod]
        public void DestroyTest(string name, string expected, bool expectException)
        {
            var args = new PoolDestroyArgs { PoolName = name };

            if (expectException)
            {
                var ex = Assert.ThrowsException<ArgumentException>(() => ZpoolCommands.Destroy(args));
                Console.WriteLine(ex);
            }
            else
            {
                var command = ZpoolCommands.Destroy(args);
                Assert.AreEqual(expected, command.FullCommandLine);
            }
        }
    }
}
