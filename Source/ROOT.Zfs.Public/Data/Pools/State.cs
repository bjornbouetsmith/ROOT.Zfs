namespace ROOT.Zfs.Public.Data.Pools
{
    /// <summary>
    /// Represents the state of either a pool, vdev or device inside a pool
    /// </summary>
    public enum State
    {
        /// <summary>
        /// Unknown status - this should not be used
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Online and everything is good
        /// </summary>
        Online = 1,
        
        /// <summary>
        /// Pool/vdev/device is working, but degraded - something needs to be done
        /// </summary>
        Degraded = 2,
        /// <summary>
        /// Pool/vdev/device is faulted and it not working - something needs to be done
        /// </summary>
        Faulted = 3,
        /// <summary>
        /// Pool/vdev/device is offline - this might be a reason for either Degraded or Faulted.
        /// Potential source for something needs to happen
        /// </summary>
        Offline = 4,
        /// <summary>
        /// A device was removed
        /// </summary>
        Removed = 5,
        /// <summary>
        /// A device is unavailable, a possible source for error statuses
        /// </summary>
        Unavailable = 6,
        /// <summary>
        /// A device is available and working in the pool
        /// </summary>
        Available = 7
    }
}