using System;
using Xunit;
using ROOT.Zfs.Public.Arguments.Pool;

namespace ROOT.Zfs.Tests.Public.Arguments.Pool;

public class PoolDetachArgsTest
{
    [InlineData("tank", "sda", true)]
    [InlineData("tank", null, false)]
    [InlineData("tank", "", false)]
    [InlineData("tank", " ", false)]
    [InlineData(null, "sda", false)]
    [InlineData("", "sda", false)]
    [InlineData(" ", "sda", false)]
    [InlineData(null, null, false)]
    [Theory]
    public void ValidateTest(string poolName, string device, bool expectedValid)
    {
        var args = new PoolDetachArgs { PoolName = poolName, Device = device };

        var valid = args.Validate(out var errors);
        Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
        Assert.Equal(expectedValid, valid);
    }

    [InlineData("tank", "sda", "detach tank sda")]
    [InlineData("tank", "/dev/sda", "detach tank /dev/sda")]
    [Theory]
    public void ToStringTest(string poolName, string device, string expected)
    {
        var args = new PoolDetachArgs { PoolName = poolName, Device = device };
        var stringVer = args.ToString();
        Console.WriteLine(stringVer);
        Assert.Equal(expected, stringVer);
    }
}