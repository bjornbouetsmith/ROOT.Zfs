using System.Collections.Generic;

namespace ROOT.Zfs.Public.Data
{
    /// <summary>
    /// Represents a property and its value/source from either dataset or pool
    /// </summary>
    public class PropertyValue
    {
        /// <summary>
        /// The name of the property
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// The source of the property.
        /// This is not meant to be set by user code
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// The value of the property
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Converts this instance into a valid mount option if possible
        /// see https://openzfs.github.io/openzfs-docs/man/7/zfsprops.7.html (Temporary Mount Point Properties) for valid properties to set
        /// </summary>
        /// <returns>A non empty string if the property can be converted to a mount option;an empty string otherwise</returns>
        public string ToMountArgument()
        {
            if (string.IsNullOrWhiteSpace(Property) || string.IsNullOrWhiteSpace(Value))
            {
                return string.Empty;
            }

            switch (Property.ToLowerInvariant())
            {
                // Special cases, default is just the mapping from the dictionary
                case "context":
                case "fscontext":
                case "defcontext":
                case "rootcontext":
                    return $"{Property}={Value}";
                default:
                    if (_propertyToMountArgs.TryGetValue(Property.ToLowerInvariant(), out var mountArgs))
                    {
                        return Value.Equals("off", System.StringComparison.OrdinalIgnoreCase) ? mountArgs.Off : mountArgs.On;
                    }

                    return string.Empty;
            }
        }

        private static readonly Dictionary<string, (string Off, string On)> _propertyToMountArgs = new Dictionary<string, (string Off, string On)>
            {
            {"atime",("noatime","atime")},
            {"canmount",("noauto","auto")},
            {"devices",("nodev","dev")},
            {"exec",("noexec","exec")},
            {"readonly",("rw","ro")},
            {"realatime",("norealatime","realatime")},
            {"setuid",("nosuid","suid")},
            {"xattr",("noxattr","xattr")},
            {"nbmand",("nomand","mand")},
            };
    }
}