using System.Collections.Generic;

namespace ROOT.Zfs.Public.Arguments.Pool
{
    /// <summary>
    /// Simple wrapper argument around a name of a pool
    /// </summary>
    public abstract class PoolNameArgs : Args
    {
        /// <summary>
        /// name of pool
        /// </summary>
        public string Name { get; set; }

        /// <inheritdoc />
        protected PoolNameArgs(string command) : base(command)
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
    }
}
