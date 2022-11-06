using System.Collections.Generic;
using System.Text;

namespace ROOT.Zfs.Public.Arguments.Properties
{
    /// <summary>
    /// Represents required arguments for resetting a property to inherited
    /// </summary>
    public class InheritPropertyArgs : PropertyArgs
    {
        /// <inheritdoc />
        public InheritPropertyArgs() : base("inherit")
        {
        }

        /// <inheritdoc />
        public override bool Validate(out IList<string> errors)
        {
            errors = null;
            if (PropertyTarget == PropertyTarget.Pool)
            {
                errors = new List<string>();
                errors.Add("You cannot reset a property to inherited on a pool");
            }

            if (string.IsNullOrWhiteSpace(Property))
            {
                errors ??= new List<string>();
                errors.Add("Please specify a property to reset");
            }

            if (string.IsNullOrWhiteSpace(Target))
            {
                errors ??= new List<string>();
                errors.Add("Please specify a dataset in which you want to reset the property");
            }

            return errors == null;
        }

        /// <inheritdoc />
        protected override string BuildArgs(string command)
        {
            var args = new StringBuilder();
            args.Append(command);
            args.Append($" -rS {Property} {Target}");

            return args.ToString();
        }
    }
}