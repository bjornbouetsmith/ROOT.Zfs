using System;
using Xunit;
using ROOT.Zfs.Public.Arguments.Pool;

namespace ROOT.Zfs.Tests.Public.Arguments.Pool;

public class PoolTrimArgsTest
{
    [InlineData("tank", true)]
    [InlineData("", false)]
    [InlineData(null, false)]
    [InlineData("  ", false)]
    [Theory]
    public void ValidateShouldValidateCorrectly(string poolName, bool expectValid)
    {
        var args = new PoolTrimArgs { PoolName = poolName };

        var valid = args.Validate(out var errors);
        Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
        Assert.Equal(expectValid, valid);

    }

    [InlineData("tank",null,false,"0B",null,"trim tank")] // only pool
    [InlineData("tank", "/dev/sda", false, "0B", null, "trim tank /dev/sda")] // with device
    [InlineData("tank", "/dev/sda", true, "0B", null, "trim -d tank /dev/sda")] // secure
    [InlineData("tank", "/dev/sda", false, "0B", TrimAction.None, "trim tank /dev/sda")] // none
    [InlineData("tank", "/dev/sda", false, "0B", TrimAction.Suspend, "trim -s tank /dev/sda")] // suspend
    [InlineData("tank", "/dev/sda", false, "0B", TrimAction.Cancel, "trim -c tank /dev/sda")] // cancel
    [InlineData("tank", "/dev/sda", false, "512M", null, "trim -r 512M tank /dev/sda")] // with rate
    [InlineData("tank", "/dev/sda", true, "512G", TrimAction.None, "trim -d -r 512G tank /dev/sda")] // with rate and secure
    [Theory]
    public void ToStringTest(string poolName, string deviceName, bool secure, string trimRate, TrimAction? trimAction, string expectedCommand)
    {
        var args = new PoolTrimArgs
        {
            PoolName = poolName,
            DeviceName = deviceName,
            Secure = secure,
            TrimRate = trimRate,
            Action = trimAction ?? TrimAction.None
        };

        var stringVer = args.ToString();
        Console.WriteLine(stringVer);
        Assert.Equal(expectedCommand, stringVer);
    }
}