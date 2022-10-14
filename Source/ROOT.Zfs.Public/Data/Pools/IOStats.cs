using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ROOT.Zfs.Public.Data.Pools
{
    /// <summary>
    /// Represents output from zpool iostats
    /// </summary>
    public class IOStats
    {
        public string Pool { get; set; }
        public List<IOStat> Stats { get; set; }
        
    }

    /// <summary>
    /// Represents stats for a sigle pool/vdev/device
    /// </summary>
    public class IOStat
    {
        public string Device { get; set; }
        public Capacity Capacity { get; set; }
        public Operations Operations { get; set; }
        public Bandwidth Bandwidth { get; set; }
        public IOLatencyStats LatencyStats { get; set; }
        public List<IOStat> ChildStats { get; set; }
    }

    public class IOLatencyStats
    {
        public Latency TotalWait { get; set; }
        public Latency DiskWait { get; set; }
        public Latency SyncqWait { get; set; }
        public Latency AsyncqWait { get; set; }
        public TimeSpan ScrubWait { get; set; }
        public TimeSpan TrimWait { get; set; }
    }

    public class Capacity
    {
        public Size Allocated { get; set; }
        public Size Free { get; set; }
    }

    public class Operations
    {
        public int Read { get; set; }
        public int Write { get; set; }
    }

    public class Bandwidth
    {
        public string Read { get; set; }
        public string Write { get; set; }
    }

    public class Latency
    {
        public Latency(string read, string write)
        {
            if(!long.TryParse(read, out var rNanos))
            {
                Trace.WriteLine($"Could not parse {read} into nano seconds");
            }
            if (!long.TryParse(write, out var wNanos))
            {
                Trace.WriteLine($"Could not parse {write} into nano seconds");
            }

            Read = new TimeSpan(rNanos / 100);
            Write = new TimeSpan(wNanos / 100);
        }

        public Latency()
        {
            
        }
        public TimeSpan Read { get; set; }
        public TimeSpan Write { get; set; }
    }

    public class Size
    {
        public Size()
        {
        }

        public Size(long bytes)
        {
            Bytes = bytes;
        }

        public Size(string bytes)
        {
            if (!long.TryParse(bytes, out var b))
            {
                Trace.WriteLine($"Cannnot parse {bytes} into a number");
            }
            Bytes = b;
        }
        public long Bytes { get; set; }
        public double KB => Bytes / 1000d;
        public double MB => KB / 1024d;
        public double GB => MB / 1024d;
        public double TB => GB / 1024d;
    }
}
