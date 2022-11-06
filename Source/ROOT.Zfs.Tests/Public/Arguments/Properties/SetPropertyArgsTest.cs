using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Arguments;
using ROOT.Zfs.Public.Arguments.Properties;

namespace ROOT.Zfs.Tests.Public.Arguments.Properties
{
    [TestClass]
    public class SetPropertyArgsTest
    {
        [DataRow(PropertyTarget.Pool, "autotrim", "on", "tank", true)]

        [DataRow(PropertyTarget.Pool, null, "on", "tank", false)]
        [DataRow(PropertyTarget.Pool, "", "on", "tank", false)]
        [DataRow(PropertyTarget.Pool, " ", "on", "tank", false)]

        [DataRow(PropertyTarget.Pool, "autotrim", null, "tank", false)]
        [DataRow(PropertyTarget.Pool, "autotrim", "", "tank", false)]
        [DataRow(PropertyTarget.Pool, "autotrim", " ", "tank", false)]

        [DataRow(PropertyTarget.Pool, "autotrim", "on", null, false)]
        [DataRow(PropertyTarget.Pool, "autotrim", "on", "", false)]
        [DataRow(PropertyTarget.Pool, "autotrim", "on", " ", false)]
        [DataRow(PropertyTarget.Pool, null, null, null, false)]

        [TestMethod]
        public void ValidateTest(PropertyTarget propertyTarget, string property, string value, string target, bool expectedValid)
        {
            var args = new SetPropertyArgs { PropertyTarget = propertyTarget, Property = property, Value = value, Target = target };
            var valid = args.Validate(out var errors);
            Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
            Assert.AreEqual(expectedValid, valid);
        }

        [DataRow(PropertyTarget.Pool, "autotrim", "on", "tank", "set autotrim=on tank")]
        [DataRow(PropertyTarget.Pool, "atime", "off", "tank/myds", "set atime=off tank/myds")]
        [TestMethod]
        public void ToStringTest(PropertyTarget propertyTarget, string property, string value, string target, string expected)
        {
            var args = new SetPropertyArgs { PropertyTarget = propertyTarget, Property = property, Value = value, Target = target };
            var stringVer = args.ToString();
            Console.WriteLine(stringVer);
            Assert.AreEqual(expected, stringVer);
        }
    }
}
