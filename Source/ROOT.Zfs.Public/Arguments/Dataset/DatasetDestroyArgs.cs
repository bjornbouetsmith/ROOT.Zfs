using System.Collections.Generic;
using System.Text;

namespace ROOT.Zfs.Public.Arguments.Dataset
{
    /// <summary>
    /// Contains required and optional arguments for zfs destroy
    /// </summary>
    public class DatasetDestroyArgs : Args
    {
        /// <inheritdoc />
        public DatasetDestroyArgs() : base("destroy")
        {
        }

        /// <summary>
        /// Name of dataset to destroy
        /// </summary>
        public string Dataset { get; set; }

        /// <summary>
        /// Flags to control how the dataset is destroyed
        /// </summary>
        public DatasetDestroyFlags DestroyFlags { get; set; }

        /// <inheritdoc />
        public override bool Validate(out IList<string> errors)
        {
            errors = null;
            ValidateString(Dataset, false, ref errors);
            return errors == null;
        }

        /// <inheritdoc />
        protected override string BuildArgs(string command)
        {
            var args = new StringBuilder();
            args.Append(command);
            if (DestroyFlags.HasFlag(DatasetDestroyFlags.Recursive))
            {
                args.Append(" -r");
            }

            if (DestroyFlags.HasFlag(DatasetDestroyFlags.RecursiveClones))
            {
                args.Append(" -R");
            }

            if (DestroyFlags.HasFlag(DatasetDestroyFlags.ForceUmount))
            {
                args.Append(" -f");
            }

            if (DestroyFlags.HasFlag(DatasetDestroyFlags.DryRun))
            {
                args.Append(" -nvp");
            }

            args.Append($" {Decode(Dataset)}");

            return args.ToString();
        }
    }
}
