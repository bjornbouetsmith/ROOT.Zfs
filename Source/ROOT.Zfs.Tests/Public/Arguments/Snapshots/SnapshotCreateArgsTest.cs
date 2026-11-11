using System;
using System.Globalization;
using Xunit;
using ROOT.Zfs.Public.Arguments.Snapshots;

namespace ROOT.Zfs.Tests.Public.Arguments.Snapshots;

public class SnapshotCreateArgsTest
{
    private readonly ITestOutputHelper _output;

    public SnapshotCreateArgsTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [InlineData("tank/myds",null,true)]
    [InlineData("tank/myds", "", true)]
    [InlineData("tank/myds", " ", true)]
    [InlineData("tank/myds", "tank/myds@test123", true)]
    [InlineData("tank/myds", "test123", true)]
    [InlineData("tank/myds", "test123/mytest", false)]
    [InlineData("tank/myds", "test123 && rm -rf /", false)]
    [InlineData(null, "test123", false)]
    [InlineData("", "test123", false)]
    [InlineData(" ", "test123", false)]
    [Theory]
    public void ValidateTest(string dataset, string snapshot, bool expectedValid)
    {
        var args = new SnapshotCreateArgs { Dataset = dataset, Snapshot = snapshot };

        var valid = args.Validate(out var errors);
        _output.WriteLine(string.Join(Environment.NewLine,errors ?? Array.Empty<string>()));
        Assert.Equal(expectedValid, valid);
    }

    [InlineData("tank/myds", "tank/myds@test123", "snap tank/myds@test123")]
    [InlineData("tank/myds", "test123", "snap tank/myds@test123")]
    [InlineData("tank/myds", null, "snap tank/myds@")]
    [InlineData("tank/myds", "", "snap tank/myds@")]
    [InlineData("tank/myds", " ", "snap tank/myds@")]
    [Theory]
    public void ToStringTest(string dataset, string snapshot,string expected)
    {
        var args = new SnapshotCreateArgs { Dataset = dataset, Snapshot = snapshot };

        var stringVer = args.ToString();
        if (!string.IsNullOrWhiteSpace(snapshot))
        {
            Assert.Equal(expected, stringVer);
        }
        else
        {
            var suffix = DateTime.UtcNow.ToLocalTime().ToString("yyyyMMddHH", CultureInfo.InvariantCulture);
            Assert.StartsWith(expected + suffix, stringVer);
        }

        _output.WriteLine(stringVer);
    }
}