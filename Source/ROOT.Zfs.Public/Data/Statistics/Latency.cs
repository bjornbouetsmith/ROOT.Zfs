using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace ROOT.Zfs.Public.Data.Statistics
{
    /// <summary>
    /// Represents read/write latency
    /// </summary>
    public struct Latency
    {
        /// <summary>
        /// Creates a <see cref="Latency"/> struct with the given read/write nano seconds represented as strings
        /// If any of the values cannot be parsed into a number, 0 is used instead
        /// </summary>
        /// <param name="read">Read latency in nanoseconds</param>
        /// <param name="write">Write latency in nanoseconds</param>
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
        /// <summary>
        /// Ctor only for serialization purposes
        /// </summary>
        [ExcludeFromCodeCoverage]
        public Latency()
        {
            Read = default;
            Write = default;
        }
        /// <summary>
        /// read latency represented as a timespan
        /// </summary>
        public TimeSpan Read { get; set; }

        /// <summary>
        /// write latency represented as a timespan
        /// </summary>
        public TimeSpan Write { get; set; }
    }
}