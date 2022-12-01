using System.Collections.Generic;
using ROOT.Zfs.Public.Arguments.Pool;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.Pools;
using ROOT.Zfs.Public.Data.Statistics;

namespace ROOT.Zfs.Public
{
    /// <summary>
    /// Contains methods that corresponds to commands you can run with the zpool binary
    /// see: https://openzfs.github.io/openzfs-docs/man/8/zpool.8.html?highlight=zpool
    /// </summary>
    public interface IZPool : IBasicZfs
    {
        /// <summary>
        /// Gets command history for the pool
        /// https://openzfs.github.io/openzfs-docs/man/8/zpool-history.8.html
        /// </summary>
        IEnumerable<CommandHistory> History(PoolHistoryArgs args);

        /// <summary>
        /// Gets the pool status for the given pool
        /// https://openzfs.github.io/openzfs-docs/man/8/zpool-status.8.html
        /// </summary>
        PoolStatus Status(PoolStatusArgs args);

        /// <summary>
        /// Gets information about pools, returns data from the command: zpool list -v -P
        /// https://openzfs.github.io/openzfs-docs/man/8/zpool-list.8.html
        /// </summary>
        IList<PoolInfo> List();

        /// <summary>
        /// Gets information about a single pool zpool list -v -P {pool}
        /// https://openzfs.github.io/openzfs-docs/man/8/zpool-list.8.html
        /// </summary>
        /// <param name="args">The pool to get info for</param>
        PoolInfo List(PoolListArgs args);

        /// <summary>
        /// Creates a new zpool using the provided args
        ///<see cref="PoolCreateArgs"/> for details
        /// https://openzfs.github.io/openzfs-docs/man/8/zpool-create.8.html
        /// </summary>
        /// <returns>The pool status for the newly created pool</returns>
        PoolStatus Create(PoolCreateArgs args);

        /// <summary>
        /// Destroys the given pool
        /// https://openzfs.github.io/openzfs-docs/man/8/zpool-destroy.8.html
        /// </summary>
        /// <param name="args">The name of the pool to destroy</param>
        void Destroy(PoolDestroyArgs args);
        
        /// <summary>
        /// Takes the specified physical device offline.
        /// While the device is offline, no attempt is made to read or write to the device.
        /// This command is not applicable to spares.
        /// https://openzfs.github.io/openzfs-docs/man/8/zpool-offline.8.html
        /// </summary>
        /// <param name="args">The arguments to the offline command</param>
        void Offline(PoolOfflineArgs args);

        /// <summary>
        /// Brings the specified physical device online.
        /// This command is not applicable to spares.
        /// https://openzfs.github.io/openzfs-docs/man/8/zpool-online.8.html
        /// </summary>
        /// <param name="args">The arguments to the online command</param>
        void Online(PoolOnlineArgs args);

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
        /// <param name="args">The arguments for the clear command</param>
        void Clear(PoolClearArgs args);
        
        /// <summary>
        /// Returns basic iostats for the given pool and optionally specific devices
        /// https://openzfs.github.io/openzfs-docs/man/8/zpool-iostat.8.html
        /// </summary>
        /// <param name="args">The arguments for zpool iostat</param>
        IOStats IOStats(PoolIOStatsArgs args);

        /// <summary>
        /// Starts a resilver of the specified pools.
        /// If an existing resilver is already running it will be restarted from the beginning.
        /// Any drives that were scheduled for a deferred resilver will be added to the new one.
        /// This requires the resilver_defer pool feature.
        /// see https://openzfs.github.io/openzfs-docs/man/8/zpool-resilver.8.html
        /// </summary>
        /// <param name="args">Arguments for the resilver command</param>
        void Resilver(PoolResilverArgs args);

        /// <summary>
        /// Begin, resume, pause or stop scrub of ZFS storage pools
        /// see https://openzfs.github.io/openzfs-docs/man/8/zpool-scrub.8.html
        /// </summary>
        /// <param name="args">The arguments for the scrub command</param>
        void Scrub(PoolScrubArgs args);

        /// <summary>
        /// Initiates an immediate on-demand TRIM operation for all of the free space in a pool.
        /// This operation informs the underlying storage devices of all blocks in the pool
        /// which are no longer allocated and allows thinly provisioned devices to reclaim the space.
        /// see https://openzfs.github.io/openzfs-docs/man/8/zpool-trim.8.html
        /// </summary>
        /// <param name="args">The arguments to the trim command</param>
        void Trim(PoolTrimArgs args);

        /// <summary>
        /// Displays pools which do not have all supported features enabled and pools formatted using a legacy ZFS version number.
        /// These pools can continue to be used, but some features may not be available.
        /// see https://openzfs.github.io/openzfs-docs/man/8/zpool-upgrade.8.html
        /// </summary>
        /// <returns>List of upgradeable pools and their missing features</returns>
        IList<UpgradeablePool> GetUpgradeablePools();

        /// <summary>
        /// Enable all features on the given pool or all pools
        /// see https://openzfs.github.io/openzfs-docs/man/8/zpool-upgrade.8.html
        /// </summary>
        /// <param name="args">Args to control whether to upgrade a single pool or all pools</param>
        void Upgrade(PoolUpgradeArgs args);
        
        /// <summary>
        /// Detaches device from a mirror. The operation is refused if there are no other valid replicas of the data.
        /// If device may be re-added to the pool later on then consider the zpool offline command instead.
        /// see https://openzfs.github.io/openzfs-docs/man/8/zpool-detach.8.html
        /// </summary>
        void Detach(PoolDetachArgs args);

        /// <summary>
        /// Attaches new_device to the existing mirrored or nonmirrored device.
        /// The existing device cannot be part of a raidz configuration.
        /// If device is not currently part of a mirrored configuration, device automatically transforms into a two-way mirror of device and new_device.
        /// If device is part of a two-way mirror, attaching new_device creates a three-way mirror, and so on.
        /// In either case, new_device begins to resilver immediately and any running scrub is cancelled.
        /// see https://openzfs.github.io/openzfs-docs/man/8/zpool-attach.8.html
        /// </summary>
        void Attach(PoolAttachArgs args);

        /// <summary>
        /// Replaces device with new-device.
        /// This is equivalent to attaching new-device, waiting for it to resilver, and then detaching device.
        /// Any in progress scrub will be cancelled.
        /// The size of new-device must be greater than or equal to the minimum size of all the devices in a mirror or raidz configuration.
        /// new-device is required if the pool is not redundant.
        /// If new-device is not specified, it defaults to device.
        /// This form of replacement is useful after an existing disk has failed and has been physically replaced.
        /// In this case, the new disk may have the same /dev path as the old device, even though it is actually a different disk.ZFS recognizes this.
        /// </summary>
        void Replace(PoolReplaceArgs args);

        /// <summary>
        /// Adds the specified virtual devices to the given pool.
        /// The vdev specification is described in the Virtual Devices section of zpoolconcepts(7).
        /// The behavior of the -f option, and the device checks performed are described in the zpool create subcommand.
        /// see https://openzfs.github.io/openzfs-docs/man/8/zpool-add.8.html
        /// </summary>
        void Add(PoolAddArgs args);

        /// <summary>
        /// Removes the specified device from the pool. This command supports removing hot spare, cache, log, and both mirrored and non-redundant primary top-level vdevs, including dedup and special vdevs.
        /// Top-level vdevs can only be removed if the primary pool storage does not contain a top-level raidz vdev, all top-level vdevs have the same sector size, and the keys for all encrypted datasets are loaded.
        /// Removing a top-level vdev reduces the total amount of space in the storage pool.The specified device will be evacuated by copying all allocated space from it to the other devices in the pool.In this case, the zpool remove command initiates the removal and returns, while the evacuation continues in the background. The removal progress can be monitored with zpool status.If an I/O error is encountered during the removal process it will be cancelled.The device_removal feature flag must be enabled to remove a top-level vdev, see zpool-features(7).
        /// A mirrored top-level device(log or data) can be removed by specifying the top-level mirror for the same.Non-log devices or data devices that are part of a mirrored configuration can be removed using the zpool detach command.
        /// see https://openzfs.github.io/openzfs-docs/man/8/zpool-remove.8.html
        /// </summary>
        void Remove(PoolRemoveArgs args);
    }
}