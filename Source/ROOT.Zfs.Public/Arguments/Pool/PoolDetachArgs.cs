using System.Text;

namespace ROOT.Zfs.Public.Arguments.Pool
{
    /// <summary>
    /// Represents all the arguments to the command zpool detach
    /// </summary>
    public class PoolDetachArgs : PoolOnlineOfflineDetachArgs
    {
        /// <inheritdoc />
        public PoolDetachArgs() : base("detach")
        {
        }

        /// <inheritdoc />
        protected override string BuildArgs(string command)
        {
            var args = new StringBuilder();
            args.Append(command);
            args.Append($" {PoolName} {Device}");

            return args.ToString();
        }
    }
}
