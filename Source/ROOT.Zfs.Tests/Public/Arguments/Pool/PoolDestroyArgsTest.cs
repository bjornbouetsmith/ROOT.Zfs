using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Arguments.Pool;

namespace ROOT.Zfs.Tests.Public.Arguments.Pool
{
    [TestClass]
    public class PoolDestroyArgsTest
    {
        [DataRow("tank", "destroy -f tank")]
        [DataRow("tank%5ftest", "destroy -f tank_test")]
        [TestMethod]
        public void ToStringTest(string pool, string expected)
        {
            var args = new PoolDestroyArgs { Name = pool };

            var stringVer = args.ToString();

            Assert.AreEqual(expected, stringVer);
        }
    }
}
