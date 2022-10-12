using System;

namespace ROOT.Zfs.Public.Data
{
    /// <summary>
    /// Represents a single line from zpool history
    /// </summary>
    public class CommandHistory
    {
        /// <summary>
        /// When the event happened
        /// </summary>
        public DateTime Time { get; set; }
        /// <summary>
        /// The command that was done
        /// </summary>
        public string Command { get; set; }
        /// <summary>
        /// Caller information
        /// </summary>
        public string Caller { get; set; }
    }
}
