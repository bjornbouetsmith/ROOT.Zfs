using System;
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

        /// <summary>
        /// Returns the raw snapshot name, where the dataset prefix if any has been removed.
        /// Snapshot name is also decoded, so its no longer url encoded if that was the case
        /// </summary>
        protected string GetRawSnapshotName()
        {
            if (string.IsNullOrWhiteSpace(Snapshot))
            {
                return string.Empty;
            }

            var dataset = Decode(Dataset);
            var rawSnapName = Decode(Snapshot);
            if (rawSnapName.StartsWith(dataset, StringComparison.OrdinalIgnoreCase))
            {
                rawSnapName = rawSnapName[(dataset.Length + 1)..];
            }

            return rawSnapName;
        }

        /// <inheritdoc />
        public override bool Validate(out IList<string> errors)
        {
            errors = null;
            ValidateString(Dataset, false, ref errors);

            var rawSnapshotName = GetRawSnapshotName();
            ValidateString(rawSnapshotName, false, ref errors, "Snapshot");

            if (rawSnapshotName.Contains('/'))
            {
                errors ??= new List<string>();
                errors.Add("Snapshot cannot contain '/' unless its a part of the dataset prefix");
            }

            return errors == null;
        }
    }
}
