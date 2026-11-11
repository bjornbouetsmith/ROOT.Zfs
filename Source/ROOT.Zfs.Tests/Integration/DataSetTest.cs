using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public.Arguments.Dataset;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.Datasets;

namespace ROOT.Zfs.Tests.Integration;

public class DataSetTest
{
    private readonly ITestOutputHelper _output;

    public DataSetTest(ITestOutputHelper output)
    {
        _output = output;
    }

    private readonly IProcessCall _remoteProcessCall = TestHelpers.RequiresRemoteConnection ? new SSHProcessCall("bbs", "zfsdev.root.dom", true) : null;

    private Datasets GetDataSets()
    {
        var ds = ZfsTestFactory.CreateDatasets(_remoteProcessCall);
        ds.RequiresSudo = TestHelpers.RequiresSudo;
        return ds;
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetDataSetList()
    {
        using var pool = TestPool.CreateSimplePool(_remoteProcessCall, _output);

        var ds = GetDataSets();
        var dataSets = await ds.ListAsync(new DatasetListArgs(), TestContext.Current.CancellationToken);
        Assert.NotNull(dataSets);
        foreach (var set in dataSets)
        {
            _output.WriteLine(set.Dump(new JsonFormatter()));
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task ListDataSetTest()
    {
        using var pool = TestPool.CreateSimplePool(_remoteProcessCall, _output);
        var ds = GetDataSets();
        var args = new DatasetListArgs { Root = pool.Name };
        var dataSets = await ds.ListAsync(args, TestContext.Current.CancellationToken);
        Assert.NotEmpty(dataSets);
        var dataset = dataSets[0];
        Assert.Equal(pool.Name, dataset.DatasetName);
    }

    [Theory]
    [Trait("Category", "Integration")]
    [InlineData(true)]
    [InlineData(false)]
    public async Task CreateDataSetTest(bool addProperties)
    {
        using var pool = TestPool.CreateSimplePool(_remoteProcessCall, _output);
        var ds = GetDataSets();
        var dataSetName = Guid.NewGuid().ToString();
        string fullName = null;
        try
        {
            var props = new[]
            {
                new PropertyValue { Property = "quota", Value = "1G" },
                new PropertyValue { Property = "compression", Value = "gzip" }
            };

            var dataSets = await ds.ListAsync(new DatasetListArgs(), TestContext.Current.CancellationToken);
            Dataset parent = null;
            foreach (var dataSetItem in dataSets)
            {
                if (dataSetItem.DatasetName == pool.Name)
                {
                    parent = dataSetItem;
                    break;
                }
            }

            Assert.NotNull(parent);
            _output.WriteLine(parent.Dump(new JsonFormatter()));
            fullName = DatasetHelper.CreateDatasetName(parent.DatasetName, dataSetName);
            var args = new DatasetCreationArgs
            {
                DatasetName = fullName,
                Type = DatasetTypes.Filesystem,
                Properties = addProperties ? props : null
            };
            var dataSet = ds.Create(args);
            Assert.NotNull(dataSet);
            _output.WriteLine(dataSet.Dump(new JsonFormatter()));
        }
        finally
        {
            // Check to prevent issues in case dataset creation failed
            if (fullName != null)
            {
                var args = new DatasetDestroyArgs { Dataset = fullName };
                ds.Destroy(args);
            }
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task ListDataSetShouldReturnDataSet()
    {
        using var pool = TestPool.CreateSimplePool(_remoteProcessCall, _output);
        var ds = GetDataSets();
        var args = new DatasetListArgs { Root = pool.Name };
        var dataSets = await ds.ListAsync(args, TestContext.Current.CancellationToken);
        Assert.NotEmpty(dataSets);
        var root = dataSets[0];

        _output.WriteLine(root.Dump(new JsonFormatter()));
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetNonExistingDataSetShouldThrowException()
    {
        var ds = GetDataSets();
        await Assert.ThrowsAsync<ProcessCallException>(() => ds.ListAsync(new DatasetListArgs { Root = "ungabunga" }, TestContext.Current.CancellationToken));
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void DestroyRecursiveDryRunTest()
    {
        using var pool = TestPool.CreateSimplePool(_remoteProcessCall, _output);
        var dsHelper = GetDataSets();
        var rootId = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var root = $"{pool.Name}/myds/{rootId}";
        var child1 = $"{pool.Name}/myds/{rootId}/child1";
        var child2 = $"{pool.Name}/myds/{rootId}/child2";
        var granchild1 = $"{pool.Name}/myds/{rootId}/child1/granchild";
        var datasets = new[] { root, child1, child2, granchild1 };

        try
        {

            foreach (var ds in datasets)
            {
                var args = new DatasetCreationArgs
                {
                    DatasetName = ds,
                    Type = DatasetTypes.Filesystem,
                    CreateParents = true,

                };
                dsHelper.Create(args);
            }

            var flags = DatasetDestroyFlags.Recursive | DatasetDestroyFlags.DryRun;
            var destroyArgs = new DatasetDestroyArgs { Dataset = root, DestroyFlags = flags };
            var response = dsHelper.Destroy(destroyArgs);
            Assert.Equal(flags, response.Flags);
            var lines = response.DryRun.Split(Constants.LinebreakCharacters, StringSplitOptions.RemoveEmptyEntries).Select(l => l.Replace("\t", " ")).ToList();
            Assert.Equal(4, lines.Count);
            Assert.Contains($"destroy {pool.Name}/myds/{rootId}/child1/granchild", lines);
            Assert.Contains($"destroy {pool.Name}/myds/{rootId}/child1", lines);
            Assert.Contains($"destroy {pool.Name}/myds/{rootId}/child2", lines);
            Assert.Contains($"destroy {pool.Name}/myds/{rootId}", lines);

            _output.WriteLine(response.DryRun);
        }
        finally
        {
            try
            {
                var destroyArgs = new DatasetDestroyArgs { Dataset = root, DestroyFlags = DatasetDestroyFlags.Recursive };
                dsHelper.Destroy(destroyArgs);
            }
            catch
            {
                //We dont care if cleaning up fails
            }
        }

    }
}