using ROOT.Shared.Utils.OS;

namespace ROOT.Zfs.Core.Commands
{
    public static class ZfsCommands
    {
        public static class ProcessCalls
        {
            public static ProcessCall GetVersion()
            {
                return new ProcessCall("/sbin/zfs", "--version");
            }
        }
    }
}
