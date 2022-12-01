using System.Text;

namespace ROOT.Zfs.Public.Arguments.Pool
{
    /// <summary>
    /// Represents all the arguments to the command zpool detach
    /// </summary>
    public class PoolDetachArgs : PoolNameWithDeviceArgs
    {
        /// <inheritdoc />
        public PoolDetachArgs() : base("detach", true)
        {
        }

        /// <inheritdoc />
        protected override string BuildArgs(string command)
        {
            var args = new StringBuilder();
            args.Append(command);
            args.Append($" {Decode(PoolName)} {Decode(Device)}");

            return args.ToString();
        }
    }
}
