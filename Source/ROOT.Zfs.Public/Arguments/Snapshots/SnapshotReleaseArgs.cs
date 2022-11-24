namespace ROOT.Zfs.Public.Arguments.Snapshots
{
    /// <summary>
    /// Contains arguments for zfs release
    /// </summary>
    public class SnapshotReleaseArgs : SnapshotHoldReleaseArgs
    {
        /// <inheritdoc />
        public SnapshotReleaseArgs() : base("release")
        {

        }
    }
}
