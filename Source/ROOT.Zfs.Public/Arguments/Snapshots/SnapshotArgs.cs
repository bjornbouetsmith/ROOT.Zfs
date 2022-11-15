using System.Collections.Generic;

namespace ROOT.Zfs.Public.Arguments.Snapshots
{
    /// <summary>
    /// arguments class for a dataset name and a snapshot name
    /// </summary>
    public abstract class SnapshotArgs : Args
    {
        /// <inheritdoc />
        protected SnapshotArgs(string command) : base(command)
        {
        }

        /// <summary>
        /// Name of dataset snapshot belongs to
        /// </summary>
        public string Dataset { get; set; }

        /// <summary>
        /// Name of snapshot.
        /// This can either be the raw name which includes the dataset or just the name of the snapshot
        /// </summary>
        public string Snapshot { get; set; }

        /// <inheritdoc />
        public override bool Validate(out IList<string> errors)
        {
            errors = null;
            ValidateString(Dataset, false, ref errors);
            var decoded = Decode(Snapshot);
            var index = decoded.IndexOf('@');
            if (index > -1)
            {
                decoded = decoded[(index + 1)..];
                ValidateString(decoded, false, ref errors);
            }
            else
            {
                ValidateString(Snapshot, false, ref errors);
            }

            return errors == null;
        }
    }
}
