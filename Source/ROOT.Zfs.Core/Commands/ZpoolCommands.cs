using System;
using System.Text;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Public.Arguments;
using ROOT.Zfs.Public.Arguments.Pool;
using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Core.Commands
{
    internal class ZpoolCommands : BaseCommands
    {
        internal static ProcessCall GetHistory(string pool)
        {
            return new ProcessCall(WhichZpool, $"history -l {pool}");
        }

        internal static ProcessCall GetStatus(string pool)
        {
            return new ProcessCall(WhichZpool, $"status -vP {pool}");
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
        /// <param name="pool"></param>
        /// <returns></returns>
        internal static ProcessCall GetPoolInfo(string pool)
        {
            return new ProcessCall(WhichZpool, $"list -PHp {pool}");
        }

        /// <summary>
        /// Builds a command to create a pool with the given arguments
        /// https://openzfs.github.io/openzfs-docs/man/8/zpool-create.8.html
        /// https://openzfs.github.io/openzfs-docs/man/7/zpoolconcepts.7.html
        /// https://openzfs.github.io/openzfs-docs/man/7/zfsprops.7.html
        /// </summary>
        /// <param name="args">The arguments used to create the pool <see cref="PoolCreationArgs"/></param>
        /// <returns></returns>
        internal static ProcessCall CreatePool(PoolCreationArgs args)
        {
            if (!args.Validate(out var errors))
            {
                var errorMessage = string.Join(Environment.NewLine, errors);
                throw new ArgumentException(errorMessage, nameof(args));
            }

            return new ProcessCall(WhichZpool,$"create{args}");
        }
        /// <summary>
        /// https://openzfs.github.io/openzfs-docs/man/8/zpool-destroy.8.html
        /// </summary>
        /// <param name="pool"></param>
        internal static ProcessCall DestroyPool(string pool)
        {
            return new ProcessCall(WhichZpool, $"destroy -f {pool}");
        }

        /// <summary>
        /// Returns basic iostats for the given pool and optinally specific devices
        /// https://openzfs.github.io/openzfs-docs/man/8/zpool-iostat.8.html
        /// </summary>
        /// <param name="pool">The pool to show stats for</param>
        /// <param name="devices">The devices if any to show stats for</param>
        internal static ProcessCall IoStats(string pool, string[] devices)
        {
            var deviceList = devices != null && devices.Length > 0 ? string.Join(" ", devices) : string.Empty;
            if (!string.IsNullOrWhiteSpace(deviceList))
            {
                deviceList = " " + deviceList;
            }

            return new ProcessCall(WhichZpool, $"iostat -LlpPvH {pool}{deviceList}");
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
                var errorMessage = string.Join(Environment.NewLine, errors);
                throw new ArgumentException(errorMessage, nameof(args));
            }

            return new ProcessCall(WhichZpool, args.BuildArgs("offline"));
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
                var errorMessage = string.Join(Environment.NewLine, errors);
                throw new ArgumentException(errorMessage, nameof(args));
            }

            return new ProcessCall(WhichZpool, args.BuildArgs("online"));
        }

        /// <summary>
        /// clear device errors in ZFS storage pool.
        /// Clears device errors in a pool.
        /// If no arguments are specified, all device errors within the pool are cleared.
        /// If one or more devices is specified, only those errors associated with the specified device or devices are cleared.
        /// If the pool was suspended it will be brought back online provided the devices can be accessed.
        /// Pools with multihost enabled which have been suspended cannot be resumed.
        /// While the pool was suspended, it may have been imported on another host, and resuming I/O could result in pool damage.
        /// </summary>
        /// <param name="pool">The pool for which to clear errors</param>
        /// <param name="device">The devices to clear errors for. This is optional - can be a vdev or a device part of a vdev</param>
        /// <returns></returns>
        internal static ProcessCall Clear(string pool, string device)
        {
            var command = new ProcessCall(WhichZpool, $"clear {pool}");
            if (string.IsNullOrWhiteSpace(device))
            {
                return command;
            }

            return new ProcessCall(WhichZpool, $"clear {pool} {device}");
        }
        /// <summary>
        /// Returns a resilver command for the given pool
        /// </summary>
        public static IProcessCall Resilver(string pool)
        {
            return new ProcessCall(WhichZpool, $"resilver {pool}");
        }

        /// <summary>
        /// returns a scrub command for the given pool
        /// </summary>
        /// <exception cref="ArgumentException">If the pool name is null or empty</exception>
        public static IProcessCall Scrub(string pool, ScrubOption option)
        {
            if (string.IsNullOrWhiteSpace(pool))
            {
                throw new ArgumentException("Please specify a pool to scrub", nameof(pool));
            }

            switch (option)
            {
                case ScrubOption.Pause:
                    return new ProcessCall(WhichZpool, $"scrub -p {pool}");
                case ScrubOption.Stop:
                    return new ProcessCall(WhichZpool, $"scrub -s {pool}");
                default:
                    return new ProcessCall(WhichZpool, $"scrub {pool}");
            }
        }

        /// <summary>
        /// Returns a trim command for the given pool and optional device
        /// </summary>
        /// <exception cref="ArgumentException">If arguments are missing required information</exception>
        public static IProcessCall Trim(PoolTrimArgs args)
        {
            if (!args.Validate(out var errors))
            {
                var errorMessage = string.Join(Environment.NewLine, errors);
                throw new ArgumentException(errorMessage, nameof(args));
            }

            return new ProcessCall(WhichZpool, $"trim{args}");
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
                var errorMessage = string.Join(Environment.NewLine, errors);
                throw new ArgumentException(errorMessage, nameof(args));
            }

            return new ProcessCall(WhichZpool, $"upgrade{args}");
        }

        /// <summary>
        /// Returns a command to detach a device from a mirror
        /// </summary>
        public static IProcessCall Detach(string pool, string device)
        {
            return new ProcessCall(WhichZpool, $"detach {pool} {device}");
        }

        /// <summary>
        /// Returns a command to attach a device to a vdev
        /// </summary>
        public static IProcessCall Attach(PoolAttachReplaceArgs attachArgs)
        {
            if (!attachArgs.Validate(out var errors))
            {
                var errorMessage = string.Join(Environment.NewLine, errors);
                throw new ArgumentException(errorMessage, nameof(attachArgs));
            }
            return new ProcessCall(WhichZpool, $"attach{attachArgs}");
        }

        /// <summary>
        /// Returns a command to replace a device to a vdev
        /// </summary>
        public static IProcessCall Replace(PoolAttachReplaceArgs attachArgs)
        {
            if (!attachArgs.Validate(out var errors))
            {
                var errorMessage = string.Join(Environment.NewLine, errors);
                throw new ArgumentException(errorMessage, nameof(attachArgs));
            }
            return new ProcessCall(WhichZpool, $"replace{attachArgs}");
        }

        public static IProcessCall Add(PoolAddArgs addArgs)
        {
            if (!addArgs.Validate(out var errors))
            {
                var errorMessage = string.Join(Environment.NewLine, errors);
                throw new ArgumentException(errorMessage, nameof(addArgs));
            }
            return new ProcessCall(WhichZpool, addArgs.BuildArgs("add"));
        }

        public static IProcessCall Remove(PoolRemoveArgs removeArgs)
        {
            if (!removeArgs.Validate(out var errors))
            {
                var errorMessage = string.Join(Environment.NewLine, errors);
                throw new ArgumentException(errorMessage, nameof(removeArgs));
            }
            return new ProcessCall(WhichZpool, $"remove{removeArgs}");
        }
    }
}
