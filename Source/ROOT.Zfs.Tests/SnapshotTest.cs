using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core.Commands;

namespace ROOT.Zfs.Tests
{
    [TestClass]
    public class SnapshotTest
    {
        private const string SnapshotList = @"1590930547      nvme/nfs@auto-20200531.1509-2w  5165371392
1591016947      nvme/nfs@auto-20200601.1509-2w  2669617152
1591103347      nvme/nfs@auto-20200602.1509-2w  2752487424

1591189747      nvme/nfs@auto-20200603.1509-2w  2914050048
1591276147      nvme/nfs@auto-20200604.1509-2w  3080941568
";


        [TestMethod]
        public void SnapshotParserTest()
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
    }
}
