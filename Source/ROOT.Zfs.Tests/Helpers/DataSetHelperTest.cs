using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Helpers
{
    [TestClass]
    public class DataSetHelperTest
    {

        [TestMethod]
        [DataRow("tank", "myds", "tank/myds")]
        [DataRow("tank/myds", "myds2", "tank/myds/myds2")]
        [DataRow("tank%2fmyds", "myds2", "tank/myds/myds2")]
        public void CreateDataSetNameTest(string parent, string dataset, string expected)
        {
            var name = DatasetHelper.CreateDatasetName(parent, dataset);
            Assert.AreEqual(expected, name);
        }

        [TestMethod]
        public void ParseDataSetTestNotAClone()
        {
            var stdOut = @"filesystem      1652262393      tank/kubedata   708100096       396648448       3566981726208   /tank/kubedata   -";

            var dataset = DatasetHelper.ParseLine(stdOut);
            Console.WriteLine(dataset.Dump(new JsonFormatter()));
            Assert.AreEqual("tank/kubedata", dataset.DatasetName);
            Assert.AreEqual("675.3M", dataset.Used.ToString());
            Assert.AreEqual("378.3M", dataset.Available.ToString());
            Assert.AreEqual("3.2T", dataset.Refer.ToString());
            Assert.AreEqual("/tank/kubedata", dataset.Mountpoint);
            Assert.IsFalse(dataset.IsClone);
        }

        [TestMethod]
        public void ParseDataSetTestAClone()
        {
            var stdOut = @"filesystem      1652262393      tank/kubedata   708100096       396648448       3566981726208   /tank/kubedata   tank/kubedata@1234";

            var dataset = DatasetHelper.ParseLine(stdOut);
            Console.WriteLine(dataset.Dump(new JsonFormatter()));
            Assert.AreEqual("tank/kubedata", dataset.DatasetName);
            Assert.AreEqual("675.3M", dataset.Used.ToString());
            Assert.AreEqual("378.3M", dataset.Available.ToString());
            Assert.AreEqual("3.2T", dataset.Refer.ToString());
            Assert.AreEqual("/tank/kubedata", dataset.Mountpoint);
            Assert.IsTrue(dataset.IsClone);
        }

        [TestMethod]
        public void ParseTruncatedFormatShouldThrow()
        {
            var stdOut = @"filesystem      1652262393      tank/kubedata   708100096       396648448";

            Assert.ThrowsException<FormatException>(() => DatasetHelper.ParseLine(stdOut));
        }

        [TestMethod]
        public void ParseUnknownDatasetTypeShouldNotThrow()
        {
            var stdOut = @"link      1652262393      tank/kubedata   708100096       396648448       3566981726208   /tank/kubedata   -";

            var dataset = DatasetHelper.ParseLine(stdOut);

            Assert.AreEqual(DatasetTypes.None, dataset.Type);
        }
    }
}
