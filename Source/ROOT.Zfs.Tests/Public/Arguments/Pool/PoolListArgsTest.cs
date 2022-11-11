using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Arguments.Pool;

namespace ROOT.Zfs.Tests.Public.Arguments.Pool
{
    [TestClass]
    public class PoolListArgsTest
    {
        [DataRow("tank","list -PHp tank")]
        [TestMethod]
        public void ToStringMethod(string name, string expected)
        {
            var args = new PoolListArgs { Name = name };

            var stringVer = args.ToString();
            Assert.AreEqual(expected, stringVer);
        }
    }
}
