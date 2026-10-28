using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Public.Arguments.Pool;

namespace ROOT.Zfs.Core.Commands;

/// <summary>
/// Contains commands related to the zpool binary.
/// </summary>
public interface IZpoolCommands
{
    /// <summary>
    /// Returns a command that returns command history for the given pool.
    /// </summary>
    IProcessCall History(PoolHistoryArgs args);

    /// <summary>
    /// Returns a command to get status for the given pool.
    /// </summary>
    IProcessCall GetStatus(PoolStatusArgs args);

    /// <summary>
    /// Returns a command that returns information about all pools.
    /// </summary>
    IProcessCall GetAllPoolInfos();

    /// <summary>
    /// Returns a command that returns information about specific pools.
    /// </summary>
    IProcessCall GetPoolInfo(PoolListArgs args);

    /// <summary>
    /// Builds a command to create a pool.
    /// </summary>
    IProcessCall Create(PoolCreateArgs args);

    /// <summary>
    /// Returns a command that destroys the given pool.
    /// </summary>
    IProcessCall Destroy(PoolDestroyArgs args);

    /// <summary>
    /// Builds a command to return iostats for the given pool.
    /// </summary>
    IProcessCall IoStats(PoolIOStatsArgs args);

    /// <summary>
    /// Returns a command to bring a device offline.
    /// </summary>
    IProcessCall Offline(PoolOfflineArgs args);

    /// <summary>
    /// Returns a command to bring a device online.
    /// </summary>
    IProcessCall Online(PoolOnlineArgs args);

    /// <summary>
    /// Returns a command to clear device errors in a pool.
    /// </summary>
    IProcessCall Clear(PoolClearArgs args);

    /// <summary>
    /// Returns a resilver command for the given pool.
    /// </summary>
    IProcessCall Resilver(PoolResilverArgs args);

    /// <summary>
    /// Returns a scrub command for the given pool.
    /// </summary>
    IProcessCall Scrub(PoolScrubArgs args);

    /// <summary>
    /// Returns a trim command for the given pool.
    /// </summary>
    IProcessCall Trim(PoolTrimArgs args);

    /// <summary>
    /// Returns a command that shows which pools can be upgraded.
    /// </summary>
    IProcessCall GetUpgradeablePools();

    /// <summary>
    /// Returns a command to upgrade a pool.
    /// </summary>
    IProcessCall Upgrade(PoolUpgradeArgs args);

    /// <summary>
    /// Returns a command to detach a device from a mirror.
    /// </summary>
    IProcessCall Detach(PoolDetachArgs args);

    /// <summary>
    /// Returns a command to attach a device to a vdev.
    /// </summary>
    IProcessCall Attach(PoolAttachArgs args);

    /// <summary>
    /// Returns a command to replace a device in a vdev.
    /// </summary>
    IProcessCall Replace(PoolReplaceArgs args);

    /// <summary>
    /// Returns a command to add a vdev to a pool.
    /// </summary>
    IProcessCall Add(PoolAddArgs addArgs);

    /// <summary>
    /// Returns a command to remove a vdev from a pool.
    /// </summary>
    IProcessCall Remove(PoolRemoveArgs removeArgs);
}