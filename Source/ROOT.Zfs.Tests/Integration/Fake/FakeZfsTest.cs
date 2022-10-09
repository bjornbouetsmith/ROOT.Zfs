﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.Serialization;

namespace ROOT.Zfs.Tests.Integration.Fake
{
    [TestClass]
    public class FakeZfsTest
    {
        readonly FakeRemoteConnection _remoteProcessCall = new ("2.1.5-2");// "bbs", "zfsdev.root.dom", true);

        [TestMethod,TestCategory("FakeIntegration")]
        public void GetVersionInfo()
        {
            var zfs = new Core.Zfs(_remoteProcessCall);
            var info = zfs.GetVersionInfo();
            Assert.IsNotNull(info);
            Assert.IsTrue(info.Lines.Any());
            foreach (var line in info.Lines)
            {
                Console.WriteLine(line);
            }
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void ListDisksTest()
        {
            var zfs = new Core.Zfs(_remoteProcessCall);
            var disks = zfs.ListDisks();
            Assert.IsNotNull(disks);
            Assert.IsTrue(disks.Any());
            Console.WriteLine(disks.Dump(new JsonFormatter()));
        }

        [TestMethod]
        public void InitializeTest()
        {
            var zfs = new Core.Zfs(_remoteProcessCall);
            zfs.Initialize();

            var commandsInvoked = _remoteProcessCall.GetCommandsInvoked();

            Assert.AreEqual(6, commandsInvoked.Count);
            var asHashSet = commandsInvoked.ToHashSet();
            Assert.IsTrue(asHashSet.Contains("/bin/which zfs"));
            Assert.IsTrue(asHashSet.Contains("/bin/which zpool"));
            Assert.IsTrue(asHashSet.Contains("/bin/which zdb"));
            Assert.IsTrue(asHashSet.Contains("/bin/which ls"));
            Assert.IsTrue(asHashSet.Contains("/bin/which lsblk"));
            Assert.IsTrue(asHashSet.Contains("/bin/which smartctl"));
        }
    }
}