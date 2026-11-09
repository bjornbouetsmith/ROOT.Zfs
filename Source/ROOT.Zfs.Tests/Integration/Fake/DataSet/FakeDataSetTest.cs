using System;
using System.Linq;
using Xunit;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Arguments.Dataset;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Integration.Fake.DataSet;

public abstract class FakeDataSetTest
{
    private readonly ITestOutputHelper _output;

    protected FakeDataSetTest(ITestOutputHelper output)
    {
        _output = output;
    }

    internal abstract FakeRemoteConnection CreateProcessCall();

    internal IDatasets GetDatasets(FakeRemoteConnection remoteConnection = null)
    {
        return ZfsTestFactory.CreateDatasets(remoteConnection ?? CreateProcessCall());
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void ListTest()
    {
        var ds = GetDatasets();
        var dataSets = ds.List(new DatasetListArgs());
        Assert.NotNull(dataSets);
        foreach (var set in dataSets)
        {
            _output.WriteLine(set.Dump(new JsonFormatter()));
        }
    }



    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void GetDataSetShouldReturnDataSet()
    {
        var ds = GetDatasets();
        var args = new DatasetListArgs { Root = "tank" };
        var root = ds.List(args).FirstOrDefault();

        Assert.NotNull(root);
        Assert.Equal("tank", root.DatasetName);
        Assert.Equal(DatasetTypes.Filesystem, root.Type);
        _output.WriteLine(root.Dump(new JsonFormatter()));
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void GetNonExistingDataSetShouldThrowException()
    {
        var ds = GetDatasets();
        Assert.Throws<ProcessCallException>(() => ds.List(new DatasetListArgs { Root = "ungabunga" }).FirstOrDefault());
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void CreateDateSetTest()
    {
        var ds = GetDatasets();
        var args = new DatasetCreationArgs
        {
            DatasetName = "tank/myds",
            Type = DatasetTypes.Filesystem,
            Properties = [new PropertyValue { Property = "atime", Value = "off" }]

        };
        var dataset = ds.Create(args);
        Assert.NotNull(dataset);
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void CreateDateSetInvalidTest()
    {
        var ds = GetDatasets();
        var args = new DatasetCreationArgs
        {
            DatasetName = "tank/myds",
            Type = DatasetTypes.Bookmark,
            Properties = [new PropertyValue { Property = "atime", Value = "off" }]

        };
        Assert.Throws<ArgumentException>(() => ds.Create(args));
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void DestroyDataSetTest()
    {
        var ds = GetDatasets();
        var destroyArgs = new DatasetDestroyArgs { Dataset = "tank/myds", DestroyFlags = DatasetDestroyFlags.Recursive };
        var response = ds.Destroy(destroyArgs);
        Assert.Equal(DatasetDestroyFlags.Recursive, response.Flags);
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void PromoteDatasetTest()
    {
        var processCall = CreateProcessCall();
        var ds = GetDatasets(processCall);
        ds.Promote(new PromoteArgs { Name = "tank/myds" });
        var commands = processCall.GetCommandsInvoked();
        var command = Assert.Single(commands);
        Assert.Equal("/sbin/zfs promote tank/myds", command);
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void MountDatasetTest()
    {
        var processCall = CreateProcessCall();
        var ds = GetDatasets(processCall);
        ds.Mount(new MountArgs { Filesystem = "tank/myds" });
        var commands = processCall.GetCommandsInvoked();
        var command = Assert.Single(commands);
        Assert.Equal("/sbin/zfs mount tank/myds", command);
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void UnmountDatasetTest()
    {
        var processCall = CreateProcessCall();
        var ds = GetDatasets(processCall);
        ds.Unmount(new UnmountArgs { Filesystem = "tank/myds" });
        var commands = processCall.GetCommandsInvoked();
        var command = Assert.Single(commands);
        Assert.Equal("/sbin/zfs unmount tank/myds", command);
    }
}