using System.Collections.Generic;
using System.Text;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Public.Arguments.Pool
{
    /// <summary>
    /// Contains the required arguments to trim either a pool or a device inside the pool
    /// </summary>
    public class PoolTrimArgs
    {
        /// <summary>
        /// The name of the pool to trim
        /// </summary>
        public string PoolName { get; set; }

        /// <summary>
        /// Optional name of a device to trim inside the pool
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// Controls the rate at which the trim operation progresses.
        /// Not setting this makes the trim go as fast as possible.
        /// </summary>
        public Size TrimRate { get; set; }

        /// <summary>
        /// Controls what to do relating to trim, i.e. start, cancel suspend
        /// Note, cancelling or suspending without an ongoing trim, will make the command fail
        /// </summary>
        public TrimAction Action { get; set; }

        /// <summary>
        /// Causes a secure trim to be initiated.
        /// Requires support from the underlying device
        /// </summary>
        public bool Secure { get; set; }

        /// <summary>
        /// Validates thats the trim arguments have all required information
        /// </summary>
        public bool Validate(out IList<string> errors)
        {
            errors = null;

            if (string.IsNullOrWhiteSpace(PoolName))
            {
                errors = new List<string>();
                errors.Add("Pool name must be specified");
            }

            return errors == null;
        }

        /// <summary>
        /// Returns a string representation of this instance, that can be passed onto zpool trim
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder args = new StringBuilder();

            if (Secure)
            {
                args.Append(" -d");
            }

            if (TrimRate.Bytes > 0)
            {
                args.Append($" -r {TrimRate}");
            }

            if (Action != TrimAction.None)
            {
                args.Append(Action == TrimAction.Cancel ? " -c" : " -s");
            }

            args.Append($" {PoolName}");

            if (!string.IsNullOrWhiteSpace(DeviceName))
            {
                args.Append($" {DeviceName}");
            }

            return args.ToString();
        }
    }

    /// <summary>
    /// Controls what to do relating to trim, i.e. start, cancel suspend
    /// </summary>
    public enum TrimAction
    {
        /// <summary>
        /// Start or resume a trim
        /// </summary>
        None = 0,

        /// <summary>
        /// Cancel an ongoing trim
        /// </summary>
        Cancel = 1,

        /// <summary>
        /// Suspend an ongoing trim
        /// </summary>
        Suspend = 2
    }
}
