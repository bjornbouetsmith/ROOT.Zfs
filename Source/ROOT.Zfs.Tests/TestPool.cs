using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core;
using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Tests
{
    /// <summary>
    /// Use this to create pools based on disks in the filesystem
    /// </summary>
    internal class TestPool : IDisposable
    {
        private readonly SSHProcessCall _remoteProcessCall;
        private PoolCreationArgs _args;

        public string Name => _args.Name;
        
        public TestPool(SSHProcessCall remoteProcessCall)
        {
            _remoteProcessCall = remoteProcessCall;
        }

        private readonly List<string> _disks = new List<string>();

        public PoolStatus CreatePool(PoolCreationArgs args)
        {
            _args = _args == null ? args : throw new InvalidOperationException("Cannot reuse TestPool for more than one creation");
            var zp = new ZPool(_remoteProcessCall);
            return zp.CreatePool(args);
        }

        public string AddDisk()
        {
            var name = "/tmp/" + Guid.NewGuid();
            _disks.Add(name);
            CreateTestDisk(name);
            return name;
        }

        private bool DestroyPool()
        {
            var pc = _remoteProcessCall | new ProcessCall("/sbin/zpool", $"destroy {_args.Name}");
            var response = pc.LoadResponse();
            if (!response.Success)
            {
                Console.WriteLine(response.StdError);
                return false;

            }

            return true;
        }


        private void CreateTestDisk(string name)
        {
            var command = $"if=/dev/zero of={name} bs=100MB count=1";
            var pc = _remoteProcessCall | new ProcessCall("/usr/bin/dd", command);
            var response = pc.LoadResponse();

            if (!response.Success)
            {
                throw response.ToException();
            }
        }

        private bool DeleteTestDisk(string name)
        {
            var command = $"-f {name}";
            var pc = _remoteProcessCall | new ProcessCall("/usr/bin/rm", command);
            var response = pc.LoadResponse();

            if (!response.Success)
            {
                Console.WriteLine(response.StdError);
                return false;
            }

            return true;
        }

        public void Dispose()
        {
            if (!DestroyPool())
            {
                Console.WriteLine("Failed to destroy pool :{0} - manual cleanup required", _args.Name);
            }

            foreach (var disk in _disks)
            {
                if (!DeleteTestDisk(disk))
                {
                    Console.WriteLine("Failed to delete disk 1 :{0} - manual cleanup required", disk);
                }
            }
        }
        /// <summary>
        /// Creates a simple mirror pool for any testing where size/type does not really matter
        /// </summary>
        public static TestPool CreateSimplePool(SSHProcessCall remoteProcessCall)
        {
            var pool = new TestPool(remoteProcessCall);
            try
            {
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
                return pool;
            }
            catch 
            {
                pool.Dispose();
                throw;
            }
        }
    }
}