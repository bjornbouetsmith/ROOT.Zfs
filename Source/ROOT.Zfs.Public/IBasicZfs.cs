using System;

namespace ROOT.Zfs.Public
{
    /// <summary>
    /// Common interface for all interfaces that exposes functionality from zfs/zpool binaries
    /// </summary>
    public interface IBasicZfs
    {
        /// <summary>
        /// Gets or set the maximum wait time to wait for a command to process fully
        /// Current default timeout is set to 30 seconds.
        /// When the timeout is reached, the command is aborted - if possible and an exception is raised.
        /// There are no guarantees that the command will be aborted - it is aborted on best effort.
        /// </summary>
        TimeSpan CommandTimeout { get; set; }

        /// <summary>
        /// Gets or sets whether or not sudo is required
        /// </summary>
        bool RequiresSudo { get; set; }
    }
}
