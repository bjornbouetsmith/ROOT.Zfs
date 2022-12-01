using System.Text;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Public.Arguments.Pool
{
    /// <summary>
    /// Contains the parameters for creating a pool
    /// </summary>
    public class PoolCreateArgs : PoolAddCreateArgs
    {
        /// <summary>
        /// Create an instance of the pool creation args class
        /// </summary>
        public PoolCreateArgs() : base("create")
        {
        }

        /// <summary>
        /// The mountpoint to use, if it should be different than the default mountpoint
        /// </summary>
        public string MountPoint { get; set; }

        /// <summary>
        /// Any properties to apply to the pool
        /// </summary>
        public PropertyValue[] PoolProperties { get; set; }

        /// <summary>
        /// Any properties to apply to the root filesystem
        /// </summary>
        public PropertyValue[] FileSystemProperties { get; set; }
        
        /// <inheritdoc />
        protected override string BuildArgs(string command)
        {
            var args = new StringBuilder();

            args.Append(command);
            args.Append($" {Decode(PoolName)}");

            if (!string.IsNullOrWhiteSpace(MountPoint))
            {
                args.Append($" -m {MountPoint}");
            }

            if (PoolProperties != null && PoolProperties.Length > 0)
            {
                foreach (var property in PoolProperties)
                {
                    args.Append($" -o {property.Property}={property.Value}");
                }
            }

            if (FileSystemProperties != null && FileSystemProperties.Length > 0)
            {
                foreach (var property in FileSystemProperties)
                {
                    args.Append($" -O {property.Property}={property.Value}");
                }
            }

            foreach (var vdevArg in VDevs)
            {
                args.Append(" " + vdevArg);
            }

            return args.ToString();
        }
    }
}