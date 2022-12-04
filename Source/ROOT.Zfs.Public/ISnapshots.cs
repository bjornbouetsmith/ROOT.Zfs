using System.Collections.Generic;
using ROOT.Zfs.Public.Arguments.Snapshots;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Public
{
    /// <summary>
    /// Contains methods to manipulate snapshots in a zfs dataset
    /// </summary>
    public interface ISnapshots : IBasicZfs
    {
        /// <summary>
        /// Gets a list of snapshots for the given dataset or volumne.
        /// i.e. zfs list -t snapshot [root]
        /// </summary>
        IList<Snapshot> List(SnapshotListArgs args);

        /// <summary>
        /// Destroy the given snapshot in the dataset
        /// see https://openzfs.github.io/openzfs-docs/man/8/zfs-destroy.8.html
        /// </summary>
        void Destroy(SnapshotDestroyArgs args);

        /// <summary>
        /// Create a snapshot in the given dataset with the specified snapshot name
        /// </summary>
        /// <remarks>If null or empty string is passed an auto generated snapshot name in the format
        /// yyyyMMddHHmmss will be used</remarks>
        void Create(SnapshotCreateArgs args);

        /// <summary>
        /// Clones a snapshot of ZFS dataset.
        /// Will create all parent datasets of the target dataset as required.
        /// </summary>
        void Clone(SnapshotCloneArgs args);

        /// <summary>
        /// Adds a single reference, named with the tag argument, to the specified snapshots.
        /// Each snapshot has its own tag namespace, and tags must be unique within that space.
        /// Beware that when making a hold on a snapshot, zfs destroy will return busy when trying to destroy the given snapshot and its only possible to destroy the snapshot if you release the hold first via
        /// <see cref=" Release(SnapshotReleaseArgs)"/>
        /// see https://openzfs.github.io/openzfs-docs/man/8/zfs-hold.8.html
        /// </summary>
        void Hold(SnapshotHoldArgs args);

        /// <summary>
        /// Lists all existing user references for the given snapshot or snapshots.
        /// see https://openzfs.github.io/openzfs-docs/man/8/zfs-hold.8.html
        /// </summary>
        /// <returns>a list of holds for the given snapsho and possibly descendent</returns>
        IList<SnapshotHold> Holds(SnapshotHoldsArgs args);

        /// <summary>
        /// Removes a single reference, named with the tag argument, from the specified snapshot or snapshots. The tag must already exist for each snapshot.
        /// see https://openzfs.github.io/openzfs-docs/man/8/zfs-hold.8.html
        /// </summary>
        void Release(SnapshotReleaseArgs args);
    }
}