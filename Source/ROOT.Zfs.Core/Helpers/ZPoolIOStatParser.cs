using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.Statistics;

namespace ROOT.Zfs.Core.Helpers
{
    /// <summary>
    /// Parses output of zpool iostats command
    /// </summary>
    internal static class ZPoolIOStatParser
    {
        /// <summary>
        /// 17 columns with latency stats
        /// 7 columns without latency stats
        /// </summary>
        /// <param name="poolName">Name of pool</param>
        /// <param name="stdOut">The std output</param>
        public static IOStats ParseStdOut(string poolName, string stdOut)
        {
            var stats = new IOStats();
            var lines = stdOut.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            int index = 0;
            var root = ParseLine(lines[index], index++);
            stats.Pool = poolName;
            stats.Stats = new List<IOStat>();

            if (root.Capacity.Allocated.Bytes == 0)
            {
                // Its an output from a zpool iostats <pool> device, device
                // so just loop over each line and add to stats
                stats.Stats.Add(root);
                foreach (var line in lines.Skip(1))
                {
                    var device = ParseLine(line, index++);
                    stats.Stats.Add(device);
                }
                return stats;
            }
            stats.Stats.Add(root);
            root.ChildStats = new List<IOStat>();
            IOStat currentDevice = null;
            foreach (var line in lines.Skip(1))
            {
                var device = ParseLine(line, index++);
                if (device.Capacity.Allocated.Bytes != 0) // A vdev
                {
                    currentDevice = device;
                    device.ChildStats = new List<IOStat>();
                    root.ChildStats.Add(currentDevice); // Vdevs gets added to the root io stat object
                    // new vdev
                }
                else if (currentDevice != null)
                {
                    currentDevice.ChildStats.Add(device);
                }
                else // This should not happen, but if it does, we want a format exception, not any other exception
                {
                    throw ExceptionHelper.FormatException(index, line);
                }
            }

            return stats;
        }
        /// <summary>
        /// Parse a single line - which can be either a pool, vdev or physical device
        /// </summary>
        /// <param name="line">The line to parse</param>
        /// <param name="index">The index of the line - only for error logging purposes</param>
        /// <exception cref="FormatException"></exception>
        internal static IOStat ParseLine(string line, int index)
        {
            var parts = line.Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var ioStat = new IOStat();
            if (parts.Length == 7)
            {
                // simple
                ParseSimple(ioStat, parts);
            }
            else if (parts.Length == 17)
            {
                // with latency stats
                ParseSimple(ioStat, parts);
                ParseLatency(ioStat, parts);
            }
            else
            {
                throw ExceptionHelper.FormatException(index, line);
            }

            return ioStat;
        }
        /// <summary>
        /// Parses latency stats
        /// </summary>
        private static void ParseLatency(IOStat ioStat, string[] parts)
        {
            // index 7 is first column
            var latency = new IOLatencyStats();

            latency.TotalWait = new Latency(parts[7], parts[8]);
            latency.DiskWait = new Latency(parts[9], parts[10]);
            latency.SyncqWait = new Latency(parts[11], parts[12]);
            latency.AsyncqWait = new Latency(parts[13], parts[14]);
            latency.ScrubWait = ParseNanos(parts[15]);
            latency.TrimWait = ParseNanos(parts[16]);
            ioStat.LatencyStats = latency;

        }

        private static TimeSpan ParseNanos(string nanos)
        {
            if (nanos.Trim() == "-")
            {
                return TimeSpan.Zero;
            }

            var nano = long.Parse(nanos);
            
            return new TimeSpan(nano / 100);
        }

        private static void ParseSimple(IOStat stat, string[] parts)
        {
            stat.Device = parts[0];
            stat.Capacity = new Capacity { Allocated = new Size(parts[1]), Free = new Size(parts[2]) };
            stat.Operations = new Operations
            {
                Read = int.Parse(parts[3]),
                Write = int.Parse(parts[4])
            };


            ulong bwReads = ulong.Parse(parts[5]);
            ulong bwWrites = ulong.Parse(parts[6]);
            
            stat.Bandwidth = new Bandwidth { Read = bwReads, Write = bwWrites };
        }
    }
}
