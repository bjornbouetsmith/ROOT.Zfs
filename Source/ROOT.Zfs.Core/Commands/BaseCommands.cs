using System.Collections.Generic;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Helpers;

namespace ROOT.Zfs.Core.Commands
{
    public class BaseCommands
    {
        public static string WhichZfs { get; set; }="/sbin/zfs";
        public static string WhichZpool { get; set; } = "/sbin/zpool";

        /// <summary>
        /// Lists the given types
        /// Root only have any effect for snapshots for wildcard purposes- unless only a single record is wanted
        /// i.e.
        /// ListTypes.Snapshot & root 'tank' will list snapshots in the root tank - but ListTypes.FileSystem & tank, will only return tank
        /// </summary>
        /// <param name="listtypes">The types to return</param>
        /// <param name="root">The root to list types for - or the single element wanted</param>
        /// <returns></returns>
        public static ProcessCall ZfsList(ListTypes listtypes, string root)
        {
            return BuildZfsListCommand(listtypes, root);
        }

        private static ProcessCall BuildZfsListCommand(ListTypes listtypes, string root)
        {
            string command = "list -Hpr -o type,creation,name,used,refer,avail,mountpoint";

            // TODO: optimize this, so we have a cached version of ListTypes -> strings - including combinations
            List<string> types = new();

            if ((listtypes & ListTypes.Bookmark) != 0)
            {
                types.Add("bookmark");
            }
            if ((listtypes & ListTypes.FileSystem) != 0)
            {
                types.Add("filesystem");
            }
            if ((listtypes & ListTypes.Snapshot) != 0)
            {
                types.Add("snapshot");
            }
            if ((listtypes & ListTypes.Volume) != 0)
            {
                types.Add("volume");
            }

            if (types.Count > 0)
            {
                command += " -t " + string.Join(",", types);
            }
            
            if (!string.IsNullOrEmpty(root))
            {
                // just to be safe
                root = DataSetHelper.Decode(root);
                command += $" {root}";
            }

            return new ProcessCall(WhichZfs, command);
        }
    }
}
