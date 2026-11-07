using System;
using Xunit;
using ROOT.Zfs.Public.Arguments.Snapshots;

namespace ROOT.Zfs.Tests.Public.Arguments.Snapshots;

public class SnapshotHoldsArgsTest
{
    private readonly ITestOutputHelper _output;

    public SnapshotHoldsArgsTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [InlineData("tank/myds", "snap123", true)]
    [InlineData("tank%2fmyds", "snap123", true)]
    [InlineData("tank/myds", null, false)]
    [InlineData("tank/myds", "", false)]
    [InlineData("tank/myds", " ", false)]
    [InlineData("tank/myds", "snap123 && rm -rf /", false)]
    [Theory]
    public void ValidateTest(string dataset, string snapshot, bool expectedValid)
    {
        var args = new SnapshotHoldsArgs { Dataset = dataset, Snapshot = snapshot };

        var valid = args.Validate(out var errors);
        _output.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
        Assert.Equal(expectedValid, valid);
    }

    [InlineData("tank/myds", "snap123", true, "holds -r -H tank/myds@snap123")] // snapshot name is only snapshot, not including dataset
    [InlineData("tank/myds", "snap123",  false, "holds -H tank/myds@snap123")]
    [InlineData("tank/myds", "tank/myds@snap123",  true, "holds -r -H tank/myds@snap123")] // Snapshot name is complete name
    [InlineData("tank%2fmyds", "tank/myds@snap123", true, "holds -r -H tank/myds@snap123")] // Dataset name is url encoded
    [Theory]
    public void ToStringTest(string dataset, string snapshot, bool recursive, string expected)
    {
        var args = new SnapshotHoldsArgs { Dataset = dataset, Snapshot = snapshot, Recursive = recursive };
        var stringVer = args.ToString();
        _output.WriteLine(stringVer);
        Assert.Equal(expected, stringVer);
    }
}