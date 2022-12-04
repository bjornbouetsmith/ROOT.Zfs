using System;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Public.Arguments.Dataset;

namespace ROOT.Zfs.Core.Commands
{
    /// <summary>
    /// Contains all command related to datasets.
    /// </summary>
    internal class DatasetCommands : Commands
    {
        /// <summary>
        /// returns a command to create a dataset
        /// </summary>
        /// <exception cref="ArgumentException">If arguments are not valid or missing</exception>
        internal static IProcessCall Create(DatasetCreationArgs arguments)
        {
            if (!arguments.Validate(out var errors))
            {
                throw ToArgumentException(errors, arguments);
            }
            
            return new ProcessCall(WhichZfs, arguments.ToString());
        }

        /// <summary>
        /// Returns a command to destroy a dataset
        /// </summary>
        /// <exception cref="ArgumentException">If arguments are not valid or missing</exception>
        internal static ProcessCall Destroy(DatasetDestroyArgs args)
        {
            if (!args.Validate(out var errors))
            {
                throw ToArgumentException(errors, args);
            }
            return new ProcessCall(WhichZfs, args.ToString());
        }

        /// <summary>
        /// Promotes the dataset or volume from a clone to a real dataset or volume.
        /// see https://openzfs.github.io/openzfs-docs/man/8/zfs-promote.8.html
        /// </summary>
        internal static ProcessCall Promote(PromoteArgs args)
        {
            if (!args.Validate(out var errors))
            {
                throw ToArgumentException(errors, args);
            }
            return new ProcessCall(WhichZfs, args.ToString());
        }

        /// <summary>
        /// Returns a command to mount a dataset
        /// </summary>
        /// <param name="mountArgs">the arguments which contains the options for mounting</param>
        /// <exception cref="ArgumentException">If arguments are not valid</exception>
        internal static ProcessCall Mount(MountArgs mountArgs)
        {
            if (!mountArgs.Validate(out var errors))
            {
                throw ToArgumentException(errors, mountArgs);
            }

            return new ProcessCall(WhichZfs, mountArgs.ToString());
        }

        /// <summary>
        /// Returns a command to unmount a filesystem or mount point
        /// </summary>
        /// <exception cref="ArgumentException">If arguments are not valid</exception>
        internal static ProcessCall Unmount(UnmountArgs unmountArgs)
        {
            if (!unmountArgs.Validate(out var errors))
            {
                throw ToArgumentException(errors, unmountArgs);
            }

            return new ProcessCall(WhichZfs, unmountArgs.ToString());
        }
    }
}
