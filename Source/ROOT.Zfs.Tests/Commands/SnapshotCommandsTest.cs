using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Public.Arguments.Snapshots;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Commands;

[TestClass]
public class SnapshotCommandsTest
{
    private readonly SnapshotCommands _commands;

    public SnapshotCommandsTest()
    {
        var binaries = new ZfsBinaries();
        _commands = new SnapshotCommands(binaries, new DatasetCommands(binaries));
    }

    [DataRow("tank/myds", "projectX", "/sbin/zfs snap tank/myds@projectX", false)]
    [DataRow("tank/myds", "", null, false)]
    [DataRow("tank/myds", "\\test", null, true)]
    [TestMethod]
    public void CreateSnapshotWithNameTest(string dataset, string snapshot, string expectedCommand, bool expectException)
    {
        var args = new SnapshotCreateArgs { Dataset = dataset, Snapshot = snapshot };

        if (expectException)
        {
            var ex = Assert.ThrowsExactly<ArgumentException>(() => _commands.CreateSnapshot(args));
            Console.WriteLine(ex);
        }
        else
        {
            var command = _commands.CreateSnapshot(args);
            Console.WriteLine(command.FullCommandLine);
            if (!string.IsNullOrWhiteSpace(snapshot))
            {
                Assert.AreEqual(expectedCommand, command.FullCommandLine);
            }
            else
            {
                var expectedPrefix = $"/sbin/zfs snap {dataset}@{DateTime.UtcNow.ToLocalTime():yyyyMMddHHmm}";
                Assert.IsTrue(command.FullCommandLine.StartsWith(expectedPrefix));
            }
        }

    }


    [DataRow("tank/myds", "/sbin/zfs list -Hpr -o type,creation,name,used,refer,avail,mountpoint,origin -d 99 -t snapshot tank/myds", false)]
    [DataRow("tank/myds && rm -rf /", null, true)]
    [TestMethod]
    public void ListSnapshotsTest(string dataset, string expectedCommand, bool expectException)
    {
        var args = new SnapshotListArgs { Root = dataset };
        if (expectException)
        {
            var ex = Assert.ThrowsExactly<ArgumentException>(() => _commands.ListSnapshots(args));
            Console.WriteLine(ex);
        }
        else
        {
            var command = _commands.ListSnapshots(args);
            Assert.AreEqual(expectedCommand, command.FullCommandLine);
        }
    }

    [DataRow("tank/myds", "projectX", "/sbin/zfs destroy tank/myds@projectX", false)]
    [DataRow("tank/myds", "tank/myds@projectX", "/sbin/zfs destroy tank/myds@projectX", false)]
    [DataRow("tank/myds", "tank/myds@projectX && rm -rf /", null, true)]
    [TestMethod]
    public void DestroyDatasetCommandTest(string dataset, string snapName, string expected, bool expectException)
    {
        var args = new SnapshotDestroyArgs { Dataset = dataset, Snapshot = snapName };
        if (expectException)
        {
            var ex = Assert.ThrowsExactly<ArgumentException>(() => _commands.DestroySnapshot(args));
            Console.WriteLine(ex);
        }
        else
        {
            var command = _commands.DestroySnapshot(args);

            Assert.AreEqual(expected, command.FullCommandLine);
        }
    }

    [DataRow("tank/myds", "mysnap", "tank2/mysnap_clone", null, "/sbin/zfs clone -p tank/myds@mysnap tank2/mysnap_clone", false)]
    [DataRow("tank/myds", "mysnap", "tank2/mysnap_clone", "atime=off", "/sbin/zfs clone -p -o atime=off tank/myds@mysnap tank2/mysnap_clone", false)]
    [DataRow("tank/myds", "mysnap", "", null, null,true)]
    [TestMethod]
    public void CloneSnapshotTest(string dataset, string snapshot, string target, string properties, string expected, bool expectException)
    {
        var props = properties?.Split(',').Select(p => p.Split('=')).Select(a => new PropertyValue { Property = a[0], Value = a[1] }).ToArray();
        var args = new SnapshotCloneArgs { Dataset = dataset, Snapshot = snapshot, TargetDataset = target, Properties = props };
        if (expectException)
        {
            var ex = Assert.ThrowsExactly<ArgumentException>(() => _commands.Clone(args));
            Console.WriteLine(ex);
        }
        else
        {
            var command = _commands.Clone(args);

            Assert.AreEqual(expected, command.FullCommandLine);
        }
    }

    [DataRow("tank/myds", "tank/myds@12345", "myhold", true, "/sbin/zfs hold -r myhold tank/myds@12345")]
    [DataRow("tank%2fmyds", "tank%2fmyds@12345", "myhold", false, "/sbin/zfs hold myhold tank/myds@12345")]
    [DataRow("tank%2fmyds", "tank%2fmyds%4012345", "myhold", false, "/sbin/zfs hold myhold tank/myds@12345")]
    [DataRow("tank%2fmyds", "tank%2fmyds%4012345", "", false, null, true)]
    [DataRow("tank%2fmyds", "tank%2fmyds%4012345", null, false, null, true)]
    [DataRow("tank/myds", "", "myhold", false, null, true)]
    [DataRow("tank/myds", null, "myhold", false, null, true)]
    [TestMethod]
    public void HoldTest(string dataset, string snapshot, string tag, bool recursive, string expected, bool expectException = false)
    {
        var args = new SnapshotHoldArgs { Dataset = dataset, Snapshot = snapshot, Tag = tag, Recursive = recursive };
        if (expectException)
        {
            Assert.ThrowsExactly<ArgumentException>(() => _commands.Hold(args));
        }
        else
        {
            var command = _commands.Hold(args);
            Console.WriteLine(command.FullCommandLine);
            Assert.AreEqual(expected, command.FullCommandLine);
        }
    }

    [DataRow("tank/myds", "tank/myds@12345", true, "/sbin/zfs holds -r -H tank/myds@12345")]
    [DataRow("tank/myds", "12345", true, "/sbin/zfs holds -r -H tank/myds@12345")]
    [DataRow("tank/myds", "tank%2fmyds@12345", false, "/sbin/zfs holds -H tank/myds@12345")]
    [DataRow("tank/myds", "tank%2fmyds%4012345", false, "/sbin/zfs holds -H tank/myds@12345")]
    [DataRow("tank/myds", "", false, null, true)]
    [DataRow("tank/myds", null, false, null, true)]
    [TestMethod]
    public void HoldsTest(string dataset, string snapshot, bool recursive, string expected, bool expectException = false)
    {
        var args = new SnapshotHoldsArgs { Dataset = dataset, Snapshot = snapshot, Recursive = recursive };
        if (expectException)
        {
            Assert.ThrowsExactly<ArgumentException>(() => _commands.Holds(args));
        }
        else
        {
            var command = _commands.Holds(args);
            Console.WriteLine(command.FullCommandLine);
            Assert.AreEqual(expected, command.FullCommandLine);
        }
    }
    [DataRow("tank/myds", "12345", "myhold", true, "/sbin/zfs release -r myhold tank/myds@12345")]
    [DataRow("tank%2fmyds", "12345", "myhold", false, "/sbin/zfs release myhold tank/myds@12345")]
    [DataRow("tank%2fmyds", "tank%2fmyds%4012345", "", false, null, true)]
    [DataRow("tank%2fmyds", "tank%2fmyds%4012345", null, false, null, true)]
    [DataRow("tank%2fmyds", "", "myhold", false, null, true)]
    [DataRow("tank%2fmyds", null, "myhold", false, null, true)]
    [TestMethod]
    public void ReleaseTest(string dataset, string snapshot, string tag, bool recursive, string expected, bool expectException = false)
    {
        var args = new SnapshotReleaseArgs { Dataset = dataset, Snapshot = snapshot, Tag = tag, Recursive = recursive };
        if (expectException)
        {
            Assert.ThrowsExactly<ArgumentException>(() => _commands.Release(args));
        }
        else
        {
            var command = _commands.Release(args);
            Console.WriteLine(command.FullCommandLine);
            Assert.AreEqual(expected, command.FullCommandLine);
        }
    }

}