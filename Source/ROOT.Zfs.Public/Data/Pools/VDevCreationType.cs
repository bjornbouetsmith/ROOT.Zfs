namespace ROOT.Zfs.Public.Data.Pools
{
    /// <summary>
    /// Type of vdev to create
    /// </summary>
    public enum VDevCreationType
    {
        Mirror,
        Raidz1,
        Raidz2,
        Raidz3,
        DRaid1,
        DRaid2,
        DRaid3,
        Cache,
        Log,
        Spare,
        Dedup,
        Special
    }
}