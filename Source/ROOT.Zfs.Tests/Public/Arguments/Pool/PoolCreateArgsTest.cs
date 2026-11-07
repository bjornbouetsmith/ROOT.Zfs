using System;
using System.Linq;
using Xunit;
using ROOT.Zfs.Public.Arguments.Pool;
using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Tests.Public.Arguments.Pool;

public class PoolCreateArgsTest
{
    [Fact]
    public void NameShouldBeSet()
    {
        var args = new PoolCreateArgs();

        var valid = args.Validate(out var errors);
        Assert.False(valid);

        Assert.Equal(2, errors.Count());
        Assert.Contains(errors, e => e.Contains("Name cannot be empty"));
        Assert.Contains(errors, e => e.Contains("Please provide vdevs"));
    }

    [Fact]
    public void VDevsMustBeSpecified()
    {
        var args = new PoolCreateArgs { PoolName = "tank" };

        var valid = args.Validate(out var errors);
        Assert.False(valid);
        Assert.Single(errors);
        Assert.Contains("Please provide vdevs", errors[0]);

        args.VDevs = Array.Empty<VDevCreationArgs>();
        valid = args.Validate(out errors);
        Assert.False(valid);
        Assert.Single(errors);
        Assert.Contains("Please provide vdevs", errors[0]);
    }

    [Fact]
    public void AnyTypeOfVdevShouldRequireAtLeastOneDevice()
    {
        var args = new PoolCreateArgs
        {
            PoolName = "tank",
            VDevs =
            [
                new VDevCreationArgs
                {
                    Type= VDevCreationType.Mirror,
                }
            ]
        };
        var valid = args.Validate(out var errors);
        Assert.False(valid);
        Console.WriteLine(string.Join(Environment.NewLine, errors));
        Assert.Single(errors);
        Assert.Contains("Please provide the proper amount of minimum", errors[0]);
    }


    [Fact]
    public void MirrorShouldRequireAtLeastTwoDevices()
    {
        var args = new PoolCreateArgs
        {
            PoolName = "tank",
            VDevs =
            [
                new VDevCreationArgs
                {
                    Type= VDevCreationType.Mirror,
                    Devices = ["/dev/sda"]
                }
            ]
        };
        var valid = args.Validate(out var errors);
        Assert.False(valid);
        Console.WriteLine(string.Join(Environment.NewLine, errors));
        Assert.Single(errors);
        Assert.Contains("Please provide at least two devices", errors[0]);

    }
}