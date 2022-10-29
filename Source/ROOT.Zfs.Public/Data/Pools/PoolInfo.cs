using ROOT.Zfs.Public.Data.Statistics;

namespace ROOT.Zfs.Public.Data.Pools
{
    /// <summary>
    /// Contains information about a pool, output from zpool list
    /// </summary>
    public class PoolInfo : PoolVersionInfo
    {
        /// <summary>
        /// Total size of the pool
        /// </summary>
        public Size Size { get; set; }

        /// <summary>
        /// Allocated bytes in the pool
        /// </summary>
        public Size Allocated { get; set; }

        /// <summary>
        /// Free bytes in the pool
        /// </summary>
        public Size Free { get; set; }

        /// <summary>
        /// Fragmentation percentage
        /// </summary>
        public Part Fragmentation { get; set; }

        /// <summary>
        /// Used space in the pool
        /// </summary>
        public Part CapacityUsed { get; set; }

        /// <summary>
        /// Dedup ratio, if deduplication is enabled
        /// </summary>
        public Ratio DedupRatio { get; set; }

        /// <summary>
        /// State of the pool
        /// </summary>
        public State State { get; set; }

        /// <summary>
        /// Alternative root for the pool
        /// </summary>
        public string AltRoot { get; set; }
    }
}
