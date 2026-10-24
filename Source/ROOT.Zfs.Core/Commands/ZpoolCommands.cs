using System;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Public.Arguments.Pool;

namespace ROOT.Zfs.Core.Commands;

/// <summary>
/// Contains commands related to the zpool binary
/// </summary>
internal sealed class ZpoolCommands : CommandBuilderBase, IZpoolCommands
{
    public ZpoolCommands(IZfsBinaries binaries) : base(binaries)
    {
    }

    /// <summary>
    /// Returns a command that returns command history for the given pool
    /// </summary>
    /// <exception cref="ArgumentException">If arguments are missing required information or is not valid</exception>
    public IProcessCall History(PoolHistoryArgs args)
    {
        if (!args.Validate(out var errors))
        {
            throw ToArgumentException(errors, args);
        }
        return new ProcessCall(Binaries.Zpool, args.ToString());
    }

    /// <summary>
    /// Returns a command to get status for the given pool, i.e. zpool status
    /// </summary>
    /// <exception cref="ArgumentException">If arguments are missing required information or is not valid</exception>
    public IProcessCall GetStatus(PoolStatusArgs args)
    {
        if (!args.Validate(out var errors))
        {
            throw ToArgumentException(errors, args);
        }
        return new ProcessCall(Binaries.Zpool, args.ToString());
    }

    /// <summary>
    /// Returns a command that returns information about all pools
    /// </summary>
    public IProcessCall GetAllPoolInfos()
    {
        return new ProcessCall(Binaries.Zpool, "list -PHp");
    }

    /// <summary>
    /// Returns a command that returns information about specific pools
    /// </summary>
    /// <exception cref="ArgumentException">If arguments are missing required information or is not valid</exception>
    public IProcessCall GetPoolInfo(PoolListArgs args)
    {
        if (!args.Validate(out var errors))
        {
            throw ToArgumentException(errors, args);
        }
        return new ProcessCall(Binaries.Zpool, args.ToString());
    }

    /// <summary>
    /// Builds a command to create a pool with the given arguments
    /// https://openzfs.github.io/openzfs-docs/man/8/zpool-create.8.html
    /// https://openzfs.github.io/openzfs-docs/man/7/zpoolconcepts.7.html
    /// https://openzfs.github.io/openzfs-docs/man/7/zfsprops.7.html
    /// </summary>
    /// <param name="args">The arguments used to create the pool <see cref="PoolCreateArgs"/></param>
    public IProcessCall Create(PoolCreateArgs args)
    {
        if (!args.Validate(out var errors))
        {
            throw ToArgumentException(errors, args);
        }

        return new ProcessCall(Binaries.Zpool, args.ToString());
    }

    /// <summary>
    /// Returns a command that destroys the given pool
    /// </summary>
    /// <exception cref="ArgumentException">If arguments are missing required information or is not valid</exception>
    public IProcessCall Destroy(PoolDestroyArgs args)
    {
        if (!args.Validate(out var errors))
        {
            throw ToArgumentException(errors, args);
        }

        return new ProcessCall(Binaries.Zpool, args.ToString());
    }

    /// <summary>
    /// Builds a command to return basic iostats for the given pool and optinally specific devices
    /// https://openzfs.github.io/openzfs-docs/man/8/zpool-iostat.8.html
    /// </summary>
    /// <param name="args">The arguments for zpool iostat</param>
    /// <exception cref="ArgumentException">If arguments are missing required information</exception>
    public IProcessCall IoStats(PoolIOStatsArgs args)
    {
        if (!args.Validate(out var errors))
        {
            throw ToArgumentException(errors, args);
        }

        return new ProcessCall(Binaries.Zpool, args.ToString());
    }

    /// <summary>
    /// Returns a command to bring a device offline
    /// </summary>
    /// <param name="args">The arguments to zpool offline</param>
    /// <exception cref="ArgumentException">If arguments are missing required information</exception>
    public IProcessCall Offline(PoolOfflineArgs args)
    {
        if (!args.Validate(out var errors))
        {
            throw ToArgumentException(errors, args);
        }

        return new ProcessCall(Binaries.Zpool, args.ToString());
    }

    /// <summary>
    /// Returns a command to bring a device online
    /// </summary>
    /// <param name="args">The arguments to zpool online</param>
    /// <exception cref="ArgumentException">If arguments are missing required information</exception>
    public IProcessCall Online(PoolOnlineArgs args)
    {
        if (!args.Validate(out var errors))
        {
            throw ToArgumentException(errors, args);
        }

        return new ProcessCall(Binaries.Zpool, args.ToString());
    }

    /// <summary>
    /// Returns a command to clear device errors in ZFS storage pool.
    /// </summary>
    /// <exception cref="ArgumentException">If arguments are missing required information</exception>
    public IProcessCall Clear(PoolClearArgs args)
    {
        if (!args.Validate(out var errors))
        {
            throw ToArgumentException(errors, args);
        }

        return new ProcessCall(Binaries.Zpool, args.ToString());
    }
    /// <summary>
    /// Returns a resilver command for the given pool
    /// </summary>
    /// <exception cref="ArgumentException">If arguments are missing required information</exception>
    public IProcessCall Resilver(PoolResilverArgs args)
    {
        if (!args.Validate(out var errors))
        {
            throw ToArgumentException(errors, args);
        }
        return new ProcessCall(Binaries.Zpool, args.ToString());
    }

    /// <summary>
    /// returns a scrub command for the given pool
    /// </summary>
    /// <exception cref="ArgumentException">If the pool name is null or empty</exception>
    public IProcessCall Scrub(PoolScrubArgs args)
    {
        if (!args.Validate(out var errors))
        {
            throw ToArgumentException(errors, args);
        }
        return new ProcessCall(Binaries.Zpool, args.ToString());
    }

    /// <summary>
    /// Returns a trim command for the given pool and optional device
    /// </summary>
    /// <exception cref="ArgumentException">If arguments are missing required information</exception>
    public IProcessCall Trim(PoolTrimArgs args)
    {
        if (!args.Validate(out var errors))
        {
            throw ToArgumentException(errors, args);
        }

        return new ProcessCall(Binaries.Zpool, args.ToString());
    }

    /// <summary>
    /// Returns a command that show what pools can be upgraded and what missing features they lack
    /// </summary>
    public IProcessCall GetUpgradeablePools()
    {
        return new ProcessCall(Binaries.Zpool, "upgrade");
    }

    /// <summary>
    /// Returns a command to upgrade either one pool to all feature flags, or all pools
    /// </summary>
    /// <exception cref="ArgumentException">If arguments are missing required information or is not valid</exception>
    public IProcessCall Upgrade(PoolUpgradeArgs args)
    {
        if (!args.Validate(out var errors))
        {
            throw ToArgumentException(errors, args);
        }

        return new ProcessCall(Binaries.Zpool, args.ToString());
    }

    /// <summary>
    /// Returns a command to detach a device from a mirror
    /// </summary>
    /// <exception cref="ArgumentException">If arguments are missing required information or is not valid</exception>
    public IProcessCall Detach(PoolDetachArgs args)
    {
        if (!args.Validate(out var errors))
        {
            throw ToArgumentException(errors, args);
        }
        return new ProcessCall(Binaries.Zpool, args.ToString());
    }

    /// <summary>
    /// Returns a command to attach a device to a vdev
    /// </summary>
    /// <exception cref="ArgumentException">If arguments are missing required information or is not valid</exception>
    public IProcessCall Attach(PoolAttachArgs args)
    {
        if (!args.Validate(out var errors))
        {
            throw ToArgumentException(errors, args);
        }
        return new ProcessCall(Binaries.Zpool, args.ToString());
    }

    /// <summary>
    /// Returns a command to replace a device to a vdev
    /// </summary>
    /// <exception cref="ArgumentException">If arguments are missing required information or is not valid</exception>
    public IProcessCall Replace(PoolReplaceArgs args)
    {
        if (!args.Validate(out var errors))
        {
            throw ToArgumentException(errors, args);
        }
        return new ProcessCall(Binaries.Zpool, args.ToString());
    }
    /// <summary>
    /// Returns a command to add a vdev to a given pool
    /// </summary>
    /// <exception cref="ArgumentException">If arguments are missing required information or is not valid</exception>
    public IProcessCall Add(PoolAddArgs addArgs)
    {
        if (!addArgs.Validate(out var errors))
        {
            throw ToArgumentException(errors, addArgs);
        }
        return new ProcessCall(Binaries.Zpool, addArgs.ToString());
    }

    /// <summary>
    /// Returns a command to remove a vdev from a pool
    /// </summary>
    /// <exception cref="ArgumentException">If arguments are missing required information or is not valid</exception>
    public IProcessCall Remove(PoolRemoveArgs removeArgs)
    {
        if (!removeArgs.Validate(out var errors))
        {
            throw ToArgumentException(errors, removeArgs);
        }
        return new ProcessCall(Binaries.Zpool, removeArgs.ToString());
    }
}