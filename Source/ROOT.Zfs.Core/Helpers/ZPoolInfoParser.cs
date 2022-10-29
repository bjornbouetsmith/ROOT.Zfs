using System.Collections.Generic;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.Pools;
using ROOT.Zfs.Public.Data.Statistics;

namespace ROOT.Zfs.Core.Helpers
{
    internal static class ZPoolInfoParser
    {
        /// <summary>
        /// Parse input based off  'zpool list -PH'
        /// with content like:
        /// 
        /// rpool   31138512896     3078807552      28059705344     -       -       10      9       1.00    ONLINE  -
        /// tank    3813930958848   128800165888    3685130792960   -       -       9       3       1.00    ONLINE  -
        /// </summary>
        public static IEnumerable<PoolInfo> ParseFromStdOut(string stdOut)
        {
            foreach (var line in stdOut.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries))
            {
                yield return ParseLine(line);
            }
        }
        /// <summary>
        /// Parses a single line
        /// tank   3.47T   119G  3.35T        -         -     9%     3%  1.00x    ONLINE  -
        /// </summary>
        public static PoolInfo ParseLine(string line)
        {
            var parts = line.Split(new[] { ' ', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
            var name = parts[0];
            var size = parts[1];
            var alloc = parts[2];
            var free = parts[3];
            var frag = parts[6];
            var cap = parts[7];
            var dedup = parts[8];
            var health = parts[9];
            var altRoot = parts[10];

            var info = new PoolInfo
            {
                Name= name,
                Size= new Size(size),
                Allocated= new Size(alloc),
                Free= new Size(free),
                Fragmentation= new Part(frag),
                CapacityUsed= new Part(cap),
                DedupRatio= new Ratio(dedup),
                State = StateParser.Parse(health),
                AltRoot=altRoot
            };

            return info;
        }
    }
}
