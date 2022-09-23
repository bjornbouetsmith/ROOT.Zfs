using System;

namespace ROOT.Zfs.Core.Info;

public class PropertyValue
{
    public string Property { get; }
    public string Source { get; }
    public string Value { get; }

    public PropertyValue(string name, string source, string value)
    {
        Property = name;
        Source = source;
        Value = value;
    }

    public static PropertyValue FromString(string line)
    {
        var parts = line.Trim().Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 4)
        {
            throw new ArgumentException($"{line} could not be parsed, expected 4 parts, got: {parts.Length} - requires an output of NAME\\tPROPERTY\\tVALUE\\tSOURCE to be used for property list i.e. zfs get all -H", nameof(line));
        }

        var property = DataSetProperties.Lookup(parts[1]);
        var value = parts[2];
        var source = PropertySources.Lookup(parts[3]);
        return new PropertyValue(property.Name, source, value);
    }
}