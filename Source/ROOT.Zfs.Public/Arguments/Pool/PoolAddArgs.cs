using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Public.Arguments.Pool
{
    /// <summary>
    /// Encapsulates all the arguments to zpool add
    /// </summary>
    public class PoolAddArgs : ArgsBase
    {
        /// <summary>
        /// Name of the pool to add one or more vdevs to
        /// </summary>
        public string PoolName { get; set; }

        /// <summary>
        /// All the vdevs, spares, cache, logs etc.
        /// </summary>
        public IList<VDevCreationArgs> VDevs { get; set; }

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

        /// <summary>
        /// Validates that the pool creation arguments contains the minimum required information
        /// </summary>
        public override bool Validate(out IList<string> errors)
        {
            errors = null;
            if (string.IsNullOrWhiteSpace(PoolName))
            {
                errors = new List<string>();
                errors.Add("Please provide a pool name to add a vdev to");
            }

            if (VDevs == null || VDevs.Count == 0)
            {
                errors ??= new List<string>();
                errors.Add("Please provide vdevs to add");
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
        public override string BuildArgs(string command)
        {
            var args = new StringBuilder();

            args.Append(command);

            if (Force)
            {
                args.Append(" -f");
            }

            args.Append($" {PoolName}");

            var ashift = PropertyValues?.FirstOrDefault(p => p.Property.Equals("ashift", System.StringComparison.OrdinalIgnoreCase));

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
