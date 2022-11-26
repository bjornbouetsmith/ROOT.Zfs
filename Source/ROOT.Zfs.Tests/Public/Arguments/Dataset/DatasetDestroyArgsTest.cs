using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Arguments.Dataset;

namespace ROOT.Zfs.Tests.Public.Arguments.Dataset
{
    [TestClass]
    public class DatasetDestroyArgsTest
    {
        [DataRow(DatasetDestroyFlags.None, "destroy tank/myds")]
        [DataRow(DatasetDestroyFlags.Recursive, "destroy -r tank/myds")]
        [DataRow(DatasetDestroyFlags.RecursiveClones, "destroy -R tank/myds")]
        [DataRow(DatasetDestroyFlags.ForceUmount, "destroy -f tank/myds")]
        [DataRow(DatasetDestroyFlags.DryRun, "destroy -nvp tank/myds")]
        [DataRow((DatasetDestroyFlags)(-1), "destroy -r -R -f -nvp tank/myds")]
        [TestMethod]
        public void ToStringTest(DatasetDestroyFlags flags, string expected)
        {
            var args = new DatasetDestroyArgs { Dataset = "tank/myds", DestroyFlags = flags };
            var stringVer = args.ToString();
            Assert.AreEqual(expected, stringVer);
        }

        [DataRow(null, false)]
        [DataRow("", false)]
        [DataRow(" ", false)]
        [DataRow("tank/myds@snap123", false)]
        [DataRow("tank/myds", true)]
        [TestMethod]
        public void ValidateTest(string dataset, bool expectValid)
        {
            var args = new DatasetDestroyArgs
            {
                Dataset = dataset
            };

            var valid = args.Validate(out var errors);
            Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
            Assert.AreEqual(expectValid, valid);
        }
    }
}