using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Arguments.Pool;

namespace ROOT.Zfs.Tests.Public.Arguments.Pool
{
    [TestClass]
    public class PoolHistoryArgsTest
    {
        [DataRow("tank",true)]
        [DataRow(null, false)]
        [DataRow("", false)]
        [DataRow(" ", false)]
        [TestMethod]
        public void ValidateTest(string poolName, bool expectedValid)
        {
            var args = new PoolHistoryArgs { PoolName = poolName };

            var valid = args.Validate(out var errors);

            Console.WriteLine(string.Join(Environment.NewLine,errors ?? Array.Empty<string>()));
            Assert.AreEqual(expectedValid, valid);
        }

        [DataRow("tank","history -l tank")]
        [TestMethod]
        public void ToStringTest(string poolName, string expected)
        {
            var args = new PoolHistoryArgs { PoolName = poolName };
            var stringVer = args.ToString();

            Assert.AreEqual(expected, stringVer);
        }
    }
}
