using ROOT.Shared.Utils.OS;

namespace ROOT.Zfs.Core.Commands
{
    internal class ZfsCommands : Commands
    {
        public static ProcessCall GetVersion()
        {
            return new ProcessCall(WhichZfs, "--version");
        }
    }
}
