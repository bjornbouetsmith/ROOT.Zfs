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
                _properties[name] = property;
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

        internal static IEnumerable<Property> PropertiesFromStdOutput(string stdOutput)
        {
            bool startParsing = false;

            foreach (var line in stdOutput.Split(new[] { '\r', '\n' }, StringSplitOptions.None)) // Do not remove blank lines, since it will ruin the logic below
            {
                // Skip blank lines
                if (line.Trim().Length == 0)
                {
                    continue;
                }

                // IF we are at "PROPERTY" - signal that lines should be parsed
                if (line.Trim().StartsWith("PROPERTY"))
                {
                    startParsing = true;
                    continue;
                }

                // If parsing is not to be started, just continue with next line
                if (!startParsing)
                {
                    continue;
                }

                // This line is after last
                if (line.Trim().StartsWith("The feature@ properties"))
                {
                    //no more data
                    break;
                }
                // data in formmat
                // whitespace [PROPERTY]whitespace [EDIT]whitespace[INHERIT]whitespace[VALUES]
                var parts = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length < 4)
                {
                    Console.Write("Error parsing line:{0}", line);
                    continue;
                }

                var property = parts[0];
                var editable = parts[1];

                if (!editable.Equals("YES", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var content = string.Join(' ', parts.Skip(3));
                var validValues = content.Split('|').Select(v => v.Trim()).ToArray();
                var prop = new Property(property, true, validValues);

                yield return prop;

            }
        }
    }
}