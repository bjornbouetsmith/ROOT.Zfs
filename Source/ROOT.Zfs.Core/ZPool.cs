using System;
using System.Collections.Generic;
using System.Linq;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Core
{
    internal class ZPool : ZfsBase, IZPool
    {
        public ZPool(IProcessCall remoteConnection) : base(remoteConnection)
        {
        }

        public IEnumerable<CommandHistory> GetHistory(string pool, int skipLines = 0, DateTime afterDate = default)
        {
            var pc = BuildCommand(ZpoolCommands.GetHistory(pool));

            var response = pc.LoadResponse();
            if (!response.Success)
            {
                throw response.ToException();
            }

            return CommandHistoryHelper.FromStdOut(response.StdOut, skipLines, afterDate);
        }

        public PoolStatus GetStatus(string pool)
        {
            var pc = BuildCommand(ZpoolCommands.GetStatus(pool));
            var response = pc.LoadResponse();
            if (!response.Success)
            {
                throw response.ToException();
            }

            return ZPoolStatusParser.Parse(response.StdOut);
        }

        public IEnumerable<PoolInfo> GetAllPoolInfos()
        {
            var pc = BuildCommand(ZpoolCommands.GetAllPoolInfos());
            var response = pc.LoadResponse();
            if (!response.Success)
            {
                throw response.ToException();
            }
            // TODO: Implement this
            return Enumerable.Empty<PoolInfo>();
        }

        public PoolInfo GetPoolInfo(string pool)
        {
            var pc = BuildCommand(ZpoolCommands.GetPoolInfo(pool));
            var response = pc.LoadResponse();
            if (!response.Success)
            {
                throw response.ToException();
            }

            return null;
        }

        public PoolStatus CreatePool(PoolCreationArgs args)
        {
            var pc = BuildCommand(ZpoolCommands.CreatePool(args));
            var response = pc.LoadResponse();
            if (!response.Success)
            {
                throw response.ToException();
            }

            return GetStatus(args.Name);
        }

        public void DestroyPool(string pool)
        {
            var pc = BuildCommand(ZpoolCommands.DestroyPool(pool));
            var response = pc.LoadResponse();
            if (!response.Success)
            {
                throw response.ToException();
            }
        }
    }
}
