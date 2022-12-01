using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Arguments.Pool;
using ROOT.Zfs.Public.Data.Pools;
using System;

namespace ROOT.Zfs.Tests.Public.Arguments.Pool
{
    [TestClass]
    public class PoolScrubArgsTest
    {
        [DataRow("tank", true)]
        [DataRow(null, false)]
        [DataRow("", false)]
        [DataRow(" ", false)]
        [DataRow("rm -rf /tank", false)]
        [TestMethod]
        public void ValidateTest(string pool, bool expectedValid)
        {
            var args = new PoolScrubArgs { Name = pool };
            var valid = args.Validate(out var errors);
            Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
            Assert.AreEqual(expectedValid, valid);
        }

        [DataRow("tank", ScrubOption.None, "scrub tank")]
        [DataRow("tank", ScrubOption.Stop, "scrub -s tank")]
        [DataRow("tank", ScrubOption.Pause, "scrub -p tank")]
        [TestMethod]
        public void ToStringTest(string pool, ScrubOption option, string expected)
        {
            var args = new PoolScrubArgs { Name = pool, Options = option };
            var stringVer = args.ToString();
            Assert.AreEqual(expected, stringVer);
        }
    }
}
