using System.Collections.Generic;

namespace ROOT.Zfs.Public.Arguments.Pool
{
    /// <summary>
    /// Simple wrapper argument around a name of a pool
    /// </summary>
    public class PoolNameArg : Args
    {
        /// <summary>
        /// name of pool
        /// </summary>
        public string Name { get; set; }

        /// <inheritdoc />
        public PoolNameArg(string command) : base(command)
        {
        }

        /// <inheritdoc />
        public override bool Validate(out IList<string> errors)
        {
            errors = null;
            ValidateString(Name, false, ref errors);
            if (errors == null && Decode(Name).Contains('/'))
            {
                //explicitly disallow / in a pool name
                errors = new List<string>();
                errors.Add("character '/' is not allowed in a pool name");
            }

            return errors == null;
        }

        /// <inheritdoc />
        protected override string BuildArgs(string command)
        {
            return Decode(Name);
        }

        /// <summary>
        /// Implicit converstion of a string into a PoolNameArg object
        /// </summary>
        public static implicit operator PoolNameArg(string name)
        {
            return new PoolNameArg(string.Empty) { Name = name };
        }
    }
}
