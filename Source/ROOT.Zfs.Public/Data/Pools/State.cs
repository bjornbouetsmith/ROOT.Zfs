namespace ROOT.Zfs.Public.Data.Pools
{
    public enum State
    {
        Unknown = 0,
        Online = 1,
        Degraded = 2,
        Faulted = 3,
        Offline = 4,
        Removed = 5,
        Unavailable = 6,
        Available = 7
    }
}