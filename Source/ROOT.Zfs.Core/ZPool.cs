using System;
using System.Collections.Generic;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Info;

namespace ROOT.Zfs.Core
{
    public class ZPool : ZfsBase
    {
        public ZPool(RemoteProcessCall remoteConnection) : base(remoteConnection)
        {
        }

        public IEnumerable<CommandHistory> GetHistory(string pool, int skipLines = 0, DateTime afterDate = default, ProcessCall previousCall = null)
        {
            var pc = BuildCommand(Commands.ZPool.GetHistory(pool), previousCall);

            var response = pc.LoadResponse();
            if (!response.Success)
            {
                throw response.ToException();
            }

            return CommandHistory.FromStdOut(response.StdOut, skipLines, afterDate);

        }
    }
}
