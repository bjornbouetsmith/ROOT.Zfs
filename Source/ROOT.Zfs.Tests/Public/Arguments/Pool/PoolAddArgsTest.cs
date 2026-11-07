using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using ROOT.Zfs.Public.Arguments.Pool;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Tests.Public.Arguments.Pool;

public class PoolAddArgsTest
{
    [InlineData("tank", true, false, true)]
    [InlineData("tank", false, true, false)]
    [InlineData("tank", false, false, false)]
    [InlineData(null, true, false, false)]
    [InlineData("", true, false, false)]
    [InlineData(" ", true, false, false)]
    [Theory]
    public void ValidateTest(string poolName, bool addValidVdev, bool addInvalidDev, bool expectValid)
    {
        IList<VDevCreationArgs> vdevs = addValidVdev ?
            [

                new()
                {
                    Type = VDevCreationType.Mirror,
                    Devices = ["a", "b"]
                }
            ]
            : addInvalidDev ? new List<VDevCreationArgs>
            {
                new() {
                    Type = VDevCreationType.Mirror,
                    Devices = ["a"]
                }
            } : null;
        var args = new PoolAddArgs { PoolName = poolName, VDevs = vdevs };

        var valid = args.Validate(out var errors);
        Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
        Assert.Equal(expectValid, valid);
    }

    [InlineData("tank", true, "add -f tank mirror a b")]
    [InlineData("tank", false, "add tank mirror a b")]
    [Theory]
    public void BuildArgsTestSimple(string pool, bool force, string expected)
    {
        var args = new PoolAddArgs
        {
            PoolName = pool,
            Force = force,
            VDevs =
            [
                new() {
                    Type = VDevCreationType.Mirror,
                    Devices = ["a", "b"]
                }
            ]
        };

        Assert.Equal(expected, args.ToString());

    }

    [InlineData("tank", "ashift=12", "add tank -o ashift=12 mirror a b")]
    [InlineData("tank", "ashift=12,atime=off", "add tank -o ashift=12 mirror a b")]
    [InlineData("tank", "atime=off", "add tank mirror a b")]
    [Theory]
    public void BuildArgsTestWithProperties(string pool, string properties, string expected)
    {
        var props = properties?.Split(',').Select(p => p.Split('=')).Select(a => new PropertyValue { Property = a[0], Value = a[1] }).ToArray();
        var args = new PoolAddArgs
        {
            PoolName = pool,
            VDevs =
            [
                new() {
                    Type = VDevCreationType.Mirror,
                    Devices = ["a", "b"]
                }
            ],
            PropertyValues = props
        };

        Assert.Equal(expected, args.ToString());
    }

    [Fact]
    public void MissingVDevsShouldThrowArgumentExceptionOnToString()
    {
        var args = new PoolAddArgs
        {
            PoolName = "tank"
        };

        var ex = Assert.Throws<ArgumentException>(() => args.ToString());
        Assert.Equal("Missing Vdevs", ex.Message);

    }
}