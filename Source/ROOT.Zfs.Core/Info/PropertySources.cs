using System;

namespace ROOT.Zfs.Core.Info;

public class PropertySources
{
    public static readonly PropertySource Local = new PropertySource("local");
    public static readonly PropertySource Inherited = new PropertySource("inherited");
    public static readonly PropertySource Default = new PropertySource("default");
    public static readonly PropertySource Zfs = new PropertySource("-");
    public static readonly PropertySource Unknown = new PropertySource("Uknown");

    public static string Lookup(string source)
    {
        if (source.Equals("local", StringComparison.OrdinalIgnoreCase))
        {
            return Local.Name;
        }

        if (source.Equals("default", StringComparison.OrdinalIgnoreCase))
        {
            return Default.Name;
        }
        if (source.Equals("-", StringComparison.OrdinalIgnoreCase))
        {
            return Zfs.Name;
        }

        if (source.StartsWith("inherited", StringComparison.OrdinalIgnoreCase))
        {
            return Inherited.Name;
        }

        return Unknown.Name;
    }
}