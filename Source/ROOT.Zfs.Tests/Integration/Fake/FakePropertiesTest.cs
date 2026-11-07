using System;
using System.Linq;
using Xunit;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;
using ROOT.Zfs.Public.Arguments;
using ROOT.Zfs.Public.Arguments.Properties;

namespace ROOT.Zfs.Tests.Integration.Fake;

public class FakePropertiesTest
{
    private readonly ITestOutputHelper _output;

    public FakePropertiesTest(ITestOutputHelper output)
    {
        _output = output;
    }

    private readonly FakeRemoteConnection _remoteProcessCall = new("2.1.5-2");

    private Properties GetProperties()
    {
        return ZfsTestFactory.CreateProperties(_remoteProcessCall);
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void ListAllProperties()
    {
        var pr = GetProperties();
        var args = new GetPropertyArgs { PropertyTarget = PropertyTarget.Dataset, Target = "tank/myds" };
        var props = pr.Get(args);
        Assert.NotNull(props);
        foreach (var prop in props)
        {
            _output.WriteLine(prop.Dump(new JsonFormatter()));
        }
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void SetDataSetProperty()
    {
        var pr = GetProperties();
        var args = new SetPropertyArgs { PropertyTarget = PropertyTarget.Dataset, Target = "tank/myds", Property = "atime", Value = "off" };
        pr.Set(args);

        var commands = _remoteProcessCall.GetCommandsInvoked();
        Assert.Equal(2, commands.Count()); // Set+Get

        Assert.Equal("/sbin/zfs set atime=off tank/myds", commands[0]);
        Assert.Equal("/sbin/zfs get atime tank/myds -H", commands[1]);
    }

    /// <summary>
    /// We cannot test that setting works with the current fake implementation.
    /// </summary>
    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void GetDataSetProperty()
    {
        var pr = GetProperties();
        var args = new GetPropertyArgs { PropertyTarget = PropertyTarget.Dataset, Target = "tank/myds", Property = "atime"};
        var val = pr.Get(args).First();
            
        Assert.Equal("off", val.Value);
        var commands = _remoteProcessCall.GetCommandsInvoked();
        Assert.Single(commands); // Get
            
        Assert.Equal("/sbin/zfs get atime tank/myds -H", commands[0]);
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void GetAvailableDatasetProperties()
    {
        var pr = GetProperties();
        var props = pr.GetAvailable(PropertyTarget.Dataset).ToList();
        Assert.NotEmpty(props);
        _output.WriteLine(props.Dump(new JsonFormatter()));
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void GetAvailablePoolProperties()
    {
        var pr = GetProperties();
        var props = pr.GetAvailable(PropertyTarget.Pool).ToList();
        Assert.NotEmpty(props);
        _output.WriteLine(props.Dump(new JsonFormatter()));
    }

    /// <summary>
    /// We cannot test that reset works with the current fake implementation.
    /// </summary>
    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void ResetPropertyTest()
    {
        var pr = GetProperties();

        var target = "tank/myds";
        var property = "atime";

        var args = new GetPropertyArgs { PropertyTarget = PropertyTarget.Dataset, Target = target, Property = property };
        var before = pr.Get(args).First();
        Assert.Equal("off", before.Value);
        var setArg = new SetPropertyArgs { PropertyTarget = PropertyTarget.Dataset, Target = target, Property = property, Value = "on" };
        pr.Set(setArg);
        var inheritArgs = new InheritPropertyArgs { PropertyTarget = PropertyTarget.Dataset, Target = target, Property = property };
        pr.Reset(inheritArgs);

        var commands = _remoteProcessCall.GetCommandsInvoked();

        _output.WriteLine(commands.Dump(new JsonFormatter()));
        // Get, set & get, inherit
        Assert.Equal(4, commands.Count());

    }
}