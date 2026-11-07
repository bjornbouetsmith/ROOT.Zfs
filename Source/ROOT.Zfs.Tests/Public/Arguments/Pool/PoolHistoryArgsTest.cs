using System;
using Xunit;
using ROOT.Zfs.Public.Arguments.Pool;

namespace ROOT.Zfs.Tests.Public.Arguments.Pool;

public class PoolHistoryArgsTest
{
    private readonly ITestOutputHelper _output;

    public PoolHistoryArgsTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [InlineData("tank",true)]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData(" ", false)]
    [Theory]
    public void ValidateTest(string poolName, bool expectedValid)
    {
        var args = new PoolHistoryArgs { PoolName = poolName };

        var valid = args.Validate(out var errors);

        _output.WriteLine(string.Join(Environment.NewLine,errors ?? Array.Empty<string>()));
        Assert.Equal(expectedValid, valid);
    }

    [InlineData("tank","history -l tank")]
    [Theory]
    public void ToStringTest(string poolName, string expected)
    {
        var args = new PoolHistoryArgs { PoolName = poolName };
        var stringVer = args.ToString();

        Assert.Equal(expected, stringVer);
    }
}