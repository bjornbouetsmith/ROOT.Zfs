using System.Collections.Generic;
using System.Text;

namespace ROOT.Zfs.Public.Arguments.Snapshots
{
    /// <summary>
    /// Contains arguments for zfs hold/release
    /// </summary>
    public class SnapshotHoldReleaseArgs : SnapshotArgs
    {
        /// <inheritdoc />
        public SnapshotHoldReleaseArgs(string command) : base(command)
        {

        }

        /// <summary>
        /// The tag to use for the hold, or the tag to release
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Specifies that a hold with the given tag is applied recursively to the snapshots of all descendent file systems.
        /// Or Recursively releases a hold with the given tag on the snapshots of all descendent file systems
        /// </summary>
        public bool Recursive { get; set; }

        /// <inheritdoc />
        public override bool Validate(out IList<string> errors)
        {

            base.Validate(out errors);
            ValidateString(Tag, false, ref errors, true);

            return errors == null;
        }


        /// <inheritdoc />
        protected override string BuildArgs(string command)
        {
            // Should return a string similar to 'hold <tag> <dataset>@<snapshot>'
            // Should return a string similar to 'release <tag> <dataset>@<snapshot>'
            var args = new StringBuilder();
            args.Append(command);
            if (Recursive)
            {
                args.Append(" -r");
            }

            args.Append($" {Decode(Tag)}");
            var rawSnap = GetRawSnapshotName();
            args.Append($" {Decode(Dataset)}@{rawSnap}");

            return args.ToString();
        }
    }
}
