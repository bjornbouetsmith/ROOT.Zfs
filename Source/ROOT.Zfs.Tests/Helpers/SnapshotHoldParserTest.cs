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
        Assert.Single(holds);
        Assert.Equal(snapshot, holds[0].Snapshot);
        Assert.Equal(tag, holds[0].Tag);
        Assert.Equal(default, holds[0].HoldTime);
    }

    [InlineData("tank/myds@20220925111346  mytag  Wed Oct 26 09:20 2022", "tank/myds@20220925111346", "mytag","2022-10-26 09:20")]
    [Theory]
    public void ExpectedDateFormatShouldParseOk(string line, string snapshot, string tag, string dateString)
    {
        var expectedDate = DateTime.ParseExact(dateString, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
        var holds = SnapshotHoldParser.ParseStdOut(line);
        Assert.Single(holds);
        Assert.Equal(snapshot, holds[0].Snapshot);
        Assert.Equal(tag, holds[0].Tag);
        Assert.Equal(expectedDate, holds[0].HoldTime);
    }
}