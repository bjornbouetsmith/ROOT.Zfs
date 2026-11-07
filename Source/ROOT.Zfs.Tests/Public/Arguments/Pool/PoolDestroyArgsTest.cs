using Xunit;
using ROOT.Zfs.Public.Arguments.Pool;

namespace ROOT.Zfs.Tests.Public.Arguments.Pool;

public class PoolDestroyArgsTest
{
    [InlineData("tank", "destroy -f tank")]
    [InlineData("tank%5ftest", "destroy -f tank_test")]
    [Theory]
    public void ToStringTest(string pool, string expected)
    {
        var args = new PoolDestroyArgs { PoolName = pool };

        var stringVer = args.ToString();

        Assert.Equal(expected, stringVer);
    }
}