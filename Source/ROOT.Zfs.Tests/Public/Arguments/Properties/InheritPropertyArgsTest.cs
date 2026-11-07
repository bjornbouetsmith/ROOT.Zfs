using System;
using Xunit;
using ROOT.Zfs.Public.Arguments;
using ROOT.Zfs.Public.Arguments.Properties;

namespace ROOT.Zfs.Tests.Public.Arguments.Properties;

public class InheritPropertyArgsTest
{
    [InlineData(PropertyTarget.Dataset, "atime", "tank", true)]
    [InlineData(PropertyTarget.Dataset, "atime", null, false)]
    [InlineData(PropertyTarget.Dataset, "atime", "", false)]
    [InlineData(PropertyTarget.Dataset, "atime", " ", false)]

    [InlineData(PropertyTarget.Dataset, null, "tank/myds", false)]
    [InlineData(PropertyTarget.Dataset, "", "tank/myds", false)]
    [InlineData(PropertyTarget.Dataset, " ", "tank/myds", false)]

    [InlineData(PropertyTarget.Dataset, null, null, false)]

    [InlineData(PropertyTarget.Pool, "atime", "tank/myds", false)]
    [Theory]
    public void ValidateTest(PropertyTarget propertyTarget, string property, string target, bool expectValid)
    {
        var args = new InheritPropertyArgs { PropertyTarget = propertyTarget, Property = property, Target = target };

        var valid = args.Validate(out var errors);
        Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
        Assert.Equal(expectValid, valid);
    }

    [InlineData(PropertyTarget.Dataset, "atime", "tank", "inherit -rS atime tank")]
    [Theory]
    public void ToStringTest(PropertyTarget propertyTarget, string property, string target, string expected)
    {
        var args = new InheritPropertyArgs { PropertyTarget = propertyTarget, Property = property, Target = target };
        var stringVer = args.ToString();
        Console.WriteLine(stringVer);
        Assert.Equal(expected, stringVer);
    }
}