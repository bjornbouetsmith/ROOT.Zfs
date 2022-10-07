using System;
using System.Collections.Generic;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Public
{
    /// <summary>
    /// Public interface for the ZFS library.
    /// Exposes functionality to manipulate a zfs installation via the binaries 'zfs' and 'zpool'
    /// </summary>
    public interface IZfs
    {
        /// <summary>
        /// Contains snapshot related functionality
        /// </summary>
        ISnapshots Snapshots { get; }
        
        /// <summary>
        /// Contains dataset related functionality
        /// </summary>
        IDataSets DataSets { get; }
        
        /// <summary>
        /// Contains property related functionality
        /// </summary>
        IProperties Properties { get; }
        
        /// <summary>
        /// Contains pool related functionality
        /// </summary>
        IZPool Pool { get; }

        /// <summary>
        /// Gets or set the maximum wait time to wait for a command to process fully
        /// Current default timeout is set to 30 seconds.
        /// When the timeout is reached, the command is aborted - if possible and an exception is raised.
        /// There are no guarantees that the command will be aborted - it is aborted on best effort.
        /// </summary>
        TimeSpan CommandTimeout { get; set; }

        /// <summary>
        /// Gets information about zfs version from the underlying OS.
        /// </summary>
        VersionInfo GetVersionInfo();

        /// <summary>
        /// Lists the disks in the system
        /// </summary>
        IEnumerable<DiskInfo> ListDisks();

        /// <summary>
        /// Gets smart info for all devices of type disk in the system
        /// </summary>
        IEnumerable<SmartInfo> GetSmartInfos();
        
        /// <summary>
        /// Gets smart info for the device
        /// </summary>
        /// <param name="name">The full id or name of the device</param>
        SmartInfo GetSmartInfo(string name);
    }
}
