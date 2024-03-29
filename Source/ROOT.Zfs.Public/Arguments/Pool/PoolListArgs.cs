﻿using System.Text;

namespace ROOT.Zfs.Public.Arguments.Pool
{
    /// <summary>
    /// Represents all arguments to zpool list pool
    /// </summary>
    public class PoolListArgs : PoolNameArgs
    {
        /// <inheritdoc />
        public PoolListArgs() : base("list")
        {
        }

        /// <inheritdoc />
        protected override string BuildArgs(string command)
        {
            var args = new StringBuilder();

            args.Append(command);
            args.Append($" -PHp {Decode(PoolName)}");
            
            return args.ToString();
        }
    }
}
