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
        public string PoolName { get; set; }

        /// <inheritdoc />
        protected PoolNameArgs(string command) : base(command)
        {
        }

        /// <inheritdoc />
        public override bool Validate(out IList<string> errors)
        {
            errors = null;
            ValidateString(PoolName, false, ref errors);
            if (errors == null && Decode(PoolName).Contains('/'))
            {
                //explicitly disallow / in a pool name
                errors = new List<string>
                {
                    "character '/' is not allowed in a pool name"
                };
            }

            return errors == null;
        }
    }
}
