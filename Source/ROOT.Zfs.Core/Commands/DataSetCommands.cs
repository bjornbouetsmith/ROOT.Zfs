using System;
using System.Linq;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public.Arguments;

namespace ROOT.Zfs.Core.Commands
{
    internal class DatasetCommands : Commands
    {
        /// <summary>
        /// returns a command to create a dataset
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        internal static IProcessCall CreateDataset(DatasetCreationArgs arguments)
        {
            if (!arguments.Validate(out var errors))
            {
                throw new ArgumentException(string.Join(Environment.NewLine, errors), nameof(arguments));
            }

            arguments.DataSetName = DatasetHelper.Decode(arguments.DataSetName);
            
            return new ProcessCall(WhichZfs, arguments.ToString());
        }

        /// <summary>
        /// Destroys a dataset
        /// </summary>
        /// <param name="datasetName">The name of the dataset</param>
        /// <param name="destroyFlags">The flags to control how to destroy the dataset <see cref="DatasetDestroyFlags"/></param>
        internal static ProcessCall DestroyDataset(string datasetName, DatasetDestroyFlags destroyFlags)
        {
            datasetName = DatasetHelper.Decode(datasetName);

            var args = string.Empty;
            if (destroyFlags.HasFlag(DatasetDestroyFlags.Recursive))
            {
                args += " -r";
            }

            if (destroyFlags.HasFlag(DatasetDestroyFlags.RecursiveClones))
            {
                args += " -R";
            }

            if (destroyFlags.HasFlag(DatasetDestroyFlags.ForceUmount))
            {
                args += " -f";
            }

            if (destroyFlags.HasFlag(DatasetDestroyFlags.DryRun))
            {
                args += " -nvp";
            }

            return new ProcessCall(WhichZfs, $"destroy{args} {datasetName}");
        }

        /// <summary>
        /// Promotes the dataset or volume from a clone to a real dataset or volume.
        /// see https://openzfs.github.io/openzfs-docs/man/8/zfs-promote.8.html
        /// </summary>
        internal static ProcessCall Promote(string dataset)
        {
            dataset = DatasetHelper.Decode(dataset);
            return new ProcessCall(WhichZfs, $"promote {dataset}");
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
                throw new ArgumentException(string.Join(Environment.NewLine, errors), nameof(mountArgs));
            }

            return new ProcessCall(WhichZfs, mountArgs.ToString());
        }
        /// <summary>
        /// Returns a command to unmount a filesystem or mount point
        /// </summary>
        /// <param name="unmountArgs">The arguments</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If arguments are not valid</exception>
        internal static ProcessCall Unmount(UnmountArgs unmountArgs)
        {
            if (!unmountArgs.Validate(out var errors))
            {
                throw new ArgumentException(string.Join(Environment.NewLine, errors), nameof(unmountArgs));
            }

            return new ProcessCall(WhichZfs, unmountArgs.ToString());
        }
    }
}
