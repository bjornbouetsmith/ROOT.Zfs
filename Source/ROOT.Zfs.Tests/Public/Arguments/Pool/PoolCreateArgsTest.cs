using System;
using Xunit;
using ROOT.Zfs.Public.Arguments.Pool;
using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Tests.Public.Arguments.Pool;

public class PoolCreateArgsTest
{
    private readonly ITestOutputHelper _output;

    public PoolCreateArgsTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void NameShouldBeSet()
    {
        var args = new PoolCreateArgs();

        var valid = args.Validate(out var errors);
        Assert.False(valid);

        Assert.Equal(2, errors.Count);
        Assert.Contains(errors, e => e.Contains("Name cannot be empty"));
        Assert.Contains(errors, e => e.Contains("Please provide vdevs"));
    }

    [Fact]
    public void VDevsMustBeSpecified()
    {
        var args = new PoolCreateArgs { PoolName = "tank" };

        var valid = args.Validate(out var errors);
        Assert.False(valid);
        var error = Assert.Single(errors);
        Assert.Contains("Please provide vdevs", error);

        args.VDevs = Array.Empty<VDevCreationArgs>();
        valid = args.Validate(out errors);
        Assert.False(valid);
        error = Assert.Single(errors);
        Assert.Contains("Please provide vdevs", error);
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
        _output.WriteLine(string.Join(Environment.NewLine, errors));
        var error = Assert.Single(errors);
        Assert.Contains("Please provide the proper amount of minimum", error);
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
        _output.WriteLine(string.Join(Environment.NewLine, errors));
        var error = Assert.Single(errors);
        Assert.Contains("Please provide at least two devices", error);

    }
}