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
        public DeviceType Type { get; set; }
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

    /// <summary>
    /// Represents the kind of device
    /// </summary>
    public enum DeviceType
    {
        /// <summary>
        /// A physical disk
        /// </summary>
        Disk,
        /// <summary>
        /// A partition on a given disk
        /// </summary>
        Partition
    }
}
