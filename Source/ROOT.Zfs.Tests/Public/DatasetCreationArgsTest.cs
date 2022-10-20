using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.Datasets;

namespace ROOT.Zfs.Tests.Public
{
    [TestClass]
    public class DatasetCreationArgsTest
    {
        [TestMethod]
        [DataRow("2M", "100G", false)]
        [DataRow("1.5K", "100G", false)]
        [DataRow("0K", "100G", false)]
        [DataRow("512K", "0G", false)]
        [DataRow("512K", "1G", true)]
        [DataRow("0.5K", "1G", true)]
        [DataRow("1M", "1G", true)]
        public void ValidateVolumeCreationTest(string blockSize, string volumeSize, bool expectedValid)
        {
            var args = new DatasetCreationArgs
            {
                DataSetName = "tank/myvol",
                Type = DatasetType.Volume,
                VolumeArguments = new VolumeCreationArgs
                {
                    BlockSize = blockSize,
                    VolumeSize = volumeSize,
                }
            };

            Console.WriteLine(args.Dump(new JsonFormatter()));
            var valid = args.Validate(out var errors);
            Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
            Assert.AreEqual(expectedValid, valid);
        }

        [TestMethod]
        [DataRow(null, DatasetType.Filesystem, false)]
        [DataRow(null, DatasetType.Volume, false)]
        [DataRow("", DatasetType.Filesystem, false)]
        [DataRow("", DatasetType.Volume, false)]
        [DataRow("  ", DatasetType.Filesystem, false)]
        [DataRow("  ", DatasetType.Volume, false)]
        [DataRow("tank", DatasetType.Snapshot, false)]
        [DataRow("tank", DatasetType.NotSet, false)]
        [DataRow("tank", DatasetType.Bookmark, false)]
        [DataRow("tank", DatasetType.Filesystem, true)]
        [DataRow("tank", DatasetType.Volume, false)] // Volume requires VolumeCreationArgs
        public void ValidateCommonDatasetArgsTest(string name, DatasetType type, bool expectedValid)
        {
            var args = new DatasetCreationArgs
            {
                DataSetName = name,
                Type = type,
            };
            Console.WriteLine(args.Dump(new JsonFormatter()));
            var valid = args.Validate(out var errors);
            Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
            Assert.AreEqual(expectedValid, valid);
        }
    }
}
