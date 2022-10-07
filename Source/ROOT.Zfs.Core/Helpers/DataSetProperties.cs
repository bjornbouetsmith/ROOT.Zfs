using System;
using System.Collections.Generic;
using System.Linq;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Core.Helpers
{
    internal static class DataSetProperties
    {
        private static readonly Dictionary<string, Property> _properties = new();

        internal static Property Lookup(string name)
        {
            if (!_properties.TryGetValue(name, out var property))
            {
                // To cater for user defined properties
                property = new Property(name, true, "Unkown property values");
            }

            return property;
        }

        internal static IEnumerable<PropertyValue> FromStdOutput(string stdOutput)
        {
            foreach (var line in stdOutput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                yield return PropertyValueHelper.FromString(line);
            }
        }

        internal static ICollection<Property> AvailableProperties
        {
            get
            {
                if (_properties.Count == 0)
                {
                    return Array.Empty<Property>();

                }

                return _properties.Values;
            }
        }

        internal static void SetAvailableDataSetProperties(IEnumerable<Property> properties)
        {
            foreach (var prop in properties)
            {
                _properties[prop.Name] = prop;
            }
        }
    }
}