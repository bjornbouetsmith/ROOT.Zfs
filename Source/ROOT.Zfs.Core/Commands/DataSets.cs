using System.Linq;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Info;

namespace ROOT.Zfs.Core.Commands
{
    public static class DataSets
    {
        public static class ProcessCalls
        {
            public static ProcessCall GetDataSets()
            {
                return new ProcessCall("/sbin/zfs", "list");
            }

            public static ProcessCall GetDataSet(string fullName)
            {
                var dataset = DataSetHelper.Decode(fullName);
                return new ProcessCall("/sbin/zfs", $"list {dataset}");
            }

            public static ProcessCall CreateDataSet(string fullName, PropertyValue[] properties = null)
            {
                var parent = DataSetHelper.Decode(fullName);

                var propCommand = properties != null ? string.Join(' ', properties.Select(p => $"-o {p.Property.Name}={p.Value}")) : string.Empty;

                return new ProcessCall("/sbin/zfs", $"create {propCommand} {parent}");
            }

            public static ProcessCall DestroyDataSet(string dataSetName)
            {
                var dataset = DataSetHelper.Decode(dataSetName);

                return new ProcessCall("/sbin/zfs", $"destroy {dataset}");
            }
        }
    }
}
