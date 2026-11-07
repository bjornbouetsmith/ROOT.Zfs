using Xunit;
using ROOT.Zfs.Public.Data.Statistics;

namespace ROOT.Zfs.Tests.Public.Data;

public class RatioTest
{
    [Theory]
    [InlineData("1.23",1.23d,"1.23x")]
    [InlineData("1.1", 1.1d, "1.10x")]
    [InlineData("0.123", 0.123d, "0.12x")]
    [InlineData("0.126", 0.126d, "0.13x")]
    public void ValidStringShouldBeParsedCorrectly(string value, double expectedValue, string expectedString)
    {
        var part = new Ratio(value);
        Assert.Equal(expectedValue, part.Value);
        Assert.Equal(expectedString, part.ToString());
    }

    [Fact]
    public void BogusValueShouldResultInZero()
    {
        var part = new Ratio("seven.3");
        Assert.Equal(0d, part.Value);
        Assert.Equal("0.00x", part.ToString());
    }
}