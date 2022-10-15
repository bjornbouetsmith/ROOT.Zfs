using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.Pools;
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
            var firstLine = ParseLine(lines[index], index++);
            stats.Pool = poolName;
            stats.Stats = new List<IOStat>();

            if (firstLine.Capacity.Allocated.Bytes == 0)
            {
                // Its an output from a zpool iostats <pool> device, device
                // so just loop over each line and add to stats
                stats.Stats.Add(firstLine);
                foreach (var line in lines.Skip(1))
                {
                    var device = ParseLine(line, index++);
                    stats.Stats.Add(device);
                }
                return stats;
            }
            stats.Stats.Add(firstLine);
            firstLine.ChildStats = new List<IOStat>();
            IOStat currentDevice = null;
            foreach (var line in lines.Skip(1))
            {
                var device = ParseLine(line, index++);
                if (device.Capacity.Allocated.Bytes != 0)
                {
                    currentDevice = device;
                    device.ChildStats = new List<IOStat>();
                    firstLine.ChildStats.Add(currentDevice);
                    // new vdev
                }
                else if (currentDevice != null)
                {
                    currentDevice.ChildStats.Add(device);
                }
                else
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
            if (!long.TryParse(nanos, out var nano))
            {
                Trace.WriteLine($"Could not parse:{nanos} into a number");
                return TimeSpan.Zero;
            }

            return new TimeSpan(nano / 100);
        }

        private static void ParseSimple(IOStat stat, string[] parts)
        {
            stat.Device = parts[0];
            stat.Capacity = new Capacity { Allocated = new Size(parts[1]), Free = new Size(parts[2]) };
            stat.Operations = new Operations();
            if (!int.TryParse(parts[3], out var reads))
            {
                Trace.WriteLine($"Could not parse:{parts[3]} into reads");
            }

            stat.Operations.Read = reads;

            if (!int.TryParse(parts[4], out var writes))
            {
                Trace.WriteLine($"Could not parse:{parts[4]} into reads");
            }

            stat.Operations.Write = writes;
            stat.Bandwidth = new Bandwidth { Read = parts[5], Write = parts[6] };
        }
    }
}
