using System;
using System.Diagnostics;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Public;

namespace ROOT.Zfs.Core
{
    /// <inheritdoc />
    public abstract class ZfsBase : IBasicZfs
    {
        private readonly IProcessCall _remoteConnection;

        /// <summary>
        /// Creates bas
        /// </summary>
        /// <param name="remoteConnection"></param>
        protected ZfsBase(IProcessCall remoteConnection)
        {
            _remoteConnection = remoteConnection;
        }

        /// <inheritdoc />
        public TimeSpan CommandTimeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <inheritdoc />
        public bool RequiresSudo { get; set; } = false;

        /// <summary>
        /// Builds a command so any settings are applied correctly
        /// </summary>
        /// <param name="current">The current command to modify if required</param>
        /// <returns>The command built</returns>
        protected IProcessCall BuildCommand(IProcessCall current)
        {
            // First use any remote connection
            IProcessCall command = _remoteConnection;

            // pipe into previous call if any
            command = ProcessCallExtensions.Pipe(command, current);


            Trace.WriteLine($"Built command:{command.FullCommandLine}");
            command.Timeout = CommandTimeout;
            command.RequiresSudo = RequiresSudo;
            return command;
        }
    }
}