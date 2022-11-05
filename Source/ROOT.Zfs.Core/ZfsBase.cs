using System;
using System.Diagnostics;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Public;

namespace ROOT.Zfs.Core
{
    internal abstract class ZfsBase : IBasicZfs
    {
        private readonly IProcessCall _remoteConnection;

        protected ZfsBase(IProcessCall remoteConnection)
        {
            _remoteConnection = remoteConnection;
        }

        public TimeSpan CommandTimeout { get; set; } = TimeSpan.FromSeconds(30);
        public bool RequiresSudo { get; set; } = false;

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