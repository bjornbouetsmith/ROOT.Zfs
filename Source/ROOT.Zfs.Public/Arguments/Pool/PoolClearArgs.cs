using System.Text;

namespace ROOT.Zfs.Public.Arguments.Pool
{
    /// <summary>
    /// Contains required and optional arguments for zpool clear
    /// </summary>
    public class PoolClearArgs : PoolNameWithDeviceArgs
    {
        /// <inheritdoc />
        public PoolClearArgs() : base("clear",false)
        {
        }

        /// <inheritdoc />
        protected override string BuildArgs(string command)
        {
            var args = new StringBuilder();
            args.Append(command);
            args.Append($" {Decode(PoolName)}");
            if (!string.IsNullOrWhiteSpace(Device))
            {
                args.Append($" {Decode(Device)}");
            }
           
            return args.ToString();
        }
    }
}
