using System.Text;

namespace ROOT.Zfs.Public.Arguments.Snapshots
{
    /// <summary>
    /// Contains required and optiona data for zfs holds command
    /// </summary>
    public class SnapshotHoldsArgs : SnapshotArgs
    {
        /// <inheritdoc />
        public SnapshotHoldsArgs() : base("holds")
        {
            
        }

        /// <summary>
        /// Lists the holds that are set on the named descendent snapshots, in addition to listing the holds on the named snapshot.
        /// </summary>
        public bool Recursive { get; set; }

        /// <inheritdoc />
        protected override string BuildArgs(string command)
        {
            var args = new StringBuilder();
            args.Append(command);

            if (Recursive)
            {
                args.Append(" -r");
            }

            var rawSnapshot = GetRawSnapshotName();
            args.Append($" -H {Decode(Dataset)}@{rawSnapshot}");

            return args.ToString();
        }
    }
}
