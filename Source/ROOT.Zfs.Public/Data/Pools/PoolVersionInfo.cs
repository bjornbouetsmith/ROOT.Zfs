namespace ROOT.Zfs.Public.Data.Pools
{
    /// <summary>
    /// Smallest set of data for a pool as returned from Zdb
    /// </summary>
    public class PoolVersionInfo
    {
        /// <summary>
        /// The name of the pool
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The underlying zfs version of the pool
        /// </summary>
        public int Version { get; set; }
    }
}
