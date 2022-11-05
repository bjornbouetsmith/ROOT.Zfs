using ROOT.Shared.Utils.OS;

namespace ROOT.Zfs.Core.Commands
{
    internal class ZdbCommands : Commands
    {
        /// <summary>
        /// Returns a command that will output from zdb
        /// </summary>
        public static IProcessCall GetRawZdbOutput()
        {
            return new ProcessCall(WhichZdb);
        }
    }
}
