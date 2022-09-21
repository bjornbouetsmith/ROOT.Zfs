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

            public static ProcessCall CreateDataSet(string parentDataSet, string dataSetName)
            {
                var parent = DataSetHelper.Decode(parentDataSet);
                var dataset = DataSetHelper.Decode(dataSetName);
                var fullName = DataSetHelper.CreateDataSetName(parent, dataset);

                return new ProcessCall("/sbin/zfs", $"create {fullName}");
            }

            public static ProcessCall DestroyDataSet(string dataSetName)
            {
                var dataset = DataSetHelper.Decode(dataSetName);

                return new ProcessCall("/sbin/zfs", $"destroy {dataset}");
            }
        }
    }
}
