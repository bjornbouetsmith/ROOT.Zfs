namespace ROOT.Zfs.Public.Data
{
    /// <summary>
    /// Contains information from the command zfs --version
    /// </summary>
    public class VersionInfo
    {
        /// <summary>
        /// The actual lines output from the command
        /// </summary>
        public string[] Lines { get; set; }
    }
}
