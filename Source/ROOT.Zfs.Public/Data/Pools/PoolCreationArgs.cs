using System.Collections.Generic;

namespace ROOT.Zfs.Public.Data.Pools
{
    /// <summary>
    /// Contains the parameters for creating a pool
    /// </summary>
    public class PoolCreationArgs
    {
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
        public bool Validate(out List<string> errors)
        {
            //TODO: Write validation logic, i.e. name must be set etc.
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
                    errors.AddRange(vdevErrors);
                }
            }

            return errors == null;
        }
    }
}