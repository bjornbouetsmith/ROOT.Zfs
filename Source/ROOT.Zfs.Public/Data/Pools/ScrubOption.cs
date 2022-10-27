namespace ROOT.Zfs.Public.Data.Pools
{
    /// <summary>
    /// Options for the scrub command
    /// </summary>
    public enum ScrubOption
    {
        /// <summary>
        /// No options mean start or resume a scrub.
        /// </summary>
        None=0,

        /// <summary>
        /// Stop a scrub
        /// </summary>
        Stop=1,

        /// <summary>
        /// Pause scrubbing. Scrub pause state and progress are periodically synced to disk.
        /// If the system is restarted or pool is exported during a paused scrub, even after import, scrub will remain paused until it is resumed.
        /// Once resumed the scrub will pick up from the place where it was last checkpointed to disk. To resume a paused scrub issue zpool scrub again.
        /// </summary>
        Pause = 2
    }
}
