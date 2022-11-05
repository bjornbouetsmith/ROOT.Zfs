using System.Collections.Generic;
using System.Text;

namespace ROOT.Zfs.Public.Arguments.Pool
{
    /// <summary>
    /// Contains the required arguments to a device offline inside a pool
    /// </summary>
    public class PoolOnlineArgs : ArgsBase
    {
        /// <summary>
        /// The name of the pool to take offline
        /// </summary>
        public string PoolName { get; set; }

        /// <summary>
        /// The device to take offline
        /// </summary>
        public string Device { get; set; }

        /// <summary>
        /// Expand the device to use all available space.
        /// If the device is part of a mirror or raidz then all devices must be expanded before the new space will become available to the pool.
        /// </summary>
        public bool ExpandSpace { get; set; }

        /// <inheritdoc />
        public override bool Validate(out IList<string> errors)
        {
            errors = null;
            if (string.IsNullOrWhiteSpace(PoolName))
            {
                errors = new List<string>();
                errors.Add("Please specify a pool name");
            }

            if (string.IsNullOrWhiteSpace(Device))
            {
                errors ??= new List<string>();
                errors.Add("Please specify a device to take online");
            }

            return errors == null;
        }

        /// <inheritdoc />
        public override string BuildArgs(string command)
        {
            var args = new StringBuilder();
            args.Append(command);

            if (ExpandSpace)
            {
                args.Append(" -e");
            }
            
            args.Append($" {PoolName}");
            args.Append($" {Device}");

            return args.ToString();
        }
    }
}
