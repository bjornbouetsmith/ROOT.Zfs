using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public.Arguments;

namespace ROOT.Zfs.Core.Commands
{
    /// <summary>
    /// Contains all the commands that relates to properties
    /// </summary>
    internal class PropertyCommands : BaseCommands
    {
        internal static ProcessCall GetProperties(PropertyTarget targetType, string target)
        {
            target = DatasetHelper.Decode(target);
            var binary = targetType == PropertyTarget.Pool ? WhichZpool : WhichZfs;
            return new ProcessCall(binary, $"get all {target} -H");
        }

        internal static ProcessCall GetDatasetProperties()
        {
            return new ProcessCall(WhichZfs, "get -H");
        }

        internal static ProcessCall GetPoolProperties()
        {
            return new ProcessCall(WhichZpool, "get -H");
        }

        internal static ProcessCall ResetPropertyToInherited(string dataset, string property)
        {
            dataset = DatasetHelper.Decode(dataset);
            return new ProcessCall(WhichZfs, $"inherit -rS {property} {dataset}");
        }

        internal static ProcessCall SetProperty(PropertyTarget targetType, string target, string name, string value)
        {
            var binary = targetType == PropertyTarget.Pool ? WhichZpool : WhichZfs;
            target = DatasetHelper.Decode(target);
            return new ProcessCall(binary, $"set {name}={value} {target}");
        }

        internal static ProcessCall GetProperty(PropertyTarget targetType, string target, string property)
        {
            var binary = targetType == PropertyTarget.Pool ? WhichZpool : WhichZfs;
            target = DatasetHelper.Decode(target);
            return new ProcessCall(binary, $"get {property} {target} -H");
        }
    }
}
