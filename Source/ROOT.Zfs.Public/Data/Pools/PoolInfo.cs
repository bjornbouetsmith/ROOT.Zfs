using System.Collections.Generic;

namespace ROOT.Zfs.Public.Data.Pools
{
    public class PoolInfo
    {
        
        public string Name { get; set; }
        public string Size { get; set; }
        public string Allocated { get; set; }
        public string Free { get; set; }
        public string Fragmentation { get; set; }
        public string CapacityUsed { get; set; }
        public string DedupRatio { get; set; }
        public State State { get; set; }
        public string AltRoot { get; set; }
    }
}
