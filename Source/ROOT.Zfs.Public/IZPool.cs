using System;
using System.Collections.Generic;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.Pools;
using ROOT.Zfs.Public.Data.Statistics;

namespace ROOT.Zfs.Public
{
    public interface IZPool
    {
        /// <summary>
        /// Gets command history for the pool
        /// https://openzfs.github.io/openzfs-docs/man/8/zpool-history.8.html
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="skipLines"></param>
        /// <param name="afterDate"></param>
        IEnumerable<CommandHistory> GetHistory(string pool, int skipLines = 0, DateTime afterDate = default);

        /// <summary>
        /// Gets the pool status for the given pool
        /// https://openzfs.github.io/openzfs-docs/man/8/zpool-status.8.html
        /// </summary>
        /// <param name="pool"></param>
        PoolStatus GetStatus(string pool); //zpool status {pool}

        /// <summary>
        /// Gets information about pools, returns data from the command: zpool list -v -P
        /// https://openzfs.github.io/openzfs-docs/man/8/zpool-list.8.html
        /// </summary>
        IEnumerable<PoolInfo> GetAllPoolInfos(); //zpool list -v -P

        /// <summary>
        /// Gets information about a single pool zpool list -v -P {pool}
        /// https://openzfs.github.io/openzfs-docs/man/8/zpool-list.8.html
        /// </summary>
        /// <param name="pool">The pool to get info for</param>
        PoolInfo GetPoolInfo(string pool); //zpool list -v -P {pool}

        /// <summary>
        /// Creates a new zpool using the provided args
        ///<see cref="PoolCreationArgs"/> for details
        /// https://openzfs.github.io/openzfs-docs/man/8/zpool-create.8.html
        /// </summary>
        /// <returns>The pool status for the newly created pool</returns>
        PoolStatus CreatePool(PoolCreationArgs args);

        /// <summary>
        /// Destroys the given pool
        /// https://openzfs.github.io/openzfs-docs/man/8/zpool-destroy.8.html
        /// </summary>
        /// <param name="pool">The name of the pool to destroy</param>
        void DestroyPool(string pool);

        /// <summary>
        /// Takes the specified physical device offline.
        /// While the device is offline, no attempt is made to read or write to the device.
        /// This command is not applicable to spares.
        /// https://openzfs.github.io/openzfs-docs/man/8/zpool-offline.8.html
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="device"></param>
        /// <param name="forceFault">Force fault. Instead of offlining the disk, put it into a faulted state.
        /// The fault will persist across imports unless <paramref name="temporary"/> is true.</param>
        /// <param name="temporary">Temporary. Upon reboot, the specified physical device reverts to its previous state.</param>
        void Offline(string pool, string device, bool forceFault, bool temporary);

        /// <summary>
        /// Brings the specified physical device online.
        /// This command is not applicable to spares.
        /// https://openzfs.github.io/openzfs-docs/man/8/zpool-online.8.html
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="device"></param>
        /// <param name="expandSpace"></param>
        /// <returns></returns>
        void Online(string pool, string device, bool expandSpace);

        /// <summary>
        /// clear device errors in ZFS storage pool.
        /// Clears device errors in a pool.
        /// If no arguments are specified, all device errors within the pool are cleared.
        /// If one or more devices is specified, only those errors associated with the specified device or devices are cleared.
        /// If the pool was suspended it will be brought back online provided the devices can be accessed.
        /// Pools with multihost enabled which have been suspended cannot be resumed.
        /// While the pool was suspended, it may have been imported on another host, and resuming I/O could result in pool damage.
        /// https://openzfs.github.io/openzfs-docs/man/8/zpool-clear.8.html
        /// </summary>
        /// <param name="pool">The pool for which to clear errors</param>
        /// <param name="device">The devices to clear errors for. This is optional - can be a vdev or a device part of a vdev</param>
        /// <returns></returns>
        void Clear(string pool, string device);
        
        /// <summary>
        /// Returns basic iostats for the given pool and optinally specific devices
        /// https://openzfs.github.io/openzfs-docs/man/8/zpool-iostat.8.html
        /// </summary>
        /// <param name="pool">The pool to show stats for</param>
        /// <param name="devices">The devices if any to show stats for</param>
        /// <param name="includeAverageLatency">Whether to include average latency stats as well (-l)</param>
        IOStats GetIOStats(string pool, string[] devices, bool includeAverageLatency);
    }
}