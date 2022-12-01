using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Arguments.Pool;

namespace ROOT.Zfs.Tests.Public.Arguments.Pool
{
    [TestClass]
    public class PoolResilverArgsTest
    {
        [DataRow("tank", true)]
        [DataRow("tank%2etest", true)]
        [DataRow("tank%2fmyds", false)]
        [DataRow("tank/myds", false)]
        [TestMethod]
        public void ValidateTest(string pool, bool expectedValid)
        {
            var args = new PoolResilverArgs { Name = pool };
            var valid = args.Validate(out var errors);
            Console.WriteLine(string.Join(Environment.NewLine,errors ?? Array.Empty<string>()));
            Assert.AreEqual(expectedValid, valid);
        }

        [DataRow("tank","resilver tank")]
        [DataRow("tank%2etest", "resilver tank.test")]
        [TestMethod]
        public void ToStringTest(string pool, string expected)
        {
            var args = new PoolResilverArgs{Name=pool};
            var stringVer  = args.ToString();
            Assert.AreEqual(expected, stringVer);
        }
    }
}
