using System;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Core.Helpers
{
    internal static class PropertyValueHelper
    {
        internal static PropertyValue FromString(string line)
        {
            var parts = line.Trim().Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 4)
            {
                throw new ArgumentException($"{line} could not be parsed, expected 4 parts, got: {parts.Length} - requires an output of NAME\\tPROPERTY\\tVALUE\\tSOURCE to be used for property list i.e. zfs get all -H", nameof(line));
            }

            var property = DataSetProperties.Lookup(parts[1]);
            var value = parts[2];
            var source = PropertySources.Lookup(parts[3]);
            return new PropertyValue { Property = property.Name, Source = source, Value = value };
        }
    }
}
