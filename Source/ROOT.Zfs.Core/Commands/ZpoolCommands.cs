using ROOT.Shared.Utils.OS;

namespace ROOT.Zfs.Core.Commands
{
    public static class ZpoolCommands
    {
        public static ProcessCall GetHistory(string pool)
        {
            return new ProcessCall("/sbin/zpool", $"history -l {pool}");
        }

        public static ProcessCall GetStatus(string pool)
        {
            return new ProcessCall("/sbin/zpool", $"status -vP {pool}");
        }

        public static ProcessCall GetPools()
        {
            return new ProcessCall("/sbin/zpool", "list -vP");
        }

        public static ProcessCall GetPoolInfo(string pool)
        {
            return new ProcessCall("/sbin/zpool", $"list -vP {pool}");
        }
    }
}
