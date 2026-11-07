using Xunit;
using ROOT.Zfs.Public.Arguments.Pool;

namespace ROOT.Zfs.Tests.Public.Arguments.Pool;

public class PoolListArgsTest
{
    [InlineData("tank","list -PHp tank")]
    [Theory]
    public void ToStringMethod(string name, string expected)
    {
        var args = new PoolListArgs { PoolName = name };

        var stringVer = args.ToString();
        Assert.Equal(expected, stringVer);
    }
}