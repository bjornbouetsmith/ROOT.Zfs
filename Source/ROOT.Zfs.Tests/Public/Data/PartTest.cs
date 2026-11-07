using Xunit;
using ROOT.Zfs.Public.Data.Statistics;

namespace ROOT.Zfs.Tests.Public.Data;

public class PartTest
{
    [Theory]
    [InlineData("1.23",1.23d,"1.23%")]
    [InlineData("1.1", 1.1d, "1.10%")]
    [InlineData("0.123", 0.123d, "0.12%")]
    [InlineData("0.126", 0.126d, "0.13%")]
    public void ValidStringShouldBeParsedCorrectly(string value, double expectedValue, string expectedString)
    {
        var part = new Part(value);
        Assert.Equal(expectedValue, part.Value);
        Assert.Equal(expectedString, part.ToString());
    }

    [Fact]
    public void BogusValueShouldResultInZero()
    {
        var part = new Part("seven.3");
        Assert.Equal(0d, part.Value);
        Assert.Equal("0.00%", part.ToString());
    }
}