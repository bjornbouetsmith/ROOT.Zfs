using ROOT.Zfs.Public.Data;
using System.Collections.Generic;
using System.Text;

namespace ROOT.Zfs.Public.Arguments.Snapshots
{
    /// <summary>
    /// Contains required and optional arguments to zfs clone
    /// </summary>
    public class SnapshotCloneArgs : SnapshotArgs
    {
        /// <inheritdoc />
        public SnapshotCloneArgs() : base("clone")
        {
        }

        /// <summary>
        /// The target dataset for the clone
        /// </summary>
        public string TargetDataset { get; set; }

        /// <summary>
        /// Any properties to set on the target dataset.
        /// this is optional
        /// </summary>
        public IList<PropertyValue> Properties { get; set; }

        /// <inheritdoc />
        public override bool Validate(out IList<string> errors)
        {
            base.Validate(out errors);

            ValidateString(TargetDataset, false, ref errors);

            if (Properties != null)
            {
                foreach (var prop in Properties)
                {
                    ValidateString(prop.Property, false, ref errors, true);
                    ValidateString(prop.Value, false, ref errors, true);
                }
            }

            return errors == null;
        }

        /// <inheritdoc />
        protected override string BuildArgs(string command)
        {
            var args = new StringBuilder();
            args.Append(command);
            args.Append(" -p");
            if (Properties != null)
            {
                foreach (var prop in Properties)
                {
                    args.Append($" -o {Decode(prop.Property)}={Decode(prop.Value)}");
                }
            }

            var rawSnapName = GetRawSnapshotName();
            args.Append($" {Decode(Dataset)}@{rawSnapName}");
            args.Append($" {Decode(TargetDataset)}");

            return args.ToString();
        }
    }
}
