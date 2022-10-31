using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Arguments;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Public.Arguments
{
    [TestClass]
    public class ZpoolAttachArgsTest
    {
        [DataRow("tank", "mirror-0", "/dev/sdc", false, false, null, " tank mirror-0 /dev/sdc")]
        [DataRow("tank", "mirror-0", "/dev/sdc", true, false, null, " -f tank mirror-0 /dev/sdc")]
        [DataRow("tank", "mirror-0", "/dev/sdc", true, true, null, " -f -s tank mirror-0 /dev/sdc")]
        [DataRow("tank", "mirror-0", "/dev/sdc", true, true, "ashift=12", " -f -s -o ashift=12 tank mirror-0 /dev/sdc")]
        [DataRow("tank", "mirror-0", "/dev/sdc", true, true, "atime=off", " -f -s tank mirror-0 /dev/sdc")] // non supported property

        [TestMethod]
        public void ToStringTest(string pool, string vdev, string newDevice, bool force, bool sequential, string properties, string expected)
        {
            var props = properties?.Split(',').Select(p => p.Split('=')).Select(a => new PropertyValue { Property = a[0], Value = a[1] }).ToArray();
            var args = new ZpoolAttachArgs
            {
                PoolName = pool,
                VDev = vdev,
                NewDevice = newDevice,
                Force = force,
                RestoreSequentially = sequential,
                PropertyValues = props
            };

            var stringVer = args.ToString();
            Assert.AreEqual(expected, stringVer);
        }
        [DataRow("tank", "mirror-0", null, false, false, null, false)]
        [DataRow("tank", "mirror-0", "", false, false, null, false)]
        [DataRow("tank", "mirror-0", " ", false, false, null, false)]
        [DataRow("tank", null, "/dev/sdc", false, false, null, false)]
        [DataRow("tank", "", "/dev/sdc", false, false, null, false)]
        [DataRow("tank", " ", "/dev/sdc", false, false, null, false)]
        [DataRow(null, "mirror-0", "/dev/sdc", false, false, null, false)]
        [DataRow("", "mirror-0", "/dev/sdc", false, false, null, false)]
        [DataRow(" ", "mirror-0", "/dev/sdc", false, false, null, false)]
        [DataRow("tank", "mirror-0", "/dev/sdc", false, false, null, true)]
        [DataRow("tank", "mirror-0", "/dev/sdc", true, false, null, true)]
        [DataRow("tank", "mirror-0", "/dev/sdc", true, true, null, true)]
        [DataRow("tank", "mirror-0", "/dev/sdc", true, true, "ashift=12", true)]
        [DataRow("tank", "mirror-0", "/dev/sdc", true, true, "atime=off", true)] // non supported property just gets ignored

        [TestMethod]
        public void ValidateTest(string pool, string vdev, string newDevice, bool force, bool sequential, string properties, bool expectedValid)
        {
            var props = properties?.Split(',').Select(p => p.Split('=')).Select(a => new PropertyValue { Property = a[0], Value = a[1] }).ToArray();
            var args = new ZpoolAttachArgs
            {
                PoolName = pool,
                VDev = vdev,
                NewDevice = newDevice,
                Force = force,
                RestoreSequentially = sequential,
                PropertyValues = props
            };

            var valid = args.Validate(out var errors);
            Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));

            Assert.AreEqual(expectedValid, valid);
        }
    }
}
