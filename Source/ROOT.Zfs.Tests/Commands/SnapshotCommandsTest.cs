using System;
using System.Linq;
using Xunit;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Public.Arguments.Snapshots;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Commands;

public class SnapshotCommandsTest
{
    private readonly ITestOutputHelper _output;

    private readonly SnapshotCommands _commands;

    public SnapshotCommandsTest(ITestOutputHelper output)
    {
        _output = output;
        var binaries = new ZfsBinaries();
        _commands = new SnapshotCommands(binaries, new DatasetCommands(binaries));
    }

    [InlineData("tank/myds", "projectX", "/sbin/zfs snap tank/myds@projectX", false)]
    [InlineData("tank/myds", "", null, false)]
    [InlineData("tank/myds", "\\test", null, true)]
    [Theory]
    public void CreateSnapshotWithNameTest(string dataset, string snapshot, string expectedCommand, bool expectException)
    {
        var args = new SnapshotCreateArgs { Dataset = dataset, Snapshot = snapshot };

        if (expectException)
        {
            var ex = Assert.Throws<ArgumentException>(() => _commands.CreateSnapshot(args));
            _output.WriteLine(ex.ToString());
        }
        else
        {
            var command = _commands.CreateSnapshot(args);
            _output.WriteLine(command.FullCommandLine);
            if (!string.IsNullOrWhiteSpace(snapshot))
            {
                Assert.Equal(expectedCommand, command.FullCommandLine);
            }
            else
            {
                var expectedPrefix = $"/sbin/zfs snap {dataset}@{DateTime.UtcNow.ToLocalTime():yyyyMMddHHmm}";
                Assert.StartsWith(expectedPrefix, command.FullCommandLine);
            }
        }

    }


    [InlineData("tank/myds", "/sbin/zfs list -Hpr -o type,creation,name,used,refer,avail,mountpoint,origin -d 99 -t snapshot tank/myds", false)]
    [InlineData("tank/myds && rm -rf /", null, true)]
    [Theory]
    public void ListSnapshotsTest(string dataset, string expectedCommand, bool expectException)
    {
        var args = new SnapshotListArgs { Root = dataset };
        if (expectException)
        {
            var ex = Assert.Throws<ArgumentException>(() => _commands.ListSnapshots(args));
            _output.WriteLine(ex.ToString());
        }
        else
        {
            var command = _commands.ListSnapshots(args);
            Assert.Equal(expectedCommand, command.FullCommandLine);
        }
    }

    [InlineData("tank/myds", "projectX", "/sbin/zfs destroy tank/myds@projectX", false)]
    [InlineData("tank/myds", "tank/myds@projectX", "/sbin/zfs destroy tank/myds@projectX", false)]
    [InlineData("tank/myds", "tank/myds@projectX && rm -rf /", null, true)]
    [Theory]
    public void DestroyDatasetCommandTest(string dataset, string snapName, string expected, bool expectException)
    {
        var args = new SnapshotDestroyArgs { Dataset = dataset, Snapshot = snapName };
        if (expectException)
        {
            var ex = Assert.Throws<ArgumentException>(() => _commands.DestroySnapshot(args));
            _output.WriteLine(ex.ToString());
        }
        else
        {
            var command = _commands.DestroySnapshot(args);

            Assert.Equal(expected, command.FullCommandLine);
        }
    }

    [InlineData("tank/myds", "mysnap", "tank2/mysnap_clone", null, "/sbin/zfs clone -p tank/myds@mysnap tank2/mysnap_clone", false)]
    [InlineData("tank/myds", "mysnap", "tank2/mysnap_clone", "atime=off", "/sbin/zfs clone -p -o atime=off tank/myds@mysnap tank2/mysnap_clone", false)]
    [InlineData("tank/myds", "mysnap", "", null, null,true)]
    [Theory]
    public void CloneSnapshotTest(string dataset, string snapshot, string target, string properties, string expected, bool expectException)
    {
        var props = properties?.Split(',').Select(p => p.Split('=')).Select(a => new PropertyValue { Property = a[0], Value = a[1] }).ToArray();
        var args = new SnapshotCloneArgs { Dataset = dataset, Snapshot = snapshot, TargetDataset = target, Properties = props };
        if (expectException)
        {
            var ex = Assert.Throws<ArgumentException>(() => _commands.Clone(args));
            _output.WriteLine(ex.ToString());
        }
        else
        {
            var command = _commands.Clone(args);

            Assert.Equal(expected, command.FullCommandLine);
        }
    }

    [InlineData("tank/myds", "tank/myds@12345", "myhold", true, "/sbin/zfs hold -r myhold tank/myds@12345")]
    [InlineData("tank%2fmyds", "tank%2fmyds@12345", "myhold", false, "/sbin/zfs hold myhold tank/myds@12345")]
    [InlineData("tank%2fmyds", "tank%2fmyds%4012345", "myhold", false, "/sbin/zfs hold myhold tank/myds@12345")]
    [InlineData("tank%2fmyds", "tank%2fmyds%4012345", "", false, null, true)]
    [InlineData("tank%2fmyds", "tank%2fmyds%4012345", null, false, null, true)]
    [InlineData("tank/myds", "", "myhold", false, null, true)]
    [InlineData("tank/myds", null, "myhold", false, null, true)]
    [Theory]
    public void HoldTest(string dataset, string snapshot, string tag, bool recursive, string expected, bool expectException = false)
    {
        var args = new SnapshotHoldArgs { Dataset = dataset, Snapshot = snapshot, Tag = tag, Recursive = recursive };
        if (expectException)
        {
            Assert.Throws<ArgumentException>(() => _commands.Hold(args));
        }
        else
        {
            var command = _commands.Hold(args);
            _output.WriteLine(command.FullCommandLine);
            Assert.Equal(expected, command.FullCommandLine);
        }
    }

    [InlineData("tank/myds", "tank/myds@12345", true, "/sbin/zfs holds -r -H tank/myds@12345")]
    [InlineData("tank/myds", "12345", true, "/sbin/zfs holds -r -H tank/myds@12345")]
    [InlineData("tank/myds", "tank%2fmyds@12345", false, "/sbin/zfs holds -H tank/myds@12345")]
    [InlineData("tank/myds", "tank%2fmyds%4012345", false, "/sbin/zfs holds -H tank/myds@12345")]
    [InlineData("tank/myds", "", false, null, true)]
    [InlineData("tank/myds", null, false, null, true)]
    [Theory]
    public void HoldsTest(string dataset, string snapshot, bool recursive, string expected, bool expectException = false)
    {
        var args = new SnapshotHoldsArgs { Dataset = dataset, Snapshot = snapshot, Recursive = recursive };
        if (expectException)
        {
            Assert.Throws<ArgumentException>(() => _commands.Holds(args));
        }
        else
        {
            var command = _commands.Holds(args);
            _output.WriteLine(command.FullCommandLine);
            Assert.Equal(expected, command.FullCommandLine);
        }
    }
    [InlineData("tank/myds", "12345", "myhold", true, "/sbin/zfs release -r myhold tank/myds@12345")]
    [InlineData("tank%2fmyds", "12345", "myhold", false, "/sbin/zfs release myhold tank/myds@12345")]
    [InlineData("tank%2fmyds", "tank%2fmyds%4012345", "", false, null, true)]
    [InlineData("tank%2fmyds", "tank%2fmyds%4012345", null, false, null, true)]
    [InlineData("tank%2fmyds", "", "myhold", false, null, true)]
    [InlineData("tank%2fmyds", null, "myhold", false, null, true)]
    [Theory]
    public void ReleaseTest(string dataset, string snapshot, string tag, bool recursive, string expected, bool expectException = false)
    {
        var args = new SnapshotReleaseArgs { Dataset = dataset, Snapshot = snapshot, Tag = tag, Recursive = recursive };
        if (expectException)
        {
            Assert.Throws<ArgumentException>(() => _commands.Release(args));
        }
        else
        {
            var command = _commands.Release(args);
            _output.WriteLine(command.FullCommandLine);
            Assert.Equal(expected, command.FullCommandLine);
        }
    }

}