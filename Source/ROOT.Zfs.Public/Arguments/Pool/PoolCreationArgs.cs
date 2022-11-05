using System.Collections.Generic;
using System.Text;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Public.Arguments.Pool
{
    /// <summary>
    /// Contains the parameters for creating a pool
    /// </summary>
    public class PoolCreationArgs : Args
    {
        /// <summary>
        /// Create an instance of the pool creation args class
        /// </summary>
        public PoolCreationArgs() : base("create")
        {
        }

        /// <summary>
        /// The name of the pool.
        /// Must be unique
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// All the vdevs, spares, cache, logs etc.
        /// </summary>
        public IList<VDevCreationArgs> VDevs { get; set; }

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

        /// <summary>
        /// Validates that the pool creation arguments contains the minimum required information
        /// </summary>
        public override bool Validate(out IList<string> errors)
        {
            errors = null;
            if (string.IsNullOrWhiteSpace(Name))
            {
                errors = new List<string>();
                errors.Add("Please provide a name for the pool");
            }

            if (VDevs == null || VDevs.Count == 0)
            {
                errors ??= new List<string>();
                errors.Add("Please provide vdevs for the pool");
            }
            else
            {
                foreach (var vdevArg in VDevs)
                {
                    if (vdevArg.Validate(out var vdevErrors))
                    {
                        continue;
                    }

                    errors ??= new List<string>();
                    foreach (var error in vdevErrors)
                    {
                        errors.Add(error);
                    }
                }
            }

            return errors == null;
        }
        
        /// <inheritdoc />
        protected override string BuildArgs(string command)
        {
            var args = new StringBuilder();

            args.Append(command);
            args.Append($" {Name}");

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