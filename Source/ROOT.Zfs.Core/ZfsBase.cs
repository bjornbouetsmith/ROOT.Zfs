using System;
using System.Diagnostics;
using ROOT.Shared.Utils.OS;

namespace ROOT.Zfs.Core
{
    public abstract class ZfsBase
    {
        private readonly SSHProcessCall _remoteConnection;

        protected ZfsBase(SSHProcessCall remoteConnection)
        {
            _remoteConnection = remoteConnection;
        }

        public TimeSpan CommandTimeout { get; set; } = TimeSpan.FromSeconds(30);

        protected ProcessCall BuildCommand(ProcessCall current, ProcessCall previousCall = null)
        {
            // First use any remote connection
            ProcessCall command = _remoteConnection;
            
            // pipe into previous call if any
            command |= previousCall;

            // finally make the command complete, by piping into the current command
            command |= current;

            Trace.WriteLine($"Built command:{command.FullCommandLine}");
            command.Timeout = CommandTimeout;
            return command;
        }
    }
}