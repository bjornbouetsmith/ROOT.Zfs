using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Arguments.Dataset;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Public.Arguments.Dataset
{
    [TestClass]
    public class DatasetListArgsTest
    {
        [DataRow("tank && rm -rf /", false)]
        [DataRow("tank;rm -rf /", false)]
        [DataRow("tank/myds", true)]
        [DataRow("tank%2fmyds", true)]
        [TestMethod]
        public void ValidateTest(string root, bool expectValid)
        {
            var args = new DatasetListArgs { Root = root };
            var valid = args.Validate(out var errors);
            Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
            Assert.AreEqual(expectValid, valid);
        }


        [DataRow("tank", default, false, "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t filesystem,volume tank")]
        [DataRow("tank", DatasetTypes.Filesystem, false, "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t filesystem tank")]
        [DataRow("tank", default, true, "list -Hpr -o type,creation,name,used,refer,avail,mountpoint,origin -d 99 -t filesystem,volume tank")]
        [DataRow(null, default, false, "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t filesystem,volume")]
        [DataRow(null, default, true, "list -Hp -o type,creation,name,used,refer,avail,mountpoint,origin -t filesystem,volume")] // include children have no effect when a root is not specified
        [TestMethod]
        public void ToStringTest(string root, DatasetTypes types, bool includeChildren, string expected)
        {
            var args = new DatasetListArgs
            {
                Root = root,
                DatasetTypes = types,
                IncludeChildren = includeChildren,
            };

            var stringVer = args.ToString();

            Assert.AreEqual(expected, stringVer);
        }
    }
}
