using System;
using Xunit;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Helpers;

public class DataSetHelperTest
{

    [Theory]
    [InlineData("tank", "myds", "tank/myds")]
    [InlineData("tank/myds", "myds2", "tank/myds/myds2")]
    [InlineData("tank%2fmyds", "myds2", "tank/myds/myds2")]
    public void CreateDataSetNameTest(string parent, string dataset, string expected)
    {
        var name = DatasetHelper.CreateDatasetName(parent, dataset);
        Assert.Equal(expected, name);
    }

    [Fact]
    public void ParseDataSetTestNotAClone()
    {
        var stdOut = @"filesystem      1652262393      tank/kubedata   708100096       396648448       3566981726208   /tank/kubedata   -";

        var dataset = DatasetHelper.ParseLine(stdOut);
        Console.WriteLine(dataset.Dump(new JsonFormatter()));
        Assert.Equal("tank/kubedata", dataset.DatasetName);
        Assert.Equal("675.3M", dataset.Used.ToString());
        Assert.Equal("378.3M", dataset.Available.ToString());
        Assert.Equal("3.2T", dataset.Refer.ToString());
        Assert.Equal("/tank/kubedata", dataset.Mountpoint);
        Assert.False(dataset.IsClone);
    }

    [Fact]
    public void ParseDataSetTestAClone()
    {
        var stdOut = @"filesystem      1652262393      tank/kubedata   708100096       396648448       3566981726208   /tank/kubedata   tank/kubedata@1234";

        var dataset = DatasetHelper.ParseLine(stdOut);
        Console.WriteLine(dataset.Dump(new JsonFormatter()));
        Assert.Equal("tank/kubedata", dataset.DatasetName);
        Assert.Equal("675.3M", dataset.Used.ToString());
        Assert.Equal("378.3M", dataset.Available.ToString());
        Assert.Equal("3.2T", dataset.Refer.ToString());
        Assert.Equal("/tank/kubedata", dataset.Mountpoint);
        Assert.True(dataset.IsClone);
    }

    [Fact]
    public void ParseTruncatedFormatShouldThrow()
    {
        var stdOut = @"filesystem      1652262393      tank/kubedata   708100096       396648448";

        Assert.Throws<FormatException>(() => DatasetHelper.ParseLine(stdOut));
    }

    [Fact]
    public void ParseUnknownDatasetTypeShouldNotThrow()
    {
        var stdOut = @"link      1652262393      tank/kubedata   708100096       396648448       3566981726208   /tank/kubedata   -";

        var dataset = DatasetHelper.ParseLine(stdOut);

        Assert.Equal(DatasetTypes.None, dataset.Type);
    }
}