using System.Linq;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Core.Commands
{
    public class DataSetCommands : BaseCommands
    {
        public static ProcessCall GetDataSet(string fullName)
        {
            var dataset = DataSetHelper.Decode(fullName);
            return ZfsList(ListTypes.FileSystem, dataset);
        }

        /// <summary>
        /// Creates a dataset with the given properties if any
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static ProcessCall CreateDataSet(string fullName, PropertyValue[] properties)
        {
            fullName = DataSetHelper.Decode(fullName);

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
        /// <param name="dataSetName">The name of the dataset</param>
        /// <param name="destroyFlags">The flags to control how to destroy the dataset <see cref="DataSetDestroyFlags"/></param>
        public static ProcessCall DestroyDataSet(string dataSetName, DataSetDestroyFlags destroyFlags)
        {
            dataSetName = DataSetHelper.Decode(dataSetName);

            var args = string.Empty;
            if (destroyFlags.HasFlag(DataSetDestroyFlags.Recursive))
            {
                args += " -r";
            }

            if (destroyFlags.HasFlag(DataSetDestroyFlags.RecursiveClones))
            {
                args += " -R";
            }

            if (destroyFlags.HasFlag(DataSetDestroyFlags.ForceUmount))
            {
                args += " -f";
            }

            if (destroyFlags.HasFlag(DataSetDestroyFlags.DryRun))
            {
                args += " -nvp";
            }

            return new ProcessCall(WhichZfs, $"destroy{args} {dataSetName}");
        }
        
        /// <summary>
        /// Promotes the dataset or volume from a clone to a real dataset or volume.
        /// see https://openzfs.github.io/openzfs-docs/man/8/zfs-promote.8.html
        /// </summary>
        /// <param name="dataset"></param>
        public static ProcessCall Promote(string dataset)
        {
            dataset = DataSetHelper.Decode(dataset);
            return new ProcessCall(WhichZfs, $"promote {dataset}");
        }
    }
}
