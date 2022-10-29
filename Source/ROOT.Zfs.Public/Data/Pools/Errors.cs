namespace ROOT.Zfs.Public.Data.Pools
{
    /// <summary>
    /// Error counters for a pool/vdev/device
    /// </summary>
    public class Errors
    {
        /// <summary>
        /// Number of read errors
        /// </summary>
        public int Read { get; set; }
        
        /// <summary>
        /// Number of write errors
        /// </summary>
        public int Write { get; set; }
        
        /// <summary>
        /// Number of checksum errors
        /// </summary>
        public int Checksum { get; set; }
    }
}