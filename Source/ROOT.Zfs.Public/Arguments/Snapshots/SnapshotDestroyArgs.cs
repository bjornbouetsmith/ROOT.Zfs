using System;
using System.Text;

namespace ROOT.Zfs.Public.Arguments.Snapshots
{
    /// <summary>
    /// Contains arguments and validation for zfs destroy
    /// </summary>
    public class SnapshotDestroyArgs : SnapshotArgs
    {
        /// <inheritdoc />
        public SnapshotDestroyArgs() : base("destroy")
        {
        }

        /// <summary>
        /// Whether or not the Snapshotname should be interpreted verbatim or as a prefix
        /// </summary>
        public bool IsExactName { get; set; }

        /// <inheritdoc />
        protected override string BuildArgs(string command)
        {
            var args = new StringBuilder();
            args.Append(command);

            var dataset = Decode(Dataset);
            var rawSnapName = Decode(Snapshot);
            if (rawSnapName.StartsWith(dataset, StringComparison.OrdinalIgnoreCase))
            {
                rawSnapName = rawSnapName[(dataset.Length + 1)..];
            }

            args.Append($" {dataset}@{rawSnapName}");

            return args.ToString();
        }
    }
}
