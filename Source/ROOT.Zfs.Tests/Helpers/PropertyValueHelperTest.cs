using System;
using Xunit;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core.Helpers;

namespace ROOT.Zfs.Tests.Helpers;

public class PropertyValueHelperTest
{
    [Fact]
    public void ValidFormatShouldParse()
    {
        var value = PropertyValueHelper.FromString("tank	atime	on	default");
        Console.WriteLine(value.Dump(new JsonFormatter()));
        Assert.Equal("on", value.Value);
        Assert.Equal("atime", value.Property);
        Assert.Equal("default", value.Source);
        Assert.Equal("tank", value.Name);
    }

    [Fact]
    public void InvalidFormatShouldThrow()
    {
        Assert.Throws<FormatException>(() => PropertyValueHelper.FromString("tank	atime	on"));
    }

    [Fact]
    public void InvalidFormatShouldThrowTabsReplacedWithSpaces()
    {
        Assert.Throws<FormatException>(() => PropertyValueHelper.FromString("tank    atime   on      default"));
    }
}