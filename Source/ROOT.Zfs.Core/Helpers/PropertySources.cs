using System;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Core.Helpers
{
    /// <summary>
    /// Point of this class is to do simple interning of strings, so all sources share the same instance of the string
    /// </summary>
    internal static class PropertySources
    {
        public static readonly PropertySource Local = new ("local");
        public static readonly PropertySource Inherited = new ("inherited");
        public static readonly PropertySource Default = new ("default");
        public static readonly PropertySource Zfs = new ("-");
        public static readonly PropertySource Unknown = new ("Uknown");

        /// <summary>
        /// Looks up the source and returns the static instance of the name
        /// </summary>
        public static string Lookup(string source)
        {
            if (source == null)
            {
                return Unknown.Name;
            }

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