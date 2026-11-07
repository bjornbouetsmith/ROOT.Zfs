using System;
using Xunit;
using ROOT.Zfs.Public.Arguments.Snapshots;

namespace ROOT.Zfs.Tests.Public.Arguments.Snapshots;

public class SnapshotDestroyArgsTest
{
    [InlineData("tank/myds", "snap123", true)]
    [InlineData("tank/myds", "tank/myds@snap123", true)]
    [InlineData("tank%2fmyds", "tank%2fmyds%40snap123", true)]
    [InlineData("tank%2fmyds", "snap123 %26%26+rm+-rf+%2f", false)]
    [InlineData("tank%2fmyds", null, false)]
    [Theory]
    public void ValidateTest(string dataset, string snapshot, bool expectValid)
    {
        var args = new SnapshotDestroyArgs { Dataset = dataset, Snapshot = snapshot };
        var valid = args.Validate(out var errors);

        Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
        Assert.Equal(expectValid, valid);
    }

    [InlineData("tank/myds", "snap123", "destroy tank/myds@snap123")]
    [InlineData("tank/myds", "tank/myds@snap123", "destroy tank/myds@snap123")]
    [InlineData("tank%2fmyds", "tank%2fmyds%40snap123", "destroy tank/myds@snap123")]
    [Theory]
    public void ToStringTest(string dataset, string snapshot, string expected)
    {
        var args = new SnapshotDestroyArgs { Dataset = dataset, Snapshot = snapshot };
        var stringVer = args.ToString();
        Assert.Equal(expected, stringVer);
    }
}