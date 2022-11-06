using System.Collections.Generic;
using System.Text;

namespace ROOT.Zfs.Public.Arguments.Properties
{
    /// <summary>
    /// Represents required arguments for setting a property
    /// </summary>
    public class SetPropertyArgs : PropertyArgs
    {
        /// <inheritdoc />
        public SetPropertyArgs() : base("set")
        {
        }

        /// <inheritdoc />
        public override bool Validate(out IList<string> errors)
        {
            errors = null;

            if (string.IsNullOrWhiteSpace(Property))
            {
                errors = new List<string>();
                errors.Add("Please specify a property to set");
            }

            if (string.IsNullOrWhiteSpace(Value))
            {
                errors ??= new List<string>();
                errors.Add("Please value to set the property to");
            }

            if (string.IsNullOrWhiteSpace(Target))
            {
                errors ??= new List<string>();
                errors.Add("Please specify a dataset in which you want to set the property");
            }

            return errors == null;
        }

        /// <inheritdoc />
        protected override string BuildArgs(string command)
        {
            var args = new StringBuilder();
            args.Append(command);
            args.Append($" {Property}={Value} {Target}");
            return args.ToString();
        }
    }
}