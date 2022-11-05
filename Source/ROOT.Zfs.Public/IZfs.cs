using System.Collections.Generic;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.Smart;

namespace ROOT.Zfs.Public
{
    /// <summary>
    /// Public interface for the ZFS library.
    /// Exposes functionality to manipulate a zfs installation via the binaries 'zfs' and 'zpool'
    /// </summary>
    public interface IZfs : IBasicZfs
    {
        /// <summary>
        /// Contains snapshot related functionality
        /// </summary>
        ISnapshots Snapshots { get; }
        
        /// <summary>
        /// Contains dataset related functionality
        /// </summary>
        IDatasets Datasets { get; }
        
        /// <summary>
        /// Contains property related functionality
        /// </summary>
        IProperties Properties { get; }

        /// <summary>
        /// Contains pool related functionality
        /// </summary>
        IZPool Pool { get; }

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

        /// <summary>
        /// Initializes the library to enable auto detection of where required binaries are located.
        /// Utilizes the command `which` - if that is not installed this will fail.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Gets a value indicating whether or not this instance has been initialized
        /// </summary>
        bool Initialized { get; }
    }
}
