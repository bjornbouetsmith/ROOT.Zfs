using System.Text;

namespace ROOT.Zfs.Public.Arguments.Pool
{
    /// <summary>
    /// Contains the required arguments to a device offline inside a pool
    /// </summary>
    public class PoolOnlineArgs : PoolNameWithDeviceArgs
    {
        /// <summary>
        /// Creates an instance of the online args
        /// </summary>
        public PoolOnlineArgs() : base("online", true)
        {
        }

        /// <summary>
        /// Expand the device to use all available space.
        /// If the device is part of a mirror or raidz then all devices must be expanded before the new space will become available to the pool.
        /// </summary>
        public bool ExpandSpace { get; set; }

        /// <inheritdoc />
        protected override string BuildArgs(string command)
        {
            var args = new StringBuilder();
            args.Append(command);

            if (ExpandSpace)
            {
                args.Append(" -e");
            }
            
            args.Append($" {PoolName}");
            args.Append($" {Device}");

            return args.ToString();
        }
    }
}
