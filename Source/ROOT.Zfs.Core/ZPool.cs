using System;
using System.Collections.Generic;
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
        public IEnumerable<PoolInfo> GetAllPoolInfos()
        {
            var pc = BuildCommand(ZpoolCommands.GetAllPoolInfos());
            var response = pc.LoadResponse(true);

            return ZPoolInfoParser.ParseFromStdOut(response.StdOut);
        }

        /// <inheritdoc />
        public PoolInfo GetPoolInfo(string pool)
        {
            var pc = BuildCommand(ZpoolCommands.GetPoolInfo(pool));
            var response = pc.LoadResponse(true);

            return ZPoolInfoParser.ParseLine(response.StdOut);
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
        public IOStats GetIOStats(string pool, string[] devices, bool includeAverageLatency)
        {
            var pc = BuildCommand(ZpoolCommands.IoStat(pool, devices, includeAverageLatency));
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

        public void Trim(ZpoolTrimArgs args)
        {
            var pc = BuildCommand(ZpoolCommands.Trim(args));
            pc.LoadResponse(true);
        }
    }
}
