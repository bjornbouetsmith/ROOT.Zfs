using System;
using System.Text;
using ROOT.Shared.Utils.OS;
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
            return new ProcessCall(WhichZpool, "list -PH");
        }
        /// <summary>
        /// https://openzfs.github.io/openzfs-docs/man/8/zpool-list.8.html
        /// </summary>
        /// <param name="pool"></param>
        /// <returns></returns>
        internal static ProcessCall GetPoolInfo(string pool)
        {
            return new ProcessCall(WhichZpool, $"list -PH {pool}");
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
            if (!args.Validate(out var errorMessage))
            {
                throw new ArgumentException(errorMessage, nameof(args));
            }

            StringBuilder command = new StringBuilder($"create {args.Name}");


            if (!string.IsNullOrEmpty(args.MountPoint))
            {
                command.Append($" -m {args.MountPoint}");
            }

            if (args.PoolProperties != null && args.PoolProperties.Length > 0)
            {
                foreach (var property in args.PoolProperties)
                {
                    command.Append($" -o {property.Property}={property.Value}");
                }
            }

            if (args.FileSystemProperties != null && args.FileSystemProperties.Length > 0)
            {
                foreach (var property in args.FileSystemProperties)
                {
                    command.Append($" -O {property.Property}={property.Value}");
                }
            }

            foreach (var vdevArg in args.VDevs)
            {
                if (!vdevArg.Validate(out errorMessage))
                {
                    throw new ArgumentException(errorMessage, nameof(args));
                }

                command.Append(" " + vdevArg);
            }

            return new ProcessCall(WhichZpool, command.ToString());
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
        /// <param name="includeAverageLatency">Whether to include average latency stats as well (-l)</param>
        internal static ProcessCall IoStat(string pool, string[] devices, bool includeAverageLatency)
        {
            var deviceList = devices != null && devices.Length > 0 ? string.Join(" ", devices) : string.Empty;
            if (!string.IsNullOrEmpty(deviceList))
            {
                deviceList = " " + deviceList;
            }

            var includeLatency = (includeAverageLatency ? "l" : "");
            return new ProcessCall(WhichZpool, $"iostat -LPv{includeLatency} {pool}{deviceList}");
        }

        /// <summary>
        /// Takes the specified physical device offline. While the device is offline, no attempt is made to read or write to the device.
        /// This command is not applicable to spares.
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="device"></param>
        /// <param name="forceFault">Force fault. Instead of offlining the disk, put it into a faulted state.
        /// The fault will persist across imports unless <paramref name="temporary"/> is true.</param>
        /// <param name="temporary">Temporary. Upon reboot, the specified physical device reverts to its previous state.</param>
        internal static ProcessCall Offline(string pool, string device, bool forceFault, bool temporary)
        {
            var args = new StringBuilder();
            if (forceFault)
            {
                args.Append(" -f");
            }

            if (temporary)
            {
                args.Append(" -t");
            }

            return new ProcessCall(WhichZpool, $"offline {pool} {device}{args}");
        }

        /// <summary>
        /// Brings the specified physical device online.
        /// This command is not applicable to spares.
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="device"></param>
        /// <param name="expandSpace"></param>
        /// <returns></returns>
        internal static ProcessCall Online(string pool, string device, bool expandSpace)
        {
            return new ProcessCall(WhichZpool, $"online {pool} {device}{(expandSpace ? " -e" : "")}");
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
    }
}
