using System.Collections.Generic;
using System.Text;

namespace ROOT.Zfs.Public.Arguments.Pool
{
    /// <summary>
    /// Contains the required arguments to a device offline inside a pool
    /// </summary>
    public class ZpoolOfflineArgs
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
        /// Force fault. Instead of offlining the disk, put it into a faulted state. The fault will persist across imports unless the -t flag was specified.
        /// </summary>
        public bool ForceFault { get; set; }

        /// <summary>
        /// Temporary. Upon reboot, the specified physical device reverts to its previous state.
        /// </summary>
        public bool Temporary { get; set; }


        /// <summary>
        /// Validates the arguments and returns whether or not they are valid
        /// </summary>
        /// <param name="errors">Any errors</param>
        /// <returns>true if valid;false otherwise</returns>
        public bool Validate(out IList<string> errors)
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
                errors.Add("Please specify a device to take offline");
            }

            return errors == null;
        }

        /// <summary>
        /// Returns a string representation of the arguments, that shoul be passed onto zpool upgrade
        /// </summary>
        public override string ToString()
        {
            var args = new StringBuilder();
            
            if (ForceFault)
            {
                args.Append(" -f");
            }

            if (Temporary)
            {
                args.Append(" -t");
            }

            args.Append($" {PoolName}");
            args.Append($" {Device}");

            return args.ToString();
        }
    }
}
