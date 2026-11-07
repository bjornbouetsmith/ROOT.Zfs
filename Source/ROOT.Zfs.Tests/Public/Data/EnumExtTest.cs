using System;
using Xunit;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Tests.Public.Data;

public class EnumExtTest
{
    private readonly ITestOutputHelper _output;

    public EnumExtTest(ITestOutputHelper output)
    {
        _output = output;
    }

    /// <summary>
    /// Test to ensure that all all <see cref="VDevCreationType"/> are handled properly
    /// </summary>
    [Fact]
    public void ReflectionBasedTest()
    {
        foreach (var type in Enum.GetValues<VDevCreationType>())
        {
            var s = type.AsString();
            _output.WriteLine("{0}={1}", type, s);
            Assert.NotNull(s);
        }
    }
    /// <summary>
    /// Known <see cref="VDevCreationType"/> to string mappings
    /// </summary>
    [InlineData(VDevCreationType.Special, "special")]
    [InlineData(VDevCreationType.Cache, "cache")]
    [InlineData(VDevCreationType.Dedup, "dedup")]
    [InlineData(VDevCreationType.DRaid1, "draid1")]
    [InlineData(VDevCreationType.DRaid2, "draid2")]
    [InlineData(VDevCreationType.DRaid3, "draid3")]
    [InlineData(VDevCreationType.Log, "log")]
    [InlineData(VDevCreationType.Mirror, "mirror")]
    [InlineData(VDevCreationType.Raidz1, "raidz1")]
    [InlineData(VDevCreationType.Raidz2, "raidz2")]
    [InlineData(VDevCreationType.Raidz3, "raidz3")]
    [InlineData(VDevCreationType.Spare, "spare")]
    [Theory]
    public void VDevCreationTypeAsStringTest(VDevCreationType type, string expected)
    {
        var s = type.AsString();
        Assert.Equal(expected, s);
    }

    [Fact]
    public void UnsupportedVDevCreationTypeShouldThrow()
    {
        VDevCreationType type = (VDevCreationType)(int)-999;

        var ex = Assert.Throws<NotSupportedException>(() => type.AsString());
        _output.WriteLine(ex.Message);
    }

    [InlineData(DatasetTypes.None, "filesystem,volume")]
    [InlineData(DatasetTypes.Filesystem, "filesystem")]
    [InlineData(DatasetTypes.Bookmark, "bookmark")]
    [InlineData(DatasetTypes.Snapshot, "snapshot")]
    [InlineData(DatasetTypes.Volume, "volume")]
    [InlineData(DatasetTypes.Volume | DatasetTypes.Bookmark, "bookmark,volume")]
    [InlineData(DatasetTypes.Volume | DatasetTypes.Bookmark | DatasetTypes.Snapshot, "bookmark,snapshot,volume")]
    [InlineData(DatasetTypes.Volume | DatasetTypes.Bookmark | DatasetTypes.Snapshot| DatasetTypes.Filesystem, "bookmark,filesystem,snapshot,volume")]
    [Theory]
    public void DataSetTypesAsStringTest(DatasetTypes types, string expected)
    {
        var stringVer = types.AsString();
        Assert.Equal(expected, stringVer);
    }
}