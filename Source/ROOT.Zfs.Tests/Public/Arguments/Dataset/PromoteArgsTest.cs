using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Arguments.Dataset;

namespace ROOT.Zfs.Tests.Public.Arguments.Dataset
{
    [TestClass]
    public class PromoteArgsTest
    {
        [DataRow("tank/myds", "promote tank/myds")]
        [DataRow("tank%2fmyds", "promote tank/myds")]
        [TestMethod]
        public void ToStringTest(string dataset, string expected)
        {
            var args = new PromoteArgs { Name = dataset };
            var stringVer = args.ToString();

            Assert.AreEqual(expected, stringVer);
        }

        [DataRow("tank/myds",true)]
        [DataRow("tank/myds && rm -ff /", false)]
        [TestMethod]
        public void ValidateTest(string dataset, bool expectedValid)
        {
            var args = new PromoteArgs { Name = dataset };
            var valid = args.Validate(out var errors);
            Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
            Assert.AreEqual(expectedValid, valid);
        }
    }
}
