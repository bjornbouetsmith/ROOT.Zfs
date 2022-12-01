using System.Text;

namespace ROOT.Zfs.Public.Arguments.Pool
{
    /// <summary>
    /// Contains required and optional arguments for zpool resilver
    /// </summary>
    public class PoolResilverArgs : PoolNameArgs
    {
        /// <inheritdoc />
        public PoolResilverArgs() : base("resilver")
        {
            
        }

        /// <inheritdoc />
        protected override string BuildArgs(string command)
        {
            var args = new StringBuilder();
            args.Append(command);
            args.Append($" {Decode(Name)}");
            return args.ToString();
        }
    }
}
