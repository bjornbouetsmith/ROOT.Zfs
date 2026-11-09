using Xunit;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Tests.Helpers;

public class ZPoolInfoParserTest
{
    [Fact]
    public void ParseLineTest()
    {
        var line = "tank\t3813930958848\t128800165888\t3685130792960\t-\t-\t9\t3\t1.00\tONLINE\t-";
        var info = ZPoolInfoParser.ParseLine(line);

        Assert.Equal("tank", info.PoolName);
        Assert.Equal("3.5T", info.Size.ToString());
        Assert.Equal("120G", info.Allocated.ToString());
        Assert.Equal("3.4T", info.Free.ToString());
        Assert.Equal("9.00%", info.Fragmentation.ToString());
        Assert.Equal("3.00%", info.CapacityUsed.ToString());
        Assert.Equal("1.00x", info.DedupRatio.ToString());
        Assert.Equal(State.Online, info.State);
        Assert.Equal("-", info.AltRoot);
    }

    [Fact]
    public void ParseFromStdOutTest()
    {
        var stdOut = @"rpool   31138512896     3078807552      28059705344     -       -       10      9       1.00    ONLINE  -
tank    3813930958848   128800165888    3685130792960   -       -       9       3       1.00    ONLINE  -";

        var list = ZPoolInfoParser.ParseFromStdOut(stdOut);

        Assert.Equal(2, list.Count);
        Assert.Equal("rpool", list[0].PoolName);
        Assert.Equal("tank", list[1].PoolName);
    }
}