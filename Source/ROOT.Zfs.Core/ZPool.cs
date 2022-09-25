using System;
using System.Collections.Generic;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Core
{
    internal class ZPool : ZfsBase, IZPool
    {
        public ZPool(RemoteProcessCall remoteConnection) : base(remoteConnection)
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
    }
}
