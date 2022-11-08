using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Public.Arguments.Dataset;

namespace ROOT.Zfs.Core.Commands
{
    /// <summary>
    /// This should be improved, so we can have commands defined per version if required
    /// And a way to register commands for a given version
    /// </summary>
    internal class Commands
    {
        public static string WhichZfs { get; set; } = "/sbin/zfs";
        public static string WhichZpool { get; set; } = "/sbin/zpool";
        public static string WhichZdb { get; set; } = "/sbin/zdb";
        public static string WhichLs { get; set; } = "/bin/ls";
        public static string WhichLsblk { get; set; } = "/bin/lsblk";
        public static string WhichWhich { get; set; } = "/bin/which";
        public static string WhichSmartctl { get; set; } = "/sbin/smartctl";

        internal static ArgumentException ToArgumentException(IList<string> errors, object args, [CallerArgumentExpression("args")] string nameOfArgs = "")
        {
            return new ArgumentException(string.Join(Environment.NewLine, errors),$"{args.GetType().Name} {nameOfArgs}");
        }

        /// <summary>
        /// Returns a command that lists datasets
        /// </summary>
        public static IProcessCall ZfsList(DatasetListArgs args)
        {
            if (!args.Validate(out var errors))
            {
                throw ToArgumentException(errors, args);
            }

            return new ProcessCall(WhichZfs, args.ToString());
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
