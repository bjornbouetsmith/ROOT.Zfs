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

            ValidateString(Property,false, ref errors);
            ValidateString(Value, false, ref errors);
            ValidateString(Target, false, ref errors);
            
            return errors == null;
        }

        /// <inheritdoc />
        protected override string BuildArgs(string command)
        {
            var args = new StringBuilder();
            args.Append(command);
            args.Append($" {Decode(Property)}={Decode(Value)} {Decode(Target)}");
            return args.ToString();
        }
    }
}