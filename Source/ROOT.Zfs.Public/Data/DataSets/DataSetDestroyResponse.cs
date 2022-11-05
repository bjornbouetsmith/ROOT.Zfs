using ROOT.Zfs.Public.Arguments;
using ROOT.Zfs.Public.Arguments.Dataset;

namespace ROOT.Zfs.Public.Data.Datasets
{
    /// <summary>
    /// Class meant to contain data for when destroying datasets and if you request a dry-run.
    /// </summary>
    public class DatasetDestroyResponse
    {
        /// <summary>
        /// The flags passed onto the destroy command
        /// </summary>
        public DatasetDestroyFlags Flags { get; set; }

        /// <summary>
        /// Output from the dry-run.
        /// Inspect this to see if Zfs did as you expected
        /// </summary>
        public string DryRun { get; set; }
    }
}
