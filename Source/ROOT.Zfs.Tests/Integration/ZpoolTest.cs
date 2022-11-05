using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;
using ROOT.Zfs.Public.Arguments;
using ROOT.Zfs.Public.Arguments.Pool;
using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Tests.Integration
{
    [TestClass]
    public class ZpoolTest
    {
        private readonly IProcessCall _remoteProcessCall = TestHelpers.RequiresRemoteConnection ? new SSHProcessCall("bbs", "zfsdev.root.dom", true) : null;

        private ZPool GetZpool()
        {
            var zp = new ZPool(_remoteProcessCall);
            zp.RequiresSudo = TestHelpers.RequiresSudo;
            return zp;
        }

        [TestMethod, TestCategory("Integration")]
        public void GetHistoryTestWithSkip()
        {
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var zp = GetZpool();
            var snap = new Snapshots(_remoteProcessCall);
            snap.RequiresSudo = TestHelpers.RequiresSudo;
            snap.CreateSnapshot(pool.Name, "test1");
            snap.CreateSnapshot(pool.Name, "test2");
            snap.CreateSnapshot(pool.Name, "test3");
            snap.CreateSnapshot(pool.Name, null);
            var lines = zp.GetHistory(pool.Name).ToList().Count;

            var history = zp.GetHistory(pool.Name, lines - 2).ToList();
            Assert.IsNotNull(history);
            Console.WriteLine(history.Dump(new JsonFormatter()));
            Assert.AreEqual(2, history.Count);
        }



        [TestMethod, TestCategory("Integration")]
        public void GetHistory()
        {
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var zp = GetZpool();
            var history = zp.GetHistory(pool.Name);
            Assert.IsNotNull(history);
            Console.WriteLine(history.Dump(new JsonFormatter()));
        }

        [TestMethod, TestCategory("Integration")]
        public void GetHistoryAfterDate()
        {
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var zp = GetZpool();
            var snap = new Snapshots(_remoteProcessCall);
            snap.RequiresSudo = TestHelpers.RequiresSudo;
            snap.CreateSnapshot(pool.Name, "test1");
            snap.CreateSnapshot(pool.Name, "test2");
            snap.CreateSnapshot(pool.Name, "test3");
            snap.CreateSnapshot(pool.Name, null);
            var last10AtMost = zp.GetHistory(pool.Name).TakeLast(10).ToList();

            // This is safe, since zfs always have at last pool creation as a history event
            var history = last10AtMost.First();

            var afterDate = zp.GetHistory(pool.Name, 0, history.Time).ToList();

            if (last10AtMost.Count > 1)
            {
                Assert.IsTrue(afterDate.Count < last10AtMost.Count);
            }

        }

        [TestMethod, TestCategory("Integration")]
        public void GetStatusTest()
        {
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var zp = GetZpool();
            var status = zp.GetStatus(pool.Name);
            Assert.IsNotNull(status);
            Assert.AreEqual(pool.Name, status.Pool.Name);
        }

        [TestMethod, TestCategory("Integration")]
        public void ZpoolStatusTest()
        {
            var zfs = new Core.Zfs(_remoteProcessCall);
            using var testPool = TestPool.CreateSimplePool(_remoteProcessCall);
            var status = zfs.Pool.GetStatus(testPool.Name);

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

        [TestMethod, TestCategory("Integration")]
        public void DestroyPoolTest()
        {
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var zp = GetZpool();
            zp.DestroyPool(pool.Name);
        }


        [TestMethod, TestCategory("Integration")]
        public void CreateMirrorPool()
        {
            using var pool = new TestPool(_remoteProcessCall);
            var disk1 = pool.AddDisk();
            var disk2 = pool.AddDisk();

            var name = "TestP" + Guid.NewGuid();

            var args = new PoolCreationArgs
            {
                Name = name,
                MountPoint = "none",
                VDevs = new VDevCreationArgs[]
                {
                    new()
                    {
                        Type = VDevCreationType.Mirror,
                        Devices = new[] { disk1, disk2 }
                    }
                }
            };

            var status = pool.CreatePool(args);
            Assert.AreEqual(State.Online, status.State);

        }

        [TestMethod, TestCategory("Integration")]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void CreateRaidZTest(int raidz)
        {
            using var pool = new TestPool(_remoteProcessCall);

            var raidDisks = new List<string>();
            var disk1 = pool.AddDisk();
            var disk2 = pool.AddDisk();
            raidDisks.Add(disk1);
            raidDisks.Add(disk2);

            var disk3 = pool.AddDisk();
            for (int x = 0; x < raidz; x++)
            {
                raidDisks.Add(pool.AddDisk());
            }

            var name = "TestP" + Guid.NewGuid();
            VDevCreationType type = raidz == 1 ? VDevCreationType.Raidz1 : raidz == 2 ? VDevCreationType.Raidz2 : VDevCreationType.Raidz3;
            var args = new PoolCreationArgs
            {
                Name = name,
                MountPoint = "none",
                VDevs = new VDevCreationArgs[]
                {
                    new()
                    {
                        Type = type,
                        Devices = raidDisks
                    },
                    new()
                    {
                        Type = VDevCreationType.Spare,
                        Devices = new[] { disk3 }
                    }


                }
            };

            var status = pool.CreatePool(args);
            Assert.AreEqual(State.Online, status.State);
            Console.WriteLine(status.Dump(new JsonFormatter()));
        }

        [TestMethod, TestCategory("Integration"), Ignore("Not supported on ubuntu 20")]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void CreateDRaidZTest(int draid)
        {
            using var pool = new TestPool(_remoteProcessCall);

            var raidDisks = new List<string>();
            var disk1 = pool.AddDisk();
            var disk2 = pool.AddDisk();
            var extra = pool.AddDisk();
            raidDisks.Add(disk1);
            raidDisks.Add(disk2);
            raidDisks.Add(extra);

            for (int x = 0; x < draid; x++)
            {
                raidDisks.Add(pool.AddDisk());
            }

            var name = "TestP" + Guid.NewGuid();
            VDevCreationType type = draid == 1 ? VDevCreationType.DRaid1 : draid == 2 ? VDevCreationType.DRaid2 : VDevCreationType.DRaid3;
            var args = new PoolCreationArgs
            {
                Name = name,
                MountPoint = "none",
                VDevs = new VDevCreationArgs[]
                {
                    new DraigVDevCreationArgs()
                    {
                        Type = type,
                        Devices = raidDisks,
                        DraidArgs = new DraidArgs
                        {
                            Spares=1,
                            DataDevices=2,
                            Children=3+draid
                        }
                    }
                }
            };

            var status = pool.CreatePool(args);
            Assert.AreEqual(State.Online, status.State);
            Console.WriteLine(status.Dump(new JsonFormatter()));
        }

        [TestMethod, TestCategory("Integration")]
        public void ScrubTest()
        {
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var zp = GetZpool();
            zp.Scrub(pool.Name, ScrubOption.None);
            var status = zp.GetStatus(pool.Name);
            Assert.AreEqual(State.Online, status.State);

        }

        [TestMethod, TestCategory("Integration")]
        public void TrimPoolTestSimple()
        {
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var zp = GetZpool();

            var args = new PoolTrimArgs { PoolName = pool.Name };
            zp.Trim(args);
            var status = zp.GetStatus(pool.Name);
            Assert.AreEqual(State.Online, status.State);
        }

        [TestMethod, TestCategory("Integration")]
        public void TrimDeviceTestSimple()
        {
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var zp = GetZpool();
            var firstDisk = pool.Disks[0];
            var args = new PoolTrimArgs { PoolName = pool.Name, DeviceName = firstDisk };
            zp.Trim(args);
            var status = zp.GetStatus(pool.Name);
            Assert.AreEqual(State.Online, status.State);
        }

        [TestMethod, TestCategory("Integration")]
        public void GetPoolInfoTest()
        {
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var zp = GetZpool();

            var info = zp.List(pool.Name);
            Console.WriteLine(info.Dump(new JsonFormatter()));
            Assert.IsNotNull(info);
            Assert.AreNotEqual(0, info.Version);
            Assert.AreEqual(pool.Name, info.Name);
        }

        [TestMethod, TestCategory("Integration")]
        public void DetachTest()
        {
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var zp = GetZpool();
            zp.Detach(pool.Name, pool.Disks[0]);
            var status = zp.GetStatus(pool.Name);
            Assert.IsNotNull(status);
            Console.WriteLine(status.Dump(new JsonFormatter()));
        }


        [TestMethod, TestCategory("Integration")]
        public void IOStatTestSpecificDevice()
        {
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var zp = GetZpool();
            var stats = zp.GetIOStats(pool.Name, new[] { pool.Disks[0] });

            Console.WriteLine(stats.Dump(new JsonFormatter()));
            Assert.IsNotNull(stats);
        }

        [TestMethod, TestCategory("Integration")]
        public void IOStatTestEntirePool()
        {
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var zp = GetZpool();
            var stats = zp.GetIOStats(pool.Name, null);

            Console.WriteLine(stats.Dump(new JsonFormatter()));
            Assert.IsNotNull(stats);
            // pool
            Assert.AreEqual(1, stats.Stats.Count);
            var poolStat = stats.Stats[0];
            //vdev
            Assert.AreEqual(1, poolStat.ChildStats.Count);
            var vdev = poolStat.ChildStats[0];
            // actual devices
            Assert.AreEqual(2, vdev.ChildStats.Count);
        }

        //Turn a two way mirror into a three way mirror
        [TestMethod, TestCategory("Integration")]
        public void AttachNewDeviceToPool()
        {
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);

            var oldDevice = pool.Disks.Last();
            var newDevice = pool.AddDisk();
            var zp = GetZpool();

            var args = new PoolAttachReplaceArgs("attach")
            {
                PoolName = pool.Name,
                OldDevice = oldDevice,
                NewDevice = newDevice
            };

            zp.Attach(args);

            var status = zp.GetStatus(pool.Name);
            Console.WriteLine(status.Dump(new JsonFormatter()));

            Assert.AreEqual(3, status.Pool.VDevs[0].Devices.Count);
        }

        [TestMethod, TestCategory("Integration")]
        public void ReplaceNewDeviceToPool()
        {
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);

            var oldDevice = pool.Disks.Last();
            var newDevice = pool.AddDisk();
            var zp = GetZpool();

            var args = new PoolAttachReplaceArgs("replace")
            {
                PoolName = pool.Name,
                OldDevice = oldDevice,
                NewDevice = newDevice,
                IsReplace = true
            };

            zp.Replace(args);

            var status = zp.GetStatus(pool.Name);
            Console.WriteLine(status.Dump(new JsonFormatter()));
            var stopwatch = Stopwatch.StartNew();
            var oldExist = status.Pool.VDevs.Any(v => v.Devices.Any(d => d.DeviceName == oldDevice));
            while (oldExist && stopwatch.Elapsed < TimeSpan.FromSeconds(5))
            {
                status = zp.GetStatus(pool.Name);
                oldExist = status.Pool.VDevs.Any(v => v.Devices.Any(d => d.DeviceName == oldDevice));
            }

            var newExist = status.Pool.VDevs.Any(v => v.Devices.Any(d => d.DeviceName == newDevice));

            Assert.IsFalse(oldExist);
            Assert.IsTrue(newExist);
        }

        [TestMethod, TestCategory("Integration")]
        public void AddExtraMirrorToMirrorPool()
        {
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var zp = GetZpool();
            var status = zp.GetStatus(pool.Name);
            Assert.AreEqual(1, status.Pool.VDevs.Count);
            var disk1 = pool.AddDisk();
            var disk2 = pool.AddDisk();

            var args = new PoolAddArgs
            {
                PoolName = pool.Name,
                Force = true,
                VDevs = new[] { new VDevCreationArgs { Type = VDevCreationType.Mirror, Devices = new[] { disk1, disk2 } } },
            };


            zp.Add(args);

            status = zp.GetStatus(pool.Name);
            Console.WriteLine(status.Dump(new JsonFormatter()));
            Assert.AreEqual(2, status.Pool.VDevs.Count);
        }

        [TestMethod, TestCategory("Integration")]
        public void RemoveExtraMirrorFromPool()
        {
            using var pool = new TestPool(_remoteProcessCall);
            var disk1 = pool.AddDisk("300M");
            var disk2 = pool.AddDisk("300M");
            var disk3 = pool.AddDisk("64M");
            var disk4 = pool.AddDisk("64M");

            var name = "TestP" + Guid.NewGuid();

            var args = new PoolCreationArgs
            {
                Name = name,
                MountPoint = "none",
                VDevs = new VDevCreationArgs[]
                {
                    new()
                    {
                        Type = VDevCreationType.Mirror,
                        Devices = new[] { disk1, disk2 },
                    },
                    new()
                    {
                        Type = VDevCreationType.Mirror,
                        Devices = new[] { disk3, disk4 }
                    }
                }
            };

            var status = pool.CreatePool(args);
            Assert.AreEqual(State.Online, status.State);

            var removeArgs = new PoolRemoveArgs
            {
                PoolName = name,
                VDevOrDevice = "mirror-1"
            };

            var zp = GetZpool();
            zp.Remove(removeArgs);

            status = zp.GetStatus(name);

            Console.WriteLine(status.Dump(new JsonFormatter()));
            var stopwatch = Stopwatch.StartNew();
            var oldExist = status.Pool.VDevs.Count == 2;
            while (oldExist && stopwatch.Elapsed < TimeSpan.FromSeconds(5))
            {
                status = zp.GetStatus(pool.Name);
                oldExist = status.Pool.VDevs.Count == 2;
            }
            Console.WriteLine(status.Dump(new JsonFormatter()));
        }

        [TestMethod, TestCategory("Integration")]
        public void OfflineTest()
        {
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var zp = GetZpool();

            var device = pool.Disks.Last();
            var args = new PoolOfflineArgs{PoolName=pool.Name,Device=device};
            zp.Offline(args);

            var status = zp.GetStatus(pool.Name);
            
            Console.WriteLine(status.Dump(new JsonFormatter()));
            Assert.AreEqual(State.Degraded, status.State);

        }

        [TestMethod, TestCategory("Integration")]
        public void OnlineTest()
        {
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var zp = GetZpool();

            var device = pool.Disks.Last();
            var args = new PoolOfflineArgs { PoolName = pool.Name, Device = device };
            zp.Offline(args);

            var status = zp.GetStatus(pool.Name);

            Console.WriteLine(status.Dump(new JsonFormatter()));
            Assert.AreEqual(State.Degraded, status.State);

            var onlineArgs = new PoolOnlineArgs{ PoolName = pool.Name, Device = device };
            zp.Online(onlineArgs);

            status = zp.GetStatus(pool.Name);

            Console.WriteLine(status.Dump(new JsonFormatter()));
            Assert.AreEqual(State.Online, status.State);

        }
    }
}
