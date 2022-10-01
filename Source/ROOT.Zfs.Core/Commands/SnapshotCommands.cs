using System;
using System.Text.RegularExpressions;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Helpers;

namespace ROOT.Zfs.Core.Commands
{
    /// <summary>
    /// Contains all snapshot related commands
    /// </summary>
    public class SnapshotCommands : BaseCommands
    {
        private static readonly Regex NameAllow = new Regex("[0-9]|[a-z]|[A-Z]|_|-");

        /// <summary>
        /// Lists snapshots in the given dataset or volume
        /// </summary>
        /// <param name="datasetOrVolume"></param>
        /// <returns></returns>
        public static ProcessCall ListSnapshots(string datasetOrVolume)
        {
            return ZfsList(ListTypes.Snapshot, datasetOrVolume);
        }

        /// <summary>
        /// Destroys the snapshot in the given dataset
        /// Snapshot has to be a child of dataset
        /// </summary>
        /// <param name="dataSetOrVolume">The dataset or volume where the snapshot resides in</param>
        /// <param name="snapName">Name of the snapshot - can be in the form: dataset@snapshot or just snapshot</param>
        /// <returns></returns>
        public static ProcessCall DestroySnapshot(string dataSetOrVolume, string snapName)
        {
            dataSetOrVolume = DataSetHelper.Decode(dataSetOrVolume);
            var rawSnapName = snapName;
            if (snapName.StartsWith(dataSetOrVolume, StringComparison.OrdinalIgnoreCase))
            {
                rawSnapName = snapName[(dataSetOrVolume.Length + 1)..];
            }

            return new ProcessCall(WhichZfs, $"destroy {dataSetOrVolume}@{rawSnapName}");
        }

        public static ProcessCall CreateSnapshot(string datasetOrVolume, string snapName)
        {
            datasetOrVolume = DataSetHelper.Decode(datasetOrVolume);
            if (NameAllow.Matches(snapName).Count != snapName.Length)
            {
                throw new ArgumentException($"{snapName} is not a valid snapshot name - valid characters are [0-9]|[a-z]|[A-Z]|_|-", nameof(snapName));
            }

            return new ProcessCall(WhichZfs, $"snap {datasetOrVolume}@{snapName}");
        }

        public static string CreateSnapshotName(DateTime dateTime) => dateTime.ToString("yyyyMMddHHmmss");

        
        /// <summary>
        /// Creates a snapshot of the dataset using the format: yyyyMMddHHmmss
        /// </summary>
        /// <param name="datasetOrVolume"></param>
        /// <returns></returns>
        public static ProcessCall CreateSnapshot(string datasetOrVolume)
        {
            datasetOrVolume = DataSetHelper.Decode(datasetOrVolume);
            return CreateSnapshot(datasetOrVolume, CreateSnapshotName(DateTime.UtcNow));
        }
    }
}
