using System.Linq;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Data;

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
        /// <param name="dataset"></param>
        internal static ProcessCall Promote(string dataset)
        {
            dataset = DatasetHelper.Decode(dataset);
            return new ProcessCall(WhichZfs, $"promote {dataset}");
        }
    }
}
