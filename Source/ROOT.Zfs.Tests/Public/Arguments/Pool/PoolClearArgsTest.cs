using System;
using Xunit;
using ROOT.Zfs.Public.Arguments.Pool;

namespace ROOT.Zfs.Tests.Public.Arguments.Pool;

public class PoolClearArgsTest
{
    private readonly ITestOutputHelper _output;

    public PoolClearArgsTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [InlineData("tank", "sda", true)]
    [InlineData("tank", "/dev/sda", true)]
    [InlineData("tank", "%2fdev%2fsda", true)]
    [InlineData("tank", null, true)]
    [InlineData("tank", "", true)]
    [InlineData("tank", " ", true)]
    [InlineData("tank", "/dev/sda@123", false)]
    [InlineData(null, "/dev/sda", false)]
    [InlineData("", "/dev/sda", false)]
    [InlineData(" ", "/dev/sda", false)]
    [InlineData("tank/myds@snap", "/dev/sda", false)]
    [InlineData("tank", "/dev/sda && rm -rf /", false)]
    [Theory]
    public void ValidateTest(string pool, string device, bool expectValid)
    {
        var args = new PoolClearArgs { PoolName = pool, Device = device };
        var valid = args.Validate(out var errors);
        _output.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
        Assert.Equal(expectValid, valid);
    }

    [InlineData("tank", "sda", "clear tank sda")]
    [InlineData("tank", "/dev/sda", "clear tank /dev/sda")]
    [InlineData("tank", "%2fdev%2fsda", "clear tank /dev/sda")]
    [InlineData("tank.2", "%2fdev%2fsda", "clear tank.2 /dev/sda")]
    [InlineData("tank", null, "clear tank")]
    [InlineData("tank", "", "clear tank")]
    [InlineData("tank", " ", "clear tank")]
    [Theory]
    public void ToStringTest(string pool, string device, string expected)
    {
        var args = new PoolClearArgs { PoolName = pool, Device = device };

        var stringVer = args.ToString();
        Assert.Equal(expected, stringVer);
    }
}