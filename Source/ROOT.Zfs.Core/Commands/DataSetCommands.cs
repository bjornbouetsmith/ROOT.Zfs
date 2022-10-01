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

        public static ProcessCall CreateDataSet(string fullName, PropertyValue[] properties)
        {
            var parent = DataSetHelper.Decode(fullName);

            var propCommand = properties != null ? string.Join(' ', properties.Select(p => $"-o {p.Property}={p.Value}")) : string.Empty;

            return new ProcessCall(WhichZfs, $"create {propCommand} {parent}");
        }

        public static ProcessCall DestroyDataSet(string dataSetName, DataSetDestroyFlags destroyFlags)
        {
            var dataset = DataSetHelper.Decode(dataSetName);

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

            return new ProcessCall(WhichZfs, $"destroy{args} {dataset}");
        }
    }
}
