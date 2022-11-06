using System.Collections.Generic;
using System.Text;

namespace ROOT.Zfs.Public.Arguments
{
    /// <summary>
    /// Represents all arguments to get/set/reset property commands
    /// </summary>
    public abstract class PropertyArgs : Args
    {
        /// <inheritdoc />
        protected PropertyArgs(string command) : base(command)
        {
        }

        /// <summary>
        /// Gets or sets the property taget type
        /// </summary>
        public PropertyTarget PropertyTarget { get; set; }
        
        /// <summary>
        /// The target of the action, i.e. dataset or pool name
        /// Optional if used as a get available properties
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// The name of the property to get/set
        /// Optional if used as a get all commanmd
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// The value of the property to set
        /// Option - if used as get property/get all
        /// </summary>
        public string Value { get; set; }
    }

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
