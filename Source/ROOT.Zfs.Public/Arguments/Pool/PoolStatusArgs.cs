using System.Text;

namespace ROOT.Zfs.Public.Arguments.Pool
{
    /// <summary>
    /// Encapsulates all the arguments to zpool status
    /// </summary>
    public class PoolStatusArgs : PoolNameArgs
    {
        /// <inheritdoc />
        public PoolStatusArgs() : base("status")
        {
        }

        /// <inheritdoc />
        protected override string BuildArgs(string command)
        {
            var args = new StringBuilder();
            args.Append(command);
            args.Append($" -vP {Decode(Name)}");
            return args.ToString();
        }
    }
}
