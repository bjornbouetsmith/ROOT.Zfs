using ROOT.Shared.Utils.OS;

namespace ROOT.Zfs.Core.Commands
{
    public class ZpoolCommands : BaseCommands
    {
        public static ProcessCall GetHistory(string pool)
        {
            return new ProcessCall(WhichZpool, $"history -l {pool}");
        }

        public static ProcessCall GetStatus(string pool)
        {
            return new ProcessCall(WhichZpool, $"status -vP {pool}");
        }

        public static ProcessCall GetAllPoolInfos()
        {
            return new ProcessCall(WhichZpool, "list -vP");
        }

        public static ProcessCall GetPoolInfo(string pool)
        {
            return new ProcessCall(WhichZpool, $"list -vP {pool}");
        }
    }
}
