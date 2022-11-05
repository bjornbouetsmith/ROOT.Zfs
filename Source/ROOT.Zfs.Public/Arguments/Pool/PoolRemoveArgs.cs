using System.Collections.Generic;
using System.Text;

namespace ROOT.Zfs.Public.Arguments.Pool
{
    /// <summary>
    /// Encapsulates all the arguments for zpool remove
    /// </summary>
    public class PoolRemoveArgs : Args
    {
        /// <summary>
        /// Creates a remove args instance
        /// </summary>
        public PoolRemoveArgs() : base("remove")
        {
        }

        /// <summary>
        /// Name of the pool to remove a device from
        /// </summary>
        public string PoolName { get; set; }
        
        /// <summary>
        /// name of vdev or name of device to remove.
        /// Please be aware that not all types of vdevs and sinle devices can be removed.
        /// </summary>
        public string VDevOrDevice { get; set; }

        /// <summary>
        /// Stops and cancel existing removal process for <see cref="PoolName"/>
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Validates that the pool remove arguments contains the minimum required information
        /// </summary>
        public override bool Validate(out IList<string> errors)
        {
            errors = null;
            if (string.IsNullOrWhiteSpace(PoolName))
            {
                errors = new List<string>();
                errors.Add("Please provide a pool name to add a vdev to");
            }

            if (string.IsNullOrWhiteSpace(VDevOrDevice) && !Cancel)
            {
                errors ??= new List<string>();
                errors.Add("Please provide a vdev or device name to remove");
            }

            return errors == null;
        }

        /// <summary>
        /// Returns a string represenation of all the arguments that can be passed onto zpool remove
        /// </summary>
        /// <returns></returns>
        protected override string BuildArgs(string command)
        {
            StringBuilder args = new StringBuilder();

            args.Append(command);

            if (Cancel)
            {
                args.Append($" -s {PoolName}");
            }
            else
            {
                args.Append($" {PoolName}");
                args.Append($" {VDevOrDevice}");
            }

            return args.ToString();
        }
    }
}
