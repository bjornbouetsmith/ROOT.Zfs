﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Arguments.Pool;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Tests
{
    /// <summary>
    /// Use this to create pools based on disks in the filesystem
    /// </summary>
    internal class TestPool : IDisposable
    {
        private readonly IProcessCall _remoteProcessCall;
        private PoolCreateArgs _args;

        public string Name => _args.PoolName;

        public TestPool(IProcessCall remoteProcessCall)
        {
            _remoteProcessCall = remoteProcessCall;
            CreateBaseDirectory();
        }

        public List<string> Disks { get; } = new List<string>();

        public PoolStatus CreatePool(PoolCreateArgs args)
        {
            _args = _args == null ? args : throw new InvalidOperationException("Cannot reuse TestPool for more than one creation");
            IZPool zp = new ZPool(_remoteProcessCall);
            zp.RequiresSudo = Environment.MachineName != "BBS-DESKTOP";
            return zp.Create(args);
        }

        public string AddDisk(Size size = default)
        {
            var name = "/tmp/zfs-pool/" + Guid.NewGuid();
            Disks.Add(name);
            if (size.Bytes == 0)
            {
                size = "100M";
            }
            
            CreateTestDisk(name, size);
            return name;
        }

        private void CreateBaseDirectory()
        {
            var pc = _remoteProcessCall | new ProcessCall("/usr/bin/mkdir", "-p /tmp/zfs-pool");
            pc.RequiresSudo = Environment.MachineName != "BBS-DESKTOP";
            pc.LoadResponse(true);
        }

        private bool DestroyPool()
        {
            var pc = _remoteProcessCall | new ProcessCall("/sbin/zpool", $"destroy {_args.PoolName}");
            pc.RequiresSudo = Environment.MachineName != "BBS-DESKTOP";
            var response = pc.LoadResponse(false);
            if (!response.Success)
            {
                Console.WriteLine(response.StdError);
                return false;

            }

            return true;
        }

        private void CreateTestDisk(string name, Size size)
        {
            var command = $"if=/dev/zero of={name} bs={size.Bytes} count=1";
            var pc = _remoteProcessCall | new ProcessCall("/usr/bin/dd", command);
            pc.RequiresSudo = Environment.MachineName != "BBS-DESKTOP";
            pc.LoadResponse(true);
        }

        private bool DeleteTestDisk(string name)
        {
            var command = $"-f {name}";
            var pc = _remoteProcessCall | new ProcessCall("/usr/bin/rm", command);
            pc.RequiresSudo = Environment.MachineName != "BBS-DESKTOP";
            var response = pc.LoadResponse(false);

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
                Console.WriteLine("Failed to destroy pool :{0} - manual cleanup required", _args.PoolName);
            }

            foreach (var disk in Disks)
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
        public static TestPool CreateSimplePool(IProcessCall remoteProcessCall)
        {
            var pool = new TestPool(remoteProcessCall);
            try
            {
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
                return pool;
            }
            catch(Exception ex)
            {
                Trace.TraceError(ex.ToString());
                pool.Dispose();
                throw;
            }
        }

    }
}