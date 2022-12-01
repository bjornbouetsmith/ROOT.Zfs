using System.Text;

namespace ROOT.Zfs.Public.Arguments.Pool
{
    /// <summary>
    /// Contains the required arguments to a device offline inside a pool
    /// </summary>
    public class PoolOfflineArgs : PoolNameWithDeviceArgs
    {
        /// <summary>
        /// Creates an offline args instance
        /// </summary>
        public PoolOfflineArgs() : base("offline", true)
        {
        }

        /// <summary>
        /// Force fault. Instead of offlining the disk, put it into a faulted state. The fault will persist across imports unless the -t flag was specified.
        /// </summary>
        public bool ForceFault { get; set; }

        /// <summary>
        /// Temporary. Upon reboot, the specified physical device reverts to its previous state.
        /// </summary>
        public bool Temporary { get; set; }

        /// <inheritdoc />
        protected override string BuildArgs(string command)
        {
            var args = new StringBuilder();
            args.Append(command);
            if (ForceFault)
            {
                args.Append(" -f");
            }

            if (Temporary)
            {
                args.Append(" -t");
            }

            args.Append($" {PoolName}");
            args.Append($" {Device}");

            return args.ToString();
        }
    }
}
