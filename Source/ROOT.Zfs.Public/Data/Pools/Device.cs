namespace ROOT.Zfs.Public.Data.Pools
{
    public class Device
    {
        public string ShortName { get; set; }
        public string DeviceName { get; set; }
        public Errors Errors { get; set; }
        public State State { get; set; }
    }
}
