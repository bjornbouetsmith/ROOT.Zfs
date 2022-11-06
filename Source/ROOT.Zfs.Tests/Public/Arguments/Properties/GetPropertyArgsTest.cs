using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Arguments;

namespace ROOT.Zfs.Tests.Public.Arguments.Properties
{
    [TestClass]
    public class GetPropertyArgsTest
    {
        [DataRow(PropertyTarget.Pool, null, null, true)]
        [DataRow(PropertyTarget.Pool, "ashift", null, true)]
        [DataRow(PropertyTarget.Pool, "ashift", "tank", true)]
        [DataRow(PropertyTarget.Pool, null, "tank", true)]
        [DataRow(PropertyTarget.Dataset, null, null, true)]
        [DataRow(PropertyTarget.Dataset, "atime", null, true)]
        [DataRow(PropertyTarget.Dataset, "atime", "tank/myds", true)]
        [DataRow(PropertyTarget.Dataset, null, "tank/myds", true)]
        [TestMethod]
        public void ValidateTest(PropertyTarget propertyTarget, string property, string target, bool expectedValid)
        {
            var args = new GetPropertyArgs
            {
                PropertyTarget = propertyTarget,
                Property = property,
                Target = target,
            };

            var valid = args.Validate(out var errors);
            Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));

            Assert.AreEqual(expectedValid, valid);
        }

        [DataRow(PropertyTarget.Pool, null, null, "get -H")]
        [DataRow(PropertyTarget.Pool, "ashift", null, "get ashift -H")]
        [DataRow(PropertyTarget.Pool, "ashift", "tank", "get ashift tank -H")]
        [DataRow(PropertyTarget.Pool, null, "tank", "get all tank -H")]
        [DataRow(PropertyTarget.Dataset, null, null, "get -H")]
        [DataRow(PropertyTarget.Dataset, "atime", null, "get atime -H")]
        [DataRow(PropertyTarget.Dataset, "atime", "tank/myds", "get atime tank/myds -H")]
        [DataRow(PropertyTarget.Dataset, null, "tank/myds", "get all tank/myds -H")]
        [TestMethod]
        public void ToStringTest(PropertyTarget propertyTarget, string property, string target, string expected)
        {
            var args = new GetPropertyArgs
            {
                PropertyTarget = propertyTarget,
                Property = property,
                Target = target,
            };
            var stringVer = args.ToString();
            Console.WriteLine(stringVer);
            Assert.AreEqual(expected, stringVer);
        }
    }
}
