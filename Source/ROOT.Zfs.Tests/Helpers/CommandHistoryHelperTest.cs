using System;
using System.Linq;
using Xunit;
using ROOT.Zfs.Core.Helpers;

namespace ROOT.Zfs.Tests.Helpers;

public class CommandHistoryHelperTest
{
    private const string ZPoolHistory = @"History for 'tank':
2022-09-20.21:30:14 zpool create tank mirror /dev/disk/by-id/ata-QEMU_HARDDISK_QM00015 /dev/disk/by-id/ata-QEMU_HARDDISK_QM00017 [user 0 (root) on zfsdev.root.dom:linux]
2022-09-20.21:30:45 zpool import -c /etc/zfs/zpool.cache -aN [user 0 (root) on zfsdev.root.dom:linux]
2022-09-21.17:17:39 zfs create tank/test [user 0 (root) on zfsdev.root.dom:linux]
2022-09-21.17:25:16 zfs create tank/c0818844-0f03-4fd9-b2ea-b24926ef7191 [user 0 (root) on zfsdev.root.dom:linux]
2022-09-21.17:37:03 zfs destroy tank/c0818844-0f03-4fd9-b2ea-b24926ef7191 [user 0 (root) on zfsdev.root.dom:linux]";


    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void SkipCountTest(int skip)
    {
        var history = CommandHistoryHelper.FromStdOut(ZPoolHistory, skip).ToList();
        Assert.Equal(5 - skip, history.Count);
        if (skip < 5)
        {
            var lastEntry = history[^1];
            var lastDate = new DateTime(2022, 09, 21, 17, 37, 3, DateTimeKind.Local);
            Assert.Equal("zfs destroy tank/c0818844-0f03-4fd9-b2ea-b24926ef7191", lastEntry.Command);
            Assert.Equal("user 0 (root) on zfsdev.root.dom:linux", lastEntry.Caller);
            Assert.Equal(lastDate, lastEntry.Time);
        }
        else
        {
            Assert.Empty(history);
        }
    }

    [Fact]
    public void ParsingTest()
    {
        var history = CommandHistoryHelper.FromStdOut(ZPoolHistory).ToList();
        Assert.Equal(5, history.Count);
        var expectedDate = new DateTime(2022, 09, 20, 21, 30, 14, DateTimeKind.Local);
        Assert.Equal("zpool create tank mirror /dev/disk/by-id/ata-QEMU_HARDDISK_QM00015 /dev/disk/by-id/ata-QEMU_HARDDISK_QM00017", history[0].Command);
        Assert.Equal("user 0 (root) on zfsdev.root.dom:linux", history[0].Caller);
        Assert.Equal(expectedDate, history[0].Time);

        var lastEntry = history[^1];
        var lastDate = new DateTime(2022, 09, 21, 17, 37, 3, DateTimeKind.Local);
        Assert.Equal("zfs destroy tank/c0818844-0f03-4fd9-b2ea-b24926ef7191", lastEntry.Command);
        Assert.Equal("user 0 (root) on zfsdev.root.dom:linux", lastEntry.Caller);
        Assert.Equal(lastDate, lastEntry.Time);
    }
        
    [Fact]
    public void GreaterThanTest()
    {
        var cutOff = new DateTime(2022, 09, 21, 17, 25, 16);
        var history = CommandHistoryHelper.FromStdOut(ZPoolHistory, 0, cutOff).ToList();
        Assert.Single(history);
        var lastEntry = history[^1];
        var lastDate = new DateTime(2022, 09, 21, 17, 37, 3, DateTimeKind.Local);
        Assert.Equal("zfs destroy tank/c0818844-0f03-4fd9-b2ea-b24926ef7191", lastEntry.Command);
        Assert.Equal("user 0 (root) on zfsdev.root.dom:linux", lastEntry.Caller);
        Assert.Equal(lastDate, lastEntry.Time);
    }
}