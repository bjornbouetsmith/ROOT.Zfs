using System.Text;

namespace ROOT.Zfs.Public.Arguments.Pool
{
    /// <summary>
    /// Contains the parameters for destroying a pool
    /// </summary>
    public class PoolDestroyArgs : PoolNameArgs
    {
        /// <inheritdoc />
        public PoolDestroyArgs() : base("destroy")
        {
        }

        /// <inheritdoc />
        protected override string BuildArgs(string command)
        {
            var args = new StringBuilder();
            args.Append(command);
            args.Append($" -f {Decode(PoolName)}");
            return args.ToString();
        }
    }
}
