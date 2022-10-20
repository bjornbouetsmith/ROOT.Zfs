using System.Collections.Generic;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Core.Commands
{
    /// <summary>
    /// This should be improved, so we can have commands defined per version if required
    /// And a way to register commands for a given version
    /// </summary>
    internal class BaseCommands
    {
        public static string WhichZfs { get; set; } = "/sbin/zfs";
        public static string WhichZpool { get; set; } = "/sbin/zpool";
        public static string WhichZdb { get; set; } = "/sbin/zdb";
        public static string WhichLs { get; set; } = "/bin/ls";
        public static string WhichLsblk { get; set; } = "/bin/lsblk";
        public static string WhichWhich { get; set; } = "/bin/which";
        public static string WhichSmartctl { get; set; } = "/sbin/smartctl";

        /// <summary>
        /// Lists the given types
        /// Root only have any effect for snapshots for wildcard purposes- unless only a single record is wanted
        /// i.e.
        /// ListTypes.Snapshot &amp; root 'tank' will list snapshots in the root tank - but ListTypes.FileSystem &amp; tank, will only return tank
        /// </summary>
        /// <param name="listtypes">The types to return</param>
        /// <param name="root">The root to list types for - or the single element wanted</param>
        /// <param name="includeChildren">Whether or not to include child datasets in the return value</param>
        public static ProcessCall ZfsList(DatasetTypes listtypes, string root, bool includeChildren)
        {
            return BuildZfsListCommand(listtypes, root, includeChildren);
        }

        private static ProcessCall BuildZfsListCommand(DatasetTypes listtypes, string root, bool includeChildren)
        {
            string command = "list -Hpr -o type,creation,name,used,refer,avail,mountpoint";

            if (includeChildren)
            {
                command += " -d 99";
            }

            List<string> types = new();
            if (listtypes == DatasetTypes.None)
            {
                // This is what zfs does, if you do not specify type, you get filesystems and volumnes
                listtypes = DatasetTypes.Filesystem | DatasetTypes.Volume;
            }

            if ((listtypes & DatasetTypes.Bookmark) != 0)
            {
                types.Add("bookmark");
            }
            if ((listtypes & DatasetTypes.Filesystem) != 0)
            {
                types.Add("filesystem");
            }
            if ((listtypes & DatasetTypes.Snapshot) != 0)
            {
                types.Add("snapshot");
            }
            if ((listtypes & DatasetTypes.Volume) != 0)
            {
                types.Add("volume");
            }

            if (types.Count > 0)
            {
                command += " -t " + string.Join(",", types);
            }

            if (!string.IsNullOrWhiteSpace(root))
            {
                // just to be safe
                root = DatasetHelper.Decode(root);
                command += $" {root}";
            }

            return new ProcessCall(WhichZfs, command);
        }

        /// <summary>
        /// Returns disks and partitions from the system by id with their /dev/xx name
        /// i.e.
        /// ata-QEMU_HARDDISK_QM00013 ../../sdg
        /// ata-QEMU_HARDDISK_QM00014 ../../sdg
        /// 
        /// </summary>
        /// <returns></returns>
        public static ProcessCall ListDisks()
        {
            return new ProcessCall(WhichLs, "-l /dev/disk/by-id/ | awk -F ' ' '{print $9,$11}'");
        }

        /// <summary>
        /// Returns a list of block devices - out from command
        /// 'lsblk --include 8 --include 259 -p|grep disk'
        /// of type "disk"
        /// </summary>
        public static ProcessCall ListBlockDevices()
        {
            return new ProcessCall(WhichLsblk, "--include 8 --include 259 -p|grep disk");
        }

        /// <summary>
        /// Returns smart info for the given device
        /// '/usr/sbin/smartctl -a /dev/disk/by-id/xxx'
        /// </summary>
        public static ProcessCall GetSmartInfo(string deviceId)
        {
            return new ProcessCall(WhichSmartctl, $"-x {deviceId}");
        }

        /// <summary>
        /// Calls `which` to find out full path to required binaries
        /// </summary>
        /// <param name="command">The command to find</param>
        public static ProcessCall Which(string command)
        {
            return new ProcessCall(WhichWhich, command);
        }
    }
}
