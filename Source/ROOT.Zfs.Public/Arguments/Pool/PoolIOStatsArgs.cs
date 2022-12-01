using System.Collections.Generic;
using System.Text;

namespace ROOT.Zfs.Public.Arguments.Pool
{
    /// <summary>
    /// Contains required and optional arguments for zpool iostats
    /// </summary>
    public class PoolIOStatsArgs : PoolNameArgs
    {
        /// <inheritdoc />
        public PoolIOStatsArgs() : base("iostat")
        {
        }

        /// <summary>
        /// Optional list of devices to retrieve io stats for.
        /// </summary>
        public IList<string> Devices { get; set; }


        /// <inheritdoc />
        public override bool Validate(out IList<string> errors)
        {
            base.Validate(out errors);
            if (Devices != null && Devices.Count > 0)
            {
                foreach (var device in Devices)
                {
                    {
                        ValidateString(device, true, ref errors, false, nameof(Devices));
                    }
                }
            }
            return errors == null;
        }

        /// <inheritdoc />
        protected override string BuildArgs(string command)
        {
            var args = new StringBuilder();
            args.Append(command);
            args.Append(" -LlpPvH");
            args.Append($" {Decode(Name)}");
            if (Devices != null && Devices.Count > 0)
            {
                foreach (var device in Devices)
                {
                    if (!string.IsNullOrWhiteSpace(device))
                    {
                        args.Append($" {Decode(device)}");
                    }
                }
            }

            return args.ToString();
        }
    }
}
