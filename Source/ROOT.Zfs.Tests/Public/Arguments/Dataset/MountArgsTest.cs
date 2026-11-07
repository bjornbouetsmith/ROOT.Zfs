using System;
using System.Linq;
using Xunit;
using ROOT.Zfs.Public.Arguments.Dataset;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Public.Arguments.Dataset;

public class MountArgsTest
{

    [Theory]
    [InlineData("tank/myds", true, false)]
    [InlineData("tank/myds", false, true)]
    [InlineData("", true, true)]
    [InlineData(null, true, true)]
    [InlineData(null, false, false)]
    [InlineData("", false, false)]
    [InlineData(" ", false, false)]
    public void ValidateTest(string dataset, bool mountAll, bool expectedValid)
    {

        var args = new MountArgs { Filesystem = dataset, MountAllFileSystems = mountAll };

        var valid = args.Validate(out var errors);
        Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
        Assert.Equal(expectedValid, valid);

    }

    [Theory]
    [InlineData(false, false, false, null, false, "tank/myds", "mount tank/myds")]
    [InlineData(false, false, false, null, true, "tank/myds", "mount -a")]
    [InlineData(false, false, false, null, true, null, "mount -a")]
    [InlineData(true, false, false, null, false, "tank/myds", "mount -O tank/myds")]
    [InlineData(true, true, false, null, false, "tank/myds", "mount -O -f tank/myds")]
    [InlineData(true, true, true, null, false, "tank/myds", "mount -O -f -l tank/myds")]
    [InlineData(true, true, true, "atime=off,xattr=off", false, "tank/myds", "mount -O -f -l -o noatime,noxattr tank/myds")]
    [InlineData(true, true, true, "atime=off,xattr=off", true, "tank/myds", "mount -O -f -l -o noatime,noxattr -a")]
    [InlineData(true, true, true, "compression=off", true, "tank/myds", "mount -O -f -l -a")] // Test with non supported property
    public void ToStringTest(bool overlayMount, bool force, bool loadKeys, string properties, bool all, string fileSystem, string expectedString)
    {
        var props = properties?.Split(',').Select(p => p.Split('=')).Select(a => new PropertyValue { Property = a[0], Value = a[1] }).ToArray();
        var args = new MountArgs
        {
            OverLayMount = overlayMount,
            Force = force,
            LoadKeys = loadKeys,
            MountAllFileSystems = all,
            Filesystem = fileSystem,
            Properties = props
        };

        var stringVersion = args.ToString();
        Console.WriteLine(stringVersion);
        Assert.Equal(expectedString, stringVersion);

    }
}