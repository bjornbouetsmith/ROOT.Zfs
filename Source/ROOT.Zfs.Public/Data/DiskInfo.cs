namespace ROOT.Zfs.Public.Data
{
    /// <summary>
    /// Represents either a disk or a partition
    /// </summary>
    public class DiskInfo
    {
        /// <summary>
        /// The type this is - disk or partition
        /// </summary>
        public DiskType Type { get; set; }
        /// <summary>
        /// The ID of the disk/partition - prefer this to create pools with
        /// in the format /dev/disk/by-id/XXX
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// The devicename - should not be used if possible, since its not stable
        /// </summary>
        public string DeviceName { get; set; }
    }

    public enum DiskType
    {
        Disk,
        Partition
    }
}
