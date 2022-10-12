using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Core.Commands
{
    /// <summary>
    /// Contains all the commands that relates to properties
    /// </summary>
    internal class PropertyCommands : BaseCommands
    {
        internal static ProcessCall GetProperties(string dataset)
        {
            dataset = DataSetHelper.Decode(dataset);
            return new ProcessCall(WhichZfs, $"get all {dataset} -H");
        }

        internal static ProcessCall GetDataSetProperties()
        {
            return new ProcessCall(WhichZfs, "get -H");
        }

        internal static ProcessCall GetPoolProperties()
        {
            return new ProcessCall(WhichZpool, "get -H");
        }

        internal static ProcessCall ResetPropertyToInherited(string dataset, Property property)
        {
            dataset = DataSetHelper.Decode(dataset);
            return new ProcessCall(WhichZfs, $"inherit -rS {property.Name} {dataset}");
        }

        internal static ProcessCall SetProperty(string dataset, Property property, string value)
        {
            dataset = DataSetHelper.Decode(dataset);
            return new ProcessCall(WhichZfs, $"set {property.Name}={value} {dataset}");
        }

        internal static ProcessCall GetProperty(string dataset, Property property)
        {
            dataset = DataSetHelper.Decode(dataset);
            return new ProcessCall(WhichZfs, $"get {property.Name} {dataset} -H");
        }
    }
}
