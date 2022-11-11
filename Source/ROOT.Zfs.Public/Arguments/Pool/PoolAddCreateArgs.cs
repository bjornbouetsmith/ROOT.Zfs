using System.Collections.Generic;

namespace ROOT.Zfs.Public.Arguments.Pool
{
    /// <summary>
    /// Contains whats common between create pool and add device(s) to a poool
    /// </summary>
    public abstract class PoolAddCreateArgs : PoolNameArgs
    {
        /// <inheritdoc />
        protected PoolAddCreateArgs(string command) : base(command)
        {
        }

        /// <summary>
        /// All the vdevs, spares, cache, logs etc.
        /// </summary>
        public IList<VDevCreationArgs> VDevs { get; set; }

        /// <summary>
        /// Validates that the pool creation arguments contains the minimum required information
        /// </summary>
        public override bool Validate(out IList<string> errors)
        {
            base.Validate(out errors);
            
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