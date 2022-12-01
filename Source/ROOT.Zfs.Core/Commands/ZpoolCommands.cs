using System;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Public.Arguments.Pool;

namespace ROOT.Zfs.Core.Commands
{
    internal class ZpoolCommands : Commands
    {
        internal static IProcessCall History(PoolHistoryArgs args)
        {
            if (!args.Validate(out var errors))
            {
                throw ToArgumentException(errors, args);
            }
            return new ProcessCall(WhichZpool, args.ToString());
        }

        internal static ProcessCall GetStatus(PoolStatusArgs args)
        {
            if (!args.Validate(out var errors))
            {
                throw ToArgumentException(errors, args);
            }
            return new ProcessCall(WhichZpool, args.ToString());
        }
        /// <summary>
        /// https://openzfs.github.io/openzfs-docs/man/8/zpool-list.8.html
        /// </summary>
        /// <returns></returns>
        internal static ProcessCall GetAllPoolInfos()
        {
            return new ProcessCall(WhichZpool, "list -PHp");
        }
        /// <summary>
        /// https://openzfs.github.io/openzfs-docs/man/8/zpool-list.8.html
        /// </summary>
        internal static ProcessCall GetPoolInfo(PoolListArgs args)
        {
            if (!args.Validate(out var errors))
            {
                throw ToArgumentException(errors, args);
            }
            return new ProcessCall(WhichZpool, args.ToString());
        }

        /// <summary>
        /// Builds a command to create a pool with the given arguments
        /// https://openzfs.github.io/openzfs-docs/man/8/zpool-create.8.html
        /// https://openzfs.github.io/openzfs-docs/man/7/zpoolconcepts.7.html
        /// https://openzfs.github.io/openzfs-docs/man/7/zfsprops.7.html
        /// </summary>
        /// <param name="args">The arguments used to create the pool <see cref="PoolCreateArgs"/></param>
        internal static ProcessCall Create(PoolCreateArgs args)
        {
            if (!args.Validate(out var errors))
            {
                throw ToArgumentException(errors, args);
            }

            return new ProcessCall(WhichZpool, args.ToString());
        }
        /// <summary>
        /// https://openzfs.github.io/openzfs-docs/man/8/zpool-destroy.8.html
        /// </summary>
        internal static ProcessCall Destroy(PoolDestroyArgs args)
        {
            if (!args.Validate(out var errors))
            {
                throw ToArgumentException(errors, args);
            }

            return new ProcessCall(WhichZpool, args.ToString());
        }

        /// <summary>
        /// Builds a command to return basic iostats for the given pool and optinally specific devices
        /// https://openzfs.github.io/openzfs-docs/man/8/zpool-iostat.8.html
        /// </summary>
        /// <param name="args">The arguments for zpool iostat</param>
        /// <exception cref="ArgumentException">If arguments are missing required information</exception>
        internal static ProcessCall IoStats(PoolIOStatsArgs args)
        {
            if (!args.Validate(out var errors))
            {
                throw ToArgumentException(errors, args);
            }

            return new ProcessCall(WhichZpool, args.ToString());
        }

        /// <summary>
        /// Returns a command to bring a device offline
        /// </summary>
        /// <param name="args">The arguments to zpool offline</param>
        /// <exception cref="ArgumentException">If arguments are missing required information</exception>
        internal static IProcessCall Offline(PoolOfflineArgs args)
        {
            if (!args.Validate(out var errors))
            {
                throw ToArgumentException(errors, args);
            }

            return new ProcessCall(WhichZpool, args.ToString());
        }

        /// <summary>
        /// Returns a command to bring a device online
        /// </summary>
        /// <param name="args">The arguments to zpool online</param>
        /// <exception cref="ArgumentException">If arguments are missing required information</exception>
        internal static IProcessCall Online(PoolOnlineArgs args)
        {
            if (!args.Validate(out var errors))
            {
                throw ToArgumentException(errors, args);
            }

            return new ProcessCall(WhichZpool, args.ToString());
        }

        /// <summary>
        /// Returns a command to clear device errors in ZFS storage pool.
        /// </summary>
        /// <exception cref="ArgumentException">If arguments are missing required information</exception>
        internal static ProcessCall Clear(PoolClearArgs args)
        {
            if (!args.Validate(out var errors))
            {
                throw ToArgumentException(errors, args);
            }

            return new ProcessCall(WhichZpool, args.ToString());
        }
        /// <summary>
        /// Returns a resilver command for the given pool
        /// </summary>
        /// <exception cref="ArgumentException">If arguments are missing required information</exception>
        public static IProcessCall Resilver(PoolResilverArgs args)
        {
            if (!args.Validate(out var errors))
            {
                throw ToArgumentException(errors, args);
            }
            return new ProcessCall(WhichZpool, args.ToString());
        }

        /// <summary>
        /// returns a scrub command for the given pool
        /// </summary>
        /// <exception cref="ArgumentException">If the pool name is null or empty</exception>
        public static IProcessCall Scrub(PoolScrubArgs args)
        {
            if (!args.Validate(out var errors))
            {
                throw ToArgumentException(errors, args);
            }
            return new ProcessCall(WhichZpool, args.ToString());
        }

        /// <summary>
        /// Returns a trim command for the given pool and optional device
        /// </summary>
        /// <exception cref="ArgumentException">If arguments are missing required information</exception>
        public static IProcessCall Trim(PoolTrimArgs args)
        {
            if (!args.Validate(out var errors))
            {
                throw ToArgumentException(errors, args);
            }

            return new ProcessCall(WhichZpool, args.ToString());
        }

        /// <summary>
        /// Returns a command that show what pools can be upgraded and what missing features they lack
        /// </summary>
        public static IProcessCall GetUpgradeablePools()
        {
            return new ProcessCall(WhichZpool, "upgrade");
        }

        /// <summary>
        /// Returns a command to upgrade either one pool to all feature flags, or all pools
        /// </summary>
        public static IProcessCall Upgrade(PoolUpgradeArgs args)
        {
            if (!args.Validate(out var errors))
            {
                throw ToArgumentException(errors, args);
            }

            return new ProcessCall(WhichZpool, args.ToString());
        }
        
        /// <summary>
        /// Returns a command to detach a device from a mirror
        /// </summary>
        public static IProcessCall Detach(PoolDetachArgs args)
        {
            if (!args.Validate(out var errors))
            {
                throw ToArgumentException(errors, args);
            }
            return new ProcessCall(WhichZpool, args.ToString());
        }

        /// <summary>
        /// Returns a command to attach a device to a vdev
        /// </summary>
        public static IProcessCall Attach(PoolAttachArgs args)
        {
            if (!args.Validate(out var errors))
            {
                throw ToArgumentException(errors, args);
            }
            return new ProcessCall(WhichZpool, args.ToString());
        }

        /// <summary>
        /// Returns a command to replace a device to a vdev
        /// </summary>
        public static IProcessCall Replace(PoolReplaceArgs args)
        {
            if (!args.Validate(out var errors))
            {
                throw ToArgumentException(errors, args);
            }
            return new ProcessCall(WhichZpool, args.ToString());
        }

        public static IProcessCall Add(PoolAddArgs addArgs)
        {
            if (!addArgs.Validate(out var errors))
            {
                throw ToArgumentException(errors, addArgs);
            }
            return new ProcessCall(WhichZpool, addArgs.ToString());
        }

        public static IProcessCall Remove(PoolRemoveArgs removeArgs)
        {
            if (!removeArgs.Validate(out var errors))
            {
                throw ToArgumentException(errors, removeArgs);
            }
            return new ProcessCall(WhichZpool, removeArgs.ToString());
        }
    }
}
