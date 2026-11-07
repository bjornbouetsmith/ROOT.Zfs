using Xunit;
using ROOT.Zfs.Public.Arguments.Pool;
using System;

namespace ROOT.Zfs.Tests.Public.Arguments.Pool;

public class PoolIOStatsArgsTest
{
    private readonly ITestOutputHelper _output;

    public PoolIOStatsArgsTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [InlineData("tank", null, true)] // only pool
    [InlineData("tank", "", true)] //only pool
    [InlineData("tank", "/dev/sda", true)] //single device
    [InlineData("tank", "%2fdev%2fsda", true)] //single device url encoded
    [InlineData("tank", "/dev/sda,/dev/sdb", true)] //multiple devices
    [InlineData(null,null,false)]
    [InlineData("", null, false)]
    [InlineData(" ", null, false)]
    [InlineData("rm -rf /", null, false)]
    [InlineData("tank/myds", null, false)]
    [InlineData("tank/myds", "/dev/sda,rm -rf /", false)]
    [Theory]
    public void ValidateTest(string pool, string devices, bool expectValid)
    {
        var args = new PoolIOStatsArgs { PoolName = pool, Devices = devices?.Split(',') };
        var valid = args.Validate(out var errors);
        _output.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
        Assert.Equal(expectValid, valid);
    }

    [InlineData("tank", null, "iostat -LlpPvH tank")] // only pool
    [InlineData("tank", "", "iostat -LlpPvH tank")] //only pool with empty string as device
    [InlineData("tank", "/dev/sda", "iostat -LlpPvH tank /dev/sda")] //single device
    [InlineData("tank", "%2fdev%2fsda", "iostat -LlpPvH tank /dev/sda")] //single device url encoded
    [InlineData("tank", "/dev/sda,/dev/sdb", "iostat -LlpPvH tank /dev/sda /dev/sdb")] //multiple devices
    [Theory]
    public void ToStringTest(string pool, string devices, string expected)
    {
        var args = new PoolIOStatsArgs { PoolName = pool, Devices = devices?.Split(',') };
        var stringVer = args.ToString();
        Assert.Equal(expected, stringVer);
    }
}