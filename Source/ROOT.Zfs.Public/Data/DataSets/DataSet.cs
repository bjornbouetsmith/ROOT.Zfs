using System;

namespace ROOT.Zfs.Public.Data.DataSets
{
    public class DataSet
    {
        public string Name { get; set; }
        public string Used { get; set; }
        public string Available { get; set; }
        public string Refer { get; set; }
        public string Mountpoint { get; set; }
    }
}
