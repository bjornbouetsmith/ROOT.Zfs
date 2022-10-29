using System;
using System.Collections.Generic;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Core.Helpers
{
    /// <summary>
    /// Class with helper methods to parse information about disk and block devices
    /// </summary>
    internal static class DiskHelper
    {
        /// <summary>
        /// Expects the output of ls -l /dev/disk/by-id/ | awk -F ' ' '{print $9,$11}'
        /// </summary>
        /// <param name="line"></param>
        /// <param name="blockDevices">Hashset with the range of known disks from the system</param>
        /// <returns></returns>
        internal static DiskInfo FromString(string line, HashSet<string> blockDevices)
        {
            var parts = line.Split(' ');
            var id = parts[0];
            var device = parts[1];
            var lastIndexOfForwardSlash = device.LastIndexOf('/')+1;
            var disk = new DiskInfo();
            disk.Id = $"/dev/disk/by-id/{id}";
            disk.DeviceName = $"/dev/{device[lastIndexOfForwardSlash..]}";
            disk.Type = blockDevices.Contains(disk.DeviceName) ? DeviceType.Disk : DeviceType.Partition;

            return disk;
        }

        /// <summary>
        /// Expects a range of lines in the format:
        ///
        /// /dev/sda      8:0    0   16G  0 disk
        /// /dev/sdb      8:16   0   16G  0 disk
        /// /dev/sdc      8:32   0   16G  0 disk
        ///
        /// i.e. the output of 'lsblk --include 8 -p|grep disk'
        /// </summary>
        /// <param name="stdOutput"></param>
        /// <returns></returns>
        internal static HashSet<string> BlockDevicesFromStdOutput(string stdOutput)
        {
            HashSet<string> devices = new HashSet<string>();
            foreach (var line in stdOutput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var device = line.Split(' ')[0];
                devices.Add(device.Trim());
            }

            return devices;
        }
    }
}
