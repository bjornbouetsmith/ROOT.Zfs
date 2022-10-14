using System;
using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Public.Data
{
    public class Snapshot
    {
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public Size Size { get; set; }
    }
}
