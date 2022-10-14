using System.Collections.Generic;

namespace ROOT.Zfs.Public.Data.Pools
{
    public class PoolInfo
    {
        
        public string Name { get; set; }
        public Size Size { get; set; }
        public Size Allocated { get; set; }
        public Size Free { get; set; }
        public Part Fragmentation { get; set; }
        public Part CapacityUsed { get; set; }
        public Ratio DedupRatio { get; set; }
        public State State { get; set; }
        public string AltRoot { get; set; }
    }
}
