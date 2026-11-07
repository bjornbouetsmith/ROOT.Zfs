using System;
using Xunit;
using ROOT.Zfs.Public.Arguments.Pool;

namespace ROOT.Zfs.Tests.Public.Arguments.Pool;

public class PoolRemoveArgsTest
{
    [InlineData("tank", "mirror-0", false, true)]
    [InlineData("tank", "mirror-0", true, true)]
    [InlineData("tank", null, true, true)]
    [InlineData("tank", "", true, true)]
    [InlineData("tank", " ", true, true)]
    [InlineData("tank", null, false, false)]
    [InlineData("tank", "", false, false)]
    [InlineData("tank", " ", false, false)]
    [InlineData(null, "mirror-0", false, false)]
    [InlineData("", "mirror-0", false, false)]
    [InlineData(" ", "mirror-0", false, false)]
    [Theory]
    public void ValidateTest(string poolName, string vdevOrDevice, bool cancel, bool expectedValid)
    {
        var args = new PoolRemoveArgs
        {
            PoolName = poolName,
            VDev = vdevOrDevice,
            Cancel = cancel,
        };

        var valid = args.Validate(out var errors);
        Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
        Assert.Equal(expectedValid, valid);
    }

    [InlineData("tank", "mirror-0", false, "remove tank mirror-0")]
    [InlineData("tank", "mirror-0", true, "remove -s tank")]
    [InlineData("tank", null, true, "remove -s tank")]
    [InlineData("tank", "", true, "remove -s tank")]
    [InlineData("tank", " ", true, "remove -s tank")]
    [Theory]
    public void ToStringTest(string poolName, string vdevOrDevice, bool cancel, string expected)
    {
        var args = new PoolRemoveArgs
        {
            PoolName = poolName,
            VDev = vdevOrDevice,
            Cancel = cancel,
        };

        Assert.Equal(expected, args.ToString());
    }
}