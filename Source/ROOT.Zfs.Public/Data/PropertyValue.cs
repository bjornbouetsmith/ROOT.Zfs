namespace ROOT.Zfs.Public.Data;

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
}