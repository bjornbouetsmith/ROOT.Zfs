using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.Smart;

namespace ROOT.Zfs.Public;

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
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A task whose result contains the version information.</returns>
    Task<VersionInfo> GetVersionInfoAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists the disks in the system
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A task whose result contains the disks and partitions in the system.</returns>
    Task<IReadOnlyList<DiskInfo>> ListDisksAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets smart info for all devices of type disk in the system
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A task whose result contains a read-only list of the smart information for all disks in the system.</returns>
    Task<IReadOnlyList<SmartInfo>> GetSmartInfosAsync(CancellationToken cancellationToken = default);
        
    /// <summary>
    /// Gets smart info for the device
    /// </summary>
    /// <param name="name">The full id or name of the device</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A task whose result contains the smart information for the device.</returns>
    Task<SmartInfo> GetSmartInfoAsync(string name, CancellationToken cancellationToken = default);
}