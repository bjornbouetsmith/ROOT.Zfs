using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;
using ROOT.Zfs.Core.Helpers;

namespace ROOT.Zfs.Tests.Helpers
{
    [TestClass]
    public class SnapshotParserAndUtilTest
    {
        private const string SnapshotList = @"snapshot        1664116031      tank/myds@20220925162707        14336   25600   -       -
snapshot        1664116109      tank/myds@20220925162825        13312   25600   -       -
snapshot        1664121534      tank/myds@20220925175851        13312   25600   -       -
snapshot        1664121611      tank/myds@20220925180007        14336   25600   -       -
snapshot        1664303433      tank/myds@20220927203033        13312   25600   -       -";

        [TestMethod]
        public void ParseTest()
        {
            var list = SnapshotParser.Parse(SnapshotList).ToList();

            Assert.AreEqual(5, list.Count);

            foreach (var snap in list)
            {
                Console.WriteLine(snap.CreationDate.AsString());
                Console.WriteLine(snap.Name);
                Console.WriteLine(snap.Size.AsString());
            }
        }
        
        [TestMethod]
        [DataRow("tank/myds", "tank/myds@testing123", "testing123", true)] // Exact match except for dataset prefix
        [DataRow("tank/myds", "tank/myds@testing123", "testing", true)] // partial match
        [DataRow("tank/myds", "tank/myds@testing123", "tank/myds@testing123", true)] // ExactMatch
        [DataRow("tank/myds", "tank/myds@testing123", "tank/myds@tes", true)] // partial match
        [DataRow("tank/myds", "tank/myds@testing123", "esting123", false)] // we only match on beginning
        [DataRow("tank2/myds", "tank/myds@testing123", "testing123", false)] //wrong dataset
        [DataRow("tank%2Fmyds", "tank/myds@testing123", "testing123", true)] //right dataset, url encoded
        public void SnapshotMatchingTest(string dataset, string snapshotName, string pattern, bool expectMatch)
        {
            var isMatch = Snapshots.SnapshotMatches(dataset, snapshotName, pattern);
            Assert.AreEqual(expectMatch, isMatch);
        }
    }
}
