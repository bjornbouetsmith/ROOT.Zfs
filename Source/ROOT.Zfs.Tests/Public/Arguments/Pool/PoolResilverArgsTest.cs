using System;
using Xunit;
using ROOT.Zfs.Public.Arguments.Pool;

namespace ROOT.Zfs.Tests.Public.Arguments.Pool;

public class PoolResilverArgsTest
{
    private readonly ITestOutputHelper _output;

    public PoolResilverArgsTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [InlineData("tank", true)]
    [InlineData("tank%2etest", true)]
    [InlineData("tank%2fmyds", false)]
    [InlineData("tank/myds", false)]
    [Theory]
    public void ValidateTest(string pool, bool expectedValid)
    {
        var args = new PoolResilverArgs { PoolName = pool };
        var valid = args.Validate(out var errors);
        _output.WriteLine(string.Join(Environment.NewLine,errors ?? Array.Empty<string>()));
        Assert.Equal(expectedValid, valid);
    }

    [InlineData("tank","resilver tank")]
    [InlineData("tank%2etest", "resilver tank.test")]
    [Theory]
    public void ToStringTest(string pool, string expected)
    {
        var args = new PoolResilverArgs{PoolName=pool};
        var stringVer  = args.ToString();
        Assert.Equal(expected, stringVer);
    }
}