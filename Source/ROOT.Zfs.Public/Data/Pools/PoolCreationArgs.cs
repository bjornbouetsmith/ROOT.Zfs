using System.Collections.Generic;

namespace ROOT.Zfs.Public.Data.Pools
{
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

        public bool Validate(out string errorMessage)
        {
            //TODO: Write validation logic, i.e. name must be set etc.

            if (string.IsNullOrWhiteSpace(Name))
            {
                errorMessage = "Please provide a name for the pool";
                return false;
            }

            foreach (var vdevArg in VDevs)
            {
                if (!vdevArg.Validate(out errorMessage))
                {
                    return false;
                }
            }

            errorMessage = null;
            return true;
        }
    }
}