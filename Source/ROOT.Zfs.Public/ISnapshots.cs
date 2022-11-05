﻿using System.Collections.Generic;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Public
{
    /// <summary>
    /// Contains methods to manipulate snapshots in a zfs dataset
    /// </summary>
    public interface ISnapshots
    {
        /// <summary>
        /// Gets a list of snapshots for the given dataset or volumne.
        /// i.e. zfs list -t snapshot <paramref name="datasetOrVolume"/>
        /// </summary>
        IEnumerable<Snapshot> List(string datasetOrVolume);

        /// <summary>
        /// Destroy the given snapshot in the dataset
        /// </summary>
        /// <param name="datasetOrVolume">The dataset or volume in which to destroy a snapshot</param>
        /// <param name="snapName">The name of the snapshot.
        /// Can be in the format of dataset@snapname - or just snapnaps</param>
        /// <param name="isExactName">Whether or not to do exact matching on snapshot name, or partial matches where datasets must being withe <paramref name="snapName"/></param>
        void Destroy(string datasetOrVolume, string snapName, bool isExactName);

        /// <summary>
        /// Create a snapshot in the given dataset with the specified snapshot name
        /// </summary>
        /// <param name="dataset">The dataset in which to create a snapshot</param>
        /// <param name="snapName">The name of the snapshot.
        /// If null or empty string is passed an auto generated snapshot name in the format
        /// yyyyMMddHHmmss will be used</param>
        void CreateSnapshot(string dataset, string snapName);

        /// <summary>
        /// Adds a single reference, named with the tag argument, to the specified snapshots.
        /// Each snapshot has its own tag namespace, and tags must be unique within that space.
        /// Beware that when making a hold on a snapshot, zfs destroy will return busy when trying to destroy the given snapshot and its only possible to destroy the snapshot if you release the hold first via
        /// <see cref=" Release(string, string, bool)"/>
        /// see https://openzfs.github.io/openzfs-docs/man/8/zfs-hold.8.html
        /// </summary>
        /// <param name="snapshot">The full name of the snapshot in which to create a hold</param>
        /// <param name="tag">The tag to use for the hold</param>
        /// <param name="recursive">Specifies that a hold with the given tag is applied recursively to the snapshots of all descendent file systems.</param>
        void Hold(string snapshot, string tag, bool recursive);

        /// <summary>
        /// Lists all existing user references for the given snapshot or snapshots.
        /// see https://openzfs.github.io/openzfs-docs/man/8/zfs-hold.8.html
        /// </summary>
        /// <param name="snapshot">The full name of the snapshot</param>
        /// <param name="recursive">Lists the holds that are set on the named descendent snapshots, in addition to listing the holds on the named snapshot.</param>
        /// <returns>a list of holds for the given snapsho and possibly descendent</returns>
        IList<SnapshotHold> Holds(string snapshot, bool recursive);

        /// <summary>
        /// Removes a single reference, named with the tag argument, from the specified snapshot or snapshots. The tag must already exist for each snapshot.
        /// see https://openzfs.github.io/openzfs-docs/man/8/zfs-hold.8.html
        /// </summary>
        /// <param name="snapshot">The snapshot in which to release the hold</param>
        /// <param name="tag">The tag to release</param>
        /// <param name="recursive">Recursively releases a hold with the given tag on the snapshots of all descendent file systems.</param>
        void Release(string snapshot, string tag, bool recursive);
    }
}