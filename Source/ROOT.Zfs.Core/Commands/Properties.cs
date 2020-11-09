using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Info;

namespace ROOT.Zfs.Core.Commands
{
    public class Properties
    {
        public static class ProcessCalls
        {
            public static ProcessCall ListProperties(string dataset)
            {
                dataset = DataSetHelper.Decode(dataset);
                return new ProcessCall("/sbin/zfs", $"get all {dataset} -H");
            }

            public static ProcessCall GetSettableProperties()
            {
                return new ProcessCall("/sbin/zfs", "get");
            }

            public static ProcessCall ResetPropertyToInherited(string dataset, Property property)
            {
                dataset = DataSetHelper.Decode(dataset);
                return new ProcessCall("/sbin/zfs", $"inherit -sR {property.Name} {dataset}");
            }

            public static ProcessCall SetProperty(string dataset, Property property, string value)
            {
                dataset = DataSetHelper.Decode(dataset);
                return new ProcessCall("/sbin/zfs", $"set {property.Name}={value} {dataset}");
            }

            public static ProcessCall GetProperty(string dataset, Property property)
            {
                dataset = DataSetHelper.Decode(dataset);
                return new ProcessCall("/sbin/zfs", $"get {property.Name} {dataset} -H");
            }
        }
    }
}
