using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Arguments.Pool;
using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Tests.Integration.Fake
{
    [TestClass]
    public class FakeZpoolTest
    {
        readonly FakeRemoteConnection _remoteProcessCall = new("2.1.5-2");
        
        private IZPool GetZpool()
        {
            return new ZPool(_remoteProcessCall);
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void GetHistoryTestWithSkip()
        {
            var zp = GetZpool();
            var args = new PoolHistoryArgs { PoolName = "tank" };
            var lines = zp.History(args).ToList().Count;

            args = new PoolHistoryArgs { PoolName = "tank",SkipLines=lines -2 };
            var history = zp.History(args).ToList();
            Assert.IsNotNull(history);
            Console.WriteLine(history.Dump(new JsonFormatter()));
            Assert.AreEqual(2, history.Count);
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void GetHistory()
        {
            var zp = GetZpool();
            var args = new PoolHistoryArgs { PoolName = "tank" };
            var history = zp.History(args);
            Assert.IsNotNull(history);
            Console.WriteLine(history.Dump(new JsonFormatter()));
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void GetHistoryAfterDate()
        {
            var zp = GetZpool();
            var args = new PoolHistoryArgs { PoolName = "tank" };
            var last10AtMost = zp.History(args).TakeLast(10).ToList();

            // This is safe, since zfs always have at last pool creation as a history event
            var history = last10AtMost.First();
            args = new PoolHistoryArgs { PoolName = "tank",AfterDate=history.Time };
            var afterDate = zp.History(args).ToList();

            if (last10AtMost.Count > 1)
            {
                Assert.IsTrue(afterDate.Count < last10AtMost.Count);
            }
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void ZpoolStatusTest()
        {
            IZfs zfs = new Core.Zfs(_remoteProcessCall);

            var status = zfs.Pool.GetStatus("tank");

            Assert.IsNotNull(status);

            if (status.State == State.Online)
            {
                return;
            }

            var pool = status.Pool;
            foreach (var vdev in pool.VDevs)
            {
                if (vdev.State == State.Online)
                {
                    continue;
                }

                Console.WriteLine("vdev:{0}, State:{1}", vdev.Name, vdev.State);
                foreach (var device in vdev.Devices)
                {
                    if (device.State == State.Online)
                    {
                        continue;
                    }

                    Console.WriteLine("Device:{0}, State:{1}", device.DeviceName, device.State);
                }
            }
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void DestroyPoolTest()
        {
            var zp = GetZpool();
            var pool = zp.CreatePool(new PoolCreationArgs
            {
                Name = "mytest",
                VDevs = new VDevCreationArgs[]
                {
                    new()
                    {
                        Type = VDevCreationType.Mirror,
                        Devices = new[] { "/dev/sda", "/dev/sdb" }
                    }
                }
            });
            Assert.IsNotNull(pool);
            zp.DestroyPool("mytest");
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void GetPoolInfosTest()
        {
            var zp = GetZpool();

            var infos = zp.List();
            Assert.AreEqual(2, infos.Count);

        }


        [TestMethod, TestCategory("FakeIntegration")]
        public void GetPoolInfoTest()
        {
            var zp = GetZpool();

            var info = zp.List("tank2");
            Assert.IsNotNull(info);
            Assert.AreEqual(5000, info.Version);
            Assert.AreEqual("tank2", info.Name);
            Assert.AreEqual("15.5G", info.Size.ToString());
            Assert.AreEqual("105K", info.Allocated.ToString());
            Assert.AreEqual("15.5G", info.Free.ToString());
            Assert.AreEqual("7.30%", info.Fragmentation.ToString());
            Assert.AreEqual("6.50%", info.CapacityUsed.ToString());
            Assert.AreEqual("1.43x", info.DedupRatio.ToString());
            Assert.AreEqual(State.Online, info.State);
            Console.WriteLine(info.Dump(new JsonFormatter()));

        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void OfflineTest()
        {
            var zp = GetZpool();
            var args = new PoolOfflineArgs { PoolName = "tank", Device = "/dev/sda" };
            zp.Offline(args);
            var commands = _remoteProcessCall.GetCommandsInvoked();
            Assert.AreEqual(1, commands.Count);
            Assert.IsTrue(commands.Contains("/sbin/zpool offline tank /dev/sda"));
        }


        [TestMethod, TestCategory("FakeIntegration")]
        public void OnlineTest()
        {
            var zp = GetZpool();
            var args = new PoolOnlineArgs { PoolName = "tank", Device = "/dev/sda" };
            zp.Online(args);
            var commands = _remoteProcessCall.GetCommandsInvoked();
            Assert.AreEqual(1, commands.Count);
            Assert.IsTrue(commands.Contains("/sbin/zpool online tank /dev/sda"));
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void ClearTest()
        {
            var zp = GetZpool(); 
            zp.Clear("tank", "/dev/sda");
            var commands = _remoteProcessCall.GetCommandsInvoked();
            Assert.AreEqual(1, commands.Count);
            Assert.IsTrue(commands.Contains("/sbin/zpool clear tank /dev/sda"));
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void IOStatTest()
        {
            var zp = GetZpool();
            var stats = zp.GetIOStats("tank", new[] { "/dev/sda" });
            Console.WriteLine(stats.Dump(new JsonFormatter()));
            var commands = _remoteProcessCall.GetCommandsInvoked();
            Assert.AreEqual(1, commands.Count);
            Assert.IsTrue(commands.Contains("/sbin/zpool iostat -LlpPvH tank /dev/sda"));
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void ResilverTest()
        {
            var zp = GetZpool();
            zp.Resilver("tank");
            var commands = _remoteProcessCall.GetCommandsInvoked();
            Assert.AreEqual(1, commands.Count);
            Assert.IsTrue(commands.Contains("/sbin/zpool resilver tank"));
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void CreateDraidTest()
        {
            var zp = GetZpool();
            zp.CreatePool(new PoolCreationArgs
            {
                Name = "mytest",
                VDevs = new VDevCreationArgs[]
                {
                    new DraigVDevCreationArgs
                    {
                        Type = VDevCreationType.DRaid1,
                        Devices = new[] { "/dev/sda", "/dev/sdb","/dev/sdc", "/dev/sdd" },
                        DraidArgs = new DraidArgs
                        {
                            Spares=1,
                            DataDevices=2,
                            Children=4
                        }
                    }
                }
            });
            var commands = _remoteProcessCall.GetCommandsInvoked();
            Assert.AreEqual(2, commands.Count);
            Assert.AreEqual("/sbin/zpool create mytest draid1:2d:4c:1s /dev/sda /dev/sdb /dev/sdc /dev/sdd", commands[0]);
            Assert.AreEqual("/sbin/zpool status -vP mytest", commands[1]);

        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void ScrubTest()
        {
            var zp = GetZpool();
            zp.Scrub("tank", default);
            var commands = _remoteProcessCall.GetCommandsInvoked();
            Assert.AreEqual(1, commands.Count);
            Assert.AreEqual("/sbin/zpool scrub tank", commands[0]);
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void TrimTest()
        {
            var zp = GetZpool();
            zp.Trim(new PoolTrimArgs { PoolName = "tank" });
            var commands = _remoteProcessCall.GetCommandsInvoked();
            Assert.AreEqual(1, commands.Count);
            Assert.AreEqual("/sbin/zpool trim tank", commands[0]);
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void UpgradeTest()
        {
            var zp = GetZpool();
            zp.Upgrade(new PoolUpgradeArgs { PoolName = "tank" });
            var commands = _remoteProcessCall.GetCommandsInvoked();
            Assert.AreEqual(1, commands.Count);
            Assert.AreEqual("/sbin/zpool upgrade tank", commands[0]);
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void GetUpgradeablePoolsTest()
        {
            var zp = GetZpool();
            var pools = zp.GetUpgradeablePools();
            Assert.AreEqual(1, pools.Count);
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void DetachTest()
        {
            var zp = GetZpool();
            var args = new PoolDetachArgs { PoolName = "tank", Device = "/dev/sdb" };
            zp.Detach(args);

            var commands = _remoteProcessCall.GetCommandsInvoked();
            Assert.AreEqual(1, commands.Count);
            Assert.AreEqual("/sbin/zpool detach tank /dev/sdb", commands[0]);
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void AttachNewDeviceToPool()
        {
            var zp = GetZpool();

            var args = new PoolAttachArgs
            {
                PoolName = "tank",
                OldDevice = "/dev/sdb",
                NewDevice = "/dev/sdc"
            };

            zp.Attach(args);
            var commands = _remoteProcessCall.GetCommandsInvoked();
            Assert.AreEqual(1, commands.Count);
            Assert.AreEqual("/sbin/zpool attach tank /dev/sdb /dev/sdc", commands[0]);
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void ReplaceNewDeviceToPool()
        {
            var zp = GetZpool();

            var args = new PoolReplaceArgs
            {
                PoolName = "tank",
                OldDevice = "/dev/sdb",
                NewDevice = "/dev/sdc",
            };

            zp.Replace(args);
            var commands = _remoteProcessCall.GetCommandsInvoked();
            Assert.AreEqual(1, commands.Count);
            Assert.AreEqual("/sbin/zpool replace tank /dev/sdb /dev/sdc", commands[0]);
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void AddExtraMirrorToMirrorPool()
        {
            var zp = GetZpool();

            var args = new PoolAddArgs
            {
                PoolName = "tank",
                Force = true,
                VDevs = new[] { new VDevCreationArgs { Type = VDevCreationType.Mirror, Devices = new[] { "/dev/sda", "/dev/sdb" } } },
            };


            zp.Add(args);

            var commands = _remoteProcessCall.GetCommandsInvoked();
            Assert.AreEqual(1, commands.Count);
            Assert.AreEqual("/sbin/zpool add -f tank mirror /dev/sda /dev/sdb", commands[0]);
        }

        [TestMethod]
        public void ZpoolRemoveTest()
        {
            var zp = GetZpool();
            var removeArgs = new PoolRemoveArgs
            {
                PoolName = "tank",
                VDevOrDevice = "mirror-1"
            };


            zp.Remove(removeArgs);
            var commands = _remoteProcessCall.GetCommandsInvoked();
            Assert.AreEqual(1, commands.Count);
            Assert.AreEqual("/sbin/zpool remove tank mirror-1", commands[0]);
        }
    }
}
