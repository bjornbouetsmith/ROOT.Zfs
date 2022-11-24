namespace ROOT.Zfs.Public.Arguments.Snapshots
{
    /// <summary>
    /// Contains arguments for zfs hold
    /// </summary>
    public class SnapshotHoldArgs : SnapshotHoldReleaseArgs
    {
        /// <inheritdoc />
        public SnapshotHoldArgs() : base("hold")
        {

        }
    }
}
