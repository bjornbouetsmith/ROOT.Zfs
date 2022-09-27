using System.Collections.Generic;

namespace ROOT.Zfs.Public.Data.Pools
{
    public class VDev
    {
        public string Name { get; set; }
        public State State { get; set; }
        public Errors Errors { get; set; }
        public List<Device> Devices { get; set; }
    }
}
