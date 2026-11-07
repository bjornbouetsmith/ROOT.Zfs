using System;
using Xunit;
using ROOT.Zfs.Public.Arguments.Pool;

namespace ROOT.Zfs.Tests.Public.Arguments.Pool;

public class PoolUpgradeArgsTest
{
    private readonly ITestOutputHelper _output;

    public PoolUpgradeArgsTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [InlineData("tank", false, true)]
    [InlineData("tank", true, true)]
    [InlineData("", true, true)]
    [InlineData(null, true, true)]
    [InlineData(" ", true, true)]
    [InlineData(" ", false, false)]
    [InlineData("", false, false)]
    [InlineData(null, false, false)]
    [Theory]
    public void ValidTest(string poolName, bool all, bool expectedValid)
    {
        var args = new PoolUpgradeArgs { PoolName = poolName, AllPools = all };
        var valid = args.Validate(out var errors);
        _output.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
        Assert.Equal(expectedValid, valid);
    }

    [InlineData("tank",true,"upgrade -a")]
    [InlineData("tank", false, "upgrade tank")]
    [InlineData(null, true, "upgrade -a")]
    [InlineData("", true, "upgrade -a")]
    [InlineData(" ", true, "upgrade -a")]
    [Theory]
    public void ToStringTest(string poolName, bool all, string expected)
    {
        var args = new PoolUpgradeArgs { PoolName = poolName, AllPools = all };
        var stringVer = args.ToString();
        Assert.Equal(expected, stringVer);
    }
}