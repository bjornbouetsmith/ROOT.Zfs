using System;
using System.Text;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Core.Commands
{
    public class ZpoolCommands : BaseCommands
    {
        public static ProcessCall GetHistory(string pool)
        {
            return new ProcessCall(WhichZpool, $"history -l {pool}");
        }

        public static ProcessCall GetStatus(string pool)
        {
            return new ProcessCall(WhichZpool, $"status -vP {pool}");
        }
        /// <summary>
        /// https://openzfs.github.io/openzfs-docs/man/8/zpool-list.8.html
        /// </summary>
        /// <returns></returns>
        public static ProcessCall GetAllPoolInfos()
        {
            return new ProcessCall(WhichZpool, "list -PH");
        }
        /// <summary>
        /// https://openzfs.github.io/openzfs-docs/man/8/zpool-list.8.html
        /// </summary>
        /// <param name="pool"></param>
        /// <returns></returns>
        public static ProcessCall GetPoolInfo(string pool)
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
        public static ProcessCall CreatePool(PoolCreationArgs args)
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
        /// <returns></returns>
        public static ProcessCall DestroyPool(string pool)
        {
            return new ProcessCall(WhichZpool, $"destroy -f {pool}");
        }
    }
}
