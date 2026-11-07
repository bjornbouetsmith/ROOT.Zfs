using Xunit;
using ROOT.Zfs.Public.Arguments.Pool;
using ROOT.Zfs.Public.Data.Pools;
using System;

namespace ROOT.Zfs.Tests.Public.Arguments.Pool;

public class PoolScrubArgsTest
{
    private readonly ITestOutputHelper _output;

    public PoolScrubArgsTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [InlineData("tank", true)]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData(" ", false)]
    [InlineData("rm -rf /tank", false)]
    [Theory]
    public void ValidateTest(string pool, bool expectedValid)
    {
        var args = new PoolScrubArgs { PoolName = pool };
        var valid = args.Validate(out var errors);
        _output.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
        Assert.Equal(expectedValid, valid);
    }

    [InlineData("tank", ScrubOption.None, "scrub tank")]
    [InlineData("tank", ScrubOption.Stop, "scrub -s tank")]
    [InlineData("tank", ScrubOption.Pause, "scrub -p tank")]
    [Theory]
    public void ToStringTest(string pool, ScrubOption option, string expected)
    {
        var args = new PoolScrubArgs { PoolName = pool, Options = option };
        var stringVer = args.ToString();
        Assert.Equal(expected, stringVer);
    }
}