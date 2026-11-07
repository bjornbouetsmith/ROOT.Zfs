using Xunit;
using ROOT.Zfs.Core.Helpers;
using System.Linq;

namespace ROOT.Zfs.Tests.Helpers;

public class DataSetPropertiesTest
{
    [Fact]
    public void FromStdOutputTest()
    {
        var stdOut = "tank\taclmode\tdiscard\tdefault\ntank\taclinherit\tdiscard\tdefault";
        var props = DatasetProperties.FromStdOutput(stdOut);

        Assert.Equal(2, props.Count());
            
        Assert.Equal("aclmode", props[0].Property);
        Assert.Equal("discard", props[0].Value);
        Assert.Equal("default", props[0].Source);
                        
        Assert.Equal("aclinherit", props[1].Property);
        Assert.Equal("discard", props[1].Value);
        Assert.Equal("default", props[1].Source);
    }

    [Fact]
    public void FromStdOutputEmptyTest()
    {
        var props = DatasetProperties.FromStdOutput(string.Empty);
        Assert.Empty(props);
    }
}