using System;
using System.Collections.Generic;
using System.Text;
using ROOT.Shared.Utils.OS;

namespace ROOT.Zfs.Core.Commands
{
    public static class DataSets
    {
        public static class ProcessCalls
        {
            public static ProcessCall GetDataSets()
            {
                return new ProcessCall("/sbin/zfs", "list");
            }
        }
    }
}
