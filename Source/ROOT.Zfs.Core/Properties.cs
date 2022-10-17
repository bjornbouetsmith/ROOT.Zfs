using System.Collections.Generic;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Core
{
    internal class Properties : ZfsBase, IProperties
    {
        internal Properties(IProcessCall remoteConnection) : base(remoteConnection)
        {
        }
        
        private ICollection<Property> GetAvailableZfsProperties()
        {
            var pc = BuildCommand(PropertyCommands.GetDatasetProperties());

            var response = pc.LoadResponse(false);

            // This is because when you call zfs get -H, you get an error, so the data gets returned in StdError
            return PropertiesParser.FromStdOutput(response.StdError, 4);
        }

        private ICollection<Property> GetAvailablePoolProperties()
        {
            var cmd = BuildCommand(PropertyCommands.GetPoolProperties());
            var response = cmd.LoadResponse(false);

            // This is because when you call zpool get -H, you get an error, so the data gets returned in StdError
            var properties = PropertiesParser.FromStdOutput(response.StdError, 3);

            return properties;
        }

        public ICollection<Property> GetAvailableProperties(PropertyTarget targetType)
        {
            return targetType == PropertyTarget.Pool
                ? GetAvailablePoolProperties()
                : GetAvailableZfsProperties();
        }

        public PropertyValue GetProperty(PropertyTarget targetType, string target, string property)
        {
            var pc = BuildCommand(PropertyCommands.GetProperty(targetType, target, property));

            var response = pc.LoadResponse(true);

            return PropertyValueHelper.FromString(response.StdOut);
        }

        public PropertyValue SetProperty(PropertyTarget targetType, string target, string property, string value)
        {
            var pc = BuildCommand(PropertyCommands.SetProperty(targetType, target, property, value));

            pc.LoadResponse(true);

            return GetProperty(targetType, target, property);
        }

        public IEnumerable<PropertyValue> GetProperties(PropertyTarget targetType, string target)
        {
            var pc = BuildCommand(PropertyCommands.GetProperties(targetType, target));

            var response = pc.LoadResponse(true);

            return DatasetProperties.FromStdOutput(response.StdOut);
        }

        public void ResetPropertyToInherited(string dataset, string property)
        {
            var pc = BuildCommand(PropertyCommands.ResetPropertyToInherited(dataset, property));

            pc.LoadResponse(true);

        }
    }
}
