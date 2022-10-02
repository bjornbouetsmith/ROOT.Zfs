using System;
using System.Diagnostics;
using ROOT.Shared.Utils.OS;

namespace ROOT.Zfs.Core
{
    public abstract class ZfsBase
    {
        private readonly IProcessCall _remoteConnection;

        protected ZfsBase(IProcessCall remoteConnection)
        {
            _remoteConnection = remoteConnection;
        }

        public TimeSpan CommandTimeout { get; set; } = TimeSpan.FromSeconds(30);

        protected IProcessCall BuildCommand(IProcessCall current)
        {
            // First use any remote connection
            IProcessCall command = _remoteConnection;
            
            // pipe into previous call if any
            command = ProcessCallExtensions.Pipe(command, current);
           

            Trace.WriteLine($"Built command:{command.FullCommandLine}");
            command.Timeout = CommandTimeout;
            return command;
        }
    }
}