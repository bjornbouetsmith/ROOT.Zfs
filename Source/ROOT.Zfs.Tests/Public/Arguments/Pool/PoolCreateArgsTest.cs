using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Arguments.Pool;
using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Tests.Public.Arguments.Pool;

[TestClass]
public class PoolCreateArgsTest
{
    [TestMethod]
    public void NameShouldBeSet()
    {
        var args = new PoolCreateArgs();

        var valid = args.Validate(out var errors);
        Assert.IsFalse(valid);

        Assert.HasCount(2, errors);
        Assert.Contains(e => e.Contains("Name cannot be empty"), errors);
        Assert.Contains(e => e.Contains("Please provide vdevs"), errors);
    }

    [TestMethod]
    public void VDevsMustBeSpecified()
    {
        var args = new PoolCreateArgs { PoolName = "tank" };

        var valid = args.Validate(out var errors);
        Assert.IsFalse(valid);
        Assert.HasCount(1, errors);
        Assert.Contains("Please provide vdevs", errors[0]);

        args.VDevs = Array.Empty<VDevCreationArgs>();
        valid = args.Validate(out errors);
        Assert.IsFalse(valid);
        Assert.HasCount(1, errors);
        Assert.Contains("Please provide vdevs", errors[0]);
    }

    [TestMethod]
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
        Assert.IsFalse(valid);
        Console.WriteLine(string.Join(Environment.NewLine, errors));
        Assert.HasCount(1, errors);
        Assert.Contains("Please provide the proper amount of minimum", errors[0]);
    }


    [TestMethod]
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
        Assert.IsFalse(valid);
        Console.WriteLine(string.Join(Environment.NewLine, errors));
        Assert.HasCount(1, errors);
        Assert.Contains("Please provide at least two devices", errors[0]);

    }
}