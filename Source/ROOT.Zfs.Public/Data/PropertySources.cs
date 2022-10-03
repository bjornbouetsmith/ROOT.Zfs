using System;

namespace ROOT.Zfs.Public.Data
{
    public static class PropertySources
    {
        public static readonly PropertySource Local = new ("local");
        public static readonly PropertySource Inherited = new ("inherited");
        public static readonly PropertySource Default = new ("default");
        public static readonly PropertySource Zfs = new ("-");
        public static readonly PropertySource Unknown = new ("Uknown");

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
}