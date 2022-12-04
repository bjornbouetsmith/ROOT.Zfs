using System;
using System.Linq;
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
        /// Returns a command to list snapshots in the given dataset or volume
        /// </summary>
        /// <exception cref="ArgumentException">If arguments are not valid</exception>
        internal static IProcessCall ListSnapshots(SnapshotListArgs args)
        {
            if (!args.Validate(out var errors))
            {
                throw ToArgumentException(errors, args);
            }

            return ZfsList(args);
        }

        /// <summary>
        /// Returns a command to destroy the snapshot in the given dataset
        /// Snapshot has to be a child of dataset
        /// </summary>
        /// <exception cref="ArgumentException">If arguments are not valid</exception>
        internal static ProcessCall DestroySnapshot(SnapshotDestroyArgs args)
        {
            if (!args.Validate(out var errors))
            {
                throw ToArgumentException(errors, args);
            }

            return new ProcessCall(WhichZfs, args.ToString());
        }

        /// <summary>
        /// Returns a command to create a snapshot of the dataset with the given name
        /// </summary>
        /// <exception cref="ArgumentException">If arguments are not valid</exception>
        internal static ProcessCall CreateSnapshot(SnapshotCreateArgs args)
        {
            if (!args.Validate(out var errors))
            {
                throw ToArgumentException(errors, args);
            }

            return new ProcessCall(WhichZfs, args.ToString());
        }

        /// <summary>
        /// Returns a command to clone a snapshot into the given target dataset
        /// </summary>
        /// <exception cref="ArgumentException">If arguments are not valid</exception>
        internal static IProcessCall Clone(SnapshotCloneArgs args)
        {
            if (!args.Validate(out var errors))
            {
                throw ToArgumentException(errors, args);
            }

            return new ProcessCall(WhichZfs, args.ToString());
        }

        /// <summary>
        /// Returns a command to add a tag to a snapshot, i.e.
        /// zfs hold
        /// </summary>
        internal static ProcessCall Hold(SnapshotHoldArgs args)
        {
            if (!args.Validate(out var errors))
            {
                throw ToArgumentException(errors, args);
            }

            return new ProcessCall(WhichZfs, args.ToString());
        }

        /// <summary>
        /// Returns a command to list holds on the given snapshot and possibly also descendents
        /// </summary>
        internal static ProcessCall Holds(SnapshotHoldsArgs args)
        {
            if (!args.Validate(out var errors))
            {
                throw ToArgumentException(errors, args);
            }

            return new ProcessCall(WhichZfs, args.ToString());
        }

        /// <summary>
        /// Returns a command that releases a hold on a snapshot with the given tag and possibly also decendents
        /// </summary>
        internal static ProcessCall Release(SnapshotReleaseArgs args)
        {
            if (!args.Validate(out var errors))
            {
                throw ToArgumentException(errors, args);
            }

            return new ProcessCall(WhichZfs, args.ToString());
        }
    }
}
