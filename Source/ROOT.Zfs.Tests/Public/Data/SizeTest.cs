using System;
using Xunit;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Public.Data;

public class SizeTest
{
    [Theory]
    [InlineData(1024UL)] //1K
    [InlineData(1024 * 1024UL)] //1M
    [InlineData(1024 * 1024 * 1024UL)] //1G
    [InlineData(1024 * 1024 * 1024 * 1024UL)] //1T
    public void RoundTripTestSourceBytes(ulong bytes)
    {
        var size = new Size(bytes);
        var sizeAsString = size.ToString();
        Size converted = sizeAsString;
        Assert.Equal(size.Bytes, converted.Bytes);
    }

    [Theory]
    [InlineData("512", 0UL, true)] //512B
    [InlineData("ZeroB", 0UL, true)] //512B
    [InlineData("0", 0UL, false)] //512B
    [InlineData("512B", 512UL)] //512B
    [InlineData("1K", 1024UL)] //1K
    [InlineData("1M", 1024 * 1024UL)] //1M
    [InlineData("1G", 1024 * 1024 * 1024UL)] //1G
    [InlineData("1T", 1024 * 1024 * 1024 * 1024UL)] //1T
    public void RoundTripTestSourceString(string sizeString, ulong expectedBytes, bool expectException = false)
    {
        if (expectException)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                Size size = sizeString;
            });
        }
        else
        {

            Size size = sizeString;
            var sizeToString = size.ToString();
            Assert.Equal(sizeString, sizeToString);
            Assert.Equal(expectedBytes, size.Bytes);
        }
    }

    [Theory]
    [InlineData("1.1K", 1126UL)]
    [InlineData("1.1M", 1.1d * 1024 * 1024UL)]
    [InlineData("1.1G", 1.1d * 1024 * 1024 * 1024UL)] //1G
    [InlineData("1.1T", 1.1d * 1024 * 1024 * 1024 * 1024UL)] //1T
    public void FloatingPointStrings(string sizeString, double expectedBytes)
    {
        Size size = sizeString;
        var sizeToString = size.ToString();
        Assert.Equal(sizeString, sizeToString);
        Assert.Equal((ulong)expectedBytes, size.Bytes);
    }

    [Theory]
    [InlineData("0", 0UL, "0")]
    [InlineData("123", 123UL, "123B")]
    [InlineData("1024", 1024UL, "1K")]
    [InlineData("1048576", 1048576UL, "1M")]
    [InlineData("1073741824", 1073741824UL, "1G")]
    [InlineData("1099511627776", 1099511627776UL, "1T")]
    [InlineData("1209462790553", 1209462790553UL, "1.1T")]
    [InlineData("12094627905530", 12094627905530UL, "11T")]
    [InlineData("13304090696083", 13304090696083UL, "12.1T")]
    public void ValidStringShouldBeParsedCorrectly(string value, ulong expectedValue, string expectedString)
    {
        var size = new Size(value);
        Assert.Equal(expectedValue, size.Bytes);
        Assert.Equal(expectedString, size.ToString());
    }

    [Fact]
    public void BogusValueShouldResultInZero()
    {
        var size = new Size("seven");
        Assert.Equal(0UL, size.Bytes);
        Assert.Equal("0", size.ToString());
    }
}