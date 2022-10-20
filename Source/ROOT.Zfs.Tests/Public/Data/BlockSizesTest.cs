using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Public.Data
{
    [TestClass]
    public class BlockSizesTest
    {
        [TestMethod]
        public void TooSmallBlockSizeShouldNotBeValid()
        {
            var valid = BlockSizes.IsValid(511UL, out var errorMessage);
            Console.WriteLine(errorMessage);
            Assert.IsFalse(valid);
            Assert.IsNotNull(errorMessage);
        }

        [TestMethod]
        public void TooBigBlockSizeShouldNotBeValid()
        {
            var valid = BlockSizes.IsValid("2M", out var errorMessage);
            Console.WriteLine(errorMessage);
            Assert.IsFalse(valid);
            Assert.IsNotNull(errorMessage);
        }
        /// <summary>
        /// Uses reflection to grab all public static fields of type Size and verify that they are valid
        /// </summary>
        [TestMethod]
        public void PredefinedBlockSizesShouldBeValid()
        {
            foreach (var field in typeof(BlockSizes).GetFields(BindingFlags.Public | BindingFlags.Static).Where(f => f.FieldType == typeof(Size)))
            {
                Size value = (Size)field.GetValue(null);
                Assert.IsTrue(BlockSizes.IsValid(value, out _));
            }
        }
    }
}
