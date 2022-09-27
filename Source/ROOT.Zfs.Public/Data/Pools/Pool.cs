using System.Collections.Generic;

namespace ROOT.Zfs.Public.Data.Pools
{
    public class Pool
    {
        public string Name { get; set; }
        public List<VDev> VDevs { get; set; }
        public State State { get; set; }
        public Errors Errors { get; set; }
    }
}
