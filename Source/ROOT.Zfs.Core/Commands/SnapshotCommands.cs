using System;
using System.Linq;
using System.Text.RegularExpressions;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Core.Commands
{
    /// <summary>
    /// Contains all snapshot related commands
    /// </summary>
    internal class SnapshotCommands : BaseCommands
    {
        private static readonly Regex NameAllow = new Regex("[0-9]|[a-z]|[A-Z]|_|-",RegexOptions.Compiled);

        /// <summary>
        /// Creates a standard snapshot name based on the time passed into the method
        /// </summary>
        internal static string CreateSnapshotName(DateTime dateTime) => dateTime.ToString("yyyyMMddHHmmss");

        /// <summary>
        /// Lists snapshots in the given dataset or volume
        /// </summary>
        /// <param name="datasetOrVolume"></param>
        internal static ProcessCall ListSnapshots(string datasetOrVolume)
        {
            return ZfsList(ListTypes.Snapshot, datasetOrVolume);
        }

        /// <summary>
        /// Destroys the snapshot in the given dataset
        /// Snapshot has to be a child of dataset
        /// </summary>
        /// <param name="datasetOrVolume">The dataset or volume where the snapshot resides in</param>
        /// <param name="snapshotName">Name of the snapshot - can be in the form: dataset@snapshot or just snapshot</param>
        internal static ProcessCall DestroySnapshot(string datasetOrVolume, string snapshotName)
        {
            datasetOrVolume = DataSetHelper.Decode(datasetOrVolume);
            var rawSnapName = snapshotName;
            if (snapshotName.StartsWith(datasetOrVolume, StringComparison.OrdinalIgnoreCase))
            {
                rawSnapName = snapshotName[(datasetOrVolume.Length + 1)..];
            }

            return new ProcessCall(WhichZfs, $"destroy {datasetOrVolume}@{rawSnapName}");
        }
        /// <summary>
        /// Creates a snapshot of the dataset with the given name
        /// </summary>
        /// <param name="datasetOrVolume">The dataset or volume where the snapshot resides in</param>
        /// <param name="snapshotName">Name of the snapshot - can be in the form: dataset@snapshot or just snapshot</param>
        internal static ProcessCall CreateSnapshot(string datasetOrVolume, string snapshotName)
        {
            datasetOrVolume = DataSetHelper.Decode(datasetOrVolume);
            if (NameAllow.Matches(snapshotName).Count != snapshotName.Length)
            {
                throw new ArgumentException($"{snapshotName} is not a valid snapshot name - valid characters are [0-9]|[a-z]|[A-Z]|_|-", nameof(snapshotName));
            }

            return new ProcessCall(WhichZfs, $"snap {datasetOrVolume}@{snapshotName}");
        }

        /// <summary>
        /// Creates a snapshot of the dataset using the format: yyyyMMddHHmmss
        /// </summary>
        internal static ProcessCall CreateSnapshot(string datasetOrVolume)
        {
            datasetOrVolume = DataSetHelper.Decode(datasetOrVolume);
            return CreateSnapshot(datasetOrVolume, CreateSnapshotName(DateTime.UtcNow));
        }

        /// <summary>
        /// Creates a clone of the given snapshot targeting the dataset or volume
        /// see https://openzfs.github.io/openzfs-docs/man/8/zfs-clone.8.html
        /// </summary>
        /// <param name="datasetOrVolume">The dataset or volume where the snapshot is from</param>
        /// <param name="snapshotName">The name of the snapshot to clone</param>
        /// <param name="targetDatasetOrVolume">The name of the dataset or volume where to clone to</param>
        /// <param name="properties">The properties to set on the target - if any</param>
        internal static ProcessCall Clone(string datasetOrVolume, string snapshotName, string targetDatasetOrVolume, PropertyValue[] properties)
        {
            datasetOrVolume = DataSetHelper.Decode(datasetOrVolume);
            targetDatasetOrVolume = DataSetHelper.Decode(targetDatasetOrVolume);
            var rawSnapName = snapshotName;
            var propCommand = properties != null ? string.Join(' ', properties.Select(p => $"-o {p.Property}={p.Value}")) : string.Empty;
            if (propCommand != string.Empty)
            {
                propCommand = " " + propCommand;
            }

            if (snapshotName.StartsWith(datasetOrVolume, StringComparison.OrdinalIgnoreCase))
            {
                rawSnapName = snapshotName[(datasetOrVolume.Length + 1)..];
            }

            return new ProcessCall(WhichZfs, $"clone -p{propCommand} {datasetOrVolume}@{rawSnapName} {targetDatasetOrVolume}");
        }
    }
}
