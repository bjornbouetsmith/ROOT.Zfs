using System;
using Xunit;
using ROOT.Zfs.Public.Arguments.Pool;

namespace ROOT.Zfs.Tests.Public.Arguments.Pool;

public class PoolOnlineArgsTest
{
    [InlineData(null, "disk", false)]
    [InlineData("", "disk", false)]
    [InlineData(" ", "disk", false)]
    [InlineData("tank", "disk", true)]
    [InlineData("tank", null, false)]
    [InlineData("tank", "", false)]
    [InlineData("tank", " ", false)]
    [Theory]
    public void ValidateTest(string pool, string device, bool expectedValid)
    {
        var args = new PoolOnlineArgs
        {
            PoolName = pool,
            Device = device,
        };

        var valid = args.Validate(out var errors);
        Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
        Assert.Equal(expectedValid, valid);
    }


    [InlineData("tank", "sda", false, "online tank sda")]
    [InlineData("tank", "sda", true, "online -e tank sda")]
    [Theory]
    public void BuildArgsTest(string pool, string device, bool expandSpace, string expected)
    {
        var args = new PoolOnlineArgs
        {
            PoolName = pool,
            Device = device,
            ExpandSpace = expandSpace
        };
        var stringVer = args.ToString();

        Assert.Equal(expected, stringVer);
    }
}