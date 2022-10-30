using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core.Helpers;

namespace ROOT.Zfs.Tests.Helpers
{
    [TestClass]
    public class PropertyValueHelperTest
    {

        [TestMethod]
        public void ValidFormatShouldParse()
        {
            var value = PropertyValueHelper.FromString("tank	atime	on	default");
            Console.WriteLine(value.Dump(new JsonFormatter()));
            Assert.AreEqual("on", value.Value);
            Assert.AreEqual("atime", value.Property); 
            Assert.AreEqual("default", value.Source);
        }

        [TestMethod]
        public void InvalidFormatShouldThrow()
        {
            Assert.ThrowsException<FormatException>(() => PropertyValueHelper.FromString("tank	atime	on"));
        }

        [TestMethod]
        public void InvalidFormatShouldThrowTabsReplacedWithSpaces()
        {
            Assert.ThrowsException<FormatException>(() => PropertyValueHelper.FromString("tank    atime   on      default"));
        }
    }
}
