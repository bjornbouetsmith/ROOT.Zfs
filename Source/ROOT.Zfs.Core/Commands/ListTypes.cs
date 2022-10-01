using System;

namespace ROOT.Zfs.Core.Commands
{
    /// <summary>
    /// Types to display when invoking zfs list
    /// This is a flags enum, so combinations can be used
    /// </summary>
    [Flags]
    public enum ListTypes
    {
        /// <summary>
        /// Nothing - will be treated as "all", since its the default value
        /// </summary>
        None = 0,
        /// <summary>
        /// Filesystems - aka datasets
        /// </summary>
        FileSystem = 1,
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