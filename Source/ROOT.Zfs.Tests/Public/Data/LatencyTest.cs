using Xunit;
using ROOT.Zfs.Public.Data.Statistics;

namespace ROOT.Zfs.Tests.Public.Data;

public class LatencyTest
{
    [Fact]
    public void ValidLatencyValuesShouldParse()
    {
        var latency = new Latency("1000", "1100");

        Assert.Equal(10L, latency.Read.Ticks);
        Assert.Equal(11L, latency.Write.Ticks);
    }

    [Fact]
    public void InvalidLatencyValuesShouldNotThrowResultInZero()
    {
        var latency = new Latency("50us", "60us");

        Assert.Equal(0L, latency.Read.Ticks);
        Assert.Equal(0L, latency.Write.Ticks);
    }

}