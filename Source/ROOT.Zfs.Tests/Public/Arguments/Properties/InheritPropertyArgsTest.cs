using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Arguments;

namespace ROOT.Zfs.Tests.Public.Arguments.Properties
{
    [TestClass]
    public class InheritPropertyArgsTest
    {
        [DataRow(PropertyTarget.Dataset, "atime", "tank", true)]
        [DataRow(PropertyTarget.Dataset, "atime", null, false)]
        [DataRow(PropertyTarget.Dataset, "atime", "", false)]
        [DataRow(PropertyTarget.Dataset, "atime", " ", false)]

        [DataRow(PropertyTarget.Dataset, null, "tank/myds", false)]
        [DataRow(PropertyTarget.Dataset, "", "tank/myds", false)]
        [DataRow(PropertyTarget.Dataset, " ", "tank/myds", false)]

        [DataRow(PropertyTarget.Dataset, null, null, false)]

        [DataRow(PropertyTarget.Pool, "atime", "tank/myds", false)]
        [TestMethod]
        public void ValidateTest(PropertyTarget propertyTarget, string property, string target, bool expectValid)
        {
            var args = new InheritPropertyArgs { PropertyTarget = propertyTarget, Property = property, Target = target };

            var valid = args.Validate(out var errors);
            Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
            Assert.AreEqual(expectValid, valid);
        }

        [DataRow(PropertyTarget.Dataset, "atime", "tank", "inherit -rS atime tank")]
        [TestMethod]
        public void ToStringTest(PropertyTarget propertyTarget, string property, string target, string expected)
        {
            var args = new InheritPropertyArgs { PropertyTarget = propertyTarget, Property = property, Target = target };
            var stringVer = args.ToString();
            Console.WriteLine(stringVer);
            Assert.AreEqual(expected, stringVer);
        }
    }
}
