using System;
using Xunit;
using ROOT.Zfs.Public.Arguments;
using ROOT.Zfs.Public.Arguments.Properties;

namespace ROOT.Zfs.Tests.Public.Arguments.Properties;

public class SetPropertyArgsTest
{
    [InlineData(PropertyTarget.Pool, "autotrim", "on", "tank", true)]
    [InlineData(PropertyTarget.Pool, "mycustom:trim:schedule", "1D", "tank", true)]
    [InlineData(PropertyTarget.Pool, "userobjquota@bbs", "1G", "tank", true)]

    [InlineData(PropertyTarget.Pool, null, "on", "tank", false)]
    [InlineData(PropertyTarget.Pool, "", "on", "tank", false)]
    [InlineData(PropertyTarget.Pool, " ", "on", "tank", false)]

    [InlineData(PropertyTarget.Pool, "autotrim", null, "tank", false)]
    [InlineData(PropertyTarget.Pool, "autotrim", "", "tank", false)]
    [InlineData(PropertyTarget.Pool, "autotrim", " ", "tank", false)]

    [InlineData(PropertyTarget.Pool, "autotrim", "on", null, false)]
    [InlineData(PropertyTarget.Pool, "autotrim", "on", "", false)]
    [InlineData(PropertyTarget.Pool, "autotrim", "on", " ", false)]
    [InlineData(PropertyTarget.Pool, null, null, null, false)]

    [Theory]
    public void ValidateTest(PropertyTarget propertyTarget, string property, string value, string target, bool expectedValid)
    {
        var args = new SetPropertyArgs { PropertyTarget = propertyTarget, Property = property, Value = value, Target = target };
        var valid = args.Validate(out var errors);
        Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
        Assert.Equal(expectedValid, valid);
    }

    [InlineData(PropertyTarget.Pool, "autotrim", "on", "tank", "set autotrim=on tank")]
    [InlineData(PropertyTarget.Pool, "atime", "off", "tank/myds", "set atime=off tank/myds")]
    [Theory]
    public void ToStringTest(PropertyTarget propertyTarget, string property, string value, string target, string expected)
    {
        var args = new SetPropertyArgs { PropertyTarget = propertyTarget, Property = property, Value = value, Target = target };
        var stringVer = args.ToString();
        Console.WriteLine(stringVer);
        Assert.Equal(expected, stringVer);
    }
}