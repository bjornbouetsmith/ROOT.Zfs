using System;
using System.Linq;
using System.Text.RegularExpressions;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public.Arguments.Snapshots;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Core.Commands
{
    /// <summary>
    /// Contains all snapshot related commands
    /// </summary>
    internal class SnapshotCommands : Commands
    {
        
        /// <summary>
        /// Creates a standard snapshot name based on the time passed into the method
        /// </summary>
        internal static string CreateSnapshotName(DateTime dateTime) => dateTime.ToString("yyyyMMddHHmmss");

        /// <summary>
        /// Lists snapshots in the given dataset or volume
        /// </summary>
        internal static IProcessCall ListSnapshots(SnapshotListArgs args)
        {
            if (!args.Validate(out var errors))
            {
                throw ToArgumentException(errors, args);
            }

            return ZfsList(args);
        }

        /// <summary>
        /// Destroys the snapshot in the given dataset
        /// Snapshot has to be a child of dataset
        /// </summary>
        internal static ProcessCall DestroySnapshot(SnapshotDestroyArgs args)
        {
            if (!args.Validate(out var errors))
            {
                throw ToArgumentException(errors, args);
            }

            return new ProcessCall(WhichZfs, args.ToString());
        }
        /// <summary>
        /// Creates a snapshot of the dataset with the given name
        /// </summary>
        internal static ProcessCall CreateSnapshot(SnapshotCreateArgs args)
        {
            if (!args.Validate(out var errors))
            {
                throw ToArgumentException(errors, args);
            }

            return new ProcessCall(WhichZfs, args.ToString());
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
            datasetOrVolume = DatasetHelper.Decode(datasetOrVolume);
            targetDatasetOrVolume = DatasetHelper.Decode(targetDatasetOrVolume);
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
        /// <summary>
        /// Returns a command to add a tag to a snapshot, i.e.
        /// zfs hold
        /// </summary>
        internal static ProcessCall Hold(string snapshot, string tag, bool recursive)
        {
            if (string.IsNullOrWhiteSpace(snapshot))
            {
                throw new ArgumentException("Please provide a snapshot name", nameof(snapshot));
            }

            if (string.IsNullOrWhiteSpace(tag))
            {
                throw new ArgumentException("Please provide a tag name", nameof(tag));
            }

            snapshot = DatasetHelper.Decode(snapshot);
            return new ProcessCall(WhichZfs, $"hold{(recursive ? " -r" : "")} {tag} {snapshot}");
        }

        /// <summary>
        /// Returns a command to list holds on the given snapshot and possibly also descendents
        /// </summary>
        internal static ProcessCall Holds(string snapshot, bool recursive)
        {
            if (string.IsNullOrWhiteSpace(snapshot))
            {
                throw new ArgumentException("Please provide a snapshot name", nameof(snapshot));
            }
            snapshot = DatasetHelper.Decode(snapshot);
            return new ProcessCall(WhichZfs, $"holds{(recursive ? " -r" : "")} -H {snapshot}");
        }

        /// <summary>
        /// Returns a command that releases a hold on a snapshot with the given tag and possibly also decendents
        /// </summary>
        internal static ProcessCall Release(string snapshot, string tag, bool recursive)
        {
            if (string.IsNullOrWhiteSpace(snapshot))
            {
                throw new ArgumentException("Please provide a snapshot name", nameof(snapshot));
            }

            if (string.IsNullOrWhiteSpace(tag))
            {
                throw new ArgumentException("Please provide a tag name", nameof(tag));
            }
            snapshot = DatasetHelper.Decode(snapshot);
            return new ProcessCall(WhichZfs, $"release{(recursive ? " -r" : "")} {tag} {snapshot}");
        }
    }
}
