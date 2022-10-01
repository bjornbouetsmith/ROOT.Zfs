namespace ROOT.Zfs.Public.Data.Pools
{
    /// <summary>
    /// Type of vdev to create
    /// </summary>
    public enum VDevCreationType
    {
        Mirror,
        Raidz,
        DRaidz,
        Cache,
        Log,
        Spare,
        Dedup,
        Special
    }
}