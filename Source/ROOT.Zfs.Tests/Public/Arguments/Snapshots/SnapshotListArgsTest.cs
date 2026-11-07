using Xunit;
using ROOT.Zfs.Public.Arguments.Snapshots;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Public.Arguments.Snapshots;

public class SnapshotListArgsTest
{
    [Fact]
    public void DefaultValuesTest()
    {
        var args = new SnapshotListArgs { Root = "tank/myds" };

        Assert.Equal(DatasetTypes.Snapshot, args.DatasetTypes);
        Assert.True(args.IncludeChildren);
        Assert.Equal("tank/myds", args.Root);
    }
}