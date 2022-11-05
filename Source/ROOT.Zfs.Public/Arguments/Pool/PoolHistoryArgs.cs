using System;
using System.Collections.Generic;

namespace ROOT.Zfs.Public.Arguments.Pool
{
    /// <summary>
    /// Contains the arguments for zpool history
    /// </summary>
    public class PoolHistoryArgs : Args
    {
        /// <summary>
        /// Name of pool to get history from
        /// </summary>
        public string PoolName { get; set; }

        /// <summary>
        /// Number of lines to skip from history.
        /// This is meant to be used for paging
        /// </summary>
        public int SkipLines { get; set; }

        /// <summary>
        /// Only return history events after given date/time
        /// </summary>
        public DateTime AfterDate { get; set; }

        /// <inheritdoc />
        public PoolHistoryArgs() : base("history")
        {
        }

        /// <inheritdoc />
        public override bool Validate(out IList<string> errors)
        {
            errors = null;

            if (string.IsNullOrWhiteSpace(PoolName))
            {
                errors = new List<string>();
                errors.Add("Please specify a pool name");
            }

            return errors == null;
        }

        /// <inheritdoc />
        protected override string BuildArgs(string command)
        {
            return $"{command} -l {PoolName}";
        }

    }
}
