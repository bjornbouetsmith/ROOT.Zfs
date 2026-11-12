using System;
using System.Threading.Tasks;
using Xunit;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;
using ROOT.Zfs.Public.Arguments;
using ROOT.Zfs.Public.Arguments.Dataset;
using ROOT.Zfs.Public.Arguments.Properties;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Integration;

public class PropertiesTest
{
    private readonly ITestOutputHelper _output;

    public PropertiesTest(ITestOutputHelper output)
    {
        _output = output;
    }

    private readonly IProcessCall _remoteProcessCall = TestHelpers.RequiresRemoteConnection ? new SSHProcessCall("bbs", "zfsdev.root.dom", true) : null;

    private Properties GetProperties()
    {
        var props =  ZfsTestFactory.CreateProperties(_remoteProcessCall);
        props.RequiresSudo = TestHelpers.RequiresSudo;
        return props;
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task ListAllPropertiesAsyncTest()
    {
        using var pool = TestPool.CreateSimplePool(_remoteProcessCall, _output);
        var pr = GetProperties();
        var args = new GetPropertyArgs { PropertyTarget = PropertyTarget.Dataset, Target = pool.Name };
        var props = await pr.GetAsync(args, TestContext.Current.CancellationToken);
        Assert.NotNull(props);
        foreach (var prop in props)
        {
            _output.WriteLine(prop.Dump(new JsonFormatter()));
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task SetDataSetPropertyAsyncTest()
    {
        using var pool = TestPool.CreateSimplePool(_remoteProcessCall, _output);
        var pr = GetProperties();
        var args = new SetPropertyArgs { PropertyTarget = PropertyTarget.Dataset, Target = pool.Name, Property = "atime", Value = "off" };
        var newVal = await pr.SetAsync(args, TestContext.Current.CancellationToken);
            
        Assert.Equal("off", newVal.Value);
        _output.WriteLine(newVal.Dump(new JsonFormatter()));
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetDataSetPropertyAsyncTest()
    {
        using var pool = TestPool.CreateSimplePool(_remoteProcessCall, _output);
        var pr = GetProperties();
        var getArgs = new GetPropertyArgs { PropertyTarget = PropertyTarget.Dataset, Target = pool.Name, Property = "atime"};
        var val = (await pr.GetAsync(getArgs, TestContext.Current.CancellationToken))[0];
        var newPropVal = val.Value == "off" ? "on" : "off";

        var setArgs = new SetPropertyArgs { PropertyTarget = PropertyTarget.Dataset, Target = pool.Name, Property = "atime", Value = newPropVal };
        var newVal = await pr.SetAsync(setArgs, TestContext.Current.CancellationToken);
        Assert.Equal(newPropVal, newVal.Value);

        _output.WriteLine(newVal.Dump(new JsonFormatter()));
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAvailableDatasetPropertiesAsyncTest()
    {
        var pr = GetProperties();
        var props = await pr.GetAvailableAsync(PropertyTarget.Dataset, TestContext.Current.CancellationToken);
        Assert.NotEmpty(props);
        _output.WriteLine(props.Dump(new JsonFormatter()));
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAvailablePoolPropertiesAsyncTest()
    {
        var pr = GetProperties();
        var props = await pr.GetAvailableAsync(PropertyTarget.Pool, TestContext.Current.CancellationToken);
        Assert.NotEmpty(props);
        _output.WriteLine(props.Dump(new JsonFormatter()));
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task ResetPropertyTestAsync()
    {
        using var pool = TestPool.CreateSimplePool(_remoteProcessCall, _output);
        var pr = GetProperties();
        var dsHelper = ZfsTestFactory.CreateDatasets(_remoteProcessCall);
        dsHelper.RequiresSudo = pr.RequiresSudo;

        var dataset = $"{pool.Name}/{Guid.NewGuid()}";
        var args = new DatasetCreationArgs
        {
            DatasetName = dataset,
            Type = DatasetTypes.Filesystem
        };
        await dsHelper.CreateAsync(args, TestContext.Current.CancellationToken);
        try
        {

            var target = dataset;
            var property = "atime";

            var getArgs = new GetPropertyArgs { PropertyTarget = PropertyTarget.Dataset, Target = target, Property = property };
            var before = (await pr.GetAsync(getArgs, TestContext.Current.CancellationToken))[0];
            Assert.Equal("on", before.Value);
            var setArg = new SetPropertyArgs { PropertyTarget = PropertyTarget.Dataset, Target = target, Property = property, Value = "off" };
            var prop = await pr.SetAsync(setArg, TestContext.Current.CancellationToken);
            Assert.Equal("off", prop.Value);
            var inheritArgs = new InheritPropertyArgs { PropertyTarget = PropertyTarget.Dataset, Target = target, Property = property };
            await pr.ResetAsync(inheritArgs, TestContext.Current.CancellationToken);
            getArgs = new GetPropertyArgs { PropertyTarget = PropertyTarget.Dataset, Target = target, Property = property };
            var reset = (await pr.GetAsync(getArgs, TestContext.Current.CancellationToken))[0];
            Assert.Equal("on", reset.Value);

        }
        finally
        {
            var destroyArgs = new DatasetDestroyArgs { Dataset = dataset };
            await dsHelper.DestroyAsync(destroyArgs, TestContext.Current.CancellationToken);
        }
    }
}