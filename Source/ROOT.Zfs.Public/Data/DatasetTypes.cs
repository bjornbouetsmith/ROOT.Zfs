using System;

namespace ROOT.Zfs.Public.Data
{
    /// <summary>
    /// Types of dataset
    /// This is a flags enum, so combinations can be used
    /// Combinations only works when listing dataset, not when creating a dataset
    /// </summary>
    [Flags]
    public enum DatasetTypes
    {
        /// <summary>
        /// None - will be treated as "FileSystem | Volume", since its the default value
        /// </summary>
        None = 0,
        /// <summary>
        /// Filesystems - aka datasets
        /// </summary>
        Filesystem = 1,
        /// <summary>
        /// Snapshots
        /// </summary>
        Snapshot = 2,
        /// <summary>
        /// Volumes - aka block devices
        /// </summary>
        Volume = 4,
        /// <summary>
        /// Bookmarks
        /// </summary>
        Bookmark = 8
    }
}