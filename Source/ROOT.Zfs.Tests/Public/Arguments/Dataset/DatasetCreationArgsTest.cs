using System;
using System.Linq;
using Xunit;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Public.Arguments.Dataset;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Public.Arguments.Dataset;

public class DatasetCreationArgsTest
{
    [Theory]
    [InlineData("2M", "100G", false)]
    [InlineData("1.5K", "100G", false)]
    [InlineData("0K", "100G", false)]
    [InlineData("512K", "0G", false)]
    [InlineData("512K", "1G", true)]
    [InlineData("0.5K", "1G", true)]
    [InlineData("1M", "1G", true)]
    public void ValidateVolumeCreationTest(string blockSize, string volumeSize, bool expectedValid)
    {
        var args = new DatasetCreationArgs
        {
            DatasetName = "tank/myvol",
            Type = DatasetTypes.Volume,
            VolumeArguments = new VolumeCreationArgs
            {
                BlockSize = blockSize,
                VolumeSize = volumeSize,
            }
        };

        Console.WriteLine(args.Dump(new JsonFormatter()));
        var valid = args.Validate(out var errors);
        Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
        Assert.Equal(expectedValid, valid);
    }

    [Theory]
    [InlineData(null, DatasetTypes.Filesystem, false)]
    [InlineData(null, DatasetTypes.Volume, false)]
    [InlineData("", DatasetTypes.Filesystem, false)]
    [InlineData("", DatasetTypes.Volume, false)]
    [InlineData("  ", DatasetTypes.Filesystem, false)]
    [InlineData("  ", DatasetTypes.Volume, false)]
    [InlineData("tank", DatasetTypes.Snapshot, false)]
    [InlineData("tank", DatasetTypes.None, false)]
    [InlineData("tank", DatasetTypes.Bookmark, false)]
    [InlineData("tank", DatasetTypes.Filesystem, true)]
    [InlineData("tank", DatasetTypes.Volume, false)] // Volume requires VolumeCreationArgs
    public void ValidateCommonDatasetArgsTest(string name, DatasetTypes type, bool expectedValid)
    {
        var args = new DatasetCreationArgs
        {
            DatasetName = name,
            Type = type,
        };
        Console.WriteLine(args.Dump(new JsonFormatter()));
        var valid = args.Validate(out var errors);
        Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
        Assert.Equal(expectedValid, valid);
    }

    [InlineData(DatasetTypes.Filesystem, true, true, null, false, null, null, "create -p -u tank/child")]
    [InlineData(DatasetTypes.Filesystem, true, false, null, false, null, null, "create -p tank/child")]
    [InlineData(DatasetTypes.Filesystem, false, false, null, false, null, null, "create tank/child")]
    [InlineData(DatasetTypes.Filesystem, false, false, "atime=off", false, null, null, "create -o atime=off tank/child")]
    [InlineData(DatasetTypes.Filesystem, false, false, "atime=off,compression=off", false, null, null, "create -o atime=off -o compression=off tank/child")]

    [InlineData(DatasetTypes.Volume, true, true, null, true, "8K", "18G", "create -b 8K -V 18G -s -p tank/child")]
    [InlineData(DatasetTypes.Volume, false, false, null, true, "8K", "18G", "create -b 8K -V 18G -s tank/child")]
    [InlineData(DatasetTypes.Volume, true, true, null, false, "8K", "18G", "create -b 8K -V 18G -p tank/child")]
    [InlineData(DatasetTypes.Volume, false, false, null, false, "8K", "18G", "create -b 8K -V 18G tank/child")]
    [Theory]
    public void ToStringTest(DatasetTypes type, bool createParents, bool doNotMount, string properties, bool sparseVolume, string blockSize, string volumeSize, string expected)
    {
        var props = properties?.Split(',').Select(p => p.Split('=')).Select(a => new PropertyValue { Property = a[0], Value = a[1] }).ToArray();
        var args = new DatasetCreationArgs
        {
            Type = type,
            CreateParents = createParents,
            DatasetName = "tank/child",
            DoNotMount = doNotMount,
            Properties = props,
            VolumeArguments = type == DatasetTypes.Filesystem ? null : new VolumeCreationArgs
            {
                BlockSize = blockSize,
                VolumeSize = volumeSize,
                Sparse = sparseVolume
            }
        };
        var stringVer = args.ToString();
        Assert.Equal(expected, stringVer);
    }
}