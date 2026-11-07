using System;
using System.Linq;
using Xunit;
using ROOT.Zfs.Public.Arguments.Pool;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Public.Arguments.Pool;

public class PoolAttachReplaceArgsTest
{
    [InlineData("tank", "/dev/sdb", "/dev/sdc", false, false, null, "attach tank /dev/sdb /dev/sdc")]
    [InlineData("tank", "/dev/sdb", null, false, false, null, "attach tank /dev/sdb")]
    [InlineData("tank", "/dev/sdb", "", false, false, null, "attach tank /dev/sdb")]
    [InlineData("tank", "/dev/sdb", " ", false, false, null, "attach tank /dev/sdb")]
    [InlineData("tank", "/dev/sdb", "/dev/sdc", true, false, null, "attach -f tank /dev/sdb /dev/sdc")]
    [InlineData("tank", "/dev/sdb", "/dev/sdc", true, true, null, "attach -f -s tank /dev/sdb /dev/sdc")]
    [InlineData("tank", "/dev/sdb", "/dev/sdc", true, true, "ashift=12", "attach -f -s -o ashift=12 tank /dev/sdb /dev/sdc")]
    [InlineData("tank", "/dev/sdb", "/dev/sdc", true, true, "atime=off", "attach -f -s tank /dev/sdb /dev/sdc")] // non supported property

    [Theory]
    public void ToStringTest(string pool, string vdev, string newDevice, bool force, bool sequential, string properties, string expected)
    {
        var props = properties?.Split(',').Select(p => p.Split('=')).Select(a => new PropertyValue { Property = a[0], Value = a[1] }).ToArray();
        var args = new PoolAttachArgs
        {
            PoolName = pool,
            OldDevice = vdev,
            NewDevice = newDevice,
            Force = force,
            RestoreSequentially = sequential,
            PropertyValues = props
        };

        var stringVer = args.ToString();
        Console.WriteLine(stringVer);
        Assert.Equal(expected, stringVer);
    }

    [InlineData("tank", "/dev/sdb", null, false, false, null, false)]
    [InlineData("tank", "/dev/sdb", "", false, false, null, false)]
    [InlineData("tank", "/dev/sdb", " ", false, false, null, false)]
    [InlineData("tank", null, "/dev/sdc", false, false, null, false)]
    [InlineData("tank", "", "/dev/sdc", false, false, null, false)]
    [InlineData("tank", " ", "/dev/sdc", false, false, null, false)]
    [InlineData(null, "/dev/sdb", "/dev/sdc", false, false, null, false)]
    [InlineData("", "/dev/sdb", "/dev/sdc", false, false, null, false)]
    [InlineData(" ", "/dev/sdb", "/dev/sdc", false, false, null, false)]
    [InlineData("tank", "/dev/sdb", "/dev/sdc", false, false, null, true)]
    [InlineData("tank", "/dev/sdb", "/dev/sdc", true, false, null, true)]
    [InlineData("tank", "/dev/sdb", "/dev/sdc", true, true, null, true)]
    [InlineData("tank", "/dev/sdb", "/dev/sdc", true, true, "ashift=12", true)]
    [InlineData("tank", "/dev/sdb", "/dev/sdc", true, true, "atime=off", true)] // non supported property just gets ignored

    [Theory]
    public void ValidateTest(string pool, string oldDevice, string newDevice, bool force, bool sequential, string properties, bool expectedValid)
    {
        var props = properties?.Split(',').Select(p => p.Split('=')).Select(a => new PropertyValue { Property = a[0], Value = a[1] }).ToArray();
        var args = new PoolAttachArgs
        {
            PoolName = pool,
            OldDevice = oldDevice,
            NewDevice = newDevice,
            Force = force,
            RestoreSequentially = sequential,
            PropertyValues = props
        };

        var valid = args.Validate(out var errors);
        Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));

        Assert.Equal(expectedValid, valid);
    }

    [InlineData("tank", "/dev/sdb", null, true)]
    [InlineData("tank", "/dev/sdb", "/dev/sdc", true)]
    [Theory]
    public void ValidateWhenReplacing(string pool, string oldDevice, string newDevice, bool expectedValid)
    {
        var args = new PoolReplaceArgs
        {
            PoolName = pool,
            OldDevice = oldDevice,
            NewDevice = newDevice,
        };

        var valid = args.Validate(out var errors);
        Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));

        Assert.Equal(expectedValid, valid);
    }
}