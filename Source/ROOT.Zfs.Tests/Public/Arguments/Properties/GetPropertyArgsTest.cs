using System;
using Xunit;
using ROOT.Zfs.Public.Arguments;
using ROOT.Zfs.Public.Arguments.Properties;

namespace ROOT.Zfs.Tests.Public.Arguments.Properties;

public class GetPropertyArgsTest
{
    private readonly ITestOutputHelper _output;

    public GetPropertyArgsTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [InlineData(PropertyTarget.Pool, null, null, true)]
    [InlineData(PropertyTarget.Pool, "ashift", null, true)]
    [InlineData(PropertyTarget.Pool, "ashift", "tank", true)]
    [InlineData(PropertyTarget.Pool, "ashift", "tank/myds", false)]
    [InlineData(PropertyTarget.Pool, null, "tank", true)]
    [InlineData(PropertyTarget.Dataset, null, null, true)]
    [InlineData(PropertyTarget.Dataset, "atime", null, true)]
    [InlineData(PropertyTarget.Dataset, "atime", "tank/myds", true)]
    [InlineData(PropertyTarget.Dataset, null, "tank/myds", true)]
    [InlineData(PropertyTarget.Dataset, null, "tank/myds && rm -rf /", false)]
    [Theory]
    public void ValidateTest(PropertyTarget propertyTarget, string property, string target, bool expectedValid)
    {
        var args = new GetPropertyArgs
        {
            PropertyTarget = propertyTarget,
            Property = property,
            Target = target,
        };

        var valid = args.Validate(out var errors);
        _output.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));

        Assert.Equal(expectedValid, valid);
    }

    [InlineData(PropertyTarget.Pool, null, null, "get -H")]
    [InlineData(PropertyTarget.Pool, "ashift", null, "get ashift -H")]
    [InlineData(PropertyTarget.Pool, "ashift", "tank", "get ashift tank -H")]
    [InlineData(PropertyTarget.Pool, null, "tank", "get all tank -H")]
    [InlineData(PropertyTarget.Dataset, null, null, "get -H")]
    [InlineData(PropertyTarget.Dataset, "atime", null, "get atime -H")]
    [InlineData(PropertyTarget.Dataset, "atime", "tank/myds", "get atime tank/myds -H")]
    [InlineData(PropertyTarget.Dataset, null, "tank/myds", "get all tank/myds -H")]
    [Theory]
    public void ToStringTest(PropertyTarget propertyTarget, string property, string target, string expected)
    {
        var args = new GetPropertyArgs
        {
            PropertyTarget = propertyTarget,
            Property = property,
            Target = target,
        };
        var stringVer = args.ToString();
        _output.WriteLine(stringVer);
        Assert.Equal(expected, stringVer);
    }
}