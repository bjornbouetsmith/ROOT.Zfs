using System.Collections.Generic;

namespace ROOT.Zfs.Public.Data.Pools
{
    /// <summary>
    /// Represents a vdev inside a pool
    /// </summary>
    public class VDev
    {
        /// <summary>
        /// Name of the vdev.
        /// This is also the type of vded, i.e. mirrors are called 'mirror-x' where x is the number of mirror, where first mirror is 0
        /// </summary>
        public string VDevName { get; set; }
        
        /// <summary>
        /// State of the vdev
        /// </summary>
        public State State { get; set; }
        
        /// <summary>
        /// Error counters for the vdev
        /// </summary>
        public Errors Errors { get; set; }
        
        /// <summary>
        /// The list of devices that is part of this vdev
        /// </summary>
        public List<Device> Devices { get; set; }
    }
}
