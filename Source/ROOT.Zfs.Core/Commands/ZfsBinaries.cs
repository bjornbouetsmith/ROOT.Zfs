using System.Diagnostics;
using ROOT.Shared.Utils.OS;

namespace ROOT.Zfs.Core.Commands;

/// <summary>
/// Default implementation of <see cref="IZfsBinaries"/>.
/// </summary>
public sealed class ZfsBinaries : ZfsBase, IZfsBinaries
{
    /// <summary>
    /// Creates an instance of the <see cref="ZfsBinaries"/> class.
    /// </summary>
    /// <param name="remoteConnection">Any remote connection that should be used when resolving binaries. If null, the local machine is used.</param>
    public ZfsBinaries(IProcessCall remoteConnection = null)
        : base(remoteConnection)
    {
    }

    /// <inheritdoc />
    public string Zfs { get; set; } = "/sbin/zfs";

    /// <inheritdoc />
    public string Zpool { get; set; } = "/sbin/zpool";

    /// <inheritdoc />
    public string Zdb { get; set; } = "/sbin/zdb";

    /// <inheritdoc />
    public string Ls { get; set; } = "/bin/ls";

    /// <inheritdoc />
    public string Lsblk { get; set; } = "/bin/lsblk";

    /// <inheritdoc />
    public string Which { get; set; } = "/bin/which";

    /// <inheritdoc />
    public string Smartctl { get; set; } = "/sbin/smartctl";

    /// <inheritdoc />
    public void Initialize()
    {
        Zfs = GetBinaryLocation("zfs", Zfs);
        Zpool = GetBinaryLocation("zpool", Zpool);
        Zdb = GetBinaryLocation("zdb", Zdb);
        Ls = GetBinaryLocation("ls", Ls);
        Lsblk = GetBinaryLocation("lsblk", Lsblk);
        Smartctl = GetBinaryLocation("smartctl", Smartctl);
        Initialized = true;
    }

    /// <inheritdoc />
    public bool Initialized { get; private set; }

    private string GetBinaryLocation(string binary, string defaultBinary)
    {
        var command = BuildCommand(new ProcessCall(Which, binary));
        var response = command.LoadResponse(false);

        if (response.Success)
        {
            Trace.TraceInformation($"{binary}={response.StdOut.Trim()}");
            return response.StdOut.Trim();
        }

        Trace.TraceError($"Could not find binary:{binary} will default to:{defaultBinary} - response from os was Out{response.StdOut}, Err:{response.StdError}");
        return defaultBinary;
    }
}
