using System;
using Xunit;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Public.Arguments;
using ROOT.Zfs.Public.Arguments.Properties;

namespace ROOT.Zfs.Tests.Commands;

public class PropertyCommandsTest
{
    private readonly PropertyCommands _commands = new(new ZfsBinaries());

    [Fact]
    public void GetAvailablePoolPropertiesTest()
    {
        var command = _commands.Get(new GetPropertyArgs { PropertyTarget = PropertyTarget.Pool });
        Assert.Equal("/sbin/zpool get -H", command.FullCommandLine);
    }

    [Fact]
    public void GetAvailableDatasetPropertiesTest()
    {
        var command = _commands.Get(new GetPropertyArgs { PropertyTarget = PropertyTarget.Dataset });
        Assert.Equal("/sbin/zfs get -H", command.FullCommandLine);
    }

    [Theory]
    [InlineData(PropertyTarget.Pool, "ashift", "tank", "/sbin/zpool get ashift tank -H")]
    [InlineData(PropertyTarget.Dataset, "atime", "tank/myds", "/sbin/zfs get atime tank/myds -H")]
    [InlineData(PropertyTarget.Dataset, "", "tank/myds", "/sbin/zfs get all tank/myds -H")]
    [InlineData(PropertyTarget.Dataset, "atime", "", "/sbin/zfs get atime -H")]
    [InlineData(PropertyTarget.Dataset, "", "", "/sbin/zfs get -H")] // These will not work runtime, since parsing will fail
    [InlineData(PropertyTarget.Pool, "", "", "/sbin/zpool get -H")] // These will not work runime, since parsing will fail
    [InlineData(PropertyTarget.Pool, "", "tank/myds", null, true)]
    [InlineData(PropertyTarget.Pool, "atime && rm -rf /", "", null, true)]
    public void GetPropertiesTest(PropertyTarget targetType, string property, string target, string expectedCommand, bool expectException = false)
    {
        var args = new GetPropertyArgs { PropertyTarget = targetType, Target = target, Property = property };
        if (expectException)
        {
            var ex = Assert.Throws<ArgumentException>(() => _commands.Get(args));
            Console.WriteLine(ex);
        }
        else
        {
            var command = _commands.Get(args);
            Assert.Equal(expectedCommand, command.FullCommandLine);
        }
    }

    [InlineData(PropertyTarget.Dataset, "atime", "tank/myds", "/sbin/zfs inherit -rS atime tank/myds", false)]
    [InlineData(PropertyTarget.Pool, "atime", "tank/myds", "/sbin/zfs inherit -rS atime tank/myds", true)]
    [Theory]
    public void ResetPropertyToInheritedTest(PropertyTarget targetType, string property, string target, string expectedCommand, bool expectException)
    {
        var args = new InheritPropertyArgs { PropertyTarget = targetType, Property = property, Target = target };
        if (expectException)
        {
            Assert.Throws<ArgumentException>(() => _commands.Inherit(args));
        }
        else
        {
            var command = _commands.Inherit(args);
            Assert.Equal(expectedCommand, command.FullCommandLine);
        }
    }

    [Theory]
    [InlineData(PropertyTarget.Pool, "tank", "compression", "off", "/sbin/zpool set compression=off tank", false)]
    [InlineData(PropertyTarget.Dataset, "tank/myds", "atime", "off", "/sbin/zfs set atime=off tank/myds", false)]
    [InlineData(PropertyTarget.Dataset, "", "atime", "off", "/sbin/zfs set atime=off tank/myds", true)]
    public void SetPropertyTest(PropertyTarget targetType, string target, string property, string value, string expectedCommand, bool expectException)
    {
        var args = new SetPropertyArgs
        {
            PropertyTarget = targetType,
            Property = property,
            Target = target,
            Value = value
        };
        if (expectException)
        {
            Assert.Throws<ArgumentException>(() => _commands.Set(args));
        }
        else
        {

            var command = _commands.Set(args);
            Console.WriteLine(command.FullCommandLine);
            Assert.Equal(expectedCommand, command.FullCommandLine);
        }
    }
}