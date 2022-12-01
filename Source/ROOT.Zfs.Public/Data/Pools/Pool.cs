using System.Collections.Generic;

namespace ROOT.Zfs.Public.Data.Pools
{
    /// <summary>
    /// Contains information about a pool as given by zpool status
    /// </summary>
    public class Pool
    {
        /// <summary>
        /// The name of the pool
        /// </summary>
        public string PoolName { get; set; }
        
        /// <summary>
        /// The vdevs part of this pool
        /// </summary>
        public List<VDev> VDevs { get; set; }
        
        /// <summary>
        /// The overall state of the pool
        /// </summary>
        public State State { get; set; }
        
        /// <summary>
        /// The error counters for the pool
        /// </summary>
        public Errors Errors { get; set; }
    }
}
