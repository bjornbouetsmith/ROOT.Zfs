using System.Diagnostics;
using ROOT.Shared.Utils.OS;

namespace ROOT.Zfs.Core;

public class ZfsBase
{
    private readonly RemoteProcessCall _remoteConnection;

    public ZfsBase(RemoteProcessCall remoteConnection)
    {
        _remoteConnection = remoteConnection;
    }

    protected ProcessCall BuildCommand(ProcessCall current, ProcessCall previousCall = null)
    {
        ProcessCall command = null;
        
        // First use any remote connection
        if (_remoteConnection != null)
        {
            command = _remoteConnection;
        }

        // pipe into previous call if any
        if (previousCall != null)
        {
            command |= previousCall;
        }
        
        // finally make the command complete, by piping into the current command
        command |= current;

        Debug.WriteLine(command.FullCommandLine);
        return command;
    }
}