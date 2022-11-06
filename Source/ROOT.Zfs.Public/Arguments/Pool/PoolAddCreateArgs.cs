using System.Collections.Generic;

namespace ROOT.Zfs.Public.Arguments.Pool
{
    /// <summary>
    /// Contains whats common between create pool and add device(s) to a poool
    /// </summary>
    public abstract class PoolAddCreateArgs : Args
    {
        /// <inheritdoc />
        protected PoolAddCreateArgs(string command) : base(command)
        {
        }

        /// <summary>
        /// The name of the pool.
        /// </summary>
        public string PoolName { get; set; }

        /// <summary>
        /// All the vdevs, spares, cache, logs etc.
        /// </summary>
        public IList<VDevCreationArgs> VDevs { get; set; }

        /// <summary>
        /// Validates that the pool creation arguments contains the minimum required information
        /// </summary>
        public override bool Validate(out IList<string> errors)
        {
            errors = null;
            if (string.IsNullOrWhiteSpace(PoolName))
            {
                errors = new List<string>();
                errors.Add("Please provide a name for the pool");
            }

            if (VDevs == null || VDevs.Count == 0)
            {
                errors ??= new List<string>();
                errors.Add("Please provide vdevs for the pool");
            }
            else
            {
                foreach (var vdevArg in VDevs)
                {
                    if (vdevArg.Validate(out var vdevErrors))
                    {
                        continue;
                    }

                    errors ??= new List<string>();
                    foreach (var error in vdevErrors)
                    {
                        errors.Add(error);
                    }
                }
            }

            return errors == null;
        }
    }
}