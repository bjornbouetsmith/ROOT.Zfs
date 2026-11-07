using System;
using Xunit;
using ROOT.Zfs.Public.Arguments.Pool;

namespace ROOT.Zfs.Tests.Public.Arguments.Pool;

public class PoolOfflineArgsTest
{

    [InlineData(null, "disk", false)]
    [InlineData("", "disk", false)]
    [InlineData(" ", "disk", false)]
    [InlineData("tank", "disk", true)]
    [InlineData("tank", null, false)]
    [InlineData("tank", "", false)]
    [InlineData("tank", " ", false)]
    [Theory]
    public void ValidateTest(string pool, string device, bool expectedValid)
    {
        var args = new PoolOfflineArgs
        {
            PoolName = pool,
            Device = device,
        };

        var valid = args.Validate(out var errors);
        Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
        Assert.Equal(expectedValid, valid);
    }

    [InlineData("tank", "sda", false, false, "offline tank sda")]
    [InlineData("tank", "sda", false, true, "offline -t tank sda")]
    [InlineData("tank", "sda", true, true, "offline -f -t tank sda")]
    [InlineData("tank", "sda", true, false, "offline -f tank sda")]
    [Theory]
    public void BuildArgsTest(string pool, string device, bool forceFault, bool temporary, string expected)
    {
        var args = new PoolOfflineArgs
        {
            PoolName = pool,
            Device = device,
            ForceFault = forceFault,
            Temporary = temporary
        };
        var stringVer = args.ToString();

        Assert.Equal(expected, stringVer);
    }
}