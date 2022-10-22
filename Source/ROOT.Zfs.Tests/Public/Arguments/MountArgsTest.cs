using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Arguments;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Public.Arguments
{
    [TestClass]
    public class MountArgsTest
    {

        [TestMethod]
        [DataRow("tank/myds", true, false)]
        [DataRow("tank/myds", false, true)]
        [DataRow("", true, true)]
        [DataRow(null, true, true)]
        [DataRow(null, false, false)]
        [DataRow("", false, false)]
        [DataRow(" ", false, false)]
        public void ValidateTest(string dataset, bool mountAll, bool expectedValid)
        {

            var args = new MountArgs { Dataset = dataset, MountAllFileSystems = mountAll };

            var valid = args.Validate(out var errors);
            Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
            Assert.AreEqual(expectedValid, valid);

        }

        [TestMethod]
        [DataRow(false, false, false, null, false, "tank/myds", " tank/myds")]
        [DataRow(false, false, false, null, true, "tank/myds", " -a")]
        [DataRow(false, false, false, null, true, null, " -a")]
        [DataRow(true, false, false, null, false, "tank/myds", " -O tank/myds")]
        [DataRow(true, true, false, null, false, "tank/myds", " -O -f tank/myds")]
        [DataRow(true, true, true, null, false, "tank/myds", " -O -f -l tank/myds")]
        [DataRow(true, true, true, "atime=off,xattr=off", false, "tank/myds", " -O -f -l -o noatime,noxattr tank/myds")]
        [DataRow(true, true, true, "atime=off,xattr=off", true, "tank/myds", " -O -f -l -o noatime,noxattr -a")]
        [DataRow(true, true, true, "compression=off", true, "tank/myds", " -O -f -l -a")] // Test with non supported property
        public void ToStringTest(bool overlayMount, bool force, bool loadKeys, string properties, bool all, string fileSystem, string expectedString)
        {
            var props = properties?.Split(',').Select(p => p.Split('=')).Select(a => new PropertyValue { Property = a[0], Value = a[1] }).ToArray();
            var args = new MountArgs
            {
                OverLayMount = overlayMount,
                Force = force,
                LoadKeys = loadKeys,
                MountAllFileSystems = all,
                Dataset = fileSystem,
                Properties = props
            };

            var stringVersion = args.ToString();
            Console.WriteLine(stringVersion);
            Assert.AreEqual(expectedString, stringVersion);

        }
    }
}
