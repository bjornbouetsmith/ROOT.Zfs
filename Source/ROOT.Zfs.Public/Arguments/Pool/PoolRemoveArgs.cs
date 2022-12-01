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
        /// name of vdev to remove.
        /// Please be aware that not all types of vdevs can be removed.
        /// see https://openzfs.github.io/openzfs-docs/man/8/zpool-remove.8.html for detals
        /// </summary>
        public string VDev { get; set; }

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
            ValidateString(PoolName, false, ref errors);
            ValidateString(VDev, Cancel,ref errors);
            
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
                args.Append($" -s {Decode(PoolName)}");
            }
            else
            {
                args.Append($" {Decode(PoolName)}");
                args.Append($" {Decode(VDev)}");
            }

            return args.ToString();
        }
    }
}
