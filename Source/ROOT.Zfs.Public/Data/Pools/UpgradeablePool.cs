using System.Collections.Generic;

namespace ROOT.Zfs.Public.Data.Pools
{
    /// <summary>
    /// Contains data about a pool that can be upgraded and what feature flags are missing
    /// </summary>
    public class UpgradeablePool
    {
        /// <summary>
        /// Name of the upgradeable pool
        /// </summary>
        public string PoolName { get; set; }

        /// <summary>
        /// Features that is not enabled in the pool
        /// </summary>
        public IList<string> MissingFeatures { get; set; }
    }
}
