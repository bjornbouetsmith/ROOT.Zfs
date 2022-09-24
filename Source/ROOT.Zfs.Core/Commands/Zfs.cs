using ROOT.Shared.Utils.OS;

namespace ROOT.Zfs.Core.Commands
{
    public static class Zfs
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
