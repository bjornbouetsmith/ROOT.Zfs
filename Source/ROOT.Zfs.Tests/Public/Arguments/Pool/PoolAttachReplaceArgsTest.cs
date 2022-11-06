using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Arguments.Pool;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Public.Arguments.Pool
{
    [TestClass]
    public class PoolAttachReplaceArgsTest
    {
        [DataRow("tank", "/dev/sdb", "/dev/sdc", false, false, null, "attach tank /dev/sdb /dev/sdc")]
        [DataRow("tank", "/dev/sdb", null, false, false, null, "attach tank /dev/sdb")]
        [DataRow("tank", "/dev/sdb", "", false, false, null, "attach tank /dev/sdb")]
        [DataRow("tank", "/dev/sdb", " ", false, false, null, "attach tank /dev/sdb")]
        [DataRow("tank", "/dev/sdb", "/dev/sdc", true, false, null, "attach -f tank /dev/sdb /dev/sdc")]
        [DataRow("tank", "/dev/sdb", "/dev/sdc", true, true, null, "attach -f -s tank /dev/sdb /dev/sdc")]
        [DataRow("tank", "/dev/sdb", "/dev/sdc", true, true, "ashift=12", "attach -f -s -o ashift=12 tank /dev/sdb /dev/sdc")]
        [DataRow("tank", "/dev/sdb", "/dev/sdc", true, true, "atime=off", "attach -f -s tank /dev/sdb /dev/sdc")] // non supported property

        [TestMethod]
        public void ToStringTest(string pool, string vdev, string newDevice, bool force, bool sequential, string properties, string expected)
        {
            var props = properties?.Split(',').Select(p => p.Split('=')).Select(a => new PropertyValue { Property = a[0], Value = a[1] }).ToArray();
            var args = new PoolAttachArgs
            {
                PoolName = pool,
                OldDevice = vdev,
                NewDevice = newDevice,
                Force = force,
                RestoreSequentially = sequential,
                PropertyValues = props
            };

            var stringVer = args.ToString();
            Console.WriteLine(stringVer);
            Assert.AreEqual(expected, stringVer);
        }

        [DataRow("tank", "/dev/sdb", null, false, false, null, false)]
        [DataRow("tank", "/dev/sdb", "", false, false, null, false)]
        [DataRow("tank", "/dev/sdb", " ", false, false, null, false)]
        [DataRow("tank", null, "/dev/sdc", false, false, null, false)]
        [DataRow("tank", "", "/dev/sdc", false, false, null, false)]
        [DataRow("tank", " ", "/dev/sdc", false, false, null, false)]
        [DataRow(null, "/dev/sdb", "/dev/sdc", false, false, null, false)]
        [DataRow("", "/dev/sdb", "/dev/sdc", false, false, null, false)]
        [DataRow(" ", "/dev/sdb", "/dev/sdc", false, false, null, false)]
        [DataRow("tank", "/dev/sdb", "/dev/sdc", false, false, null, true)]
        [DataRow("tank", "/dev/sdb", "/dev/sdc", true, false, null, true)]
        [DataRow("tank", "/dev/sdb", "/dev/sdc", true, true, null, true)]
        [DataRow("tank", "/dev/sdb", "/dev/sdc", true, true, "ashift=12", true)]
        [DataRow("tank", "/dev/sdb", "/dev/sdc", true, true, "atime=off", true)] // non supported property just gets ignored

        [TestMethod]
        public void ValidateTest(string pool, string oldDevice, string newDevice, bool force, bool sequential, string properties, bool expectedValid)
        {
            var props = properties?.Split(',').Select(p => p.Split('=')).Select(a => new PropertyValue { Property = a[0], Value = a[1] }).ToArray();
            var args = new PoolAttachArgs
            {
                PoolName = pool,
                OldDevice = oldDevice,
                NewDevice = newDevice,
                Force = force,
                RestoreSequentially = sequential,
                PropertyValues = props
            };

            var valid = args.Validate(out var errors);
            Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));

            Assert.AreEqual(expectedValid, valid);
        }

        [DataRow("tank", "/dev/sdb", null, true)]
        [DataRow("tank", "/dev/sdb", "/dev/sdc", true)]
        [TestMethod]
        public void ValidateWhenReplacing(string pool, string oldDevice, string newDevice, bool expectedValid)
        {
            var args = new PoolReplaceArgs
            {
                PoolName = pool,
                OldDevice = oldDevice,
                NewDevice = newDevice,
            };

            var valid = args.Validate(out var errors);
            Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));

            Assert.AreEqual(expectedValid, valid);
        }
    }
}
