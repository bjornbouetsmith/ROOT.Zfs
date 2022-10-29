namespace ROOT.Zfs.Public.Data.Pools
{
    /// <summary>
    /// Type of vdev to create
    /// </summary>
    public enum VDevCreationType
    {
        /// <summary>
        /// A mirror of two or more devices. 
        /// </summary>
        Mirror,

        /// <summary>
        /// A variation on RAID-5 that allows for better distribution of parity and eliminates the RAID-5 “write hole” 
        /// The raidz1 vdev type specifies a single-parity raidz group
        /// </summary>
        Raidz1,

        /// <summary>
        /// A variation on RAID-5 that allows for better distribution of parity and eliminates the RAID-5 “write hole” 
        /// The raidz2 vdev type specifies a double-parity raidz group
        /// </summary>
        Raidz2,

        /// <summary>
        /// A variation on RAID-5 that allows for better distribution of parity and eliminates the RAID-5 “write hole” 
        /// The raidz3 vdev type specifies a triple-parity raidz group
        /// </summary>
        Raidz3,

        /// <summary>
        /// A variant of raidz that provides integrated distributed hot spares which allows for faster resilvering while retaining the benefits of raidz.
        /// Distributed raid1, single parity
        /// </summary>
        DRaid1,

        /// <summary>
        /// A variant of raidz that provides integrated distributed hot spares which allows for faster resilvering while retaining the benefits of raidz.
        /// Distributed raid2, double parity
        /// </summary>
        DRaid2,

        /// <summary>
        /// A variant of raidz that provides integrated distributed hot spares which allows for faster resilvering while retaining the benefits of raidz.
        /// Distributed raid3m, triple parity
        /// </summary>
        DRaid3,

        /// <summary>
        /// A device used to cache storage pool data. A cache device cannot be configured as a mirror or raidz group. , i.e. L2ARC
        /// </summary>
        Cache,

        /// <summary>
        /// A separate intent log device., i.e. Zil
        /// </summary>
        Log,

        /// <summary>
        /// A pseudo-vdev which keeps track of available hot spares for a pool. 
        /// </summary>
        Spare,

        /// <summary>
        /// A device dedicated solely for deduplication tables. 
        /// </summary>
        Dedup,

        /// <summary>
        /// A device dedicated solely for allocating various kinds of internal metadata, and optionally small file blocks. 
        /// </summary>
        Special
    }
}