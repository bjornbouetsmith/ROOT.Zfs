using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Public.Arguments;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Public.Arguments
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
                Type = DatasetTypes.Volume,
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
        [DataRow(null, DatasetTypes.Filesystem, false)]
        [DataRow(null, DatasetTypes.Volume, false)]
        [DataRow("", DatasetTypes.Filesystem, false)]
        [DataRow("", DatasetTypes.Volume, false)]
        [DataRow("  ", DatasetTypes.Filesystem, false)]
        [DataRow("  ", DatasetTypes.Volume, false)]
        [DataRow("tank", DatasetTypes.Snapshot, false)]
        [DataRow("tank", DatasetTypes.None, false)]
        [DataRow("tank", DatasetTypes.Bookmark, false)]
        [DataRow("tank", DatasetTypes.Filesystem, true)]
        [DataRow("tank", DatasetTypes.Volume, false)] // Volume requires VolumeCreationArgs
        public void ValidateCommonDatasetArgsTest(string name, DatasetTypes type, bool expectedValid)
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
