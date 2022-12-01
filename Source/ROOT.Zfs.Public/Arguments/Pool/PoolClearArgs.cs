using System.Text;

namespace ROOT.Zfs.Public.Arguments.Pool
{
    /// <summary>
    /// Contains required and optional arguments for zpool clear
    /// </summary>
    public class PoolClearArgs : PoolNameWithDeviceArgs
    {
        /// <inheritdoc />
        public PoolClearArgs() : base("clear", false)
        {
        }

        /// <inheritdoc />
        protected override string BuildArgs(string command)
        {
            var args = new StringBuilder();
            args.Append(command);
            args.Append($" {Decode(PoolName)}");
            var device = Decode(Device);
            if (!string.IsNullOrWhiteSpace(device))
            {
                args.Append($" {device}");
            }

            return args.ToString();
        }
    }
}
