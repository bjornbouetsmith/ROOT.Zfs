using System;
using Xunit;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Public.Arguments.Dataset;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Commands;

public class DataSetCommandsTest
{
    private readonly ITestOutputHelper _output;

    public DataSetCommandsTest(ITestOutputHelper output)
    {
        _output = output;
    }

    private readonly DatasetCommands _commands = new(new ZfsBinaries());

    [InlineData("tank/myds", DatasetDestroyFlags.None, "/sbin/zfs destroy tank/myds", false)]
    [InlineData("tank/myds", DatasetDestroyFlags.Recursive, "/sbin/zfs destroy -r tank/myds", false)]
    [InlineData("tank/myds", DatasetDestroyFlags.RecursiveClones, "/sbin/zfs destroy -R tank/myds", false)]
    [InlineData("tank/myds", DatasetDestroyFlags.ForceUmount, "/sbin/zfs destroy -f tank/myds", false)]
    [InlineData("tank/myds", DatasetDestroyFlags.DryRun, "/sbin/zfs destroy -nvp tank/myds", false)]
    [InlineData("tank/myds", (DatasetDestroyFlags)(int)-1, "/sbin/zfs destroy -r -R -f -nvp tank/myds", false)]
    [InlineData("tank/myds@test", (DatasetDestroyFlags)(int)-1, "/sbin/zfs destroy -r -R -f -nvp tank/myds", true)]
    [Theory]
    public void DestroyFlagsShouldBeCorrectlyReflected(string dataset, DatasetDestroyFlags flags, string expectedCommand, bool throwException)
    {
        var args = new DatasetDestroyArgs { Dataset = dataset, DestroyFlags = flags };
        if (throwException)
        {
            var ex = Assert.Throws<ArgumentException>(() => _commands.Destroy(args));
            _output.WriteLine(ex.Message);
        }
        else
        {
            var command = _commands.Destroy(args);
            _output.WriteLine(command.FullCommandLine);
            Assert.Equal(expectedCommand, command.FullCommandLine);
        }
    }

    [InlineData("tank/myds/clone", "/sbin/zfs promote tank/myds/clone", false)]
    [InlineData("tank/myds/clone && rm -rf /", "/sbin/zfs promote tank/myds/clone", true)]
    [Theory]
    public void PromoteCommandTest(string dataset, string expectedCommand, bool expectException)
    {
        var args = new PromoteArgs { Name = dataset };

        if (expectException)
        {
            var ex = Assert.Throws<ArgumentException>(() => _commands.Promote(args));
            _output.WriteLine(ex.ToString());
        }
        else
        {

            var command = _commands.Promote(args);
            _output.WriteLine(command.FullCommandLine);
            Assert.Equal(expectedCommand, command.FullCommandLine);
        }
    }

    [Theory]
    [InlineData("tank/myds", false, false, "/sbin/zfs mount tank/myds")]
    [InlineData("tank/myds", true, true, "")]
    [InlineData("", true, false, "/sbin/zfs mount -a")]
    [InlineData(null, true, false, "/sbin/zfs mount -a")]
    public void DataSetMountTest(string dataset, bool all, bool throwException, string expectedCommand)
    {
        var args = new MountArgs { Filesystem = dataset, MountAllFileSystems = all };
        if (throwException)
        {
            Assert.Throws<ArgumentException>(() => _commands.Mount(args));
        }
        else
        {
            var command = _commands.Mount(args);
            Assert.Equal(expectedCommand, command.FullCommandLine);
        }
    }

    [Theory]
    [InlineData("tank/myds", false, false, "/sbin/zfs unmount tank/myds")]
    [InlineData("tank/myds", true, true, "")]
    [InlineData("", true, false, "/sbin/zfs unmount -a")]
    [InlineData(null, true, false, "/sbin/zfs unmount -a")]
    public void DataSetUnmountTest(string dataset, bool all, bool throwException, string expectedCommand)
    {
        var args = new UnmountArgs { Filesystem = dataset, UnmountAllFileSystems = all };
        if (throwException)
        {
            Assert.Throws<ArgumentException>(() => _commands.Unmount(args));
        }
        else
        {
            var command = _commands.Unmount(args);
            Assert.Equal(expectedCommand, command.FullCommandLine);
        }
    }

    [InlineData("tank/myds", DatasetTypes.Filesystem, "/sbin/zfs create tank/myds", false)]
    [InlineData("tank/myds", DatasetTypes.Bookmark, "/sbin/zfs create tank/myds", true)]
    [InlineData(null, DatasetTypes.Filesystem, "/sbin/zfs create tank/myds", true)]
    [Theory]
    public void CreateTest(string dataset, DatasetTypes type, string expectedCommand, bool expectException)
    {
        var args = new DatasetCreationArgs { DatasetName = dataset, Type = type };

        if (expectException)
        {
            var ex = Assert.Throws<ArgumentException>(() => _commands.Create(args));
            _output.WriteLine(ex.ToString());
        }
        else
        {
            var command = _commands.Create(args);
            Assert.Equal(expectedCommand, command.FullCommandLine);
        }
    }

}