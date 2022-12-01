using System;
using System.Collections.Generic;
using System.Linq;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Arguments.Pool;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.Pools;
using ROOT.Zfs.Public.Data.Statistics;

namespace ROOT.Zfs.Core
{
    /// <inheritdoc cref="IZPool" />
    internal class ZPool : ZfsBase, IZPool
    {
        public ZPool(IProcessCall remoteConnection) : base(remoteConnection)
        {
        }
        
        public IEnumerable<CommandHistory> History(PoolHistoryArgs args)
        {
            var pc = BuildCommand(ZpoolCommands.History(args));

            var response = pc.LoadResponse(true);

            return CommandHistoryHelper.FromStdOut(response.StdOut, args.SkipLines, args.AfterDate);
        }

        /// <inheritdoc />
        public PoolStatus Status(PoolStatusArgs args)
        {
            var pc = BuildCommand(ZpoolCommands.GetStatus(args));
            var response = pc.LoadResponse(true);

            return ZPoolStatusParser.Parse(response.StdOut);
        }

        /// <inheritdoc />
        public IList<PoolInfo> List()
        {
            var pc = BuildCommand(ZpoolCommands.GetAllPoolInfos());
            var response = pc.LoadResponse(true);
            
            var list = ZPoolInfoParser.ParseFromStdOut(response.StdOut);
            var versionCommand = BuildCommand(ZdbCommands.GetRawZdbOutput());
            response = versionCommand.LoadResponse(true);
            var versions = ZdbHelper.ParsePoolVersions(response.StdOut);

            foreach (var info in list)
            {
                // Should be safe to do a .First, since zfs should contain data about the pool if the pool exist
                var versionInfo = versions.First(v => v.Name.Equals(info.Name, StringComparison.OrdinalIgnoreCase));
                info.Version = versionInfo.Version;
            }

            return list;
        }

        /// <inheritdoc />
        public PoolInfo List(PoolListArgs args)
        {
            var pc = BuildCommand(ZpoolCommands.GetPoolInfo(args));
            var response = pc.LoadResponse(true);

            var info = ZPoolInfoParser.ParseLine(response.StdOut);

            var versionCommand = BuildCommand(ZdbCommands.GetRawZdbOutput());
            response = versionCommand.LoadResponse(true);

            var versions = ZdbHelper.ParsePoolVersions(response.StdOut);
            // Should be safe to do a .First, since zfs should contain data about the pool if the pool exist
            var versionInfo = versions.First(v=>v.Name.Equals(args.Name, StringComparison.OrdinalIgnoreCase));
            info.Version = versionInfo.Version;

            return info;
        }

        /// <inheritdoc />
        public PoolStatus Create(PoolCreateArgs args)
        {
            var pc = BuildCommand(ZpoolCommands.Create(args));
            pc.LoadResponse(true);
            var statusArg = new PoolStatusArgs { Name = args.Name };
            
            return Status(statusArg);
        }

        /// <inheritdoc />
        public void Destroy(PoolDestroyArgs args)
        {
            var pc = BuildCommand(ZpoolCommands.Destroy(args));
            pc.LoadResponse(true);
        }

        /// <inheritdoc />
        public void Offline(PoolOfflineArgs args)
        {
            var pc = BuildCommand(ZpoolCommands.Offline(args));
            pc.LoadResponse(true);
        }

        /// <inheritdoc />
        public void Online(PoolOnlineArgs args)
        {
            var pc = BuildCommand(ZpoolCommands.Online(args));
            pc.LoadResponse(true);
        }

        /// <inheritdoc />
        public void Clear(PoolClearArgs args)
        {
            var pc = BuildCommand(ZpoolCommands.Clear(args));
            pc.LoadResponse(true);
        }

        /// <inheritdoc />
        public IOStats IOStats(string pool, string[] devices)
        {
            var pc = BuildCommand(ZpoolCommands.IoStats(pool, devices));
            var response = pc.LoadResponse(true);
            return ZPoolIOStatParser.ParseStdOut(pool, response.StdOut);
        }

        /// <inheritdoc />
        public void Resilver(PoolResilverArgs args)
        {
            var pc = BuildCommand(ZpoolCommands.Resilver(args));
            pc.LoadResponse(true);
        }

        /// <inheritdoc />
        public void Scrub(PoolScrubArgs args)
        {
            var pc = BuildCommand(ZpoolCommands.Scrub(args));
            pc.LoadResponse(true);
        }

        /// <inheritdoc />
        public void Trim(PoolTrimArgs args)
        {
            var pc = BuildCommand(ZpoolCommands.Trim(args));
            pc.LoadResponse(true);
        }

        /// <inheritdoc />
        public IList<UpgradeablePool> GetUpgradeablePools()
        {
            var pc = BuildCommand(ZpoolCommands.GetUpgradeablePools());
            var response = pc.LoadResponse(true);
            return UpgradeablePoolsParser.ParseStdOut(response.StdOut);
        }

        /// <inheritdoc />
        public void Upgrade(PoolUpgradeArgs args)
        {
            var pc = BuildCommand(ZpoolCommands.Upgrade(args));
            pc.LoadResponse(true);
        }
        
        public void Detach(PoolDetachArgs args)
        {
            var pc = BuildCommand(ZpoolCommands.Detach(args));
            pc.LoadResponse(true);
        }

        /// <inheritdoc />
        public void Attach(PoolAttachArgs args)
        {
            var pc = BuildCommand(ZpoolCommands.Attach(args));
            pc.LoadResponse(true);
        }

        /// <inheritdoc />
        public void Replace(PoolReplaceArgs args)
        {
            var pc = BuildCommand(ZpoolCommands.Replace(args));
            pc.LoadResponse(true);
        }
        /// <inheritdoc />
        public void Add(PoolAddArgs args)
        {
            var pc = BuildCommand(ZpoolCommands.Add(args));
            pc.LoadResponse(true);
        }

        public void Remove(PoolRemoveArgs args)
        {
            var pc = BuildCommand(ZpoolCommands.Remove(args));
            pc.LoadResponse(true);
        }
    }
}
