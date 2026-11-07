using System;
using Xunit;
using ROOT.Zfs.Public.Arguments.Dataset;

namespace ROOT.Zfs.Tests.Public.Arguments.Dataset;

public class UnmountArgsTest
{

    [InlineData("tank/myds", true, false)]
    [InlineData("tank/myds", false, true)]
    [InlineData("", false, false)]
    [InlineData("  ", false, false)]
    [InlineData(null, false, false)]
    [InlineData(null, true, true)]
    [InlineData("", true, true)]
    [InlineData("  ", true, true)]
    [Theory]
    public void ValidateTest(string mountpoint, bool all, bool expectedValid)
    {
        var args = new UnmountArgs
        {
            Filesystem = mountpoint,
            UnmountAllFileSystems = all
        };

        var valid = args.Validate(out var errors);
        Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>())); ;
        Assert.Equal(expectedValid, valid);
    }

    [InlineData(true, true, "tank/myds", false, "unmount -f -u tank/myds")]
    [InlineData(true, false, "tank/myds", false, "unmount -f tank/myds")]
    [InlineData(false, false, "tank/myds", false, "unmount tank/myds")]
    [InlineData(true, true, null, true, "unmount -f -u -a")]
    [InlineData(true, false, null, true, "unmount -f -a")]
    [InlineData(false, false, null, true, "unmount -a")]
    [Theory]
    public void ToStringTest(bool force, bool unloadKeys, string mountpoint, bool all, string expectedString)
    {
        var args = new UnmountArgs
        {
            Force = force,
            UnloadKeys = unloadKeys,
            Filesystem = mountpoint,
            UnmountAllFileSystems = all
        };

        var stringVer = args.ToString();
        Console.WriteLine(stringVer);
        Assert.Equal(expectedString, stringVer);
    }
}