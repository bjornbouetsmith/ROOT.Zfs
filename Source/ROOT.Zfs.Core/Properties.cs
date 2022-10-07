using System.Collections.Generic;
using System.Linq;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Core
{
    internal class Properties : ZfsBase, IProperties
    {
        public Properties(IProcessCall remoteConnection) : base(remoteConnection)
        {
        }

        public IEnumerable<PropertyValue> GetProperties(string dataset)
        {
            var pc = BuildCommand(PropertyCommands.GetProperties(dataset));

            var response = pc.LoadResponse(true);

            return DataSetProperties.FromStdOutput(response.StdOut);
        }

        public PropertyValue GetProperty(string dataset, string property)
        {
            EnsureAllDataSetPropertiesCache();
            var prop = DataSetProperties.Lookup(property);

            var pc = BuildCommand(PropertyCommands.GetProperty(dataset, prop));

            var response = pc.LoadResponse(true);

            return PropertyValueHelper.FromString(response.StdOut);

        }

        public PropertyValue SetProperty(string dataset, string property, string value)
        {
            EnsureAllDataSetPropertiesCache();

            var prop = DataSetProperties.Lookup(property);

            var pc = BuildCommand(PropertyCommands.SetProperty(dataset, prop, value));

            pc.LoadResponse(true);

            return GetProperty(dataset, property);

        }

        private void EnsureAllDataSetPropertiesCache()
        {
            if (DataSetProperties.AvailableProperties.Count == 0)
            {
                var pc = BuildCommand(PropertyCommands.GetDataSetProperties());

                IEnumerable<Property> properties;
                var response = pc.LoadResponse(false);
                if (response.Success)
                {
                    properties = PropertiesParser.FromStdOutput(response.StdOut, 4);
                }
                else
                {
                    // This is because when you call zfs get -H, you get an error, so the data gets returned in StdError
                    properties = PropertiesParser.FromStdOutput(response.StdError, 4);
                }

                DataSetProperties.SetAvailableDataSetProperties(properties);
            }
        }

        public ICollection<Property> GetAvailableDataSetProperties()
        {
            EnsureAllDataSetPropertiesCache();

            return DataSetProperties.AvailableProperties;
        }

        public ICollection<Property> GetAvailablePoolProperties()
        {
            var cmd = BuildCommand(PropertyCommands.GetPoolProperties());
            var response = cmd.LoadResponse(false);
            ICollection<Property> properties;
            if (response.Success)
            {
                properties = PropertiesParser.FromStdOutput(response.StdOut, 3);
            }
            else
            {
                // This is because when you call zpool get -H, you get an error, so the data gets returned in StdError
                properties = PropertiesParser.FromStdOutput(response.StdError,3);
            }
            return properties;
        }

        public void ResetPropertyToInherited(string dataset, string property)
        {
            EnsureAllDataSetPropertiesCache();
            var prop = DataSetProperties.Lookup(property);

            var pc = BuildCommand(PropertyCommands.ResetPropertyToInherited(dataset, prop));

            pc.LoadResponse(true);

        }
    }
}
