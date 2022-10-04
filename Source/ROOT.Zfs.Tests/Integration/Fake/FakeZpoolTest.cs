using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;
using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Tests.Integration.Fake
{
    [TestClass]
    public class FakeZpoolTest
    {
        readonly IProcessCall _remoteProcessCall = new FakeRemoteConnection("2.1.5-2");

        [TestMethod, TestCategory("FakeIntegration")]
        public void GetHistoryTestWithSkip()
        {
            var zp = new ZPool(_remoteProcessCall);
            var lines = zp.GetHistory("tank").ToList().Count;

            var history = zp.GetHistory("tank", lines - 2).ToList();
            Assert.IsNotNull(history);
            Console.WriteLine(history.Dump(new JsonFormatter()));
            Assert.AreEqual(2, history.Count);
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void GetHistory()
        {
            var zp = new ZPool(_remoteProcessCall);
            var history = zp.GetHistory("tank");
            Assert.IsNotNull(history);
            Console.WriteLine(history.Dump(new JsonFormatter()));
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void GetHistoryAfterDate()
        {
            var zp = new ZPool(_remoteProcessCall);
            var last10AtMost = zp.GetHistory("tank").TakeLast(10).ToList();

            // This is safe, since zfs always have at last pool creation as a history event
            var history = last10AtMost.First();

            var afterDate = zp.GetHistory("tank", 0, history.Time).ToList();

            if (last10AtMost.Count > 1)
            {
                Assert.IsTrue(afterDate.Count < last10AtMost.Count);
            }

        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void ZpoolStatusTest()
        {
            var zfs = new Core.Zfs(_remoteProcessCall);

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
            var zp = new ZPool(_remoteProcessCall);
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
            var zp = new ZPool(_remoteProcessCall);

            var infos = zp.GetAllPoolInfos().ToList();
            Assert.AreEqual(2,infos.Count);

        }


        [TestMethod, TestCategory("FakeIntegration")]
        public void GetPoolInfoTest()
        {
            var zp = new ZPool(_remoteProcessCall);

            var info = zp.GetPoolInfo("tank2");
            Assert.IsNotNull(info);
            Assert.AreEqual("tank2",info.Name);
            Assert.AreEqual("15.5T", info.Size);
            Assert.AreEqual("2.66T", info.Allocated);
            Assert.AreEqual("15.5T", info.Free);
            Assert.AreEqual("7.3%", info.Fragmentation);
            Assert.AreEqual("6.5%", info.CapacityUsed);
            Assert.AreEqual("1.43x", info.DedupRatio);
            Assert.AreEqual(State.Online,info.State);
            Console.WriteLine(info.Dump(new JsonFormatter()));

        }
    }
}
