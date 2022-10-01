using ROOT.Shared.Utils.OS;

namespace ROOT.Zfs.Core.Commands
{
    public class ZfsCommands : BaseCommands
    {
        public static ProcessCall GetVersion()
        {
            return new ProcessCall(WhichZfs, "--version");
        }
    }
}
