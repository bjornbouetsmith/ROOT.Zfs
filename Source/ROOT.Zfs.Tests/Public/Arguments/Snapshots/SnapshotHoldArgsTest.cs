using System;
using Xunit;
using ROOT.Zfs.Public.Arguments.Snapshots;

namespace ROOT.Zfs.Tests.Public.Arguments.Snapshots;

public class SnapshotHoldArgsTest
{
    [InlineData("tank/myds","snap123","mytag",true)]
    [InlineData("tank/myds", "snap123", null, false)]
    [InlineData("tank/myds", "snap123", "", false)]
    [InlineData("tank/myds", "snap123", " ", false)]
    [InlineData("tank/myds", "snap123", "mytag/test", true)]
    [InlineData("tank/myds", "snap123", "mytag/test@1234", true)]
    [InlineData("tank/myds", "snap123", "mytag && rm -rf /", false)]
    [Theory]
    public void ValidateTest(string dataset, string snapshot, string tag, bool expectedValid)
    {
        var args = new SnapshotHoldArgs { Dataset = dataset, Snapshot = snapshot, Tag = tag };

        var valid = args.Validate(out var errors);
        Console.WriteLine(string.Join(Environment.NewLine,errors ?? Array.Empty<string>()));
        Assert.Equal(expectedValid, valid);
    }

    [InlineData("tank/myds","snap123","mytag",true,"hold -r mytag tank/myds@snap123")] // snapshot name is only snapshot, not including dataset
    [InlineData("tank/myds", "snap123", "mytag", false, "hold mytag tank/myds@snap123")]
    [InlineData("tank/myds", "tank/myds@snap123", "mytag", true, "hold -r mytag tank/myds@snap123")] // Snapshot name is complete name
    [Theory]
    public void ToStringTest(string dataset, string snapshot, string tag, bool recursive, string expected)
    {
        var args = new SnapshotHoldArgs { Dataset = dataset, Snapshot = snapshot, Tag = tag,Recursive=recursive };
        var stringVer = args.ToString();
        Console.WriteLine(stringVer);
        Assert.Equal(expected, stringVer);
    }
}