using System;

namespace ROOT.Zfs.Public.Data.Datasets
{
    public class Dataset
    {
        public string Name { get; set; }
        public Size Used { get; set; }
        public Size Available { get; set; }
        public Size Refer { get; set; }
        public string Mountpoint { get; set; }
    }
}
