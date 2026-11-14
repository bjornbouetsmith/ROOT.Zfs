using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public.Arguments.Dataset;
using ROOT.Zfs.Public.Arguments.Snapshots;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.Datasets;
using System.Globalization;

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
            Dataset parent = dataSets.FirstOrDefault(d => d.DatasetName == pool.Name);

            Assert.NotNull(parent);
            _output.WriteLine(parent.Dump(new JsonFormatter()));
            fullName = DatasetHelper.CreateDatasetName(parent.DatasetName, dataSetName);
            var args = new DatasetCreationArgs
            {
                DatasetName = fullName,
                Type = DatasetTypes.Filesystem,
                Properties = addProperties ? props : null
            };
            var dataSet = await ds.CreateAsync(args, TestContext.Current.CancellationToken);
            Assert.NotNull(dataSet);
            _output.WriteLine(dataSet.Dump(new JsonFormatter()));
        }
        finally
        {
            // Check to prevent issues in case dataset creation failed
            if (fullName != null)
            {
                var args = new DatasetDestroyArgs { Dataset = fullName };
                await ds.DestroyAsync(args, TestContext.Current.CancellationToken);
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
    public async Task DestroyRecursiveDryRunTest()
    {
        using var pool = TestPool.CreateSimplePool(_remoteProcessCall, _output);
        var dsHelper = GetDataSets();
        var rootId = DateTime.UtcNow.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
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
                await dsHelper.CreateAsync(args, TestContext.Current.CancellationToken);
            }

            var flags = DatasetDestroyFlags.Recursive | DatasetDestroyFlags.DryRun;
            var destroyArgs = new DatasetDestroyArgs { Dataset = root, DestroyFlags = flags };
            var response = await dsHelper.DestroyAsync(destroyArgs, TestContext.Current.CancellationToken);
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
                await dsHelper.DestroyAsync(destroyArgs, TestContext.Current.CancellationToken);
            }
            catch
            {
                //We dont care if cleaning up fails
            }
        }

    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task PromoteDataSetTest()
    {
        using var pool = TestPool.CreateSimplePool(_remoteProcessCall, _output);
        var ds = GetDataSets();
        var sn = ZfsTestFactory.CreateSnapshots(_remoteProcessCall);
        sn.RequiresSudo = TestHelpers.RequiresSudo;
        var cancellationToken = TestContext.Current.CancellationToken;

        await sn.CreateAsync(new SnapshotCreateArgs { Dataset = pool.Name, Snapshot = "mysnap" }, cancellationToken);
        var clone = $"{pool.Name}/mysnap_clone";
        await sn.CloneAsync(new SnapshotCloneArgs { Dataset = pool.Name, Snapshot = "mysnap", TargetDataset = clone }, cancellationToken);

        await ds.PromoteAsync(new PromoteArgs { Name = clone }, cancellationToken);

        var listArgs = new DatasetListArgs { Root = pool.Name, IncludeChildren = true };
        var datasets = await ds.ListAsync(listArgs, TestContext.Current.CancellationToken);
        var promoted = datasets.FirstOrDefault(d => d.DatasetName == clone);
        Assert.NotNull(promoted);
        Assert.False(promoted.IsClone);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task MountDataSetTest()
    {
        using var pool = TestPool.CreateSimplePool(_remoteProcessCall, _output);
        var ds = GetDataSets();
        var dataset = $"{pool.Name}/{Guid.NewGuid()}";
        var mountPoint = $"/tmp/zfs-mount/{Guid.NewGuid()}";
        var cancellationToken = TestContext.Current.CancellationToken;

        var mkdir = _remoteProcessCall | new ProcessCall("/usr/bin/mkdir", $"-p {mountPoint}");
        mkdir.RequiresSudo = TestHelpers.RequiresSudo;
        mkdir.LoadResponse(true);

        var createArgs = new DatasetCreationArgs
        {
            DatasetName = dataset,
            Type = DatasetTypes.Filesystem,
            DoNotMount = true,
            Properties = [new PropertyValue { Property = "mountpoint", Value = mountPoint }]
        };
        await ds.CreateAsync(createArgs, cancellationToken);

        try
        {
            await ds.MountAsync(new MountArgs { Filesystem = dataset }, cancellationToken);

            var mountPointCheck = _remoteProcessCall | new ProcessCall("/usr/bin/mountpoint", $"-q {mountPoint}");
            mountPointCheck.RequiresSudo = TestHelpers.RequiresSudo;
            Assert.True(mountPointCheck.LoadResponse(false).Success);
        }
        finally
        {
            try
            {
                await ds.UnmountAsync(new UnmountArgs { Filesystem = dataset }, cancellationToken);
            }
            catch
            {
                // We don't care if the unmount fails during cleanup
            }

            await ds.DestroyAsync(new DatasetDestroyArgs { Dataset = dataset }, cancellationToken);
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task UnmountDataSetTest()
    {
        using var pool = TestPool.CreateSimplePool(_remoteProcessCall, _output);
        var ds = GetDataSets();
        var dataset = $"{pool.Name}/{Guid.NewGuid()}";
        var mountPoint = $"/tmp/zfs-mount/{Guid.NewGuid()}";
        var cancellationToken = TestContext.Current.CancellationToken;

        var mkdir = _remoteProcessCall | new ProcessCall("/usr/bin/mkdir", $"-p {mountPoint}");
        mkdir.RequiresSudo = TestHelpers.RequiresSudo;
        mkdir.LoadResponse(true);

        var createArgs = new DatasetCreationArgs
        {
            DatasetName = dataset,
            Type = DatasetTypes.Filesystem,
            DoNotMount = true,
            Properties = [new PropertyValue { Property = "mountpoint", Value = mountPoint }]
        };
        await ds.CreateAsync(createArgs, cancellationToken);

        try
        {
            await ds.MountAsync(new MountArgs { Filesystem = dataset }, cancellationToken);

            var mountPointCheck = _remoteProcessCall | new ProcessCall("/usr/bin/mountpoint", $"-q {mountPoint}");
            mountPointCheck.RequiresSudo = TestHelpers.RequiresSudo;
            Assert.True(mountPointCheck.LoadResponse(false).Success);

            await ds.UnmountAsync(new UnmountArgs { Filesystem = dataset }, cancellationToken);

            mountPointCheck = _remoteProcessCall | new ProcessCall("/usr/bin/mountpoint", $"-q {mountPoint}");
            mountPointCheck.RequiresSudo = TestHelpers.RequiresSudo;
            Assert.False(mountPointCheck.LoadResponse(false).Success);
        }
        finally
        {
            await ds.DestroyAsync(new DatasetDestroyArgs { Dataset = dataset }, cancellationToken);
        }
    }
}