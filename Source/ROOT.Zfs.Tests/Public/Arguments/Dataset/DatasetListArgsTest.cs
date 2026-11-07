using System;
using Xunit;
using ROOT.Zfs.Public.Arguments.Dataset;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Public.Arguments.Dataset;

public class DatasetListArgsTest
{
    private readonly ITestOutputHelper _output;

    public DatasetListArgsTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [InlineData("tank && rm -rf /", false)]
    [InlineData("tank;rm -rf /", false)]
    [InlineData("tank/myds", true)]
    [InlineData("tank%2fmyds", true)]
    [Theory]
    public void ValidateTest(string root, bool expectValid)
    {
        var args = new DatasetListArgs { Root = root };
        var valid = args.Validate(out var errors);
        _output.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
        Assert.Equal(expectValid, valid);
    }


    [InlineData("tank", DatasetTypes.None, false, "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t filesystem,volume tank")]
    [InlineData("tank", DatasetTypes.Filesystem, false, "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t filesystem tank")]
    [InlineData("tank", DatasetTypes.None, true, "list -Hpr -o type,creation,name,used,refer,avail,mountpoint,origin -d 99 -t filesystem,volume tank")]
    [InlineData(null, DatasetTypes.None, false, "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t filesystem,volume")]
    [InlineData(null, DatasetTypes.None, true, "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t filesystem,volume")] // include children have no effect when a root is not specified
    [Theory]
    public void ToStringTest(string root, DatasetTypes types, bool includeChildren, string expected)
    {
        var args = new DatasetListArgs
        {
            Root = root,
            DatasetTypes = types,
            IncludeChildren = includeChildren,
        };

        var stringVer = args.ToString();

        Assert.Equal(expected, stringVer);
    }
}