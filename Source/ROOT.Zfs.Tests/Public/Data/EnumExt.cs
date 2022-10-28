using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Tests.Public.Data
{
    [TestClass]
    public class EnumExt
    {

        /// <summary>
        /// Test to ensure that all all <see cref="VDevCreationType"/> are handled properly
        /// </summary>
        [TestMethod]
        public void ReflectionBasedTest()
        {
            foreach (var type in Enum.GetValues(typeof(VDevCreationType)).OfType<VDevCreationType>())
            {
                var s = type.AsString();
                Console.WriteLine("{0}={1}", type, s);
                Assert.IsNotNull(s);
            }
        }
        /// <summary>
        /// Known <see cref="VDevCreationType"/> to string mappings
        /// </summary>
        [DataRow(VDevCreationType.Special, "special")]
        [DataRow(VDevCreationType.Cache, "cache")]
        [DataRow(VDevCreationType.Dedup, "dedup")]
        [DataRow(VDevCreationType.DRaid1, "draid1")]
        [DataRow(VDevCreationType.DRaid2, "draid2")]
        [DataRow(VDevCreationType.DRaid3, "draid3")]
        [DataRow(VDevCreationType.Log, "log")]
        [DataRow(VDevCreationType.Mirror, "mirror")]
        [DataRow(VDevCreationType.Raidz1, "raidz1")]
        [DataRow(VDevCreationType.Raidz2, "raidz2")]
        [DataRow(VDevCreationType.Raidz3, "raidz3")]
        [DataRow(VDevCreationType.Spare, "spare")]
        [TestMethod]
        public void VDevCreationTypeAsStringTest(VDevCreationType type, string expected)
        {
            var s = type.AsString();
            Assert.AreEqual(expected, s);
        }

        [TestMethod]
        public void UnsupportedVDevCreationTypeShouldThrow()
        {
            VDevCreationType type = (VDevCreationType)(int)-999;

            var ex = Assert.ThrowsException<NotSupportedException>(() => type.AsString());
            Console.WriteLine(ex.Message);
        }

        [DataRow(default, "filesystem,volume")]
        [DataRow(DatasetTypes.Filesystem, "filesystem")]
        [DataRow(DatasetTypes.Bookmark, "bookmark")]
        [DataRow(DatasetTypes.Snapshot, "snapshot")]
        [DataRow(DatasetTypes.Volume, "volume")]
        [DataRow(DatasetTypes.Volume | DatasetTypes.Bookmark, "bookmark,volume")]
        [DataRow(DatasetTypes.Volume | DatasetTypes.Bookmark | DatasetTypes.Snapshot, "bookmark,snapshot,volume")]
        [DataRow(DatasetTypes.Volume | DatasetTypes.Bookmark | DatasetTypes.Snapshot| DatasetTypes.Filesystem, "bookmark,filesystem,snapshot,volume")]
        [TestMethod]
        public void DataSetTypesAsStringTest(DatasetTypes types, string expected)
        {
            var stringVer = types.AsString();
            Assert.AreEqual(expected, stringVer);
        }
    }
}
