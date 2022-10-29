using System;
using System.Collections.Generic;
using System.Linq;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Arguments;
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

        /// <inheritdoc />
        public IEnumerable<CommandHistory> GetHistory(string pool, int skipLines = 0, DateTime afterDate = default)
        {
            var pc = BuildCommand(ZpoolCommands.GetHistory(pool));

            var response = pc.LoadResponse(true);

            return CommandHistoryHelper.FromStdOut(response.StdOut, skipLines, afterDate);
        }

        /// <inheritdoc />
        public PoolStatus GetStatus(string pool)
        {
            var pc = BuildCommand(ZpoolCommands.GetStatus(pool));
            var response = pc.LoadResponse(true);

            return ZPoolStatusParser.Parse(response.StdOut);
        }

        /// <inheritdoc />
        public IList<PoolInfo> GetAllPoolInfos()
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
        public PoolInfo GetPoolInfo(string pool)
        {
            var pc = BuildCommand(ZpoolCommands.GetPoolInfo(pool));
            var response = pc.LoadResponse(true);

            var info = ZPoolInfoParser.ParseLine(response.StdOut);

            var versionCommand = BuildCommand(ZdbCommands.GetRawZdbOutput());
            response = versionCommand.LoadResponse(true);

            var versions = ZdbHelper.ParsePoolVersions(response.StdOut);
            // Should be safe to do a .First, since zfs should contain data about the pool if the pool exist
            var versionInfo = versions.First(v=>v.Name.Equals(pool, StringComparison.OrdinalIgnoreCase));
            info.Version = versionInfo.Version;

            return info;
        }

        /// <inheritdoc />
        public PoolStatus CreatePool(PoolCreationArgs args)
        {
            var pc = BuildCommand(ZpoolCommands.CreatePool(args));
            pc.LoadResponse(true);

            return GetStatus(args.Name);
        }

        /// <inheritdoc />
        public void DestroyPool(string pool)
        {
            var pc = BuildCommand(ZpoolCommands.DestroyPool(pool));
            pc.LoadResponse(true);
        }

        /// <inheritdoc />
        public void Offline(string pool, string device, bool forceFault, bool temporary)
        {
            var pc = BuildCommand(ZpoolCommands.Offline(pool, device, forceFault, temporary));
            pc.LoadResponse(true);
        }

        /// <inheritdoc />
        public void Online(string pool, string device, bool expandSpace)
        {
            var pc = BuildCommand(ZpoolCommands.Online(pool, device, expandSpace));
            pc.LoadResponse(true);
        }

        /// <inheritdoc />
        public void Clear(string pool, string device)
        {
            var pc = BuildCommand(ZpoolCommands.Clear(pool, device));
            pc.LoadResponse(true);
        }

        /// <inheritdoc />
        public IOStats GetIOStats(string pool, string[] devices)
        {
            var pc = BuildCommand(ZpoolCommands.IoStats(pool, devices));
            var response = pc.LoadResponse(true);
            return ZPoolIOStatParser.ParseStdOut(pool, response.StdOut);
        }

        /// <inheritdoc />
        public void Resilver(string pool)
        {
            var pc = BuildCommand(ZpoolCommands.Resilver(pool));
            pc.LoadResponse(true);
        }

        /// <inheritdoc />
        public void Scrub(string pool, ScrubOption option)
        {
            var pc = BuildCommand(ZpoolCommands.Scrub(pool, option));
            pc.LoadResponse(true);
        }

        /// <inheritdoc />
        public void Trim(ZpoolTrimArgs args)
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
        public void Upgrade(ZpoolUpgradeArgs args)
        {
            var pc = BuildCommand(ZpoolCommands.Upgrade(args));
            pc.LoadResponse(true);
        }

        /// <inheritdoc />
        public void Detach(string pool, string device)
        {
            var pc = BuildCommand(ZpoolCommands.Detach(pool, device));
            pc.LoadResponse(true);
        }
    }
}
