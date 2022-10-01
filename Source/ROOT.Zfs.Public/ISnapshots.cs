﻿using System.Collections.Generic;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Public
{
    /// <summary>
    /// Contains methods to manipulate snapshots in a zfs dataset
    /// </summary>
    public interface ISnapshots
    {
        IEnumerable<Snapshot> GetSnapshots(string datasetOrVolume);

        /// <summary>
        /// Destroy the given snapshot in the dataset
        /// </summary>
        /// <param name="datasetOrVolume">The dataset or volume in which to destroy a snapshot</param>
        /// <param name="snapName">The name of the snapshot.
        /// Can be in the format of dataset@snapname - or just snapnaps</param>
        /// <param name="isExactName">Whether or not to do exact matching on snapshot name, or partial matches where datasets must being withe <paramref name="snapName"/></param>
        void DestroySnapshot(string datasetOrVolume, string snapName, bool isExactName);

        /// <summary>
        /// Create a snapshot in the given dataset with an autogenerated snapshot name in the format
        /// yyyyMMddHHmmss
        /// </summary>
        /// <param name="dataset">The dataset in which to create a snapshot</param>
        void CreateSnapshot(string dataset);

        /// <summary>
        /// Create a snapshot in the given dataset with the specified snapshot name
        /// </summary>
        /// <param name="dataset">The dataset in which to create a snapshot</param>
        /// <param name="snapName">The name of the snapshot</param>
        void CreateSnapshot(string dataset, string snapName);
    }
}