using System.Collections.Generic;

namespace ROOT.Zfs.Public.Arguments.Pool
{
    /// <summary>
    /// Class with shared properties between online/offline
    /// </summary>
    public abstract class PoolOnlineOfflineArgs : Args
    {
        /// <inheritdoc />
        protected PoolOnlineOfflineArgs(string command) : base(command)
        {
        }

        /// <summary>
        /// The name of the pool in which to either take a device offline or online
        /// </summary>
        public string PoolName { get; set; }

        /// <summary>
        /// The device to take either offline or online
        /// </summary>
        public string Device { get; set; }

        /// <inheritdoc />
        public override bool Validate(out IList<string> errors)
        {
            errors = null;
            if (string.IsNullOrWhiteSpace(PoolName))
            {
                errors = new List<string>();
                errors.Add("Please specify a pool name");
            }

            if (string.IsNullOrWhiteSpace(Device))
            {
                errors ??= new List<string>();
                errors.Add("Please specify a device");
            }

            return errors == null;
        }
    }
}