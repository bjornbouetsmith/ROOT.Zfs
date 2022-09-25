using ROOT.Shared.Utils.OS;

namespace ROOT.Zfs.Core.Commands
{
    public static class ZpoolCommands
    {
        public static ProcessCall GetHistory(string pool)
        {
            return new ProcessCall("/sbin/zpool", $"history -l {pool}");
        }
    }
}
