using System;
using Xunit;
using ROOT.Zfs.Public.Arguments.Snapshots;
using ROOT.Zfs.Public.Data;
using System.Linq;

namespace ROOT.Zfs.Tests.Public.Arguments.Snapshots;

public class SnapshotCloneArgsTest
{
    [InlineData("tank/myds", "mysnap", "tank2/mysnap_clone", null, true)]
    [InlineData("tank/myds", "tank/myds@mysnap", "tank2/mysnap_clone", null, true)]
    [InlineData("tank/myds", "mysnap", "tank2/mysnap_clone", "atime=off", true)]
    [InlineData("tank/myds", "mysnap", "tank2/mysnap_clone", "atime=off,compression=off", true)]
    [InlineData("tank/myds", "mysnap", "tank2/mysnap_clone", "userobjquota@bbs=1G", true)]
    [InlineData("tank/myds", "mysnap", "", null, false)]
    [InlineData("tank/myds", "", "tank2/mysnap_clone", null, false)]
    [Theory]
    public void ValidateTest(string dataset, string snapshot, string target, string properties, bool expectedValid)
    {
        var props = properties?.Split(',').Select(p => p.Split('=')).Select(a => new PropertyValue { Property = a[0], Value = a[1] }).ToArray();
        var args = new SnapshotCloneArgs { Dataset = dataset, Snapshot = snapshot, TargetDataset = target, Properties = props };

        var valid = args.Validate(out var errors);
        Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
        Assert.Equal(expectedValid, valid);
    }

    [InlineData("tank/myds","mysnap","tank2/mysnap_clone",null,"clone -p tank/myds@mysnap tank2/mysnap_clone")]
    [InlineData("tank/myds", "tank/myds@mysnap", "tank2/mysnap_clone", null, "clone -p tank/myds@mysnap tank2/mysnap_clone")]
    [InlineData("tank/myds", "mysnap", "tank2/mysnap_clone", "atime=off", "clone -p -o atime=off tank/myds@mysnap tank2/mysnap_clone")]
    [InlineData("tank/myds", "mysnap", "tank2/mysnap_clone", "atime=off,compression=off", "clone -p -o atime=off -o compression=off tank/myds@mysnap tank2/mysnap_clone")]
    [Theory]
    public void ToStringTest(string dataset, string snapshot, string target, string properties, string expected)
    {
        var props = properties?.Split(',').Select(p => p.Split('=')).Select(a => new PropertyValue { Property = a[0], Value = a[1] }).ToArray();
        var args = new SnapshotCloneArgs { Dataset = dataset, Snapshot = snapshot, TargetDataset = target, Properties = props };

        var stringVer = args.ToString();
        Console.WriteLine(stringVer);
        Assert.Equal(expected, stringVer);
    }
}