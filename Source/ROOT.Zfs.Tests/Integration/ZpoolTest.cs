using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Arguments.Pool;
using ROOT.Zfs.Public.Arguments.Snapshots;
using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Tests.Integration
{
    [TestClass]
    public class ZpoolTest
    {
        private readonly IProcessCall _remoteProcessCall = TestHelpers.RequiresRemoteConnection ? new SSHProcessCall("bbs", "zfsdev.root.dom", true) : null;

        private IZPool GetZpool()
        {
            var zp = new ZPool(_remoteProcessCall);
            zp.RequiresSudo = TestHelpers.RequiresSudo;
            return zp;
        }

        [TestMethod, TestCategory("Integration")]
        public void HistoryTestWithSkip()
        {
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var zp = GetZpool();
            var snap = new Snapshots(_remoteProcessCall);
            snap.RequiresSudo = TestHelpers.RequiresSudo;
            snap.Create(new SnapshotCreateArgs { Dataset = pool.Name, Snapshot = "test1" });
            snap.Create(new SnapshotCreateArgs { Dataset = pool.Name, Snapshot = "test2" });
            snap.Create(new SnapshotCreateArgs { Dataset = pool.Name, Snapshot = "test3" });
            snap.Create(new SnapshotCreateArgs { Dataset = pool.Name });
            var args = new PoolHistoryArgs { PoolName = pool.Name };
            var lines = zp.History(args).ToList().Count;
            args = new PoolHistoryArgs { PoolName = pool.Name, SkipLines = lines - 2 };
            var history = zp.History(args).ToList();
            Assert.IsNotNull(history);
            Console.WriteLine(history.Dump(new JsonFormatter()));
            Assert.AreEqual(2, history.Count);
        }



        [TestMethod, TestCategory("Integration")]
        public void HistoryTest()
        {
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var zp = GetZpool();
            var args = new PoolHistoryArgs { PoolName = pool.Name };
            var history = zp.History(args);
            Assert.IsNotNull(history);
            Console.WriteLine(history.Dump(new JsonFormatter()));
        }

        [TestMethod, TestCategory("Integration")]
        public void HistoryAfterDateTest()
        {
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var zp = GetZpool();
            var snap = new Snapshots(_remoteProcessCall);
            snap.RequiresSudo = TestHelpers.RequiresSudo;
            snap.Create(new SnapshotCreateArgs { Dataset = pool.Name, Snapshot = "test1" });
            snap.Create(new SnapshotCreateArgs { Dataset = pool.Name, Snapshot = "test2" });
            snap.Create(new SnapshotCreateArgs { Dataset = pool.Name, Snapshot = "test3" });
            snap.Create(new SnapshotCreateArgs { Dataset = pool.Name });
            var args = new PoolHistoryArgs { PoolName = pool.Name };
            var last10AtMost = zp.History(args).TakeLast(10).ToList();

            // This is safe, since zfs always have at last pool creation as a history event
            var history = last10AtMost.First();
            args = new PoolHistoryArgs { PoolName = pool.Name, AfterDate = history.Time };
            var afterDate = zp.History(args).ToList();

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
            var arg = new PoolStatusArgs { PoolName = pool.Name };
            var status = zp.Status(arg);
            Assert.IsNotNull(status);
            Assert.AreEqual(pool.Name, status.Pool.Name);
        }

        [TestMethod, TestCategory("Integration")]
        public void NonExistingPoolStatus()
        {
            var zp = GetZpool();
            var ex = Assert.ThrowsException<ProcessCallException>(()=>zp.Status(new PoolStatusArgs{PoolName="A"+Guid.NewGuid().ToString()}));
            Console.WriteLine(ex.Message);
        }

        [TestMethod, TestCategory("Integration")]
        public void ZpoolStatusTest()
        {
            IZfs zfs = new Core.Zfs(_remoteProcessCall);
            using var testPool = TestPool.CreateSimplePool(_remoteProcessCall);
            var arg = new PoolStatusArgs { PoolName = testPool.Name };
            var status = zfs.Pool.Status(arg);

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
            zp.Destroy(new PoolDestroyArgs { PoolName = pool.Name });
        }


        [TestMethod, TestCategory("Integration")]
        public void CreateMirrorPool()
        {
            using var pool = new TestPool(_remoteProcessCall);
            var disk1 = pool.AddDisk();
            var disk2 = pool.AddDisk();

            var name = "TestP" + Guid.NewGuid();

            var args = new PoolCreateArgs
            {
                PoolName = name,
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
            var args = new PoolCreateArgs
            {
                PoolName = name,
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
            var args = new PoolCreateArgs
            {
                PoolName = name,
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
            var args = new PoolScrubArgs { PoolName = pool.Name };
            zp.Scrub(args);
            var arg = new PoolStatusArgs { PoolName = pool.Name };
            var status = zp.Status(arg);
            Assert.AreEqual(State.Online, status.State);

        }

        [TestMethod, TestCategory("Integration")]
        public void TrimPoolTestSimple()
        {
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var zp = GetZpool();

            var args = new PoolTrimArgs { PoolName = pool.Name };
            zp.Trim(args);
            var arg = new PoolStatusArgs { PoolName = pool.Name };
            var status = zp.Status(arg);
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
            var arg = new PoolStatusArgs { PoolName = pool.Name };
            var status = zp.Status(arg);
            Assert.AreEqual(State.Online, status.State);
        }

        [TestMethod, TestCategory("Integration")]
        public void GetPoolInfoTest()
        {
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var zp = GetZpool();

            var info = zp.List(new PoolListArgs { PoolName = pool.Name });
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
            var args = new PoolDetachArgs { PoolName = pool.Name, Device = pool.Disks[1] };
            zp.Detach(args);
            var arg = new PoolStatusArgs { PoolName = pool.Name };
            var status = zp.Status(arg);
            Assert.IsNotNull(status);
            Console.WriteLine(status.Dump(new JsonFormatter()));
        }


        [TestMethod, TestCategory("Integration")]
        public void IOStatTestSpecificDevice()
        {
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var zp = GetZpool();
            var args = new PoolIOStatsArgs { PoolName = pool.Name, Devices = new[] { pool.Disks[0] } };
            var stats = zp.IOStats(args);

            Console.WriteLine(stats.Dump(new JsonFormatter()));
            Assert.IsNotNull(stats);
        }

        [TestMethod, TestCategory("Integration")]
        public void IOStatTestEntirePool()
        {
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var zp = GetZpool();
            var args = new PoolIOStatsArgs { PoolName = pool.Name };
            var stats = zp.IOStats(args);

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

            var args = new PoolAttachArgs
            {
                PoolName = pool.Name,
                OldDevice = oldDevice,
                NewDevice = newDevice
            };

            zp.Attach(args);
            var arg = new PoolStatusArgs { PoolName = pool.Name };
            var status = zp.Status(arg);
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

            var args = new PoolReplaceArgs
            {
                PoolName = pool.Name,
                OldDevice = oldDevice,
                NewDevice = newDevice
            };

            zp.Replace(args);
            var arg = new PoolStatusArgs { PoolName = pool.Name };
            var status = zp.Status(arg);
            Console.WriteLine(status.Dump(new JsonFormatter()));
            var stopwatch = Stopwatch.StartNew();
            var oldExist = status.Pool.VDevs.Any(v => v.Devices.Any(d => d.DeviceName == oldDevice));
            while (oldExist && stopwatch.Elapsed < TimeSpan.FromSeconds(5))
            {
                status = zp.Status(arg);
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
            var arg = new PoolStatusArgs { PoolName = pool.Name };
            var status = zp.Status(arg);
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

            status = zp.Status(arg);
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

            var args = new PoolCreateArgs
            {
                PoolName = name,
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
            var arg = new PoolStatusArgs { PoolName = pool.Name };
            status = zp.Status(arg);

            Console.WriteLine(status.Dump(new JsonFormatter()));
            var stopwatch = Stopwatch.StartNew();
            var oldExist = status.Pool.VDevs.Count == 2;
            while (oldExist && stopwatch.Elapsed < TimeSpan.FromSeconds(5))
            {
                status = zp.Status(arg);
                oldExist = status.Pool.VDevs.Count == 2;
            }
            Console.WriteLine(status.Dump(new JsonFormatter()));
        }

        [TestMethod, TestCategory("Integration")]
        public void OfflineTest()
        {
            using var pool = TestPool.CreateSimplePool(_remoteProcessCall);
            var zp = GetZpool();
            var arg = new PoolStatusArgs { PoolName = pool.Name };
            var device = pool.Disks.Last();
            var args = new PoolOfflineArgs { PoolName = pool.Name, Device = device };
            zp.Offline(args);

            var status = zp.Status(arg);

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
            var arg = new PoolStatusArgs { PoolName = pool.Name };
            var status = zp.Status(arg);

            Console.WriteLine(status.Dump(new JsonFormatter()));
            Assert.AreEqual(State.Degraded, status.State);

            var onlineArgs = new PoolOnlineArgs { PoolName = pool.Name, Device = device };
            zp.Online(onlineArgs);

            status = zp.Status(arg);

            Console.WriteLine(status.Dump(new JsonFormatter()));
            Assert.AreEqual(State.Online, status.State);

        }
    }
}
