using System.Collections.Generic;
using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Core.Helpers
{
    internal static class ZPoolInfoParser
    {
        /// <summary>
        /// Parse input based off  'zpool list -PH'
        /// with content like:
        /// 
        /// rpool    29G  2.86G  26.1G        -         -    10%     9%  1.00x    ONLINE  -
        /// tank   3.47T   119G  3.35T        -         -     9%     3%  1.00x    ONLINE  -
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

            var info = new PoolInfo
            {
                Name= name,
                Size= size,
                Allocated= alloc,
                Free= free,
                Fragmentation= frag,
                CapacityUsed= cap,
                DedupRatio= dedup,
                State = StateParser.Parse(health)
            };

            return info;
        }
    }
}
