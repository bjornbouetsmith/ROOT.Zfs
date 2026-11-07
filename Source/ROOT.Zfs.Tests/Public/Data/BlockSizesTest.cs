using System;
using System.Linq;
using System.Reflection;
using Xunit;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Public.Data;

public class BlockSizesTest
{
    [Fact]
    public void TooSmallBlockSizeShouldNotBeValid()
    {
        var valid = BlockSizes.IsValid(511UL, out var errorMessage);
        Console.WriteLine(errorMessage);
        Assert.False(valid);
        Assert.NotNull(errorMessage);
    }

    [Fact]
    public void TooBigBlockSizeShouldNotBeValid()
    {
        var valid = BlockSizes.IsValid("2M", out var errorMessage);
        Console.WriteLine(errorMessage);
        Assert.False(valid);
        Assert.NotNull(errorMessage);
    }
    /// <summary>
    /// Uses reflection to grab all public static fields of type Size and verify that they are valid
    /// </summary>
    [Fact]
    public void PredefinedBlockSizesShouldBeValid()
    {
        foreach (var field in typeof(BlockSizes).GetFields(BindingFlags.Public | BindingFlags.Static).Where(f => f.FieldType == typeof(Size)))
        {
            Size value = (Size)field.GetValue(null);
            Assert.True(BlockSizes.IsValid(value, out _));
        }
    }
}