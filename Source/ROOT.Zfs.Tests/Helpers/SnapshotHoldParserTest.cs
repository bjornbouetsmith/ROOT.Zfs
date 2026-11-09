using System;
using System.Globalization;
using Xunit;
using ROOT.Zfs.Core.Helpers;

namespace ROOT.Zfs.Tests.Helpers;

public class SnapshotHoldParserTest
{
    [InlineData("tank/myds@20220925111346  mytag  Ons Oct 26 09:20 2022", "tank/myds@20220925111346","mytag")]
    [Theory]
    public void BadDateFormatShouldNotCauseParsingErrrors(string line, string snapshot, string tag)
    {
        var holds = SnapshotHoldParser.ParseStdOut(line);
        var hold = Assert.Single(holds);
        Assert.Equal(snapshot, hold.Snapshot);
        Assert.Equal(tag, hold.Tag);
        Assert.Equal(default, hold.HoldTime);
    }

    [InlineData("tank/myds@20220925111346  mytag  Wed Oct 26 09:20 2022", "tank/myds@20220925111346", "mytag","2022-10-26 09:20")]
    [Theory]
    public void ExpectedDateFormatShouldParseOk(string line, string snapshot, string tag, string dateString)
    {
        var expectedDate = DateTime.ParseExact(dateString, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
        var holds = SnapshotHoldParser.ParseStdOut(line);
        var hold = Assert.Single(holds);
        Assert.Equal(snapshot, hold.Snapshot);
        Assert.Equal(tag, hold.Tag);
        Assert.Equal(expectedDate, hold.HoldTime);
    }
}