using System;

namespace ROOT.Zfs.Public.Data
{
    /// <summary>
    /// Represents a snapshot inside a pool
    /// </summary>
    public class Snapshot
    {
        /// <summary>
        /// The dataset the snapshot belongs to
        /// </summary>
        public string Dataset { get; set; }

        /// <summary>
        /// The name of the snapshot
        /// </summary>
        public string SnapshotName { get; set; }

        /// <summary>
        /// Time when snapshot was created
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Size information for the snapshot
        /// </summary>
        public Size Size { get; set; }
    }
}
