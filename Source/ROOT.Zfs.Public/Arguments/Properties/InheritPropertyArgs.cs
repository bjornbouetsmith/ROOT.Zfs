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
                errors = new List<string>
                {
                    "You cannot reset a property to inherited on a pool"
                };
            }

            ValidateString(Property, false, ref errors);
            ValidateString(Target, false, ref errors);

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