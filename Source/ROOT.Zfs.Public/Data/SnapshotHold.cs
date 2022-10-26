using System;

namespace ROOT.Zfs.Public.Data
{
    /// <summary>
    /// Represents a hold on a snapshot.
    /// see https://openzfs.github.io/openzfs-docs/man/8/zfs-hold.8.html
    /// </summary>
    public class SnapshotHold
    {
        /// <summary>
        /// The snapshot this hold relates to
        /// </summary>
        public string Snapshot { get; set; }
        
        /// <summary>
        /// The tag for the hold
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// When the hold was made.
        /// This property might be incorrect, since zfs returns the time in a string format, possibly dependent on the locale.
        /// So parsing of this is best effort
        /// </summary>
        public DateTime HoldTime { get; set; }

    }
}
