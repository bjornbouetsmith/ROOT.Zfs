using ROOT.Zfs.Public.Arguments.Dataset;

namespace ROOT.Zfs.Public.Arguments.Snapshots
{
    /// <summary>
    /// Contains validation logic+parameters for zfs list -t snapshot
    /// </summary>
    public class SnapshotListArgs : DatasetListArgs
    {
        /// <inheritdoc />
        public SnapshotListArgs()
        {
            DatasetTypes = Data.DatasetTypes.Snapshot;
            IncludeChildren = true;
        }
    }
}
