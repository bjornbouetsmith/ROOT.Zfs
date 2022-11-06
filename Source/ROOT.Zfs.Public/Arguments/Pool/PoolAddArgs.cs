using System;
using System.Linq;
using System.Text;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Public.Arguments.Pool
{
    /// <summary>
    /// Encapsulates all the arguments to zpool add
    /// </summary>
    public class PoolAddArgs : PoolAddCreateArgs
    {
        /// <summary>
        /// Creates an add args instance
        /// </summary>
        public PoolAddArgs() : base("add")
        {
        }

        /// <summary>
        /// Forces use of vdevs, even if they appear in use or specify a conflicting replication level.
        /// Not all devices can be overridden in this manner.
        /// </summary>
        public bool Force { get; set; }

        /// <summary>
        /// Sets the given pool properties.
        /// The only property supported at the moment is ashift.
        /// </summary>
        public PropertyValue[] PropertyValues { get; set; }

        /// <inheritdoc />
        protected override string BuildArgs(string command)
        {
            var args = new StringBuilder();

            args.Append(command);

            if (Force)
            {
                args.Append(" -f");
            }

            args.Append($" {PoolName}");

            var ashift = PropertyValues?.FirstOrDefault(p => p.Property.Equals("ashift", StringComparison.OrdinalIgnoreCase));

            if (ashift != null)
            {
                args.Append($" -o ashift={ashift.Value}");
            }

            foreach (var vdevArg in VDevs ?? throw new ArgumentException("Missing Vdevs"))
            {
                args.Append(" " + vdevArg);
            }

            return args.ToString();
        }
    }
}
