using System;
using Xunit;
using ROOT.Zfs.Public.Arguments.Dataset;

namespace ROOT.Zfs.Tests.Public.Arguments.Dataset;

public class DatasetDestroyArgsTest
{
    [InlineData(DatasetDestroyFlags.None, "destroy tank/myds")]
    [InlineData(DatasetDestroyFlags.Recursive, "destroy -r tank/myds")]
    [InlineData(DatasetDestroyFlags.RecursiveClones, "destroy -R tank/myds")]
    [InlineData(DatasetDestroyFlags.ForceUmount, "destroy -f tank/myds")]
    [InlineData(DatasetDestroyFlags.DryRun, "destroy -nvp tank/myds")]
    [InlineData((DatasetDestroyFlags)(-1), "destroy -r -R -f -nvp tank/myds")]
    [Theory]
    public void ToStringTest(DatasetDestroyFlags flags, string expected)
    {
        var args = new DatasetDestroyArgs { Dataset = "tank/myds", DestroyFlags = flags };
        var stringVer = args.ToString();
        Assert.Equal(expected, stringVer);
    }

    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData(" ", false)]
    [InlineData("tank/myds@snap123", false)]
    [InlineData("tank/myds", true)]
    [Theory]
    public void ValidateTest(string dataset, bool expectValid)
    {
        var args = new DatasetDestroyArgs
        {
            Dataset = dataset
        };

        var valid = args.Validate(out var errors);
        Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
        Assert.Equal(expectValid, valid);
    }
}