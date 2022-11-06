using System.Collections.Generic;
using System.Text;

namespace ROOT.Zfs.Public.Arguments.Pool
{
    /// <summary>
    /// Represents all the arguments to the command zpool detach
    /// </summary>
    public class PoolDetachArgs : Args
    {
        /// <inheritdoc />
        public PoolDetachArgs() : base("detach")
        {
        }

        /// <summary>
        /// Name of the pool to detach a device from
        /// </summary>
        public string PoolName { get; set; }

        /// <summary>
        /// Name of the device to detach,
        /// </summary>
        public string Device { get; set; }

        /// <inheritdoc />
        public override bool Validate(out IList<string> errors)
        {
            errors = null;

            if (string.IsNullOrWhiteSpace(PoolName))
            {
                errors = new List<string>();
                errors.Add("Please set the pool name");
            }

            if (string.IsNullOrWhiteSpace(Device))
            {
                errors ??=new List<string>();
                errors.Add("Please set the device name to detach");
            }

            return errors == null;
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
