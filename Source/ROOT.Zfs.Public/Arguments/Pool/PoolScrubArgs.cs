using ROOT.Zfs.Public.Data.Pools;
using System;
using System.Text;

namespace ROOT.Zfs.Public.Arguments.Pool
{
    /// <summary>
    /// Contains required and optional arguments for zpool scrub
    /// </summary>
    public class PoolScrubArgs : PoolNameArgs
    {
        /// <inheritdoc />
        public PoolScrubArgs() : base("scrub")
        {
        }

        /// <summary>
        /// Sets options to control the scrub process
        /// <see cref="ScrubOption"/> for details
        /// </summary>
        public ScrubOption Options { get; set; }


        /// <inheritdoc />
        protected override string BuildArgs(string command)
        {
            var args = new StringBuilder();
            args.Append(command);
            switch (Options)
            {
                case ScrubOption.Pause:
                    args.Append(" -p");
                    break;
                case ScrubOption.Stop:
                    args.Append(" -s");
                    break;
                case ScrubOption.None:
                    break;
            }

            args.Append($" {Decode(PoolName)}");
            return args.ToString();
        }
    }
}
