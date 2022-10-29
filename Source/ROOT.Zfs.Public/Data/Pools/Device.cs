namespace ROOT.Zfs.Public.Data.Pools
{
    /// <summary>
    /// Represents a device within a vdev inside a pool.
    /// </summary>
    public class Device
    {
        /// <summary>
        /// The device name, i.e.  /dev/sda
        /// </summary>
        public string DeviceName { get; set; }
        
        /// <summary>
        /// The error counts for this device
        /// </summary>
        public Errors Errors { get; set; }
        
        /// <summary>
        /// The state of this device, i.e. Online, offline etc.
        /// </summary>
        public State State { get; set; }
    }
}
