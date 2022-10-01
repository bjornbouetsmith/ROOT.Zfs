using System;
using System.Collections.Generic;
using System.Text;
using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Core.Helpers
{
    /*
pool: tank
state: ONLINE
scan: resilvered 126K in 00:00:00 with 0 errors on Tue Sep 27 17:57:17 2022
config:

    NAME                           STATE     READ WRITE CKSUM
    tank                           ONLINE       0     0     0
      mirror-0                     ONLINE       0     0     0
        ata-QEMU_HARDDISK_QM00015  ONLINE       0     0     0
        ata-QEMU_HARDDISK_QM00017  ONLINE       0     0     0

pool: tank
state: DEGRADED
status: One or more devices could not be used because the label is missing or
    invalid.  Sufficient replicas exist for the pool to continue
    functioning in a degraded state.
action: Replace the device using 'zpool replace'.
see: https://openzfs.github.io/openzfs-docs/msg/ZFS-8000-4J
config:

    NAME                           STATE     READ WRITE CKSUM
    tank                           DEGRADED     0     0     0
      mirror-0                     DEGRADED     0     0     0
        ata-QEMU_HARDDISK_QM00015  ONLINE       0     0     0
        2252240780663618111        UNAVAIL      0     0     0  was /dev/disk/by-id/ata-QEMU_HARDDISK_QM00017-part1
 */
    internal class ZPoolStatusParser
    {
        public static PoolStatus Parse(string input)
        {
            // Handle not found
            // cannot open 'tank2': no such pool
            if (input.StartsWith("cannot open") && input.Contains("no such pool"))
            {
                return null;
            }
            int currentIndex = 0;
            try
            {
                var poolStatus = new PoolStatus();
                var lines = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                var poolLine = lines[currentIndex++];
                var poolParts = poolLine.Split(':');
                if (poolParts.Length == 2 && poolParts[0].Trim().Equals("pool", StringComparison.OrdinalIgnoreCase))
                {
                    poolStatus.Pool = new Pool();
                    poolStatus.Pool.Name = poolParts[1].Trim();
                    ParseStatusLines(poolStatus, lines, ref currentIndex);
                    ParseVDevs(poolStatus, lines, ref currentIndex);
                }

                return poolStatus;
            }
            catch
            {
                throw new ParseException(currentIndex, input);
            }
        }

        private static void ParseStatusLines(PoolStatus status, string[] lines, ref int currentIndex)
        {
            var line = lines[currentIndex++];
            string trimStart;
            bool inStatusText = false;
            StringBuilder sb = null;
            do
            {
                trimStart = line.TrimStart();
                // parse stuff
                if (trimStart.StartsWith("state:"))
                {
                    status.State = ParseState(line);
                }
                else if (trimStart.StartsWith("scan:"))
                {
                    ParseScanText(status, line);
                }
                else if (trimStart.StartsWith("status:"))
                {
                    sb = new StringBuilder();
                    sb.AppendLine(line);
                    inStatusText = true;
                    // Include all up until config:
                }
                else if (inStatusText && !trimStart.StartsWith("config:"))
                {
                    sb.AppendLine(line);
                }

                line = lines[currentIndex++];

            } while (!trimStart.StartsWith("config:"));

            if (sb != null)
            {
                status.StatusText = sb.ToString().Trim();
            }


        }

        private static void ParseVDevs(PoolStatus status, string[] lines, ref int currentIndex)
        {
            // first line is the pool status itself
            var parts = lines[currentIndex++].Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            status.Pool.State = ParseState(parts[1]);
            status.Pool.Errors = ParseErrors(parts);
            status.Pool.VDevs = new List<VDev>();
            VDev vdev;
            do
            {
                if (lines[currentIndex].Trim().StartsWith("errors:"))
                {
                    // No more vdevs
                    break;
                }

                vdev = ParseVDev(lines, ref currentIndex);
                status.Pool.VDevs.Add(vdev);


            } while (vdev != null && currentIndex < lines.Length);

        }

        private static VDev ParseVDev(string[] lines, ref int currentIndex)
        {
            // First line is vdev name
            // lines following is devices that start with /dev
            // until a line no longer starts with /dev
            // parse devices

            var vdev = new VDev();
            var data = ParseDevLine(lines, ref currentIndex);
            vdev.Name = data.Name;
            vdev.State = data.State;
            vdev.Errors = data.Errors;
            vdev.Devices = new List<Device>();

            string trimmed;
            do
            {
                Device dev = ParseDevice(lines, ref currentIndex);
                vdev.Devices.Add(dev);
                if (currentIndex < lines.Length)
                {
                    trimmed = lines[currentIndex].Trim();
                }
                else
                {
                    break;
                }
            } while (trimmed.StartsWith("/") && currentIndex <= lines.Length);

            return vdev;
        }

        private static Device ParseDevice(string[] lines, ref int currentIndex)
        {
            var parts = ParseDevLine(lines, ref currentIndex);
            var dev = new Device();
            dev.DeviceName = parts.Name;
            dev.State = parts.State;
            dev.Errors = parts.Errors;
            return dev;

        }

        private static (string Name, State State, Errors Errors) ParseDevLine(string[] lines, ref int currentIndex)
        {
            var parts = lines[currentIndex++].Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var name = parts[0].Trim();
            if (name == "spares" && parts.Length == 1)
            {
                return (name, State.Available, default);
            }

            var state = ParseState(parts[1]);
            var errors = ParseErrors(parts);
            return (name, state, new Errors());
        }

        private static Errors ParseErrors(string[] parts)
        {
            // first two are used by name and state
            var errors = new Errors();
            if (parts.Length == 2)
            {
                // spares only have two parts, name and status
                return errors;
            }

            if (long.TryParse(parts[2].Trim(), out var reads))
            {
                errors.Read = (int)reads;
            }
            if (long.TryParse(parts[3].Trim(), out var writes))
            {
                errors.Write = (int)writes;
            }
            if (long.TryParse(parts[4].Trim(), out var checksums))
            {
                errors.Checksum = (int)checksums;
            }

            return errors;
        }

        private static void ParseScanText(PoolStatus status, string line)
        {
            var indexOfFirstColon = line.IndexOf(':');
            status.ScanStatus = line[(indexOfFirstColon + 1)..].Trim();
        }

        private static State ParseState(string line)
        {
            var poolParts = line.Split(':');
            var stateText = poolParts.Length > 1 ? poolParts[1].Trim() : poolParts[0].Trim();

            switch (stateText)
            {
                case "ONLINE":
                    return State.Online;
                case "DEGRADED":
                    return State.Degraded;
                case "OFFLINE":
                    return State.Offline;
                case "FAULTED":
                    return State.Faulted;
                case "REMOVED":
                    return State.Removed;
                case "UNAVAIL":
                    return State.Unavailable;
                case "AVAIL":
                    return State.Available;
                default:
                    return State.Unknown;
            }
        }
    }

    public class ParseException : Exception
    {
        public int Index { get; }
        public string Contents { get; }

        public ParseException(int index, string contents) : base($@"Failed to parse trimmed input:
Look to index:{index} when discarding empty linies,
Content:
{contents}")
        {
            Index = index;
            Contents = contents;
        }
    }
}
