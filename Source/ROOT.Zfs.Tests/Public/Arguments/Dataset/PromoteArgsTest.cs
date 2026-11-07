using System;
using Xunit;
using ROOT.Zfs.Public.Arguments.Dataset;

namespace ROOT.Zfs.Tests.Public.Arguments.Dataset;

public class PromoteArgsTest
{
    [InlineData("tank/myds", "promote tank/myds")]
    [InlineData("tank%2fmyds", "promote tank/myds")]
    [Theory]
    public void ToStringTest(string dataset, string expected)
    {
        var args = new PromoteArgs { Name = dataset };
        var stringVer = args.ToString();

        Assert.Equal(expected, stringVer);
    }

    [InlineData("tank/myds",true)]
    [InlineData("tank/myds && rm -ff /", false)]
    [Theory]
    public void ValidateTest(string dataset, bool expectedValid)
    {
        var args = new PromoteArgs { Name = dataset };
        var valid = args.Validate(out var errors);
        Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
        Assert.Equal(expectedValid, valid);
    }
}