using System;

namespace ROOT.Zfs.Public.Data
{
    public class Snapshot
    {
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public long Size { get; set; }
    }
}
