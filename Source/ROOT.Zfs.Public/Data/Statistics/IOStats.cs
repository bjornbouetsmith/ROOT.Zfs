using System;
using System.Collections.Generic;

namespace ROOT.Zfs.Public.Data.Statistics
{
    /// <summary>
    /// Represents output from zpool iostats
    /// </summary>
    public class IOStats
    {
        /// <summary>
        /// The name of the pool the stats are related to
        /// </summary>
        public string Pool { get; set; }
        
        /// <summary>
        /// The actual stats for the pool
        /// </summary>
        public List<IOStat> Stats { get; set; }
        
    }

    /// <summary>
    /// Represents stats for a sigle pool/vdev/device
    /// </summary>
    public class IOStat
    {
        /// <summary>
        /// The device the stats are for
        /// </summary>
        public string Device { get; set; }
        
        /// <summary>
        /// Capacity related information
        /// </summary>
        public Capacity Capacity { get; set; }
        
        /// <summary>
        /// Number of operations
        /// </summary>
        public Operations Operations { get; set; }
        
        /// <summary>
        /// Bandwidth consumed
        /// </summary>
        public Bandwidth Bandwidth { get; set; }
        
        /// <summary>
        /// Latency stats for the device
        /// </summary>
        public IOLatencyStats LatencyStats { get; set; }
        
        /// <summary>
        /// Statis for child devices - this is only relevant if this instance is either a pool or vdev
        /// </summary>
        public List<IOStat> ChildStats { get; set; }
    }

    /// <summary>
    /// Represents the latency stats for a single pool/vdev/device
    /// </summary>
    public class IOLatencyStats
    {
        /// <summary>
        /// Total read/write wait latency
        /// </summary>
        public Latency TotalWait { get; set; }
        
        /// <summary>
        /// Disk read/write latency
        /// </summary>
        public Latency DiskWait { get; set; }
        
        /// <summary>
        /// syncq read/write latency
        /// </summary>
        public Latency SyncqWait { get; set; }

        /// <summary>
        /// asyncq read/write latency
        /// </summary>
        public Latency AsyncqWait { get; set; }

        /// <summary>
        /// scrub wait latency
        /// </summary>
        public TimeSpan ScrubWait { get; set; }

        /// <summary>
        /// scrub wait latency
        /// </summary>
        public TimeSpan TrimWait { get; set; }
    }

    /// <summary>
    /// Represents capacity information
    /// </summary>
    public class Capacity
    {
        /// <summary>
        /// Allocated bytes
        /// </summary>
        public Size Allocated { get; set; }

        /// <summary>
        /// Free bytes
        /// </summary>
        public Size Free { get; set; }
    }

    /// <summary>
    /// Represents a count of operations
    /// </summary>
    public class Operations
    {
        /// <summary>
        /// count of read operations
        /// </summary>
        public int Read { get; set; }

        /// <summary>
        /// count of write operations
        /// </summary>
        public int Write { get; set; }
    }

    /// <summary>
    /// Represents bandwidth usage
    /// </summary>
    public class Bandwidth
    {
        /// <summary>
        /// Read bandwith consumed
        /// </summary>
        public Size Read { get; set; }
        
        /// <summary>
        /// Write bandwith consumed
        /// </summary>
        public Size Write { get; set; }
    }
}
