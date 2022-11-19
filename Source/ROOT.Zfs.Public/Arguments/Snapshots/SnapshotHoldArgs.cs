using System.Collections.Generic;
using System.Text;

namespace ROOT.Zfs.Public.Arguments.Snapshots
{
    /// <summary>
    /// Contains arguments for zfs hold
    /// </summary>
    public class SnapshotHoldArgs : SnapshotArgs
    {
        /// <inheritdoc />
        public SnapshotHoldArgs() : base("hold")
        {

        }

        /// <summary>
        /// The tag to use for the hold
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Specifies that a hold with the given tag is applied recursively to the snapshots of all descendent file systems.
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
