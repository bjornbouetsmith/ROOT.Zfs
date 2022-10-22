using System;
using System.Linq;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Arguments;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.Datasets;

namespace ROOT.Zfs.Core.Commands
{
    internal class DatasetCommands : BaseCommands
    {
        /// <summary>
        /// Creates a dataset with the given properties if any
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        internal static ProcessCall CreateDataset(string fullName, PropertyValue[] properties)
        {
            fullName = DatasetHelper.Decode(fullName);

            var propCommand = properties != null ? string.Join(' ', properties.Select(p => $"-o {p.Property}={p.Value}")) : string.Empty;
            if (propCommand != string.Empty)
            {
                propCommand = " " + propCommand;
            }

            return new ProcessCall(WhichZfs, $"create{propCommand} {fullName}");
        }

        internal static ProcessCall CreateDataset(DatasetCreationArgs arguments)
        {
            arguments.DataSetName = DatasetHelper.Decode(arguments.DataSetName);

            string args = string.Empty;
            if (arguments.Type == DatasetTypes.Volume)
            {
                args = $" -b {arguments.VolumeArguments.BlockSize} -V {arguments.VolumeArguments.VolumeSize}";
                if (arguments.VolumeArguments.Sparse)
                {
                    args += " -s";
                }
            }

            if (arguments.CreateParents)
            {
                args += " -p";
            }

            if (arguments.DoNotMount && arguments.Type == DatasetTypes.Filesystem)
            {
                args += " -u";
            }

            var propCommand = arguments.Properties != null ? string.Join(' ', arguments.Properties.Select(p => $"-o {p.Property}={p.Value}")) : string.Empty;
            if (propCommand != string.Empty)
            {
                args += " " + propCommand;
            }


            return new ProcessCall(WhichZfs, $"create{args} {arguments.DataSetName}");
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

            return new ProcessCall(WhichZfs, $"mount{mountArgs}");

        }
    }
}
