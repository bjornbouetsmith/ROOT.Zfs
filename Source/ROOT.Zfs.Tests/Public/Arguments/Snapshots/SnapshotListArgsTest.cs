using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Arguments.Snapshots;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Public.Arguments.Snapshots
{
    [TestClass]
    public class SnapshotListArgsTest
    {
        [TestMethod]
        public void DefaultValuesTest()
        {
            var args = new SnapshotListArgs { Root = "tank/myds" };

            Assert.AreEqual(DatasetTypes.Snapshot, args.DatasetTypes);
            Assert.IsTrue(args.IncludeChildren);
            Assert.AreEqual("tank/myds", args.Root);
        }
    }
}
