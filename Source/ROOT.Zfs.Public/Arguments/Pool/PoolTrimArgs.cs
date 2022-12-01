using System.Collections.Generic;
using System.Text;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Public.Arguments.Pool
{
    /// <summary>
    /// Contains the required arguments to trim either a pool or a device inside the pool
    /// </summary>
    public class PoolTrimArgs : Args
    {
        /// <summary>
        /// Creates an instance of the trim args
        /// </summary>
        public PoolTrimArgs() : base("trim")
        {
        }

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
        public override bool Validate(out IList<string> errors)
        {
            errors = null;
            ValidateString(PoolName, false, ref errors);
            ValidateString(DeviceName, true, ref errors);

            return errors == null;
        }

        /// <inheritdoc />
        protected override string BuildArgs(string command)
        {
            StringBuilder args = new StringBuilder();

            args.Append(command);

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

            args.Append($" {Decode(PoolName)}");
            var device = Decode(DeviceName);
            if (!string.IsNullOrWhiteSpace(device))
            {
                args.Append($" {device}");
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
