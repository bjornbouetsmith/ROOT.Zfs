using System.Collections.Generic;
using System.Text;

namespace ROOT.Zfs.Public.Arguments.Properties
{
    /// <summary>
    /// Represents required arguments for getting all/one property
    /// </summary>
    public class GetPropertyArgs : PropertyArgs
    {
        /// <inheritdoc />
        public GetPropertyArgs() : base("get")
        {
        }

        /// <inheritdoc />
        public override bool Validate(out IList<string> errors)
        {
            errors = null;
            // all combinations are valid, since this is being used to handle
            // get (to get available properties)
            // get property
            // get property target -H
            // get all target -H

            if (!string.IsNullOrWhiteSpace(Property))
            {
                // if property is set, it has to be valid
                ValidateString(Property,false, ref errors);
            }

            if (!string.IsNullOrEmpty(Target))
            {
                // If target is set it has to be valid
                ValidateString(Target, false, ref errors);

                if (errors == null 
                    && PropertyTarget == PropertyTarget.Pool
                    && Decode(Target).Contains('/'))
                {
                    //explicitly disallow / in a pool name
                    errors = new List<string>();
                    errors.Add("character '/' is not allowed in a pool name");
                }
            }

            return errors == null;
        }

        /// <inheritdoc />
        protected override string BuildArgs(string command)
        {
            var args = new StringBuilder();
            args.Append(command);

            // Append all if no property has been specified, but a target dataset has been specified
            if (string.IsNullOrWhiteSpace(Property))
            {
                if (!string.IsNullOrWhiteSpace(Target))
                {
                    args.Append(" all");
                }
            }
            else
            {
                args.Append($" {Property}");
            }

            if (!string.IsNullOrWhiteSpace(Target))
            {
                args.Append($" {Target}");
            }

            args.Append(" -H");
            return args.ToString();
        }

    }
}