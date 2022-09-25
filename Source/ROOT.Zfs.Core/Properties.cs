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
        public Properties(RemoteProcessCall remoteConnection) : base(remoteConnection)
        {
        }

        public IEnumerable<PropertyValue> GetProperties(string dataset)
        {

            var pc = BuildCommand(PropertyCommands.ProcessCalls.GetProperties(dataset));

            var response = pc.LoadResponse();
            if (!response.Success)
            {
                throw response.ToException();
            }

            return DataSetProperties.FromStdOutput(response.StdOut);
        }

        public PropertyValue GetProperty(string dataset, string property)
        {
            EnsureAllDataSetPropertiesCache();
            var prop = DataSetProperties.Lookup(property);

            var pc = BuildCommand(PropertyCommands.ProcessCalls.GetProperty(dataset, prop));

            var response = pc.LoadResponse();
            if (!response.Success)
            {
                throw response.ToException();
            }

            return PropertyValueHelper.FromString(response.StdOut);

        }
        public PropertyValue SetProperty(string dataset, string property, string value)
        {
            EnsureAllDataSetPropertiesCache();

            var prop = DataSetProperties.Lookup(property);

            var pc = BuildCommand(PropertyCommands.ProcessCalls.SetProperty(dataset, prop, value));
            
            var response = pc.LoadResponse();
            if (!response.Success)
            {
                throw response.ToException();
            }

            return GetProperty(dataset, property);

        }

        private void EnsureAllDataSetPropertiesCache()
        {
            if (DataSetProperties.GetAvailableProperties().FirstOrDefault() == null)
            {
                var pc = BuildCommand(PropertyCommands.ProcessCalls.GetDataSetProperties());

                IEnumerable<Property> properties;
                var response = pc.LoadResponse();
                if (response.Success)
                {
                    properties = DataSetProperties.PropertiesFromStdOutput(response.StdOut);
                }
                else
                {
                    // This is because when you call zfs get -H, you get an error, so the data gets returned in StdError
                    properties = DataSetProperties.PropertiesFromStdOutput(response.StdError);
                }

                DataSetProperties.SetAvailableDataSetProperties(properties);
            }
        }

        public IEnumerable<Property> GetAvailableDataSetProperties()
        {
            EnsureAllDataSetPropertiesCache();

            return DataSetProperties.GetAvailableProperties();
        }

        public void ResetPropertyToInherited(string dataset, string property)
        {
            EnsureAllDataSetPropertiesCache();
            var prop = DataSetProperties.Lookup(property);

            var pc = BuildCommand(PropertyCommands.ProcessCalls.ResetPropertyToInherited(dataset, prop));

            var response = pc.LoadResponse();
            if (!response.Success)
            {
                throw response.ToException();
            }
        }
    }
}
