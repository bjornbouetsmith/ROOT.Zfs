using System;
using Xunit;
using ROOT.Zfs.Core;
using ROOT.Zfs.Core.Helpers;

namespace ROOT.Zfs.Tests.Helpers;

public class SnapshotHelperTest
{
    private const string SnapshotList = @"snapshot        1664116031      tank/myds@20220925162707        14336   25600   -       -   -";

    private const string JunkLine = @"snapshot        1664116031      tank/myds@20220925162707        14336   25600";
    private const string JunkLine2 = @"snapshot        2022-05-05      tank/myds@20220925162707        40G   80G   -       -   -";

    [Fact]
    public void BadInputTest1()
    {
        var ex = Assert.Throws<FormatException>(() => SnapshotHelper.FromString(JunkLine));
        Assert.Contains("expected 8 parts", ex.Message);

    }

    [Fact]
    public void BadInputTest2()
    {
        var snapshot = SnapshotHelper.FromString(JunkLine2);
        Assert.Equal(default, snapshot.CreationDate);
    }

    [Fact]
    public void GoodInput()
    {
        var snapshot = SnapshotHelper.FromString(SnapshotList);
        Assert.Equal("tank/myds", snapshot.Dataset);
        Assert.Equal("20220925162707", snapshot.SnapshotName);
        Assert.Equal(14336UL, snapshot.Size.Bytes);
        Assert.Equal(new DateTime(2022, 09, 25, 14, 27, 11, DateTimeKind.Utc), snapshot.CreationDate);
    }

    [Theory]
    [InlineData("tank/myds", "tank/myds@testing123", "testing123", true)] // Exact match except for dataset prefix
    [InlineData("tank/mydsnameisverylong", "testing123", "testing123", true)] // Exact match
    [InlineData("tank/myds", "tank/myds@testing123", "testing", true)] // partial match
    [InlineData("tank/myds", "tank/myds@testing123", "tank/myds@testing123", true)] // ExactMatch
    [InlineData("tank/myds", "tank/myds@testing123", "tank/myds@tes", true)] // partial match
    [InlineData("tank/myds", "tank/myds@testing123", "esting123", false)] // we only match on beginning
    [InlineData("tank2/myds", "tank/myds@testing123", "testing123", false)] //wrong dataset
    [InlineData("tank%2Fmyds", "tank/myds@testing123", "testing123", true)] //right dataset, url encoded
    public void SnapshotMatchingTest(string dataset, string snapshotName, string pattern, bool expectMatch)
    {
        var isMatch = Snapshots.SnapshotMatches(dataset, snapshotName, pattern);
        Assert.Equal(expectMatch, isMatch);
    }
}