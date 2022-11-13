using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Arguments.Dataset;

namespace ROOT.Zfs.Tests.Public.Arguments.Dataset
{
    [TestClass]
    public class UnmountArgsTest
    {

        [DataRow("tank/myds", true, false)]
        [DataRow("tank/myds", false, true)]
        [DataRow("", false, false)]
        [DataRow("  ", false, false)]
        [DataRow(null, false, false)]
        [DataRow(null, true, true)]
        [DataRow("", true, true)]
        [DataRow("  ", true, true)]
        [TestMethod]
        public void ValidateTest(string mountpoint, bool all, bool expectedValid)
        {
            var args = new UnmountArgs
            {
                Filesystem = mountpoint,
                UnmountAllFileSystems = all
            };

            var valid = args.Validate(out var errors);
            Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>())); ;
            Assert.AreEqual(expectedValid, valid);
        }

        [DataRow(true, true, "tank/myds", false, "unmount -f -u tank/myds")]
        [DataRow(true, false, "tank/myds", false, "unmount -f tank/myds")]
        [DataRow(false, false, "tank/myds", false, "unmount tank/myds")]
        [DataRow(true, true, null, true, "unmount -f -u -a")]
        [DataRow(true, false, null, true, "unmount -f -a")]
        [DataRow(false, false, null, true, "unmount -a")]
        [TestMethod]
        public void ToStringTest(bool force, bool unloadKeys, string mountpoint, bool all, string expectedString)
        {
            var args = new UnmountArgs
            {
                Force = force,
                UnloadKeys = unloadKeys,
                Filesystem = mountpoint,
                UnmountAllFileSystems = all
            };

            var stringVer = args.ToString();
            Console.WriteLine(stringVer);
            Assert.AreEqual(expectedString, stringVer);
        }
    }
}
