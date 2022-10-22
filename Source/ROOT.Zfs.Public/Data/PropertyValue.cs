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
                case "atime":
                    return Value.Equals("off", System.StringComparison.OrdinalIgnoreCase) ? "noatime" : "atime";
                case "canmount":
                    return Value.Equals("off", System.StringComparison.OrdinalIgnoreCase) ? "noauto" : "auto";
                case "devices":
                    return Value.Equals("off", System.StringComparison.OrdinalIgnoreCase) ? "nodev" : "dev";
                case "exec":
                    return Value.Equals("off", System.StringComparison.OrdinalIgnoreCase) ? "noexec" : "exec";
                case "readonly":
                    return Value.Equals("off", System.StringComparison.OrdinalIgnoreCase) ? "rw" : "ro";
                case "realatime":
                    return Value.Equals("off", System.StringComparison.OrdinalIgnoreCase) ? "norealatime" : "realatime";
                case "setuid":
                    return Value.Equals("off", System.StringComparison.OrdinalIgnoreCase) ? "nosuid" : "suid";
                case "xattr":
                    return Value.Equals("off", System.StringComparison.OrdinalIgnoreCase) ? "noxattr" : "xattr";
                case "nbmand":
                    return Value.Equals("off", System.StringComparison.OrdinalIgnoreCase) ? "nomand" : "mand";
                case "context":
                case "fscontext":
                case "defcontext":
                case "rootcontext":
                    return $"{Property}={Value}";
                default:
                    return string.Empty;
            }
        }
    }
}