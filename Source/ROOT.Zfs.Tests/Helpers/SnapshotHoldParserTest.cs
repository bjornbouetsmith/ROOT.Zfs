using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Core.Helpers;

namespace ROOT.Zfs.Tests.Helpers
{
    [TestClass]
    public class SnapshotHoldParserTest
    {
        [DataRow("tank/myds@20220925111346  mytag  Ons Oct 26 09:20 2022", "tank/myds@20220925111346","mytag")]
        [TestMethod]
        public void BadDateFormatShouldNotCauseParsingErrrors(string line, string snapshot, string tag)
        {
            var holds = SnapshotHoldParser.ParseStdOut(line);
            Assert.AreEqual(1, holds.Count);
            Assert.AreEqual(snapshot, holds[0].Snapshot);
            Assert.AreEqual(tag, holds[0].Tag);
            Assert.AreEqual(default, holds[0].HoldTime);
        }

        [DataRow("tank/myds@20220925111346  mytag  Wed Oct 26 09:20 2022", "tank/myds@20220925111346", "mytag","2022-10-26 09:20")]
        [TestMethod]
        public void ExpectedDateFormatShouldParseOk(string line, string snapshot, string tag, string dateString)
        {
            var expectedDate = DateTime.ParseExact(dateString, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            var holds = SnapshotHoldParser.ParseStdOut(line);
            Assert.AreEqual(1, holds.Count);
            Assert.AreEqual(snapshot, holds[0].Snapshot);
            Assert.AreEqual(tag, holds[0].Tag);
            Assert.AreEqual(expectedDate, holds[0].HoldTime);
        }
    }
}
