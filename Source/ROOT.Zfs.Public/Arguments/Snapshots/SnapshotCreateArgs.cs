using System;
using System.Collections.Generic;
using System.Text;

namespace ROOT.Zfs.Public.Arguments.Snapshots
{
    /// <summary>
    /// Contains required arguments to zfs snap
    /// </summary>
    public class SnapshotCreateArgs : SnapshotArgs
    {
        /// <inheritdoc />
        public SnapshotCreateArgs() : base("snap")
        {

        }

        /// <inheritdoc />
        public override bool Validate(out IList<string> errors)
        {
            errors = null;
            if (string.IsNullOrWhiteSpace(Snapshot))
            {
                ValidateString(Dataset, false, ref errors);
                
                return errors == null;
            }

            // If snapshot is set, then we can just use entirey base validation
            return base.Validate(out errors);
        }

        /// <inheritdoc />
        protected override string BuildArgs(string command)
        {
            var args = new StringBuilder();
            args.Append(command);
            var snapName = GetRawSnapshotName();
            if (string.IsNullOrWhiteSpace(snapName))
            {
                snapName = CreateSnapshotName(DateTime.UtcNow.ToLocalTime());
            }

            args.Append($" {Decode(Dataset)}@{snapName}");
            return args.ToString();
        }

        /// <summary>
        /// Creates a standard snapshot name based on the time passed into the method
        /// </summary>
        internal static string CreateSnapshotName(DateTime dateTime) => dateTime.ToString("yyyyMMddHHmmss");
    }
}
