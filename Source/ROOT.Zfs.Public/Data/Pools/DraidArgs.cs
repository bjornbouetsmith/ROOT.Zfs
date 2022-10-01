namespace ROOT.Zfs.Public.Data.Pools
{
    /// <summary>
    /// Extra arguments only for draid
    /// </summary>
    public class DraidArgs
    {
        public int DataDevices { get; set; }
        public int Children { get; set; }
        public int Spares { get; set; }
    }
}